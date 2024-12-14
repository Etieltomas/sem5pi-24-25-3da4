
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.UserEntity;
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
    
            public async Task AddAsync(Specialization specialization)
            {
                await _context.Specializations.AddAsync(specialization);
                await _context.SaveChangesAsync();
            }
    
            public async Task<Specialization?> GetByIdAsync(SpecializationID id)
            {
                return await _context.Specializations.FindAsync(id);
            }

        public Task<Specialization?> GetByName(string name)
        {
            return _context.Specializations.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task UpdateAsync(Specialization specialization)
            {
                _context.Specializations.Update(specialization);
                await _context.SaveChangesAsync();
            }
        }

        
    }
