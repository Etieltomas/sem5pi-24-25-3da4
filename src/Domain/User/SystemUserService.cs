using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.User;
using Sempi5.Infrastructure.UserRepository;

namespace Sempi5.Domain.Staff
{
    public class SystemUserService
    {
        private readonly IUserRepository _repo;

        public SystemUserService(IUserRepository repo)
        {
            this._repo = repo;
        }

        public async Task<SystemUserDTO> AddUser(SystemUserDTO systemUserDTO){
            return await _repo.AddUser(systemUserDTO);
        }

        public async Task<SystemUserDTO> GetSystemUser(long id){

            var staff = await _repo.GetUserById(id);

            return new SystemUserDTO { Email = staff.Email, Username = staff.Username, Role = staff.Role };
        }
    
        public async Task<ActionResult<IEnumerable<SystemUserDTO>>> GetAllSystemUsers(){
            var result = await _repo.GetAllUsers();

            if (result == null)
            {
                return null;
            }

            var list = result.Value.ToList();

            List<SystemUserDTO> listDto = list.ConvertAll(cat => new SystemUserDTO
            {
                Email = cat.Email,
                Username = cat.Username,
                Role = cat.Role
            });

            return listDto;
        }
    }
}