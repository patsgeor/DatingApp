using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Humanizer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Middleware;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbcontext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();//για να επιτρεψει αιτησεις απο angular

builder.Services.AddScoped<ITokenService, TokenService>();//dependency injection για το token service
builder.Services.AddScoped<IMemberRepository, MemberRepository>();//dependency injection για το member repository

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

//custom middleware για global error handling
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseCors(policy => policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithOrigins("http://localhost:4200",
                                        "https://localhost:4200"));//angular


app.UseAuthentication();// για να αναγνωριζει το jwt token που στελνει το angular με καθε αιτηση.Το επικυρώνει και δημιουργεί το HttpContext.User
app.UseAuthorization();// για να επιτρεπει η οχι την προσβαση σε καθε endpoint αναλογα με το αν το endpoint έχει [Authorize] και αν ειναι authenticated ο χρηστης. 
app.MapControllers();

//============================== Seed data=============================//
// Δημιουργούμε ένα scope από τον ServiceProvider του app.
// Ένα scope ζει για λίγο και μετά πεθαίνει → τέλειο για χρήση DbContext.
using var scope = app.Services.CreateScope();

// Παίρνουμε τον ServiceProvider μέσα από το scope.
// Μέσω αυτού θα ζητήσουμε services (όπως DbContext, Logger, κτλ).
var service = scope.ServiceProvider;

try
{
    // Ζητάμε από το DI container να μας δώσει ένα AppDbcontext.
    // Επειδή ο DbContext είναι Scoped, πρέπει να βρίσκεται μέσα σε scope.
    var context = service.GetRequiredService<AppDbcontext>();

    // Εφαρμόζει ΟΛΕΣ τις migration που υπάρχουν
    // και δημιουργεί τη βάση αν δεν υπάρχει.
    await context.Database.MigrateAsync();
    
    // Τρέχει τη δική σου μέθοδο για seed των Users στο database.
    await Seed.SeedUsers(context);
}
catch (Exception ex)
{
    // Αν προκύψει Exception, ζητάμε ένα Logger<Program> από τα services.
    var logger = service.GetRequiredService<ILogger<Program>>();

    // Καταγράφουμε το error με μήνυμα.
    logger.LogError(ex, "An error occurred during migration on program.cs");
}
//============================== End of Seed data=============================//


app.Run();
