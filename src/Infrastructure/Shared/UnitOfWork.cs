using System.Threading.Tasks;
using Sempi5.Domain;
using Sempi5.Domain.Shared;
using Sempi5.Domain.TodoItem;

namespace Sempi5.Infrastructure.Shared
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TodoContext _context;

        public UnitOfWork(TodoContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}