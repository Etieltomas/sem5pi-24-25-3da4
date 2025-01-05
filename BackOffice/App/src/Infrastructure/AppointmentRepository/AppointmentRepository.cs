using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.AppointmentEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.AppointmentRepository
{
    /// <summary>
    ///  @author Simão Lopes
    ///  @date 1/12/2024
    /// Repository for managing appointment data operations.
    /// </summary>
    public class AppointmentRepository : BaseRepository<Appointment, AppointmentID>, IAppointmentRepository
    {
        private readonly DataBaseContext _context;

        public AppointmentRepository(DataBaseContext context) : base(context.Appointments)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsByStaff(Staff staff)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Room)
                .Include(a => a.OperationRequest)
                .Where(a => a.AppointmentStatus == AppointmentStatus.Scheduled)
                .ToListAsync(); // Fetch data from the database first

            return appointments
                .Where(a => a.OperationRequest.Staffs.Contains(staff.Id)) // Perform in-memory filtering
                .ToList();
            
        }

        public async Task<List<Appointment>> GetAppointmentsByRoom(Room room)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Room)
                .Include(a => a.OperationRequest)
                .Where(a => a.AppointmentStatus == AppointmentStatus.Scheduled)
                .ToListAsync(); // Fetch data from the database first

            return appointments
                .Where(a => a.Room.Id.AsLong() == room.Id.AsLong()) // Perform in-memory filtering
                .ToList();
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctor(string doctorEmail)
        {
            var list = await _context.Appointments
                .Include(a => a.Room)
                .Include(a => a.OperationRequest)
                .ThenInclude(o => o.Staff)
                .Where(a => a.AppointmentStatus == AppointmentStatus.Scheduled && a.OperationRequest.Staff.Email.Equals(new Email(doctorEmail)))
                .ToListAsync();

            return list;
        }

        public async Task<Appointment> GetAppointmentsByID(AppointmentID id)
        {
            return await _context.Appointments
            .Include(a => a.Room)
            .Include(a => a.OperationRequest)
                .ThenInclude(o => o.Staff)
            .Include(a => a.OperationRequest)
                .ThenInclude(o => o.OperationType)
            .FirstOrDefaultAsync(a => a.Id == id);

        }
    }

}