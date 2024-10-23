using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.StaffEntity;

public class OperationRequestEntityTypeConfiguration : IEntityTypeConfiguration<OperationRequest>
{
    public void Configure(EntityTypeBuilder<OperationRequest> builder)
    {
        builder.ToTable("OperationRequests");
        builder.HasKey(t => t.Id);

        //id
        builder.Property(t => t.Id)
            .HasColumnName("Id")
            .HasConversion(
                v => v.AsString(),
                v => new OperationRequestID(v)
            )
            .ValueGeneratedNever();

        //patientId foreign key
        builder.Property(t => t.PatientId)
            .HasColumnName("PatientId")
            .HasConversion(
                v => v.AsString(),
                v => new PatientID(v)
            )
            .IsRequired();

        //patient 1-1
        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(t => t.PatientId)
            .IsRequired();

        //staffID foreign key
        builder.Property(t => t.StaffId)
            .HasColumnName("StaffId")
            .HasConversion(
                v => v.AsString(),
                v => new StaffID(v)
            )
            .IsRequired();

        //staff 1-1
        builder.HasOne<Staff>()
            .WithMany()
            .HasForeignKey(t => t.StaffId)
            .IsRequired();

        //operationType
        builder.Property(t => t.OperationType)
            .HasColumnName("OperationType")
            .IsRequired();

        //priority
        builder.Property(t => t.Priority)
            .HasColumnName("Priority")
            .HasConversion(
                v => v.Value, 
                v => Priority.FromString(v)
            )
            .IsRequired();

        //deadline
        builder.Property(t => t.Deadline)
            .HasColumnName("Deadline")
            .HasConversion(
                v => v.Value, 
                v => new Deadline(v)
            )
            .IsRequired();

        //status
        builder.Property(t => t.Status)
            .HasColumnName("Status")
            .HasConversion(
                v => v.Value, 
                v => Status.FromString(v)
            )
            .IsRequired();
    }
    
}
