using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Shared;



namespace Sempi5.Infrastructure.Databases
{
    public class InMemoryDatabase : IDatabase
    {
        public void connectDB(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DataBaseContext>(opt => opt.UseInMemoryDatabase("InMemoryDatabase"));
        }
    }
}