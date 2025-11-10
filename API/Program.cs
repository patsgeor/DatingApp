using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbcontext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();//για να επιτρεψει αιτησεις απο angular

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(policy => policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithOrigins("http://localhost:4200",
                                        "https://localhost:4200"));//angular

app.MapControllers();

app.Run();
