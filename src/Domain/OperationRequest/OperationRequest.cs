using System;
using Sempi5.Domain.Patient;
using Sempi5.Domain.Shared;
using Sempi5.Domain.Staff;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class OperationRequest : Entity<OperationRequestID>, IAggregateRoot
    {
        public PatientID PatientId { get; set; }
        public StaffID StaffId { get; set; }
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


