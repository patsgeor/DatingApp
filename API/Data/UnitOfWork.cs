using System;
using System.Data.Common;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UnitOfWork (AppDbcontext context): IUnitOfWork
{
    private IMemberRepository? _memberRepository;
    private ILikesRepository? _likesRepository;
    private IMessageRepository? _messageRepository;

    public IMemberRepository MemberRepository => _memberRepository??=new MemberRepository(context);

    public ILikesRepository LikesRepository => _likesRepository ??= new LikeRepository(context);

    public IMessageRepository MessageRepository => _messageRepository ??= new MessageRepository(context);

    public async Task<bool> Completed()
    {
        try
        {
            return await context.SaveChangesAsync()>0;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occurred while saving changes", ex);
        }
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
