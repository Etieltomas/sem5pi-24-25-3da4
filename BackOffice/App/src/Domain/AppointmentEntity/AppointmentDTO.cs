using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.AppointmentEntity {
    public class AppointmentDTO {
        public long? Id { get; set; }
        public string? AppointmentStatus  { get; set; }
        public string? DateOperation  { get; set; }
        public string? AppointmentType { get; set; }
        public long? OperationRequest { get; set; }
        public long? Room { get; set; }
        public List<string>? Team { get; set; }
    }
}