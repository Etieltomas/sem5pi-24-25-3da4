using System;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class OperationRequest : Entity<OperationRequestID>, IAggregateRoot
    {
        public virtual Patient Patient { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual OperationType OperationType { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual Deadline Deadline { get; set; }
        public virtual Status Status { get; set; }

        public void UpdatePriority(Priority newPriority)
        {
            Priority = newPriority;
        }

        public void UpdateDeadline(Deadline newDeadline)
        {
            Deadline = newDeadline;
        }

        public void MarkAsDeleted()
        {
        Status = Status.Cancelled;
        }
    }
}


