using Sempi5.Domain.Shared;

namespace Sempi5.Domain.SpecializationEntity
{
    public interface ISpecializationRepository : IRepository<Specialization,SpecializationID>
    {

        Task AddAsync(Specialization specialization);
        Task<List<Specialization>> GetAllAsync();
        Task<Specialization?> GetByIdAsync(SpecializationID id);
        Task UpdateAsync(Specialization specialization);
        Task<Specialization?> GetByName(string name);
    }
}