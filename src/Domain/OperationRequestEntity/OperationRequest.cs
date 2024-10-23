using System;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class OperationRequest : Entity<OperationRequestID>, IAggregateRoot
    {
        public Patient Patient { get; set; }
        public Staff Staff { get; set; }
        public OperationType OperationType { get; set; }
        public Priority Priority { get; set; }
        public Deadline Deadline { get; set; }
        public Status Status { get; set; }

        public void UpdatePriority(Priority newPriority)
        {
            Priority = newPriority;
        }

        public void UpdateDeadline(Deadline newDeadline)
        {
            Deadline = newDeadline;
        }

    }
}


