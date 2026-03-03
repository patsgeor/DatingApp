using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{

 [Authorize]
public class LikesController(ILikesRepository likesRepository) : BaseApiController
{
    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult> ToggleLike(string targetMemberId)
    {
        var memberId = User.GetMemberId();

        if (memberId == targetMemberId) return BadRequest("you can not do like to yourself!");

        var existingLike = await likesRepository.GetMemberLike(memberId, targetMemberId);

        if (existingLike != null)
        {
            likesRepository.DeleteLike(existingLike);
        }
        else
        {
            MemberLike memberLike = new MemberLike
            {
                SourceMemberId = memberId,
                TargetMemberId = targetMemberId
            };

            likesRepository.AddLike(memberLike);
        }

        if (await likesRepository.SaveAllChanges()) return Ok();

        return BadRequest("Failed to update like!");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberList()
    {
        return Ok( await likesRepository.GetCurrentMemberLikesIds(User.GetMemberId()));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<Member>>> GetMemberList([FromQuery] LikesParams likesParams)
    {
        var members = await likesRepository.GetMemberLikes(likesParams,User.GetMemberId());

        return Ok(members);
    }
}
}

