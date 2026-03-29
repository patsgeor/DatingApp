using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

public class AppDbcontext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Member>  Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> MemberLikes {get; set;}
    public DbSet<Message> Messages {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);// ειναι για να μην χαθούν οι ρυθμίσεις που έχουμε κάνει στο IdentityDbContext

        modelBuilder.Entity<IdentityRole>().HasData(// για να προσθέσουμε κάποιους ρόλους στη βάση δεδομένων κατά την δημιουργία της
            new IdentityRole {Id="member-id", Name = "Member", NormalizedName = "MEMBER" },
            new IdentityRole {Id="admin-id",Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole {Id="moderator-id", Name = "Moderator", NormalizedName = "MODERATOR" }
        );


        //MemberLike
        modelBuilder.Entity<MemberLike>()
            .HasKey(ml => new {ml.SourceMemberId,ml.TargetMemberId});

        modelBuilder.Entity<MemberLike>()
            .HasOne(X => X.SourceMember)
            .WithMany(X => X.LikedMembers)
            .HasForeignKey(x => x.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<MemberLike>()
            .HasOne(X => X.TargetMember)
            .WithMany(X => X.LikedByMembers)
            .HasForeignKey(x => x.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        //Message
         modelBuilder.Entity<Message>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.MessagesSent)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

         modelBuilder.Entity<Message>()
                .HasOne(x => x.Recipient)
                .WithMany(x => x.MessagesReceived)
                .HasForeignKey(x => x.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

        // Δημιουργούμε έναν ValueConverter για να μετατρέπουμε τα DateTime σε UTC όταν αποθηκεύονται στη βάση 
        // και να τα ορίζουμε ως UTC όταν ανακτώνται από τη βάση
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(), // Όταν ΠΑΕΙ στη βάση να μετατρέπεται σε UTC
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Όταν ΕΡΧΕΤΑΙ από τη βάση να ορίζεται ως UTC, ωστε να έχει και το 'Z' στο τέλος
            );

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : null, // Όταν ΠΑΕΙ στη βάση να μετατρέπεται σε UTC
                v => v.HasValue  ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null // Όταν ΕΡΧΕΤΑΙ από τη βάση να ορίζεται ως UTC, ωστε να έχει και το 'Z' στο τέλος
            );

        // για κάθε πίνακα και για κάθε ιδιότητα που είναι τύπου DateTime ή DateTime? εφαρμόζουμε τον ValueConverter
        foreach ( var entityType in modelBuilder.Model.GetEntityTypes())
        {
           foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) )
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}
