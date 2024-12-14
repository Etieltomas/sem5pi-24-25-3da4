using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Sempi5.Infrastructure
{
    public class OperationTypeRepository : BaseRepository<OperationType, OperationTypeID> , IOperationTypeRepository
    {
        private readonly DataBaseContext _context;

        public OperationTypeRepository(DataBaseContext context) : base(context.OperationTypes)
        {
            _context = context;
        }

        public async Task<IEnumerable<OperationType>> GetAllAsync()
        {
            return await _context.OperationTypes.Include(o => o.Specialization).ToListAsync();
        }

        public async Task<OperationType> GetOperationTypeById(OperationTypeID id)
        {
            return await _context.OperationTypes
                .Include(o => o.Specialization)
                .FirstOrDefaultAsync(o => o.Id.Equals(id));
        }

        public async Task<OperationType> GetOperationTypeByName(string name)
        {
            var test = new Specialization{ Name = name};

            var pat = await _context.OperationTypes
                .Include(p => p.Specialization)
                .FirstOrDefaultAsync(p => p.Specialization.Name.Equals(test.Name));

            return pat;
        }
    }
}

