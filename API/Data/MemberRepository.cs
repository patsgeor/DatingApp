using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbcontext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await context.Members.FindAsync(id);
    }

public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
{
    var query = context.Members.AsQueryable();
    query = query.Where(x => x.Id != memberParams.CurrentMemberId);

    if (!string.IsNullOrEmpty(memberParams.Gender)) query = query.Where(x => x.Gender == memberParams.Gender);

    var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge - 1));
    var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge));

    query = query.Where(x => x.DateOfBirth <= minDob && x.DateOfBirth >= maxDob);

    query = memberParams.OrderBy switch
    {
        "created" => query.OrderByDescending(x => x.Created),
        _ =>query.OrderByDescending(x => x.LastActive) 
    };

    
    
    return await PaginationHelper.CreateAsync(query, memberParams.PageNumber, memberParams.PageSize);
}

    public async Task<Member?> GetMemberForUpdateByIdAsync(string id)
    {
        return await context.Members
                        .Include(u => u.User)
                        .Include(u => u.Photos)
                        .SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        //return await context.Photos.Where(p=> p.MemberId==memberId).ToListAsync();
        return await context.Members.Where(m => m.Id == memberId).SelectMany(m => m.Photos).ToListAsync();
    }



    public void Update(Member member)
    {
        //το χρησιμοποιούμε για να πούμε στο entity framework οτι το συγκεκριμένο αντικείμενο έχει τροποποιηθεί 
        // ακομα και να μην εχουμε κανει αλλαγες στις ιδιοτητες του 
        // ωστε να το καταλάβει και να το ενημερώσει στην βάση δεδομένων όταν καλέσουμε SaveChangesAsync
        context.Entry(member).State = EntityState.Modified;
    }
}
