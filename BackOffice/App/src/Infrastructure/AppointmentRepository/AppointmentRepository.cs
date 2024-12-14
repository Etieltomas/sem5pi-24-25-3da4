using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.AppointmentEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.AppointmentRepository
{
    public class AppointmentRepository : BaseRepository<Appointment, AppointmentID>, IAppointmentRepository
    {
        private readonly DataBaseContext _context;

        public AppointmentRepository(DataBaseContext context) : base(context.Appointments)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsByStaff(Staff staff)
        {
            return await _context.Appointments.Where(a => a.OperationRequest.Staffs.Contains(staff) &&
                                                          a.AppointmentStatus != AppointmentStatus.Scheduled).ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByRoom(Room room)
        {
            return await _context.Appointments.Where(a => a.Room.Id.AsLong() == room.Id.AsLong() &&
                                                a.AppointmentStatus != AppointmentStatus.Scheduled).ToListAsync();
        }
    }

}