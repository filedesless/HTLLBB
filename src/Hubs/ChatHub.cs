using System;
using System.Threading.Tasks;
using HTLLBB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace HTLLBB.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        ConnectionMultiplexer _redis;
        IDatabase _db;
        String _msgKey = "chatbox_msg", _usrKey = "chatbox_user";
        long _maxLength = 1024;

        public ChatHub(IRedisConnection redis)
        {
            _redis = redis.GetInstance();
            _db = _redis.GetDatabase();
        }

        [Authorize]
        public async Task RetrieveMessages()
        {
            var users = await _db.ListRangeAsync(_usrKey);
            var messages = await _db.ListRangeAsync(_msgKey);

            for (int i = 0; i < users.Length && i < messages.Length; ++i)
                await Clients.All.InvokeAsync("Send", users[i].ToString(), messages[i].ToString());
        }

        [Authorize]
        public async Task<Task> Send(string message)
        {
            long n = _db.ListLength(_usrKey);

            await _db.ListRightPushAsync(_usrKey, Context.User.Identity.Name);
            await _db.ListRightPushAsync(_msgKey, message);

            if (n >= _maxLength)
            {
                await _db.ListLeftPopAsync(_usrKey);
                await _db.ListLeftPopAsync(_msgKey);
            }

            return Clients.All.InvokeAsync("Send", Context.User.Identity.Name, message);
        }
    }
}
