using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IBDirect.API.SignalR
{
    [Authorize(Roles = "1,2,3,4,5")]
    public class PresenceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User.Identity.Name);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.Identity.Name);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
