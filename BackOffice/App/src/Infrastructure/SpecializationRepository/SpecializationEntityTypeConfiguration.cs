using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.SpecializationEntity;

namespace Sempi5.Infrastructure.SpecializationRepository
{
    public class SpecializationEntityTypeConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.ToTable("Specialization");
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasConversion(
                    v => v.AsLong(),
                    v => new SpecializationID(v)
                )
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(t => t.Code)
                .HasColumnName("Code")
                .IsRequired()
                .HasMaxLength(50);
            
            // Description Configuration (optional)
            builder.Property(t => t.Description)
                .HasColumnName("Description")
                .HasMaxLength(500)
                .IsRequired(false);

            // Ensure Code is Unique
            builder.HasIndex(t => t.Code)
                .IsUnique();

                builder.HasIndex(t => t.Name)
                .IsUnique();
           
        }
    }
}
