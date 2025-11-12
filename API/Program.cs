using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Humanizer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbcontext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();//για να επιτρεψει αιτησεις απο angular

builder.Services.AddScoped<ITokenService, TokenService>();//dependency injection για το token service

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] 
            ?? throw new Exception("TokenKey not found in configuration - program.cs");
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
           ValidateIssuer = false,
           ValidateAudience = false
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(policy => policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithOrigins("http://localhost:4200",
                                        "https://localhost:4200"));//angular


app.UseAuthentication();// για να αναγνωριζει το jwt token που στελνει το angular με καθε αιτηση.Το επικυρώνει και δημιουργεί το HttpContext.User
app.UseAuthorization();// για να επιτρεπει η οχι την προσβαση σε καθε endpoint αναλογα με το αν το endpoint έχει [Authorize] και αν ειναι authenticated ο χρηστης. 
app.MapControllers();

app.Run();
