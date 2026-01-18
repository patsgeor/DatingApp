using API.Entities;

namespace API.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetMemberByIdAsync(string id);
    Task<Member?> GetMemberForUpdateByIdAsync(string id);
    Task<IReadOnlyList<Member>> GetMembersAsync();
    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
    Task<bool> SaveAllAsync();
    void Update(Member member);

}
