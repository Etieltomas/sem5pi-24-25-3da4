using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class OperationType : Entity<OperationTypeID>, IAggregateRoot
    {
        public string Name { get; set; }
        public Specialization Specialization { get; set; }

    }

}

