using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTLLBB.Models;

namespace HTLLBB.Repository
{
    public interface IForumRepository
    {
        Task<Forum> GetForumByName(string name);
        Task<Forum> GetForumById(int id);
        Task<Boolean> ForumExists(string name);

        Task AddForum(Forum forum, int catId);
        Task UpdForum(Forum forum);
        Task DelForum(int id);
    }
}
