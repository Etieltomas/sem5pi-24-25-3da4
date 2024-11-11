using Sempi5.Domain.Shared;

public enum RoomType
{
    OperatingRoom,
    ConsultationRoom,
    ICU
}

public static class RoomTypeExtensions
    {
        public static RoomType FromString(string roomString)
        {
            roomString = roomString?.Replace(" ", string.Empty).ToLower();
            if (string.IsNullOrEmpty(roomString))
            {
                throw new BusinessRuleValidationException("Room Type cannot be null or empty.");
            }

            switch (roomString)
            {
                case "operatingroom":
                    return RoomType.OperatingRoom;
                case "consultationroom":
                    return RoomType.ConsultationRoom;
                case "icu":
                    return RoomType.ICU;
                default:
                    throw new BusinessRuleValidationException("Room Type does not exist");
            }
        }
    }

