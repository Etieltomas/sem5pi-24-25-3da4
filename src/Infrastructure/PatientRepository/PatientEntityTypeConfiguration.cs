using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.Patient;
using Sempi5.Domain.User;

namespace Sempi5.Infrastructure.PatientRepository
{
    public class PatientEntityTypeConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    v => v.AsString(),        
                    v => new PatientID(v)
                )
                .HasValueGenerator<PatientIDGenerator>()
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Name).IsRequired();
            builder.Property(t => t.Email).IsRequired();
            builder.Property(t => t.Phone).IsRequired();
            builder.Property(t => t.Conditions).IsRequired();
            builder.Property(t => t.EmergencyContact).IsRequired();
            builder.Property(t => t.DateOfBirth).IsRequired();

            builder.HasIndex(t => t.Email).IsUnique();
            builder.HasIndex(t => t.Phone).IsUnique();

            builder.HasOne(t => t.SystemUser)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<Patient>("SystemUserId");
        }
    }


}
