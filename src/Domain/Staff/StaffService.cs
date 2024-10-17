using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Shared;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.StaffRepository;

namespace Sempi5.Domain.Staff
{
    public class StaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffRepository _repo;

        public StaffService(IStaffRepository repo, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<StaffDTO> AddStaffMember(StaffDTO staffDTO)
        {
            var availabilitySlots = staffDTO.AvailabilitySlots
                .Select(slot => new AvailabilitySlot(slot))
                .ToList();

            var staff = new Staff
            {
                LicenseNumber = new LicenseNumber(staffDTO.LicenseNumber),
                Name = new Name(staffDTO.Name),
                Email = new Email(staffDTO.Email),
                Phone = new Phone(staffDTO.Phone),
                AvailabilitySlots = availabilitySlots,
                Specialization = staffDTO.Specialization
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
                Specialization = staff.Specialization
            };
        }
    }
}
