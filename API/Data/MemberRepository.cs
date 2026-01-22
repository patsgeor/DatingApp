using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbcontext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await context.Members.FindAsync(id);
    }

    public async Task<IReadOnlyList<Member>> GetMembersAsync()
    {
        return await context.Members.ToListAsync();    }

    public async Task<Member?> GetMemberForUpdateByIdAsync(string id)
    {
        return await context.Members
                        .Include(u => u.User)
                        .Include(u => u.Photos)
                        .SingleOrDefaultAsync(u => u.Id==id);
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        //return await context.Photos.Where(p=> p.MemberId==memberId).ToListAsync();
        return await context.Members.Where(m => m.Id==memberId).SelectMany(m =>m.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        //το χρησιμοποιούμε για να πούμε στο entity framework οτι το συγκεκριμένο αντικείμενο έχει τροποποιηθεί 
        // ακομα και να μην εχουμε κανει αλλαγες στις ιδιοτητες του 
        // ωστε να το καταλάβει και να το ενημερώσει στην βάση δεδομένων όταν καλέσουμε SaveChangesAsync
        context.Entry(member).State=EntityState.Modified;
    }
}
