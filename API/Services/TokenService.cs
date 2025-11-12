using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService( IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        // Ανάκτηση του TokenKey από το configuration
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot find TokenKey in configuration.");

        // Ελεγχος μήκους του TokenKey για ασφάλεια
        if (tokenKey.Length < 64)
        {
            throw new Exception("TokenKey must be at least 64 characters long for security reasons.");
        }

        // Create symmetric security key
        var _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        // δημιουργία claims για το χρήστη 
        var claims = new List<Claim>
        {
            new Claim (ClaimTypes.Email, user.Email),
            new Claim (ClaimTypes.NameIdentifier, user.Id),
            new Claim("custom-claim", "custom-value")
        };

        // δημιουργία των credentials χρησιμοποιώντας το κλειδί και τον αλγόριθμο HMAC SHA512
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        // περιγραφή του token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.Now.AddDays(7),// DateTime.UtcNow.AddMinutes(7),// ορισμός λήξης του token για παραγωγικό περιβάλλον
            SigningCredentials = creds
        };

        // δημιουργία του token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
