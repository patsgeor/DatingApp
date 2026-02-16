using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

public class AppDbcontext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Member>  Members { get; set; }
    public DbSet<Photo> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);// ειναι για να μην χαθούν οι ρυθμίσεις που έχουμε κάνει στο IdentityDbContext

        // Δημιουργούμε έναν ValueConverter για να μετατρέπουμε τα DateTime σε UTC όταν αποθηκεύονται στη βάση 
        // και να τα ορίζουμε ως UTC όταν ανακτώνται από τη βάση
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(), // Όταν ΠΑΕΙ στη βάση να μετατρέπεται σε UTC
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Όταν ΕΡΧΕΤΑΙ από τη βάση να ορίζεται ως UTC, ωστε να έχει και το 'Z' στο τέλος
            );
        
        // για κάθε πίνακα και για κάθε ιδιότητα που είναι τύπου DateTime ή DateTime? εφαρμόζουμε τον ValueConverter
        foreach ( var entityType in modelBuilder.Model.GetEntityTypes())
        {
           foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }
    }
}
