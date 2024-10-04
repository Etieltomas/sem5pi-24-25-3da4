using Sempi5;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.TodoItem
{
    public class TodoItem : Entity<TodoItemId>, IAggregateRoot
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
        public string? Secret { get; set; }     
    }
}