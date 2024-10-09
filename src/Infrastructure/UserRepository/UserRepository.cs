
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.User;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataBaseContext _context;
        public UserRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<SystemUserDTO> AddUser(SystemUserDTO systemUserDTO)
        {
            var user = new SystemUser
            {
                Email = systemUserDTO.Email,
                Username = systemUserDTO.Username,
                Role = systemUserDTO.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new SystemUserDTO { Email = user.Email, Username = user.Username, Role = user.Role };
        }
        

        public async Task<SystemUser> GetUserById(long id)
        { 
            var user = await  _context.Users.FindAsync(id);

            if(user == null)
                return null;

            return new SystemUser { Email = user.Email, Username = user.Username, Role = user.Role };
        }

         public async Task<SystemUser> GetUserByEmail(string email)
        {
            var user = await  _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if(user == null){
                return null;
            }

            return new SystemUser { Email = user.Email, Username = user.Username, Role = user.Role} ;
        }

        public async Task<ActionResult<List<SystemUser>>> GetAllUsers()
        {
            var list = await _context.Users.ToListAsync();
            
            return list;
        }
    }
}