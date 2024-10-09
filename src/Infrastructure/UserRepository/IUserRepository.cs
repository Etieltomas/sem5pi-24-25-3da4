
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.User;

namespace Sempi5.Infrastructure.UserRepository
{
    public interface IUserRepository
    {
        public  Task<SystemUserDTO> AddUser(SystemUserDTO systemUserDTO);
        public  Task<SystemUser> GetUserById(long id);
        public  Task<SystemUser> GetUserByEmail(string email);
        public  Task<ActionResult<List<SystemUser>>> GetAllUsers();
    }
}