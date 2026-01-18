using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var memberId=User.GetMemberId();
            if (memberId ==null) return BadRequest("oops - member id not found in token!");

            var member =await memberRepository.GetMemberForUpdateByIdAsync(memberId);

            if(member==null) return BadRequest("oops - not found member!");

            member.DisplayName=memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description=memberUpdateDto.Description ?? member.Description;
            member.Country=memberUpdateDto.Country ?? member.Country;
            member.City=memberUpdateDto.City ?? member.City;
            
            member.User.DisplayName=memberUpdateDto.DisplayName ?? member.User.DisplayName;

            memberRepository.Update(member);// ακομα και να  μην έχει καμία αλλαγη να φαινεται σαν αλλαγμένο

            if (await memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user!");
        }

    }//end class
}// end namespace
