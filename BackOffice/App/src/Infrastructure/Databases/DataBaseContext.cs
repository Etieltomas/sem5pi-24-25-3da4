using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.UserEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.TokenEntity;
using Sempi5.Infrastructure.UserRepository;
using Sempi5.Infrastructure.StaffRepository;
using Sempi5.Infrastructure.PatientRepository;
using Sempi5.Infrastructure.SpecializationRepository;
using Sempi5.Infrastructure.RoomRepository;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Infrastructure.TokenRepository;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.AppointmentEntity;
using Sempi5.Infrastructure.AppointmentRepository;

namespace Sempi5.Infrastructure.Databases
{
    public class DataBaseContext : DbContext
    {
        public DbSet<SystemUser> Users { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Specialization> Specializations { get; internal set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<OperationRequest> OperationRequests {get; set;}
        public DbSet<OperationType> OperationTypes {get; set;}
        public DbSet<Room> Rooms {get; set;}
        public DbSet<Appointment> Appointments {get; set;}
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
            modelBuilder.ApplyConfiguration(new OperationRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperationTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoomEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentEntityTypeConfiguration());
        }
        
    }
}