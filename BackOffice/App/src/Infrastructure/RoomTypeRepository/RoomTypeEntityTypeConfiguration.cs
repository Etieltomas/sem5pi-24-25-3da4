using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.RoomEntity;

namespace Sempi5.Infrastructure.RoomTypeRepository
{
    public class RoomTypeEntityTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.ToTable("RoomTypes");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Id)
                .HasColumnName("Id")
                .HasConversion(
                    v => v.AsLong(),
                    v => new RoomTypeID(v)
                )
                .IsRequired()
                .ValueGeneratedOnAdd();
        
            builder.Property(rt => rt.Name)
                .IsRequired()
                .HasMaxLength(100); 

        }
    }
}
