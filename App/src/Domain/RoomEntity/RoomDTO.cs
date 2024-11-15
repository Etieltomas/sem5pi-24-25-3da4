namespace Sempi5.Domain.RoomEntity {
    public class RoomDTO {
        public long RoomNumber { get; set; }
        public int Capacity { get; set; }
        public List<string> AssignedEquipment { get; set; }
        public string RoomStatus { get; set; }
        public List<string> MaintenanceSlot { get; set; }
        public string Type { get; set; }
    }
}