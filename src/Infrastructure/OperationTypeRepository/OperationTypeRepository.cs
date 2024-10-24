using Sempi5.Domain.OperationRequestEntity;
using System.Collections.Generic;
using System.Linq;

namespace Sempi5.Infrastructure
{
    public class OperationTypeRepository : IOperationTypeRepository
    {
        private readonly List<OperationType> _operationTypes = new List<OperationType>();

        public Task AddAsync(OperationType operationType)
        {
            _operationTypes.Add(operationType);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<OperationType>> GetAllAsync()
        {
             return Task.FromResult<IEnumerable<OperationType>>(_operationTypes);
        }

        public async Task<OperationType> GetOperationTypeById(OperationTypeID id)
        {
            return _operationTypes.FirstOrDefault(o => o.Id.Equals(id));
        }
    }
}

