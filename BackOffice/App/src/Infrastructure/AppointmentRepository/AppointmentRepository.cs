using Sempi5.Domain.AppointmentEntity;
using Sempi5.Domain.RoomEntity;
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

        
    }

}