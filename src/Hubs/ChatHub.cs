using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HTLLBB.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        [Authorize]
        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", Context.User.Identity.Name, message);
        }
    }
}
