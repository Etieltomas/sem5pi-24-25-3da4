using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.OperationRequestEntity;

public class OperationTypeEntityTypeConfiguration : IEntityTypeConfiguration<OperationType>
{
    public void Configure(EntityTypeBuilder<OperationType> builder)
    {
        builder.ToTable("OperationType");
        builder.HasKey(t => t.Id);

        // ID
        builder.Property(t => t.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        // Nome da operação
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Relacionamento com Specialization (muitos para um)
        builder.HasOne(t => t.Specialization)
            .WithMany()
            .HasForeignKey("SpecializationId")
            .IsRequired();

        // Índice único para o nome
        builder.HasIndex(t => t.Name)
            .HasDatabaseName("IX_OperationType_Name")
            .IsUnique();
    }
}
