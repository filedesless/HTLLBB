using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonMark;
using Ganss.XSS;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HTLLBB.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IClient>
    {
        ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly int _blockNumber = 1024;
        protected readonly HtmlSanitizer _sanitizer;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _sanitizer = new HtmlSanitizer();
        }

        String ConvertToMarkdown(String msg) => 
            _sanitizer.Sanitize(CommonMarkConverter.Convert(msg));

        public async Task RetrieveMessages(int channelId)
        {
            var channel = await _context.ChatboxChannels
                                    .Include(c => c.Blocks)
                                        .ThenInclude(b => b.Author)
                                    .Include(c => c.Blocks)
                                        .ThenInclude(b => b.Messages)
                                    .FirstAsync(c => c.ID == channelId);

            var blocks = await Task.WhenAll(channel.Blocks
                .OrderBy(block => block.TimeStamp)
                .Select(async block =>
                        new ChatBlock(
                            block.Author.UserName,
                            block.Author.AvatarPath,
                            block.TimeStamp,
                            block.Messages
                            .ToDictionary(
                                m => m.ID,
                                m => ConvertToMarkdown(m.Content)
                               ),
                            await HasEditRight(block.Messages.First().ID)
                       )
                   )
             );
            
            await Clients.All.Send(blocks);
        }

        public async Task Send(string message, int channelId)
        {
            if (String.IsNullOrWhiteSpace(ConvertToMarkdown(message)))
                return;

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

            if (channel.Blocks.Count > _blockNumber)
                channel.Blocks.RemoveRange(0, _blockNumber / 2);

            await _context.SaveChangesAsync();

            await RetrieveMessages(channelId);
        }

        public async Task DeleteMessage(int channelId, int messageId)
        {
            if (!await HasEditRight(messageId))
                return;

            String userName = Context.User.Identity.Name;;
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            var msg = await _context.ChatboxMessages
                                    .Include(m => m.Block)
                                        .ThenInclude(b => b.Messages)
                                    .SingleOrDefaultAsync(m => m.ID == messageId);

            msg.Block.Messages.Remove(msg);
            
           if (msg.Block.Messages.Count < 1)
                _context.Remove(msg.Block);

            await _context.SaveChangesAsync();

            await RetrieveMessages(channelId);
        }

        public async Task EditMessage(int channelId, int messageId, String message)
        {
            if (String.IsNullOrWhiteSpace(message))
                return;
            
            if (!await HasEditRight(messageId))
                return;

            var msg = await _context.ChatboxMessages
                                    .SingleOrDefaultAsync(m => m.ID == messageId);

            msg.Content = message;

            await _context.SaveChangesAsync();

            await RetrieveMessages(channelId);
        }

        async Task<bool> HasEditRight(int messageId)
        {
            String userName = Context.User.Identity.Name;
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            var msg = await _context.ChatboxMessages
                                    .Include(m => m.Block)
                                        .ThenInclude(b => b.Author)
                                    .SingleOrDefaultAsync(m => m.ID == messageId);

            if (msg.Block.Author.Id == user.Id)
                return true;

            if (await _userManager.IsInRoleAsync(user, Roles.Admin))
                return true;

            return false;
        }
    }

    public interface IClient
    {
        Task Send(IEnumerable<ChatBlock> blocks);
    }

    public class ChatBlock
    {
        public ChatBlock(String userName, String avatarPath, DateTime timeStamp, IDictionary<int, String> messages, bool hasEditRight)
        {
            UserName = userName;
            AvatarPath = avatarPath;
            TimeStamp = timeStamp;
            Messages = messages;
            HasEditRight = hasEditRight;
        }

        public String UserName;
        public String AvatarPath;
        public DateTime TimeStamp;
        public IDictionary<int, String> Messages;
        public bool HasEditRight;
    }
}
