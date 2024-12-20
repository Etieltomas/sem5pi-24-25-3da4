
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.TokenEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.RoomRepository
{
    public class RoomRepository : BaseRepository<Room, RoomID>, IRoomRepository
    {
        private readonly DataBaseContext _context;

        public RoomRepository(DataBaseContext context) : base(context.Rooms)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllRooms()
        {
            return await _context.Rooms
                .Include(room => room.Type)
                .ToListAsync();
        }        

        public async Task<Room> GetRoomByID(RoomID roomId)
        {
            return await _context.Rooms
                .Include(room => room.Type)
                .FirstOrDefaultAsync(r => r.Id.Equals(roomId));
        }
    }

}