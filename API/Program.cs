using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Humanizer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Middleware;
using API.Helpers;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using API.SignalIR;



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
builder.Services.AddScoped<LogUserActivity>();//dependency injection για το log user activity action filter
builder.Services.AddScoped<IPhotoService, PhotoService>();//dependency injection για το photo service
builder.Services.AddScoped<ILikesRepository, LikeRepository>();//dependency injection για το LikeRepository
builder.Services.AddScoped<IMessageRepository, MessageRepository>();//dependency injection για το MessageRepository
builder.Services.AddSignalR();//για να προσθέσουμε υποστήριξη για SignalR (real-time communication)

builder.Services.AddSingleton<PresenceTracker>();

// ------------------------------------------------------------------------------
//  Identity configuration
builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.Password.RequireNonAlphanumeric = false;// για να μην απαιτείται ειδικός χαρακτήρας στο password
    options.User.RequireUniqueEmail = true;// για να απαιτείται μοναδικό email
})
.AddRoles<IdentityRole>() // για να προσθέσουμε υποστήριξη για ρόλους (π.χ. admin, moderator, κτλ)
.AddEntityFrameworkStores<AppDbcontext>();// για να χρησιμοποιήσουμε το AppDbcontext για να αποθηκεύσουμε τα δεδομένα του Identity (users, roles, κτλ)
// ------------------------------------------------------------------------------


// ------------------------------------------------------------------------------

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] 
            ?? throw new Exception("TokenKey not found in configuration - program.cs");
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true, // ελέγχει αν το token έχει υπογραφεί με το σωστό κλειδί
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)), // το κλειδί που χρησιμοποιείται για την υπογραφή του token
           ValidateIssuer = false, // δεν ελέγχει τον εκδότη του token (π.χ. το domain που το δημιούργησε)
           ValidateAudience = false // δεν ελέγχει το κοινό του token (π.χ. ποιος είναι ο αποδέκτης του token)
        };
        
        // Αυτό το κομμάτι είναι για να επιτρέψουμε στο SignalR να λαμβάνει το JWT token από το Query String, 
        // επειδή το SignalR δεν μπορεί να στείλει headers όπως το Authorization header.
        options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context => 
                {
                    // 1. Λήψη του token από το Query String
                    var accessToken = context.Request.Query["access_token"];// bydefault, το SignalR client στέλνει το token με το query parameter "access_token"
                    // 2. Έλεγχος της διαδρομής (Path)
                    var path = context.HttpContext.Request.Path;
                    
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        // 3. Χειροκίνητη απόδοση του token στο Context
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
    });
// ------------------------------------------------------------------------------

builder.Services.AddAuthorizationBuilder()
.AddPolicy("RequireAdminRole", p => p.RequireRole("Admin"))
.AddPolicy("ModeratePhotoRole", p => p.RequireRole("Admin","Moderator"));


builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


var app = builder.Build();

//custom middleware για global error handling
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseCors(policy => policy.AllowAnyHeader()  // για να επιτρέψει όλα τα headers που στέλνει το angular (π.χ. Authorization header με το jwt token)
                            .AllowAnyMethod() // για να επιτρέψει όλα τα headers και όλες τις μεθόδους (GET, POST, κτλ) από το angular
                            .AllowCredentials() // για να επιτρέψει την αποστολή cookies από το angular
                            .WithOrigins("http://localhost:4200",
                                        "https://localhost:4200"));//angular


app.UseAuthentication();// για να αναγνωριζει το jwt token που στελνει το angular με καθε αιτηση.Το επικυρώνει και δημιουργεί το HttpContext.User
app.UseAuthorization();// για να επιτρεπει η οχι την προσβαση σε καθε endpoint αναλογα με το αν το endpoint έχει [Authorize] και αν ειναι authenticated ο χρηστης. 
app.MapControllers();

app.MapHub<PresenceHub>("hubs/presence");// για να ορίσουμε το endpoint για το PresenceHub του SignalR (real-time communication)
app.MapHub<MessageHub>("hubs/message");

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
    var userManager = service.GetRequiredService<UserManager<AppUser>>();

    // Εφαρμόζει ΟΛΕΣ τις migration που υπάρχουν
    // και δημιουργεί τη βάση αν δεν υπάρχει.
    await context.Database.MigrateAsync();
    await context.Connections.ExecuteDeleteAsync(); // Διαγράφει όλες τις εγγραφές από τον πίνακα Connections (για το SignalR presence tracking) κάθε φορά που τρέχει η εφαρμογή, για να ξεκινάει με καθαρό slate.
    
    // Τρέχει τη δική σου μέθοδο για seed των Users στο database.
    await Seed.SeedUsers(userManager);
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
