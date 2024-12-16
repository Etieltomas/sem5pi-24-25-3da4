using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.AppointmentEntity;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.RoomEntity;


namespace Sempi5.Infrastructure.AppointmentRepository
{
    public class AppointmentEntityTypeConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasConversion(
                    v => v.AsLong(),
                    v => new AppointmentID(v)
                )
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(a => a.AppointmentStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => AppointmentStatusExtensions.FromString(v)
                )
                .IsRequired();

            builder.Property(a => a.AppointmentType)
                .HasConversion(
                    v => v.ToString(),
                    v => AppointmentTypeExtensions.FromString(v)
                )
                .IsRequired();

            builder.Property(a => a.DateOperation)
                .HasConversion(
                    v => v.Value.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                    v => new DateOperation(DateTime.ParseExact(v, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture))
                )
                .IsRequired();

            builder.HasOne(a => a.OperationRequest)
            	.WithOne() 
                .HasForeignKey<Appointment>("OperationRequestId") 
                .IsRequired();

            builder.HasOne(a => a.Room)
                .WithMany()
                .HasForeignKey("RoomId")
                .IsRequired();
        }
    }


}
