using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.TokenEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.RoomRepository
{
    /// <summary>
    /// Repository class responsible for managing Room entities in the database.
    /// @author Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    public class RoomRepository : BaseRepository<Room, RoomID>, IRoomRepository
    {
        private readonly DataBaseContext _context;

        public RoomRepository(DataBaseContext context) : base(context.Rooms)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all rooms from the database, including their room type information.
        /// @author Tomás Leite
        /// @date 30/11/2024
        /// </summary>
        /// <returns>A list of Room objects.</returns>
        public async Task<List<Room>> GetAllRooms()
        {
            return await _context.Rooms
                .Include(room => room.Type)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific room by its ID from the database, including its room type information.
        /// @author Tomás Leite
        /// @date 30/11/2024
        /// </summary>
        /// <param name="roomId">The ID of the room to retrieve.</param>
        /// <returns>The Room object corresponding to the given room ID, or null if not found.</returns>
        public async Task<Room> GetRoomByID(RoomID roomId)
        {
            return await _context.Rooms
                .Include(room => room.Type)
                .FirstOrDefaultAsync(r => r.Id.Equals(roomId));
        }
    }
}
