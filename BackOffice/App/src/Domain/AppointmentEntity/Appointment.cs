using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.AppointmentEntity {
    public class Appointment : Entity<AppointmentID>, IAggregateRoot{
        public required AppointmentStatus AppointmentStatus  { get; set; }
        public required DateOperation DateOperation  { get; set; }
        public required AppointmentType AppointmentType { get; set; }
        public required OperationRequest OperationRequest { get; set; }

        public required Room Room { get; set; }
    }
}