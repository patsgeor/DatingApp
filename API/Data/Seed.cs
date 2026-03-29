using System.Security.Cryptography;
using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{

    public static async Task SeedUsers(UserManager<AppUser> userManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var listUsers = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

        if (listUsers == null){
            Console.WriteLine("no data found in seedData users");
             return;
        }


        foreach (var user in listUsers)
        {
            var newUser = new AppUser
            {
                Id = user.Id,
                Email = user.Email.ToLower(),
                DisplayName = user.DisplayName,
                UserName = user.Email.ToLower(),
                ImageUrl = user.ImageUrl,
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
            var result= await userManager.CreateAsync(newUser,"Pa$$w0rd");
            if(!result.Succeeded) Console.WriteLine(result.Errors.First().Description);
            
            result = await userManager.AddToRoleAsync(newUser,"Member");
            if(!result.Succeeded) Console.WriteLine(result.Errors.First().Description);

        }
        
        //δημιουργία και ενος Admin
        var admin=new AppUser{
            Id="Admin-id",
            DisplayName="Admin",
            Email="admin@test.com",
            UserName="admin@test.com",
        };
        await userManager.CreateAsync(admin,"Pa$$w0rd");
        await userManager.AddToRolesAsync(admin,["Admin","Moderator"]);

    }
}

