using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IBDirect.API.SignalR
{
    [Authorize(Roles = "1,2,3,4,5")]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Context.ConnectionId
            );
            await Clients.Others.SendAsync(
                "UserIsOnline",
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            );

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _tracker.UserDisconnected(
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Context.ConnectionId
            );

            await Clients.All.SendAsync(
                "UserIsOffline",
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            );

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
