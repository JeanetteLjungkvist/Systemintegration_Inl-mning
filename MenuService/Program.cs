using System.Text;
using MenuService;
using MenuService.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MenuContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

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

});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MenuContext>();
    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/menu", async(MenuContext ctx) => {
    
    return await ctx.Menus.ToListAsync();
});

// Admin kan lägga till ett menu item

app.MapPost("/menu", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")] async (Menu name, MenuContext ctx) => {
    
    ctx.Menus.Add(name);
    await ctx.SaveChangesAsync();

    return Results.Created($"/menu/{name.Id}", name);
});

// Admin kan ändra på ett existerande menu item

app.MapPut("/menu/{Id}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")] async (string id, Menu name, MenuContext ctx) => {

    var guidId = new Guid(id);

    var menuItem = await ctx.Menus.FindAsync(guidId);

    if(menuItem == null) return Results.NotFound("Menu item was not found");

    menuItem.Name = name.Name;
    menuItem.Category = name.Category;
    menuItem.Allergen = name.Allergen;
    menuItem.Price = name.Price;
    menuItem.Available = name.Available;

    await ctx.SaveChangesAsync();

    return Results.Ok("Menu item has been updated");
});

// Admin kan radera en menu item

app.MapDelete("/menu/{Id}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")] async (Guid Id, MenuContext ctx) => {

var name = await ctx.Menus.FindAsync(Id);

if(name == null) return Results.NotFound();

ctx.Menus.Remove(name);
await ctx.SaveChangesAsync();

return Results.Ok("Menu item removed");

});

app.Run();