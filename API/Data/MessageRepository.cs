using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository(AppDbcontext context) : IMessageRepository
{
    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
       context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return  await context.Groups
                        .Include(g => g.Connections)
                        .Where(g => g.Connections.Any(c => c.ConnectionId==connectionId))
                        .FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessage(string messageId)
    {
        return await context.Messages.FindAsync(messageId);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await  context.Groups
                        .Include(g => g.Connections)
                        .Where(g => g.Name==groupName)
                        .FirstOrDefaultAsync();
    }

    public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
    {
        var query  = context.Messages.OrderByDescending(x => x.DateSent).AsQueryable();

        query = messageParams.Container switch
        {
            "Outbox" => query.Where(m => m.SenderId==messageParams.MemberId && m.SenderDeleted==false),
            _=> query.Where(m => m.RecipientId== messageParams.MemberId && m.RecipientDeleted== false)
        };

        var messageQuery= query.Select(MessageExtensions.ToDtoProjection());

        return await PaginationHelper.CreateAsync(messageQuery,messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
    {
        // 1. Μαρκάρισμα των αδιάβαστων μηνυμάτων ως διαβασμένα χρησιμοποιώντας άμεση ενημέρωση
        await context.Messages.Where(m => m.RecipientId==currentMemberId 
                                    && m.SenderId==recipientId
                                    && m.DateRead==null)
                                .ExecuteUpdateAsync(set => set.SetProperty( m => m.DateRead,DateTime.UtcNow));

        // 2. Ανάκτηση του ιστορικού συνομιλίας φιλτράροντας τα διαγραμμένα μηνύματα
         return await context.Messages
            .Where(x => 
                x.RecipientId == currentMemberId && x.SenderId == recipientId && x.RecipientDeleted == false || 
                x.SenderId == currentMemberId && x.RecipientId == recipientId && x.SenderDeleted == false)
            .OrderBy(x => x.DateSent)
            .Select(MessageExtensions.ToDtoProjection())
            .ToListAsync();
    }

    public async Task RemoveConnection(string connectionId)
    {
        await context.Connections
                .Where(c => c.ConnectionId==connectionId)
                .ExecuteDeleteAsync();        
    }


}
