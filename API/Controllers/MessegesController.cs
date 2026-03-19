using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController (IMemberRepository memberRepository , IMessageRepository messageRepository): BaseApiController
{

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessage)
    {
        var senderId= User.GetMemberId();
        var recipientId=createMessage.RecipientId;
        if(senderId== recipientId ) BadRequest("You can not to send message to yourself!");

        var sender= await memberRepository.GetMemberByIdAsync(senderId);
        var recipient= await memberRepository.GetMemberByIdAsync(recipientId);

        if (sender==null || recipient ==null) BadRequest("Error - Can not found Sender or Recipint ");

        var message =new Message
        {
            SenderId=senderId,
            RecipientId=recipientId,
            Content=createMessage.Content            
        };

        messageRepository.AddMessage(message);

        if(await memberRepository.SaveAllAsync()) return Ok(message.ToDto());

        return BadRequest("Something gone wrong! Failed to send message!");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessageForMember([FromQuery] MessageParams messageParams)
    {
        messageParams.MemberId=User.GetMemberId();

        var messages= await messageRepository.GetMessagesForMember(messageParams);
        return Ok(messages);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
    {
        var currentMemberId= User.GetMemberId();
        var messages = await messageRepository.GetMessageThread(currentMemberId, recipientId);
        return Ok(messages);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(string id)
    {
        var memberId = User.GetMemberId();
        var message = await messageRepository.GetMessage(id);
        if (message== null) return BadRequest("Not found message.Failed to delete!");

        if(message.SenderId==memberId && !message.SenderDeleted)    message.SenderDeleted = true;
        if(message.RecipientId==memberId && !message.RecipientDeleted)    message.RecipientDeleted = true;

        if(message.RecipientDeleted == true && message.SenderDeleted == true)//== if( message is {message.RecipientDeleted : true , message.SenderDeleted : true})
        {
            messageRepository.DeleteMessage(message);
        }

        if(await messageRepository.SaveAllChange()) return NoContent();

        return BadRequest("Problem deleting the message!");     
    }

}
