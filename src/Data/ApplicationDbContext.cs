using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HTLLBB.Models;
using Microsoft.AspNetCore.Identity;

namespace HTLLBB.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Forum>()
                   .HasIndex((Forum f) => f.Name)
                   .IsUnique();

            builder.Entity<Thread>()
                   .HasIndex((Thread t) => t.Title)
                   .IsUnique();

        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<ChatboxChannel> ChatboxChannels { get; set; }
        public DbSet<ChatboxMessage> ChatboxMessages { get; set; }
    }
}
