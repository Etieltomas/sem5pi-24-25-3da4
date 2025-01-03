using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.SpecializationRepository
{
    /**
     * SpecializationRepository.cs created by Ricardo Guimarães on 10/12/2024
     */
    public class SpecializationRepository : BaseRepository<Specialization, SpecializationID>, ISpecializationRepository
    {
        private readonly DataBaseContext _context;

        public SpecializationRepository(DataBaseContext context) : base(context.Specializations)
        {
            _context = context;
        }

        public async Task<Specialization> GetByName(string name)
        {

            return await _context.Specializations.FirstOrDefaultAsync(rt => rt.Name.Equals(name));
        }

        public Task<List<Specialization>> SearchSpecializations(string? name, string? code, string? description, int page, int pageSize)
        {
            var query = _context.Specializations.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(rt => rt.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(rt => rt.Code.Equals(code));
            }

            if (!string.IsNullOrEmpty(description) )
            {
                query = query.Where(rt => rt.Description != null && rt.Description.Contains(description));
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
