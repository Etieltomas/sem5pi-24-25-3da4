using Sempi5.Domain.OperationRequestEntity;
using System.Collections.Generic;
using System.Linq;

namespace Sempi5.Infrastructure
{
    public class OperationTypeRepository : IOperationTypeRepository
    {
        private readonly List<OperationType> _operationTypes = new List<OperationType>();

        public OperationType GetOperationTypeById(OperationTypeID id)
        {
            return _operationTypes.FirstOrDefault(o => o.Id.Equals(id));
        }
    }
}

