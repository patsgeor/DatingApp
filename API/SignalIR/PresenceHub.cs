using System;
using System.Security.Claims;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalIR;

[Authorize]
public class PresenceHub (PresenceTracker presenceTracker): Hub
{
    public override async Task OnConnectedAsync()
    {
        // προσθέτουμε τον χρήστη στη λίστα των online χρηστών όταν συνδεθεί στο hub
        await presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);
        // ενημερώνουμε όλους τους άλλους χρήστες ότι ο συγκεκριμένος χρήστης είναι online
        await Clients.Others.SendAsync("UserOnline",GetUserId());

        // επιστρέφουμε στον χρήστη που συνδέθηκε τη λίστα με τα id των online χρηστών
        var OnlineUsersId = await presenceTracker.GetOnlineUsersId();
        await Clients.Caller.SendAsync("GetOnlineUsersId",OnlineUsersId);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        await presenceTracker.UserDisconnected(GetUserId(),Context.ConnectionId);
        
        await Clients.Others.SendAsync("UserOffline",GetUserId());
        
        // επιστρέφουμε στον χρήστη που συνδέθηκε τη λίστα με τα id των online χρηστών
        // var OnlineUsersId = await presenceTracker.GetOnlineUsersId();
        // await Clients.Caller.SendAsync("GetOnlineUsersId",OnlineUsersId);

        await base.OnDisconnectedAsync(exception);// εδω καλουμε την βασικη υλοποιηση για να μην χαλασει η λειτουργια του SignalR
    }
    

    private string GetUserId()
    {
        return Context.User?.GetMemberId()   ?? throw new HubException("Can not get the MemberId");
    }

}
