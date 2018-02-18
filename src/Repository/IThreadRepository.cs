using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTLLBB.Models;

namespace HTLLBB.Repository
{
    public interface IThreadRepository
    {
        Task<Thread> GetThreadByTitle(string title);
        Task<Thread> GetThreadById(int id);
        Task<Boolean> ThreadExists(string title);

        Task AddThread(Thread thread);
        Task UpdThread(Thread thread);
        Task DelThread(int id);
    }
}
