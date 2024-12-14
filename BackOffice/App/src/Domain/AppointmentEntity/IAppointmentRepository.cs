using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;


namespace Sempi5.Domain.AppointmentEntity
{
    public interface IAppointmentRepository : IRepository<Appointment, AppointmentID>
    {
        Task<List<Appointment>> GetAppointmentsByStaff(Staff staff);
        Task<List<Appointment>> GetAppointmentsByRoom(Room room);
    }
}