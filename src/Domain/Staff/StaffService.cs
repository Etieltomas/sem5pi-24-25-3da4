using System;
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
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<StaffDTO> AddStaffMember(StaffDTO staffDTO){
            var staff = new Staff 
            {
                Name = staffDTO.Name,
                Email = staffDTO.Email,
                Phone = staffDTO.Phone,
                AvailabilitySlots = staffDTO.AvailabilitySlots,
                Specialization = staffDTO.Specialization
            };

            await _repo.AddAsync(staff);

            await _unitOfWork.CommitAsync();

            return new StaffDTO { LicenseNumber = staff.Id.Value, Name = staff.Name, Email = staff.Email, Phone = staff.Phone, AvailabilitySlots = staff.AvailabilitySlots, Specialization = staff.Specialization };
        }

        public async Task<StaffDTO> GetStaffMember(StaffID id){

            var staff = await _repo.GetByIdAsync(id);

            if(staff == null)
                return null;

            return new StaffDTO { LicenseNumber = staff.Id.Value, Name = staff.Name, Email = staff.Email, Phone = staff.Phone, AvailabilitySlots = staff.AvailabilitySlots, Specialization = staff.Specialization };      
        }
    
        public async Task<List<StaffDTO>> GetAllStaffMembers(){
            var list = await _repo.GetAllAsync();

            List<StaffDTO> listDto = list.ConvertAll<StaffDTO>(staff => new StaffDTO { LicenseNumber = staff.Id.Value, Name = staff.Name, Email = staff.Email, Phone = staff.Phone, AvailabilitySlots = staff.AvailabilitySlots, Specialization = staff.Specialization });
            
            return listDto;
        }
    }
}