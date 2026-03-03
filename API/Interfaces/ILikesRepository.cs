using System;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    // για να παρουμε το like αν ο χρηστης εχει κανει like σε καποιον αλλο χρηστη
    Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);

    //για να παρουμε ολους τους χρηστες που εχουν κανει like σε εναν χρηστη 
    // ή ολους τους χρηστες που εχουν λαβει like απο εναν χρηστη
    // ή τα κοινα mutual likes μεταξυ δυο χρηστων 
    // αναλογα με το τι θα του δωσουμε σαν παραμετρο στο predicate
    Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams,string memberId);

    // για να παρουμε ολα τα ids των χρηστων που εχουν κανει like σε εναν χρηστη
    Task<IReadOnlyList<string>> GetCurrentMemberLikesIds(string memberId);

    void DeleteLike(MemberLike memberLike);
    void AddLike(MemberLike memberLike);
    Task<bool> SaveAllChanges();
}
