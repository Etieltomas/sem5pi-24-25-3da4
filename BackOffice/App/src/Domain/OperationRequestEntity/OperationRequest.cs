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

        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual List<StaffID>? Staffs { get; set; }

        public void UpdatePriority(Priority newPriority)
        {
            Priority = newPriority;
        }

        public void UpdateDeadline(Deadline newDeadline)
        {
            Deadline = newDeadline;
        }

        public void UpdateStatus(Status status){
            Status = status;
        }
        public void MarkAsDeleted()
        {
        Status = Status.Cancelled;
        }
        public void SetStartAndEndDate(DateTime startDate, DateTime endDate)
        {
        if (endDate <= startDate)
        {
        throw new ArgumentException("EndDate must be greater than StartDate.");
        }
        StartDate = startDate;
        EndDate = endDate;

        Status = Status.scheduled;
        }
    }
}


