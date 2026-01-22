using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   [Authorize]
    public class MembersController(IMemberRepository memberRepository, IPhotoService photoService) : BaseApiController
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

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file) {
            var memberId = User.GetMemberId();
            var member= await memberRepository.GetMemberForUpdateByIdAsync(memberId);
            
            if(member==null) return BadRequest("oops - not found member!");
            
            var result= await photoService.AddPhotoAsync(file);
            if(result.Error!=null) return BadRequest(result.Error.Message);

            var photo= new Photo
            {
                Url= result.SecureUrl.AbsoluteUri,
                PublicId= result.PublicId,
                MemberId= memberId
            };

            if(member.ImageUrl== null)
            {
                member.ImageUrl= photo.Url;
                member.User.ImageUrl= photo.Url;
            }

            member.Photos.Add(photo);
            if (await memberRepository.SaveAllAsync()) return photo;
            return BadRequest("Problem adding photo");            
        }
    
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member= await memberRepository.GetMemberForUpdateByIdAsync(User.GetMemberId());
            if (member==null) return BadRequest("Cannot get member from token");
            var photo = member.Photos.SingleOrDefault(m => m.Id==photoId);

           if(photo==null || member.ImageUrl==photo.Url) return BadRequest("Cannot save this as main image");

            member.ImageUrl=photo.Url;
            member.User.ImageUrl=photo.Url;
            if(await memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("something goes wrong");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult<DeletionResult>> DeletePhoto(int photoId)
        {
            var member= await memberRepository.GetMemberForUpdateByIdAsync(User.GetMemberId());
            if (member==null) return BadRequest("Cannot get member from token");
            var photo = member.Photos.SingleOrDefault(m => m.Id==photoId);

           if(photo==null || member.ImageUrl==photo.Url) return BadRequest("Cannot delete this as main image");

            if(photo.PublicId!=null) {
                var result =await photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error!=null) return BadRequest(result.Error.Message);

                member.Photos.Remove(photo);
                }
            if(await memberRepository.SaveAllAsync()) return Ok();
            return BadRequest("something goes wrong");
        }
    }//end class
}// end namespace
