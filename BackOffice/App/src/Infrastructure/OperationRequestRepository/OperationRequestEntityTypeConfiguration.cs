using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.StaffEntity;

public class OperationRequestEntityTypeConfiguration : IEntityTypeConfiguration<OperationRequest>
{
    /**
     * Configure created by Ricardo Guimarães on 10/12/2024
     */
    public void Configure(EntityTypeBuilder<OperationRequest> builder)
    {
        builder.ToTable("OperationRequests");
        builder.HasKey(t => t.Id);

        //id
        builder.Property(t => t.Id)
            .HasColumnName("Id")
            .HasConversion(
                v => v.AsLong(),
                v => new OperationRequestID(v)
            )
            .IsRequired()
            .ValueGeneratedOnAdd();

        //patientId foreign key
        builder.HasOne(t => t.Patient)
                .WithMany() 
                .HasForeignKey("PatientId")
                .IsRequired();  

        //staffID foreign key
        builder.HasOne(t => t.Staff)
                .WithMany() 
                .HasForeignKey("StaffId")
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
                v => v.Value.ToString("dd-MM-yyyy"), 
                v => new Deadline(DateTime.ParseExact(v, "dd-MM-yyyy", CultureInfo.InvariantCulture))
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

        // staff list
        builder.Property(t => t.Staffs)
            .HasColumnName("Staffs")
            .HasConversion(
                v => string.Join(",", v.Select(x => x.AsString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => new StaffID(x)).ToList()
            )
            .IsRequired();

    }
    
}
