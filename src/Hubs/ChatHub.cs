using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonMark;
using Ganss.XSS;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HTLLBB.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IClient>
    {
        ApplicationDbContext _context;
        readonly int blockNumber = 1024;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task RetrieveMessages(int channelId)
        {
            var channel = await _context.ChatboxChannels
                                    .Include(c => c.Blocks)
                                        .ThenInclude(b => b.Author)
                                    .Include(c => c.Blocks)
                                        .ThenInclude(b => b.Messages)
                                    .FirstAsync(c => c.ID == channelId);

            var sanitizer = new HtmlSanitizer();

            await Clients.All.Send(
                channel.Blocks
                .OrderBy(block => block.TimeStamp)
                .Select(block => new ChatBlock(
                    block.Author.UserName,
                    block.Author.AvatarPath,
                    block.TimeStamp,
                    block.Messages.Select(message => sanitizer.Sanitize(CommonMarkConverter.Convert(message.Content)))
                ))
            );
        }

        [Authorize]
        public async Task Send(string message, int channelId)
        {
            var channel = await _context.ChatboxChannels
                                    .Include(c => c.Blocks)
                                        .ThenInclude(b => b.Author)
                                    .Include(c => c.Blocks)
                                        .ThenInclude(b => b.Messages)
                                    .FirstAsync(c => c.ID == channelId);
            
            String userName = Context.User.Identity.Name;
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
            var curTime = DateTime.UtcNow;

            var newBlock = new ChatboxMessageBlock 
            {
                Author = user,
                TimeStamp = curTime,
                Messages = new List<ChatboxMessage>
                    {
                        new ChatboxMessage
                        {
                            Content = message,
                        }
                    }
            };

            if (channel.Blocks.Count < 1)
                channel.Blocks.Add(newBlock);
            else 
            {
                var lastBlock = channel.Blocks.Last();

                bool closeInTime = curTime - lastBlock.TimeStamp < TimeSpan.FromMinutes(5);

                if (lastBlock.Author.Id == user.Id && closeInTime)
                    lastBlock.Messages.Add(new ChatboxMessage
                    {
                        Content = message,
                    });
                else
                    channel.Blocks.Add(newBlock);
            }

            if (channel.Blocks.Count > blockNumber)
                channel.Blocks.RemoveRange(0, blockNumber / 2);

            await _context.SaveChangesAsync();

            await RetrieveMessages(channelId);
        }
    }

    public interface IClient
    {
        Task Send(IEnumerable<ChatBlock> blocks);
    }

    public class ChatBlock
    {
        public ChatBlock(String userName, String avatarPath, DateTime timeStamp, IEnumerable<String> messages)
        {
            UserName = userName;
            AvatarPath = avatarPath;
            TimeStamp = timeStamp;
            Messages = messages;
        }

        public String UserName;
        public String AvatarPath;
        public DateTime TimeStamp;
        public IEnumerable<String> Messages;
    }
}
