
using Sempi5.Domain.TodoItem;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.TodoItemRepository
{
    public class TodoItemRepository : BaseRepository<TodoItem, TodoItemId>, ITodoItemRepository
    {
    
        public TodoItemRepository(TodoContext context):base(context.TodoItems)
        {
           
        }


    }
}