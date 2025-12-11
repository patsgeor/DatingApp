using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   [Authorize]
    public class MembersController(IMemberRepository memberRepository) : BaseApiController
    {
        [HttpGet]//https://localhost:5001/api/members
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var members = await memberRepository.GetMembersAsync();
            return Ok(members);
        }//end GetMembers

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(String id)   //https://localhost:5001/api/members/1 || localhost:5000/api/members?id=bob123
        {
            var member = await memberRepository.GetMemberByIdAsync(id);

            if (member == null){ return NotFound(); }
            return member;
        }//end GetMember

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            var photos = await memberRepository.GetPhotosForMemberAsync(id);

            return Ok(photos);
        }

    }//end class
}// end namespace
