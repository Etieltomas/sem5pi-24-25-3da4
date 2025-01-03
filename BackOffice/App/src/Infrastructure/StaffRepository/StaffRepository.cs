using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.StaffRepository
{
    public class StaffRepository : BaseRepository<Staff, StaffID>, IStaffRepository
    {
        private readonly DataBaseContext _context;

        public StaffRepository(DataBaseContext context) : base(context.Staff)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a staff member by their email.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the staff member.</param>
        /// <returns>The staff member or null if not found.</returns>
        public async Task<Staff> GetStaffMemberByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var staff = await _context.Staff
                .Include(s => s.SystemUser)
                .Include(s => s.Specialization)
                .FirstOrDefaultAsync(s => s.Email.Equals(new Email(email)));
                    
            return staff;
        }

        /// <summary>
        /// Retrieves all staff members.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <returns>A list of all staff members.</returns>
        public async Task<List<Staff>> GetAllStaffMembers()
        {
            return await _context.Staff
                .Include(s => s.SystemUser)
                .Include(s => s.Specialization)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a staff member by their ID.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="id">The ID of the staff member.</param>
        /// <returns>The staff member or null if not found.</returns>
        public async Task<Staff> GetStaffMemberById(StaffID id)
        {
            if (id == null)
            {
                return null;
            }

            var staff = await _context.Staff
                .Include(s => s.SystemUser)
                .Include(s => s.Specialization)
                .FirstOrDefaultAsync(s => s.Id.Equals(id)); 
            
            return staff;
        }

        /// <summary>
        /// Searches for staff members based on the given criteria.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="name">The name of the staff member to search for.</param>
        /// <param name="email">The email of the staff member to search for.</param>
        /// <param name="specialization">The specialization of the staff member to search for.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of results per page.</param>
        /// <returns>A list of staff members matching the criteria.</returns>
        public async Task<List<Staff>> SearchStaff(string? name, string? email, string? specialization,
                             int page, int pageSize)
        {
            var query = _context.Staff
                .Include(s => s.SystemUser)
                .Include(s => s.Specialization)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Equals(new Name(name)));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(s => s.Email.Equals(new Email(email)));
            }

            if (!string.IsNullOrEmpty(specialization))
            {
                query = query.Where(s => s.Specialization.Name.Equals(specialization));
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }
    }
}
