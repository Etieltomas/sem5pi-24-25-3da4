using Sempi5.Domain.Shared;

/// <summary>
/// Enumeration representing the status of a room.
/// @auth Tomás Leite
/// @date 30/11/2024
/// </summary>
public enum RoomStatus
{
    Available,
    Occupied,
    UnderMaintenance
}

/// <summary>
/// Extension methods for the RoomStatus enum.
/// Provides functionality to convert string values to RoomStatus enum values.
/// @auth Tomás Leite
/// @date 30/11/2024
/// </summary>
public static class RoomStatusExtensions
{
    /// <summary>
    /// Converts a string to a corresponding RoomStatus enum value.
    /// Removes spaces and converts to lowercase for case-insensitive matching.
    /// @auth Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    /// <param name="roomString">The string representing the room status.</param>
    /// <returns>The RoomStatus enum value corresponding to the string.</returns>
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
