using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.RoomTypeEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

/**
 * RoomTypeRepository.cs created by Ricardo Guimarães on 10/12/2024
 */
namespace Sempi5.Infrastructure.RoomTypeRepository
{
    public class RoomTypeRepository : BaseRepository<RoomType, RoomTypeID>, IRoomTypeRepository
    {
        private readonly DataBaseContext _context;

        public RoomTypeRepository(DataBaseContext context) : base(context.RoomTypes)
        {
            _context = context;
        }

        public async Task<RoomType> GetByNameAsync(string name)
        {

            return await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Name.Equals(name));
        }
    }
}

