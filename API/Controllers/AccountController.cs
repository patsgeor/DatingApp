using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) :BaseApiController
{
    [HttpPost("register")]//api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email.ToLower(),
            DisplayName = registerDto.DisplayName,
            UserName = registerDto.Email.ToLower(),
            Member= new Member
            {
                DisplayName= registerDto.DisplayName,
                Gender= registerDto.Gender,
                City= registerDto.City,
                Country= registerDto.Country,
                DateOfBirth = registerDto.DateOfBirth
            }
        };

        var result =await userManager.CreateAsync(user,registerDto.Password); 
        
        if(!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Identity-",error.Description);
            }
            return ValidationProblem();
        }

        await userManager.AddToRoleAsync(user,"Member");

        await SetRefreshTokenCookie(user);  // δημιουργία και αποθήκευση του refresh token στο cookie

        return await user.ToDto(tokenService);

    }//register

    [HttpPost("login")]//api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return Unauthorized("Invalid email");//error 401 δηλαδη χωρις εξουσιοδότηση

        var result= await userManager.CheckPasswordAsync(user,loginDto.Password);
        if(!result)   return Unauthorized("Invalid password");   

        await SetRefreshTokenCookie(user);// δημιουργία και αποθήκευση του refresh token στο cookie

        return await user.ToDto(tokenService);
    }//login
    
    //other methods
    //------------------------------------------------------------------------------- 
    [HttpPost("refresh-token")]//api/account/refresh-token
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"]; // ανάκτηση του refresh token από το cookie
        if (string.IsNullOrEmpty(refreshToken)) return NoContent();

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken 
                                                            && u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null)
        {
            return Unauthorized("Invalid or expired refresh token");
        }

        await SetRefreshTokenCookie(user); // δημιουργία και αποθήκευση νέου refresh token στο cookie

        return await user.ToDto(tokenService);
    }

    private async Task SetRefreshTokenCookie(AppUser appUser)
    {
        //1 δημιουργία του refresh token
        var refreshToken= tokenService.GenerateRefreshToken(); // δημιουργία του refresh token

        //2 το αποθηκεύουμε στη βάση δεδομένων
        appUser.RefreshToken=refreshToken; 
        appUser.RefreshTokenExpiry=DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(appUser);

        //3 ρυθμίζουμε το cookie για να αποθηκεύσουμε το refresh token στο browser του χρήστη
        var cookieOptions = new CookieOptions
        {
            HttpOnly=true,// για να μην μπορεί να προσπελαστεί από JavaScript (π.χ. σε περίπτωση XSS επίθεσης)
            Secure= true,// για να αποστέλλεται μόνο μέσω HTTPS
            SameSite = SameSiteMode.Strict,// για να μην αποστέλλεται σε αιτήσεις από άλλες ιστοσελίδες (π.χ. σε περίπτωση CSRF επίθεσης)
            Expires= DateTime.UtcNow.AddDays(7)  // για να λήγει μετά από 7 ημέρες (πρέπει να είναι το ίδιο με το expiry του refresh token στη βάση δεδομένων)          
        };

        //3 αποθήκευση του refresh token στο cookie
        Response.Cookies.Append("refreshToken",refreshToken,cookieOptions); 
    }  
 
}//Account