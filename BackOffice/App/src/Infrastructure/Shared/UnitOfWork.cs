using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain;
using Sempi5.Domain.Shared;
using Sempi5.Infrastructure.Databases;

namespace Sempi5.Infrastructure.Shared
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataBaseContext _context;

        public UnitOfWork(DataBaseContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }

        public void MarkAsModified<TEntity>(TEntity entity) where TEntity : class
        {
           _context.Entry(entity).State = EntityState.Modified;
        }
    }
}