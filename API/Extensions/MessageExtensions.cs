using System;
using System.Linq.Expressions;
using API.DTOs;
using API.Entities;

namespace API.Extensions;

public static class MessageExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto
        {
            Id=message.Id,
            Content=message.Content,
            DateSent=message.DateSent,
            SenderId=message.SenderId,
            SenderDisplayName=message.Sender.DisplayName,
            SenderImageUrl=message.Sender.ImageUrl,
            RecipientId=message.RecipientId,
            RecipientDisplayName=message.Recipient.DisplayName,
            RecipientImageUrl=message.Recipient.ImageUrl,           
            DateRead=message.DateRead
        };
    }

    public static Expression<Func<Message, MessageDto>> ToDtoProjection(){
        return message => new MessageDto
        {
            Id=message.Id,
            Content=message.Content,
            DateSent=message.DateSent,
            SenderId=message.SenderId,
            SenderDisplayName=message.Sender.DisplayName,
            SenderImageUrl=message.Sender.ImageUrl,
            RecipientId=message.RecipientId,
            RecipientDisplayName=message.Recipient.DisplayName,
            RecipientImageUrl=message.Recipient.ImageUrl,           
            DateRead=message.DateRead
        };
    }

}

