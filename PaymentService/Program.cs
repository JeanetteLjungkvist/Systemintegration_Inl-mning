using System.Text;
using PaymentService;
using PaymentService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PaymentContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHttpClient<MenuClient>(client => {
    client.BaseAddress = new Uri("http://menuservice");
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.SaveToken = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentContext>();
    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();

// En användare kan köpa en existerande menu item

app.MapPost("/payment/{id}", async ( string menuId, PaymentContext ctx, MenuClient menuClient, HttpContext http) => {

    var menuItem = await menuClient.GetMenu(menuId);

    if(menuItem == null) return Results.NotFound("This menu item does not exist");

    var customerId = http.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

    if(customerId == null) return Results.BadRequest("Bad Token");

    var payment = new Payment();

    payment.Id = Guid.NewGuid();
    payment.CustomerId = customerId;
    payment.Total = menuItem.Price;
    payment.Date = DateTime.UtcNow;

    await ctx.Payments.AddAsync(payment);
    await ctx.SaveChangesAsync();

    return Results.Created($"/payment/{payment.Id}", "Payment completed");
});

app.Run();
