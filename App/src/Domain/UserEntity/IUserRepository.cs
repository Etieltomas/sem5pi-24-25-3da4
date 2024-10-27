
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;

namespace Sempi5.Domain.UserEntity
{
    public interface IUserRepository : IRepository<SystemUser,SystemUserId>
    {
        public  Task<SystemUser> GetUserByEmail(string email);
        public  Task<SystemUser> GetUserByID(long id);
        public Task DeleteAsync(SystemUser user);
    }
}