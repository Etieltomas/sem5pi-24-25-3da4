using Microsoft.AspNetCore.Http.HttpResults;
using Sempi5.Domain.Shared;


namespace Sempi5.Domain.RoomEntity
{
    public class RoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomRepository _repo;

        public RoomService(IRoomRepository repo, IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<List<RoomDTO>> GetAllRooms()
        {
            var list = await _repo.GetAllRooms();  

            if (list == null)
            {
                return null;
            }

            
            List<RoomDTO> listDto = list.ConvertAll(ConvertToDTO);

            return listDto;  
        }

        public async Task<RoomDTO> GetRoomByID(RoomID roomID)
        {
           var room = await _repo.GetRoomByID(roomID);
           return ConvertToDTO(room);
        }

        public async Task<string> GetRoomOperatingStatus(RoomID roomID)
        {
            var room = await _repo.GetRoomByID(roomID);
            return room.RoomStatus.ToString();
        }

        private RoomDTO ConvertToDTO(Room room)
        {
            return new RoomDTO {
                RoomNumber = room.Id.AsLong(),
                Capacity = room.Capacity.AsInt(),  
                AssignedEquipment = room.AssignedEquipment.equipment.Select(e => e.ToString()).ToList(), 
                RoomStatus = room.RoomStatus.ToString(),  
                Slots = room.Slots.Select(slot => slot.ToString()).ToList(), 
                Type = room.Type.Name
            };
        }
    }
}