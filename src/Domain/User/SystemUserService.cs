using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.Shared;
using Sempi5.Domain.User;
using Sempi5.Infrastructure.UserRepository;

namespace Sempi5.Domain.User
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

            return ConvertToDTO(user);
        }

        public async Task<SystemUserDTO> GetSystemUser(SystemUserId id){

            var user = await _repo.GetByIdAsync(id);

            return ConvertToDTO(user);
        }
    
        public async Task<List<SystemUserDTO>> GetAllSystemUsers(){
            var result = await _repo.GetAllAsync();

            List<SystemUserDTO> listDto = result.ConvertAll(cat => ConvertToDTO(cat));

            return listDto;
        }

        public async Task<SystemUserDTO> GetUserByEmail(string email)
        {
            var patient = await _repo.GetUserByEmail(email);
            return patient == null ? null : ConvertToDTO(patient);
        }

        private SystemUserDTO ConvertToDTO(SystemUser user)
        {
            return new SystemUserDTO
            {
                Id = user.Id.AsString(),
                Email = user.Email,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}