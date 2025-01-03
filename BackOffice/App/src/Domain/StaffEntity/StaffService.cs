using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;

namespace Sempi5.Domain.StaffEntity
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

        /// <summary>
        /// Adds a new staff member to the system.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="staffDTO">The staff member details to be added.</param>
        /// <returns>The newly added staff member.</returns>
        public async Task<StaffDTO> AddStaffMember(StaffDTO staffDTO)
        {
            var address = staffDTO.Address.Split(", ");

            var availabilitySlots = staffDTO.AvailabilitySlots
                .Select(slot => new AvailabilitySlot(slot))
                .ToList();

            var specialization = await _specRepo.GetByName(staffDTO.Specialization.ToLower());

            var staff = new Staff
            {
                LicenseNumber = new LicenseNumber(staffDTO.LicenseNumber),
                Name = new Name(staffDTO.Name),
                Email = new Email(staffDTO.Email),
                Phone = new Phone(staffDTO.Phone),
                AvailabilitySlots = availabilitySlots,
                Address = new Address(address[0], address[1], address[2]),
                Specialization = specialization
            };

            await _repo.AddAsync(staff);
            await _unitOfWork.CommitAsync();

            return ConvertToDTO(staff);
        }

        /// <summary>
        /// Retrieves a staff member by their ID.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="id">The ID of the staff member to retrieve.</param>
        /// <returns>The staff member or null if not found.</returns>
        public async Task<StaffDTO> GetStaffMember(StaffID id)
        {
            var staff = await _repo.GetStaffMemberById(id);
            return staff == null ? null : ConvertToDTO(staff);
        }

        /// <summary>
        /// Retrieves a staff member by their email.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the staff member to retrieve.</param>
        /// <returns>The staff member or null if not found.</returns>
        public async Task<StaffDTO> GetStaffMemberByEmail(string email)
        {
            var staff = await _repo.GetStaffMemberByEmail(email);
            return staff == null ? null : ConvertToDTO(staff);
        }

        /// <summary>
        /// Retrieves all staff members.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <returns>A list of all staff members.</returns>
        public async Task<List<StaffDTO>> GetAllStaffMembers()
        {
            var staffList = await _repo.GetAllStaffMembers();
            return staffList.Select(ConvertToDTO).ToList();
        }

        /// <summary>
        /// Searches for staff members based on given criteria.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="name">The name of the staff member to search for.</param>
        /// <param name="email">The email of the staff member to search for.</param>
        /// <param name="specialization">The specialization of the staff member to search for.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of staff members per page.</param>
        /// <returns>A list of staff members matching the search criteria.</returns>
        public async Task<List<StaffDTO>> SearchStaff(string? name, string? email, string? specialization, int page, int pageSize)
        {
            var staff = await _repo.SearchStaff(name, email, specialization, page, pageSize);

            return staff.Select(ConvertToDTO).ToList();
        }

        /// <summary>
        /// Edits an existing staff member's profile.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the staff member to edit.</param>
        /// <param name="staffDTO">The new staff details to update.</param>
        /// <param name="isEmailComfirmed">Whether the email confirmation is completed.</param>
        /// <returns>The updated staff member.</returns>
        public async Task<StaffDTO> EditStaff(string email, StaffDTO staffDTO, bool isEmailComfirmed)
        {
            var staff = await _repo.GetStaffMemberByEmail(email);

            var originalEmail = staff.Email;

            if (staff == null)
            {
                return null;
            }

            var confirmationEmailNeeded = false;
            if (staffDTO.AvailabilitySlots != null)
            {
                staff.AvailabilitySlots = staffDTO.AvailabilitySlots
                    .Select(slot => new AvailabilitySlot(slot))
                    .ToList();
            }

            if (staffDTO.Specialization != null) 
            {
                var specialization = await _specRepo.GetByName(staffDTO.Specialization.ToLower());
                staff.Specialization = specialization;
            }

            if (staffDTO.Email != null)
            {
                staff.Email = new Email(staffDTO.Email);
                confirmationEmailNeeded = true;
            }

            if (staffDTO.Phone != null)
            {
                staff.Phone = new Phone(staffDTO.Phone);
                confirmationEmailNeeded = true;
            }

            if (staffDTO.Address != null)
            {
                var address = staffDTO.Address.Split(", ");
                staff.Address = new Address(address[0], address[1], address[2]);
                confirmationEmailNeeded = true;
            }

            if (!confirmationEmailNeeded || isEmailComfirmed)
            {
                await _unitOfWork.CommitAsync();
            }

            return ConvertToDTO(staff);
        }

        private StaffDTO ConvertToDTO(Staff staff)
        {
            var availabilitySlotsDTO = staff.AvailabilitySlots?
                .Select(slot => slot.ToString())
                .ToList();

            return new StaffDTO
            {
                Id = staff.Id?.Value,
                LicenseNumber = staff.LicenseNumber?.ToString(),
                Name = staff.Name?.ToString(),
                Email = staff.Email?.ToString(),
                Phone = staff.Phone?.ToString(),
                Address = staff.Address?.ToString(),
                AvailabilitySlots = availabilitySlotsDTO,
                Specialization = staff.Specialization?.Name
            };
        }
    }
}
