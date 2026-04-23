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
public class LikesController(IUnitOfWork uow) : BaseApiController
{
    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult> ToggleLike(string targetMemberId)
    {
        var memberId = User.GetMemberId();

        if (memberId == targetMemberId) return BadRequest("you can not do like to yourself!");

        var existingLike = await uow.LikesRepository.GetMemberLike(memberId, targetMemberId);

        if (existingLike != null)
        {
            uow.LikesRepository.DeleteLike(existingLike);
        }
        else
        {
            MemberLike memberLike = new MemberLike
            {
                SourceMemberId = memberId,
                TargetMemberId = targetMemberId
            };

            uow.LikesRepository.AddLike(memberLike);
        }

        if (await uow.Completed()) return Ok();

        return BadRequest("Failed to update like!");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberList()
    {
        return Ok( await uow.LikesRepository.GetCurrentMemberLikesIds(User.GetMemberId()));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<Member>>> GetMemberList([FromQuery] LikesParams likesParams)
    {
        var members = await uow.LikesRepository.GetMemberLikes(likesParams,User.GetMemberId());

        return Ok(members);
    }
}
}

