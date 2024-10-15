using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.Staff;

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
                .IsRequired();

            builder.Property(t => t.Name)
                .IsRequired();

            builder.Property(t => t.Email)
                .IsRequired();
            
            builder.Property(t => t.Phone)
                .IsRequired();

            builder.Property(t => t.AvailabilitySlots)
                .IsRequired();
            
            builder.Property(t => t.Specialization)
                .IsRequired();
            
            builder.HasOne(t => t.SystemUser)
                .WithOne()
                .HasForeignKey<Staff>("SystemUserId");

            builder.HasIndex(t => t.Email).IsUnique();
            builder.HasIndex(t => t.LicenseNumber).IsUnique();
            builder.HasIndex(t => t.Phone).IsUnique();
        }
    }
}
