using System;
using System.ComponentModel;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikeRepository(AppDbcontext context) : ILikesRepository
{

    public async Task<IReadOnlyList<string>> GetCurrentMemberLikesIds(string memberId)
    {
        return await context.MemberLikes
                .Where(x => x.SourceMemberId == memberId)
                .Select(x => x.TargetMemberId)
                .ToListAsync();
    }

    public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
    {
        return await context.MemberLikes.FindAsync(sourceMemberId, targetMemberId);

    }

    public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams,string memberId)
    {
        var query = context.MemberLikes.AsQueryable();
        IQueryable<Member> results;

    
        switch (likesParams.Predicate)
        {
            case "liked":
                results =query.Where(m => m.SourceMemberId == memberId).Select(m =>m.TargetMember);
                break;
            case "likedBy":
                results = query.Where(m => m.TargetMemberId == memberId)
                            .Select(m => m.TargetMember);
                break;
            default:
                var likeIds = await GetCurrentMemberLikesIds(memberId);
                results =  query.Where(x => x.TargetMemberId == memberId && likeIds.Contains(x.SourceMemberId))
                                .Select(x => x.SourceMember);
                break;
        }
            return await PaginationHelper.CreateAsync(results, likesParams.PageNumber, likesParams.PageSize);

    }

    public void AddLike(MemberLike memberLike)
    {
        context.MemberLikes.Add(memberLike);

    }

    public void DeleteLike(MemberLike memberLike)
    {
        context.MemberLikes.Remove(memberLike);
    }

    public async Task<bool> SaveAllChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
