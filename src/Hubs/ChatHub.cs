using System;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HTLLBB.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        ApplicationDbContext _ctx;
        readonly int messageNumber = 1024;

        public ChatHub(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [Authorize]
        public async Task RetrieveMessages(int channelId)
        {
            var channel = await _ctx.ChatboxChannels
                                    .Include(c => c.Messages)
                                        .ThenInclude(m => m.Author)
                                    .FirstAsync(c => c.ID == channelId);

            foreach (var message in channel.Messages) 
                await Clients.All.InvokeAsync(
                    "Send",
                    message.Author.UserName,
                    message.Author.AvatarPath,
                    message.Timestamp.ToString(),
                    message.Content
                );
        }

        [Authorize]
        public async Task Send(string message, int channelId)
        {
            var channel = await _ctx.ChatboxChannels
                                    .Include(c => c.Messages)
                                    .FirstAsync(c => c.ID == channelId);
            
            String userName = Context.User.Identity.Name;
            var user = await _ctx.Users.SingleOrDefaultAsync(u => u.UserName == userName);
            var curTime = DateTime.UtcNow;

            channel.Messages.Add(new ChatboxMessage
            {
                Author = user,
                Timestamp = curTime,
                Content = message,
            });

            if (channel.Messages.Count > messageNumber)
                channel.Messages.RemoveRange(0, messageNumber / 2);

            await _ctx.SaveChangesAsync();

            await Clients.All.InvokeAsync("Send", userName, user.AvatarPath, curTime.ToString(),  message);
        }
    }
}
