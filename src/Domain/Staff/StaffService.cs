using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;


namespace Sempi5.Domain.Staff
{
    public class StaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffRepository _repo;
        private readonly ISpecializationRepository _specRepo;

        public StaffService(ISpecializationRepository specRepo, IStaffRepository repo, IUnitOfWork unitOfWork)
        {
            _specRepo = specRepo;
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<StaffDTO> AddStaffMember(StaffDTO staffDTO)
        {
            var availabilitySlots = staffDTO.AvailabilitySlots
                .Select(slot => new AvailabilitySlot(slot))
                .ToList();

            // TODO: Verify this
            var specialization = await _specRepo.GetByIdAsync(new SpecializationID(staffDTO.Specialization.ToLower()));

            var staff = new Staff
            {
                LicenseNumber = new LicenseNumber(staffDTO.LicenseNumber),
                Name = new Name(staffDTO.Name),
                Email = new Email(staffDTO.Email),
                Phone = new Phone(staffDTO.Phone),
                AvailabilitySlots = availabilitySlots,
                Specialization = specialization
            };

            await _repo.AddAsync(staff);
            await _unitOfWork.CommitAsync();

            return ConvertToDTO(staff);
        }

        public async Task<StaffDTO> GetStaffMember(StaffID id)
        {
            var staff = await _repo.GetByIdAsync(id);
            return staff == null ? null : ConvertToDTO(staff);
        }

        public async Task<List<StaffDTO>> GetAllStaffMembers()
        {
            var staffList = await _repo.GetAllAsync();
            return staffList.Select(ConvertToDTO).ToList();
        }

        private StaffDTO ConvertToDTO(Staff staff)
        {
            var availabilitySlotsDTO = staff.AvailabilitySlots
                .Select(slot => slot.ToString())
                .ToList();

            return new StaffDTO
            {
                Id = staff.Id.Value,
                LicenseNumber = staff.LicenseNumber.ToString(),
                Name = staff.Name.ToString(),
                Email = staff.Email.ToString(),
                Phone = staff.Phone.ToString(),
                AvailabilitySlots = availabilitySlotsDTO,
                Specialization = staff.Specialization.Id.AsString()
            };
        }
    }
}
