using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.EntityFrameworkCore;

namespace HTLLBB.Repository
{
    public class EFForumRepository : IForumRepository
    {
        readonly ApplicationDbContext _ctx;

        public EFForumRepository(ApplicationDbContext ctx)
            => _ctx = ctx;

        public async Task AddForum(Forum forum, int catId)
        {
            var cat = await _ctx.Categories.FindAsync(catId);
            cat?.Forums.Add(forum);
            await _ctx.SaveChangesAsync();
        }

        public async Task DelForum(int id)
        {
            _ctx.Remove(await GetForumById(id));
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> ForumExists(string name)
            => await _ctx.Forums.AnyAsync(f => f.Name == name);

        public async Task<Forum> GetForumById(int id)
            => await _ctx.Forums.FindAsync(id);

        public async Task<Forum> GetForumByName(string name)
        {
            var forums = _ctx.Forums
                             .Include(f => f.Category)
                             .Include(f => f.Threads)
                                .ThenInclude(t => t.Posts)
                                .ThenInclude(p => p.Author);

            return await forums.SingleOrDefaultAsync(f => f.Name == name);
        }

        public async Task UpdForum(Forum forum)
        {
            _ctx.Update(forum);
            await _ctx.SaveChangesAsync();
        }
    }
}
