using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

namespace Sempi5.Domain.RoomEntity {
    public class Room : Entity<RoomID>, IAggregateRoot{
        public Capacity Capacity  { get; set; }
        public AssignedEquipment AssignedEquipment  { get; set; }
        public RoomStatus RoomStatus  { get; set; }
        public List<Slot> Slots  { get; set; }
        public RoomType Type  { get; set; }
    }
}