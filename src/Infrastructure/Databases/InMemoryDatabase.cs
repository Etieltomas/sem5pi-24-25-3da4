using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.TodoItem;



namespace Sempi5.Infrastructure.Databases
{
    public class InMemoryDatabase : IDatabase
    {
        public void connectDB(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
        }
    }
}