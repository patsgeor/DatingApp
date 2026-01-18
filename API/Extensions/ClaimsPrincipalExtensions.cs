using System;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetMemberId(this ClaimsPrincipal user)
    {
        // επιστρέφει το τιμη του claim με όνομα ClaimTypes.NameIdentifier
        // αν δεν βρεθεί πετάει εξαίρεση καθώς δεν μπορεί να συνεχίσει χωρίς το member id αφου ειναι athorized
        return user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new Exception("Cannot get member id from claims - token");
    }

}
