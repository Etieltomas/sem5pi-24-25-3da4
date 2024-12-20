using Sempi5.Domain.Shared;

namespace Sempi5.Domain.SpecializationEntity
{
    public interface ISpecializationRepository : IRepository<Specialization, SpecializationID>
    {
        Task<Specialization> GetByName(string name);
        Task<List<Specialization>> SearchSpecializations(string? name, string? code, string? description, int page, int pageSize);
    }
}
