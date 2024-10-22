using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.User;
using Sempi5.Domain.Staff;
using Sempi5.Domain.Patient;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.Token;
using Sempi5.Infrastructure.UserRepository;
using Sempi5.Infrastructure.StaffRepository;
using Sempi5.Infrastructure.PatientRepository;
using Sempi5.Infrastructure.SpecializationRepository;
using Sempi5.Infrastructure.TokenRepository;

namespace Sempi5.Infrastructure.Databases
{
    public class DataBaseContext : DbContext
    {
        public DbSet<SystemUser> Users { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Specialization> Specializations { get; internal set; }
        public DbSet<Token> Tokens { get; set; }
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
            modelBuilder.ApplyConfiguration(new SpecializationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TokenEntityTypeConfiguration());
        }
        
    }
}