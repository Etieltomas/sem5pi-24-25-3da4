using Sempi5.Domain.Shared;


namespace Sempi5.Domain.RoomEntity
{
    public interface IRoomRepository : IRepository<Room, RoomID>
    {
        Task<List<Room>> GetAllRooms();
        Task<Room> GetRoomByID(RoomID room);
    }
}