
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.UserEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.UserRepository
{
    public class UserRepository : BaseRepository<SystemUser,SystemUserId>, IUserRepository
    {
        private readonly DataBaseContext _context;
        public UserRepository(DataBaseContext context) : base(context.Users)
        {
            _context = context;
        }
         public async Task<SystemUser> GetUserByEmail(string email)
        {
            var user = await  _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(new Email(email)));

            if(user == null){
                return null;
            }

            return user;
        }

    }
}