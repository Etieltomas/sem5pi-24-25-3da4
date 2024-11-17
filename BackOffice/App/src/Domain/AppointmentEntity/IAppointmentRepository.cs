using Sempi5.Domain.Shared;


namespace Sempi5.Domain.AppointmentEntity
{
    public interface IAppointmentRepository : IRepository<Appointment, AppointmentID>
    {
    }
}