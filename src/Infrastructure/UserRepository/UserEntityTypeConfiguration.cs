using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.User;

namespace Sempi5.Infrastructure.UserRepository
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<SystemUser>
    {
        public void Configure(EntityTypeBuilder<SystemUser> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasConversion(
                    v => v.AsLong(),
                    v => new SystemUserId(v)
                )
                .ValueGeneratedOnAdd();

            builder.HasIndex(t => t.Username).IsUnique();

            builder.HasIndex(t => t.Email).IsUnique();

            builder.Property(t => t.Email)
                .IsRequired();

            builder.Property(t => t.Username)
                .IsRequired();
        }
    }
}
