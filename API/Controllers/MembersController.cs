using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   [Authorize]
    public class MembersController(AppDbcontext context) : BaseApiController
    {
        [HttpGet]//https://localhost:5001/api/members
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await context.Users.ToListAsync();
            return members;
        }//end GetMembers

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetMember(String id)   //https://localhost:5001/api/members/1 || localhost:5000/api/members?id=bob123
        {
            var member = await context.Users.FindAsync(id);

            if (member == null){ return NotFound(); }
            return member;
        }//end GetMember

    }//end class
}// end namespace
