using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalIR;

[Authorize]
public class MessageHub (IUnitOfWork uow,
                        IHubContext<PresenceHub> presenceHub): Hub
{

    
    public override async Task OnConnectedAsync()
    {
        var httpContext= Context.GetHttpContext();
        var otherUserId=httpContext?.Request.Query["userId"].ToString()
                    ?? throw new HubException("Other user not found");

        var groupName =GetGroupName(GetUserId(),otherUserId);

        await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
        await AddToGroup(groupName);

        var messages = await uow.MessageRepository.GetMessageThread(GetUserId(), otherUserId);

        await Clients.Group(groupName).SendAsync("GetMessageThread",messages);
       
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(CreateMessageDto createMessage)
    {
        
        var senderId= GetUserId();
        var recipientId=createMessage.RecipientId;
        if(senderId== recipientId ) throw new HubException("You can not to send message to yourself!");

        var sender= await uow.MemberRepository.GetMemberByIdAsync(senderId);
        var recipient= await uow.MemberRepository.GetMemberByIdAsync(recipientId);

        if (sender==null || recipient ==null) throw new HubException("Error - Can not found Sender or Recipint ");

        var message =new Message
        {
            SenderId=senderId,
            RecipientId=recipientId,
            Content=createMessage.Content            
        };
        
        var groupName= GetGroupName(senderId,recipientId);
        var group= await uow.MessageRepository.GetMessageGroup(groupName);
        bool userInGroup=group != null && group.Connections.Any(x => x.UserId==recipientId);

        if(userInGroup)
        {
            message.DateRead= DateTime.UtcNow;
        }

        uow.MessageRepository.AddMessage(message);

        if(await uow.Completed())
        {
            await Clients.Group(groupName).SendAsync("NewMessage",message.ToDto());
            var connections = await PresenceTracker.GetConnectionForUser(recipientId);

            // αν ειναι συνδεδεμένος σε άλλο tab ή συσκευή δλδ οχι μεσα στο group, στέλνουμε ειδοποίηση για νέο μήνυμα
            if(connections != null && connections.Count > 0 && !userInGroup) 
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", message.ToDto());
            }

        }
    }

    public async Task<bool> AddToGroup(string groupName)
    {
        var group = await uow.MessageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId,GetUserId());

        if(group == null)
        {
            group= new Group(groupName);
            uow.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);
        return await uow.Completed();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await uow.MessageRepository.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private static string GetGroupName(string? caller, string? other)
    {
        return string.CompareOrdinal(caller,other)<0 ? caller+"-"+ other : other+"-"+caller;
    }

    private string GetUserId()
    {
        return Context.User?.GetMemberId()   ?? throw new HubException("Can not get the MemberId");
    }
}
