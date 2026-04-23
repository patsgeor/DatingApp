using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users= await userManager.Users.ToListAsync();
        var userList= new List<object>();

        foreach (var user in users)
        {
            var roles= await userManager.GetRolesAsync(user);
            userList.Add(new{
                user.Id,
                user.Email,
                Roles=roles.ToList()
            });

        }
        return Ok(userList);
    }

[Authorize(Policy ="RequireAdminRole")]
[HttpPost("edit-roles/{userId}")]
public async Task<ActionResult<IList<string>>> EditRoles(string userId,[FromQuery] string roles)
{
    if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

    var rolesSelected= roles.Split(",").ToArray();

    var user= await userManager.FindByIdAsync(userId);
    if(user== null) return BadRequest("not found the user");

    var oldRoles= await userManager.GetRolesAsync(user);

    var result=await userManager.AddToRolesAsync(user,rolesSelected.Except(oldRoles));
    if(!result.Succeeded) return BadRequest("failed to add new roles");

    result=await userManager.RemoveFromRolesAsync(user, oldRoles.Except(rolesSelected));
    if(!result.Succeeded) return BadRequest("failed to remove the old roles");

    return Ok(await userManager.GetRolesAsync(user));
}

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult>  GetPhotosForModeration()
    {
        return await Task.FromResult(Ok("Admins or moderators can see this"));
    }

}
