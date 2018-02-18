using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTLLBB.Models;

namespace HTLLBB.Repository
{
    public interface IPostRepository
    {
        Task<Thread> GetThreadById(int threadId);
        Task<Post> GetPostById(int id);

        Task AddPost(Post post, int threadId);
        Task UpdPost(Post post);
        Task DelPost(int id);
        Task DelThread(int id);
    }
}
