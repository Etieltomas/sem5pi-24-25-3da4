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

        /// <summary>
        /// Retrieves all rooms and converts them to RoomDTO objects.
        /// @author Tomás Leite
        /// @date 30/11/2024
        /// </summary>
        /// <returns>A list of RoomDTO objects, or null if no rooms exist.</returns>
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

        /// <summary>
        /// Retrieves a room by its ID and converts it to a RoomDTO object.
        /// @author Tomás Leite
        /// @date 30/11/2024
        /// </summary>
        /// <param name="roomID">The ID of the room to retrieve.</param>
        /// <returns>The RoomDTO object representing the room.</returns>
        public async Task<RoomDTO> GetRoomByID(RoomID roomID)
        {
           var room = await _repo.GetRoomByID(roomID);
           return ConvertToDTO(room);
        }

        /// <summary>
        /// Retrieves the operating status of a room by its ID.
        /// @author Tomás Leite
        /// @date 30/11/2024
        /// </summary>
        /// <param name="roomID">The ID of the room.</param>
        /// <returns>The operating status of the room as a string.</returns>
        public async Task<string> GetRoomOperatingStatus(RoomID roomID)
        {
            var room = await _repo.GetRoomByID(roomID);
            return room.RoomStatus.ToString();
        }

        /// <summary>
        /// Converts a Room object to a RoomDTO object.
        /// @author Tomás Leite
        /// @date 30/11/2024
        /// </summary>
        /// <param name="room">The Room object to convert.</param>
        /// <returns>The converted RoomDTO object.</returns>
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
