using Sempi5.Domain.Shared;

public enum RoomStatus
{
    Available,
    Occupied,
    UnderMaintenance
}

public static class RoomStatusExtensions
    {
        public static RoomStatus FromString(string roomString)
        {
            roomString = roomString?.Replace(" ", string.Empty).ToLower();
            if (string.IsNullOrEmpty(roomString))
            {
                throw new BusinessRuleValidationException("Room Status cannot be null or empty.");
            }

            switch (roomString)
            {
                case "available":
                    return RoomStatus.Available;
                case "occupied":
                    return RoomStatus.Occupied;
                case "undermaintenance":
                    return RoomStatus.UnderMaintenance;
                default:
                    throw new BusinessRuleValidationException("Room Status does not exist");
            }
        }
    }
