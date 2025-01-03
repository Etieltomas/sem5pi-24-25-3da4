using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.UserEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.UserRepository
{
    public class UserRepository : BaseRepository<SystemUser, SystemUserId>, IUserRepository
    {
        private readonly DataBaseContext _context;
        public UserRepository(DataBaseContext context) : base(context.Users)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a system user by their email.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the system user to retrieve.</param>
        /// <returns>The system user or null if no user is found.</returns>
        public async Task<SystemUser> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(new Email(email)));

            if (user == null)
            {
                return null;
            }

            return user;
        }

        /// <summary>
        /// Retrieves a system user by their ID.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="userID">The ID of the system user to retrieve.</param>
        /// <returns>The system user or null if no user is found.</returns>
        public async Task<SystemUser> GetUserByID(long userID)
        {
            var user = await  _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(new SystemUserId(userID)));

            if (user == null)
            {
                return null;
            }

            return user;
        }

        /// <summary>
        /// Deletes a system user from the database.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="user">The system user to delete.</param>
        /// <returns>Task representing the asynchronous delete operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the user is null.</exception>
        public async Task DeleteAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
