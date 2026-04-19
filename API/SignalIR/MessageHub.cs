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
public class MessageHub (IMessageRepository messageRepository, 
                        IMemberRepository memberRepository,
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

        var messages = await messageRepository.GetMessageThread(GetUserId(), otherUserId);

        await Clients.Group(groupName).SendAsync("GetMessageThread",messages);
       
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(CreateMessageDto createMessage)
    {
        
        var senderId= GetUserId();
        var recipientId=createMessage.RecipientId;
        if(senderId== recipientId ) throw new HubException("You can not to send message to yourself!");

        var sender= await memberRepository.GetMemberByIdAsync(senderId);
        var recipient= await memberRepository.GetMemberByIdAsync(recipientId);

        if (sender==null || recipient ==null) throw new HubException("Error - Can not found Sender or Recipint ");

        var message =new Message
        {
            SenderId=senderId,
            RecipientId=recipientId,
            Content=createMessage.Content            
        };
        
        var groupName= GetGroupName(senderId,recipientId);
        var group= await messageRepository.GetMessageGroup(groupName);
        bool userInGroup=group != null && group.Connections.Any(x => x.UserId==recipientId);

        if(userInGroup)
        {
            message.DateRead= DateTime.UtcNow;
        }

        messageRepository.AddMessage(message);

        if(await memberRepository.SaveAllAsync())
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
        var group = await messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId,GetUserId());

        if(group == null)
        {
            group= new Group(groupName);
            messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);
        return await messageRepository.SaveAllChange();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await messageRepository.RemoveConnection(Context.ConnectionId);
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
