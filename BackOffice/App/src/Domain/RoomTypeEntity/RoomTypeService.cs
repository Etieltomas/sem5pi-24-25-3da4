using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.RoomTypeEntity;

namespace Sempi5.Domain.RoomTypeEntity
{
    public class RoomTypeService
    {
        private readonly IRoomTypeRepository _repo;

        public RoomTypeService(IRoomTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<RoomTypeDTO> AddRoomType(RoomTypeDTO newRoomTypeDto)
        {
            var existingRoomType = await _repo.GetByNameAsync(newRoomTypeDto.Name);

            if (existingRoomType != null)
            {
                return null; 
            }

            var roomType = new RoomType(newRoomTypeDto.Name);
            await _repo.AddAsync(roomType);

            return new RoomTypeDTO { Name = roomType.Name };
        }

        public async Task<List<RoomTypeDTO>> GetAllRoomTypes()
        {
            var roomTypes = await _repo.GetAllAsync();
            return roomTypes.ConvertAll(rt => new RoomTypeDTO { Name = rt.Name });
        }
    }
}

