using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IBDirect.API.SignalR
{
    [Authorize(Roles = "1,2,3,4,5")]
    public class PresenceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync(
                "UserIsOnline",
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            );
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync(
                "UserIsOffline",
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            );

            await base.OnDisconnectedAsync(exception);
        }
    }
}
