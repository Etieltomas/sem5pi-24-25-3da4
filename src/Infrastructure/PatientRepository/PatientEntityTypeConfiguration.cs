using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.Patient;

namespace Sempi5.Infrastructure.PatientRepository
{
    public class PatientEntityTypeConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");
            builder.HasKey(t => t.MedicalRecordNumber);

            builder.Property(t => t.Name)
                .IsRequired();

            builder.Property(t => t.Email)
                .IsRequired();
            builder.HasIndex(t => t.Email) 
                .IsUnique();
            
            builder.Property(t => t.Phone)
                .IsRequired();
            builder.HasIndex(t => t.Phone)
                .IsUnique();

            builder.Property(t => t.Conditions)
                .IsRequired();
            
            builder.Property(t => t.EmergencyContact)
                .IsRequired();

            builder.Property(t => t.DateOfBirth)
                .IsRequired();
            

            builder.HasOne(t => t.SystemUser)
                .WithOne()
                .HasForeignKey<Patient>("SystemUserId");
        }
    }
}
