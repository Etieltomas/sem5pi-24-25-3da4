using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.User;
using Sempi5.Domain.Staff;
using Sempi5.Domain.Patient;
using Sempi5.Infrastructure.UserRepository;
using Sempi5.Infrastructure.StaffRepository;
using Sempi5.Infrastructure.PatientRepository;

namespace Sempi5.Infrastructure.Databases
{
    public class DataBaseContext : DbContext
    {
        public DbSet<SystemUser> Users { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Patient> Patients { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StaffEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientEntityTypeConfiguration());
        }
        
    }
}