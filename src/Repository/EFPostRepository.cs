using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.EntityFrameworkCore;

namespace HTLLBB.Repository
{
    public class EFPostRepository : IPostRepository
    {
        readonly ApplicationDbContext _ctx;

        public EFPostRepository(ApplicationDbContext ctx) => 
            _ctx = ctx;

        public async Task AddPost(Post post, int threadId)
        {
            Thread thread = await GetThreadById(threadId);
            thread?.Posts.Add(post);
            await _ctx.SaveChangesAsync();
        }

        public async Task DelPost(int id)
        {
            _ctx.Remove(await GetPostById(id));
            await _ctx.SaveChangesAsync();
        }

        public async Task<Thread> GetThreadById(int threadId)
            => await _ctx.Thread
                         .Include(t => t.Posts)
                         .SingleOrDefaultAsync(t => t.ID == threadId);

        public async Task<Post> GetPostById(int id)
            => await _ctx.Posts
                         .Include(p => p.Thread)
                            .ThenInclude(t => t.Forum)
                         .SingleOrDefaultAsync(m => m.ID == id);

        public async Task UpdPost(Post post)
        {
            _ctx.Update(post);
            await _ctx.SaveChangesAsync();
        }

        public async Task DelThread(int id)
        {
            _ctx.Remove(await GetThreadById(id));
            await _ctx.SaveChangesAsync();
        }
    }
}
