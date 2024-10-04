using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain;
using Sempi5.Domain.TodoItem;

namespace Sempi5.Infrastructure.TodoItemRepository
{
    public class TodoItemEntityTypeConfiguration : IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> builder)
        {
            builder.ToTable("TodoItems");
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Name)
                .IsRequired();

            builder.Property(t => t.IsComplete)
                .HasDefaultValue(false);
        }
    }
}
