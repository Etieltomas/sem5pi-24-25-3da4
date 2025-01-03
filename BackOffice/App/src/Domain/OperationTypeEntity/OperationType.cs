using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;

namespace Sempi5.Domain.OperationRequestEntity
{
    /**
     * OperationType.cs created by Ricardo Guimarães on 10/12/2024
     */
    public class OperationType : Entity<OperationTypeID>, IAggregateRoot
    {
        public virtual string Name { get; set; }
        public virtual int Anesthesia_Duration { get; set; }
        public virtual int Surgery_Duration { get; set; }
        public virtual int Cleaning_Duration { get; set; }
        public virtual Specialization Specialization { get; set; }

    }

}

