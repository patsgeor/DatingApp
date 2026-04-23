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
public class MessagesController (IUnitOfWork uow): BaseApiController
{

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessage)
    {
        var senderId= User.GetMemberId();
        var recipientId=createMessage.RecipientId;
        if(senderId== recipientId ) BadRequest("You can not to send message to yourself!");

        var sender= await uow.MemberRepository.GetMemberByIdAsync(senderId);
        var recipient= await uow.MemberRepository.GetMemberByIdAsync(recipientId);

        if (sender==null || recipient ==null) BadRequest("Error - Can not found Sender or Recipint ");

        var message =new Message
        {
            SenderId=senderId,
            RecipientId=recipientId,
            Content=createMessage.Content            
        };

        uow.MessageRepository.AddMessage(message);

        if(await uow.Completed()) return Ok(message.ToDto());

        return BadRequest("Something gone wrong! Failed to send message!");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessageForMember([FromQuery] MessageParams messageParams)
    {
        messageParams.MemberId=User.GetMemberId();

        var messages= await uow.MessageRepository.GetMessagesForMember(messageParams);
        return Ok(messages);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
    {
        var currentMemberId= User.GetMemberId();
        var messages = await uow.MessageRepository.GetMessageThread(currentMemberId, recipientId);
        return Ok(messages);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(string id)
    {
        var memberId = User.GetMemberId();
        var message = await uow.MessageRepository.GetMessage(id);
        if (message== null) return BadRequest("Not found message.Failed to delete!");

        if(message.SenderId==memberId && !message.SenderDeleted)    message.SenderDeleted = true;
        if(message.RecipientId==memberId && !message.RecipientDeleted)    message.RecipientDeleted = true;

        if(message.RecipientDeleted == true && message.SenderDeleted == true)//== if( message is {message.RecipientDeleted : true , message.SenderDeleted : true})
        {
            uow.MessageRepository.DeleteMessage(message);
        }

        if(await uow.Completed()) return NoContent();

        return BadRequest("Problem deleting the message!");     
    }

}
