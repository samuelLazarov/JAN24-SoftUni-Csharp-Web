using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Data.Configuration;

namespace TaskBoard.Data
{
    public class TaskBoardAppDbContext : IdentityDbContext
    {
        public TaskBoardAppDbContext(DbContextOptions<TaskBoardAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new BoardConfiguration());
            builder.ApplyConfiguration(new TaskConfiguration());


            base.OnModelCreating(builder);  
        }

        public DbSet<Board> Boards { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}