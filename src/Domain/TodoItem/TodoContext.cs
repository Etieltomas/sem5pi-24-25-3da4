using Microsoft.EntityFrameworkCore;
using Sempi5.Infrastructure.TodoItemRepository;

namespace Sempi5.Domain.TodoItem
{
    public class TodoContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }

        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TodoItemEntityTypeConfiguration());
        }
        
    }
}