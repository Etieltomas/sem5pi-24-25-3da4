using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.Shared;
using Sempi5.Domain.User;
using Sempi5.Infrastructure.UserRepository;

namespace Sempi5.Domain.Staff
{
    public class SystemUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;

        public SystemUserService(IUserRepository repo, IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<SystemUserDTO> AddUser(SystemUserDTO systemUserDTO){

            var user = new SystemUser{
                Username = systemUserDTO.Username,
                Email = systemUserDTO.Email,
                Role = systemUserDTO.Role   
            };

            await _repo.AddAsync(user);
            
            await _unitOfWork.CommitAsync();

            return new SystemUserDTO { Email = user.Email, Username = user.Username, Role = user.Role };
        }

        public async Task<SystemUserDTO> GetSystemUser(SystemUserId id){

            var staff = await _repo.GetByIdAsync(id);

            return new SystemUserDTO { Email = staff.Email, Username = staff.Username, Role = staff.Role };
        }
    
        public async Task<List<SystemUserDTO>> GetAllSystemUsers(){
            var result = await _repo.GetAllAsync();

            List<SystemUserDTO> listDto = result.ConvertAll(cat => new SystemUserDTO
            {
                Email = cat.Email,
                Username = cat.Username,
                Role = cat.Role
            });

            return listDto;
        }
    }
}