using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.SpecializationRepository
{
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
    }
}
