using EFCore.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new BlogEntityTypeConfiguration().Configure(modelBuilder.Entity<Blog>());

            modelBuilder.Entity<Post>()
                .HasData(new Post { Id = 1, Title = "Test" });

        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
