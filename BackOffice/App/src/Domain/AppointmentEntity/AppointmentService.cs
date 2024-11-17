using Sempi5.Domain.Shared;


namespace Sempi5.Domain.AppointmentEntity
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo, IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }
    }
}