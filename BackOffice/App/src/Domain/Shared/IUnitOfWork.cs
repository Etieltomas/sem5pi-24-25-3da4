using System.Threading.Tasks;

namespace Sempi5.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();

        void MarkAsModified<TEntity>(TEntity entity) where TEntity : class;
    }
}