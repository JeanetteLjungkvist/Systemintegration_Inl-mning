using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LoginService;
using LoginService.Model;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LoginContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LoginContext>();
    db.Database.Migrate();
}

app.MapPost("/register", async (User user, LoginContext ctx) =>
{
    await ctx.Users.AddAsync(user);
    await ctx.SaveChangesAsync();

    return Results.Created("/login", "Success!!!");
});

app.MapPost("/login", async (UserLogin userLogin, LoginContext ctx) =>
{
    var user = await ctx.Users.FirstOrDefaultAsync(user => user.Email.Equals(userLogin.Email) && user.Password.Equals(userLogin.Password));

    if (user == null) return Results.NotFound("Wrong username or password!");

    var secretKey = builder.Configuration["Jwt:Key"];

    if (secretKey == null) return Results.StatusCode(500);

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var token = new JwtSecurityToken
    (
        issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(15),
        notBefore: DateTime.UtcNow,
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256)
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(tokenString);

});

app.Run();
