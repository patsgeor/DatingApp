using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetMemberByIdAsync(string id);
    Task<Member?> GetMemberForUpdateByIdAsync(string id);
    Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);
    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
    void Update(Member member);

}
