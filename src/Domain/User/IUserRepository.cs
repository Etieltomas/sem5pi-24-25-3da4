
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.Shared;
using Sempi5.Domain.User;

namespace Sempi5.Domain.User
{
    public interface IUserRepository : IRepository<SystemUser,SystemUserId>
    {
        public  Task<SystemUser> GetUserByEmail(string email);
    }
}