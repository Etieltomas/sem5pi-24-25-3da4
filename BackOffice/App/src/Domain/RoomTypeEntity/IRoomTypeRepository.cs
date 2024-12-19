using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.Shared;
namespace Sempi5.Domain.RoomTypeEntity
{
    public interface IRoomTypeRepository : IRepository<RoomType, RoomTypeID>
    {
        Task<RoomType> GetByNameAsync(string name);
    }
}