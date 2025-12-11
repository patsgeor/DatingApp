using System.Security.Cryptography;
using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{

    public static async Task SeedUsers(AppDbcontext context)
    {
        if (await context.Users.AnyAsync()) return;

        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var listUsers = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

        if (listUsers == null){
            Console.WriteLine("no data found in seedData users");
             return;
        }


        foreach (var user in listUsers)
        {
        using var hmac = new HMACSHA512();

            var newUser = new AppUser
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                ImageUrl = user.ImageUrl,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key,
                Member = new Member
                {
                    Id = user.Id,
                    DateOfBirth = user.DateOfBirth,
                    ImageUrl = user.ImageUrl,
                    DisplayName = user.DisplayName,
                    Created = user.Created,
                    LastActive = user.LastActive,
                    Gender = user.Gender,
                    Description = user.Description,
                    City = user.City,
                    Country = user.Country
                }
            };

            newUser.Member.Photos.Add(new Photo
            {
                Url = user.ImageUrl!,
                MemberId = user.Id
            });
            context.Users.Add(newUser);

        }
        await context.SaveChangesAsync();

    }
}

