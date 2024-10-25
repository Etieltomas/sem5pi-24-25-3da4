using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.OperationRequestEntity;

public class OperationTypeEntityTypeConfiguration : IEntityTypeConfiguration<OperationType>
{
    public void Configure(EntityTypeBuilder<OperationType> builder)
    {
        builder.ToTable("OperationType");
        builder.HasKey(t => t.Id);


        builder.Property(t => t.Id)
            .HasConversion(
                v => v.AsLong(),
                v => new OperationTypeID(v)
            )
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(t => t.Specialization)
            .WithMany()
            .HasForeignKey("SpecializationId")
            .IsRequired();

        builder.HasIndex(t => t.Name)
            .IsUnique();
    }
}
