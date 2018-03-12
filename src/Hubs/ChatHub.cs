using System;
using System.Threading.Tasks;
using CommonMark;
using Ganss.XSS;
using HTLLBB.Data;
using HTLLBB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace HTLLBB.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        ConnectionMultiplexer _redis;
        IDatabase _db;
        String  _msgKey = "chatbox_msg", 
                _usrKey = "chatbox_user",
                _timeKey = "chatbox_time";
        long _maxLength = 1024;
        ApplicationDbContext _ctx;

        public ChatHub(IRedisConnection redis, ApplicationDbContext ctx)
        {
            _redis = redis.GetInstance();
            _db = _redis.GetDatabase();
            _ctx = ctx;
        }

        [Authorize]
        public async Task RetrieveMessages()
        {
            var users = await _db.ListRangeAsync(_usrKey);
            var times = await _db.ListRangeAsync(_timeKey);
            var messages = await _db.ListRangeAsync(_msgKey);

            for (int i = 0; i < users.Length && i < messages.Length; ++i)
            {
                String userName = users[i].ToString();
                var user = await _ctx.Users.SingleOrDefaultAsync(u => u.UserName == userName);
                await Clients.All.InvokeAsync("Send", userName, user.AvatarPath, times[i].ToString(), messages[i].ToString());
            }
        }

        [Authorize]
        public async Task<Task> Send(string message)
        {
            long n = _db.ListLength(_usrKey);

            var curTime = DateTime.UtcNow.ToString();

            await _db.ListRightPushAsync(_usrKey, Context.User.Identity.Name);
            await _db.ListRightPushAsync(_timeKey, curTime);
            await _db.ListRightPushAsync(_msgKey, message);

            if (n >= _maxLength)
            {
                await _db.ListLeftPopAsync(_usrKey);
                await _db.ListLeftPopAsync(_timeKey);
                await _db.ListLeftPopAsync(_msgKey);
            }

            String userName = Context.User.Identity.Name;
            var user = await _ctx.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            return Clients.All.InvokeAsync("Send", userName, user.AvatarPath, curTime, message);
        }
    }
}
