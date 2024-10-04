
using Sempi5;
using Sempi5.Domain;
using Sempi5.Domain.Shared;
using Sempi5.Domain.TodoItem;

namespace Sempi5.Infrastructure.TodoItemRepository
{
    public interface ITodoItemRepository: IRepository<TodoItem, TodoItemId>
    {
    }
}