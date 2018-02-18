using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.EntityFrameworkCore;

namespace HTLLBB.Repository
{
    public class EFThreadRepository : IThreadRepository
    {
        readonly ApplicationDbContext _ctx;

        public EFThreadRepository(ApplicationDbContext ctx)
            => _ctx = ctx;

        public async Task AddThread(Thread thread)
        {
            _ctx.Add(thread);
            await _ctx.SaveChangesAsync();
        }

        public async Task DelThread(int id)
        {
            _ctx.Remove(await GetThreadById(id));
            await _ctx.SaveChangesAsync();
        }

        public async Task<Thread> GetThreadById(int id)
            => await _ctx.Thread
                         .Include(t => t.Forum)
                         .SingleOrDefaultAsync(t => t.ID == id);

        public async Task<Thread> GetThreadByTitle(string title)
            => await _ctx.Thread
                              .Include((Thread t) => t.Forum)
                                .ThenInclude((Forum f) => f.Category)
                              .Include((Thread t) => t.Posts)
                                .ThenInclude((Post p) => p.Author)
                              .SingleOrDefaultAsync(t => t.Title == title);

        public async Task<bool> ThreadExists(string title)
            => await _ctx.Thread.AnyAsync(t => t.Title == title);


        public async Task UpdThread(Thread thread)
        {
            _ctx.Update(thread);
            await _ctx.SaveChangesAsync();
        }
    }
}
