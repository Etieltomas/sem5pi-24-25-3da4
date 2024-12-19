using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.TokenEntity;
using System.Globalization;
using System.Text.Json;


namespace Sempi5.Infrastructure.RoomRepository
{
    public class RoomEntityTypeConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");
            builder.HasKey(t => t.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    v => v.AsLong(),        
                    v => new RoomID(v)
                )
                .IsRequired()
                .ValueGeneratedOnAdd();
                    
            builder.Property(t => t.Capacity)
                .HasConversion(
                    v => v.ToString(),
                    v => new Capacity(int.Parse(v))
                )
                .IsRequired();

            builder.Property(t => t.RoomStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => RoomStatusExtensions.FromString(v)
                )
                .IsRequired();

            builder.Property(t => t.Slots)
                .HasConversion(new MaintenanceSlotConverter())
                .IsRequired();

            builder.Property(t => t.AssignedEquipment)
                .HasConversion(new EquipmentConverter())
                .IsRequired();

            builder.HasOne(t => t.Type)
                .WithMany() 
                .HasForeignKey("TypeId")
                .IsRequired();

        }
    }


}
