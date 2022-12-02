using Microsoft.EntityFrameworkCore;
using LoginService;
using LoginService.Model;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LoginContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapPost("/register", async (User user, LoginContext db) =>
{
    await db.Users.AddAsync(user);
    await db.SaveChanges();

    return Results.Created("/login", "Success!!!")
});

app.Run();
