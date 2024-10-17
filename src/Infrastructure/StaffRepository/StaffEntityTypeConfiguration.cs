using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.Staff;
using System.Text.Json;


namespace Sempi5.Infrastructure.StaffRepository
{
    public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("Staff");
            builder.HasKey(t => t.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    v => v.AsString(),        
                    v => new StaffID(v)
                )
                .HasValueGenerator<StaffIDGenerator>()
                .IsRequired()
                .ValueGeneratedOnAdd();
                    
            builder.Property(t => t.LicenseNumber)
                .HasConversion(
                    v => v.ToString(),
                    v => new LicenseNumber(v)
                )
                .IsRequired();

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

            
            builder.Property(t => t.AvailabilitySlots)
                .HasConversion(new AvailabilitySlotListConverter())
                .IsRequired();

            builder.HasOne(t => t.Specialization)
                .WithMany() 
                .HasForeignKey("SpecializationId")
                .IsRequired();    
            
            builder.HasOne(t => t.SystemUser)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<Staff>("SystemUserId");

            builder.HasIndex(t => t.Email).IsUnique();
            builder.HasIndex(t => t.LicenseNumber).IsUnique();
            builder.HasIndex(t => t.Phone).IsUnique();
        }
    }


}
