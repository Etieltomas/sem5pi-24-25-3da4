using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.UserEntity;

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

            builder.Property(t => t.Name)
                .HasConversion(
                    v => v.ToString(),
                    v => new Name(v)
                )
                .IsRequired();

            builder.Property(t => t.Email)
                .HasConversion(
                    v => v.ToString(),
                    v => new Email(v)
                )
                .IsRequired();

            builder.Property(t => t.Phone)
                .HasConversion(
                    v => v.ToString(),
                    v => new Phone(v)
                )
                .IsRequired();

            builder.Property(t => t.EmergencyContact)
                .HasConversion(
                    v => v.ToString(),
                    v => new Phone(v)
                )
                .IsRequired();

            
            builder.Property(t => t.DateOfBirth)
                .HasConversion(
                    v => v.ToString("dd-MM-yyyy"),
                    v => DateTime.ParseExact(v, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                )
                .IsRequired();

            builder.Property(t => t.Gender)
                .HasConversion(
                    v => v.ToString(),
                    v => GenderExtensions.FromString(v.ToLower())
                )
                .IsRequired();

            builder.Property(t => t.Address)
                .HasConversion(
                    v => v.ToString(),
                    v => AddressConversions(v)
                )
                .IsRequired();

            builder.HasIndex(t => t.Email).IsUnique();
            builder.HasIndex(t => t.Phone).IsUnique();

            builder.HasOne(t => t.SystemUser)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<Patient>("SystemUserId");
        }

        private Address AddressConversions(string v)
        {

            var parts = v.Split(", ");
            return new Address(parts[0], parts[1], parts[2]);        
        }
    }


}
