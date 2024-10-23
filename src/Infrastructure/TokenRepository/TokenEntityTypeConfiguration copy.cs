using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.Staff;
using Sempi5.Domain.Token;
using System.Globalization;
using System.Text.Json;


namespace Sempi5.Infrastructure.TokenRepository
{
    public class TokenEntityTypeConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable("Tokens");
            builder.HasKey(t => t.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    v => v.AsString(),        
                    v => new TokenID(v)
                )
                .IsRequired()
                .ValueGeneratedOnAdd();
                    
            builder.Property(t => t.Email)
                .HasConversion(
                    v => v.ToString(),
                    v => new Email(v)
                )
                .IsRequired();

            builder.Property(t => t.ExpirationDate)
                .HasConversion(
                    v => v.ToString("dd-MM-yyyy HH:mm"),
                    v => DateTime.ParseExact(v, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)
                )
                .IsRequired();

            
            builder.Property(t => t.IsUsed)
                .IsRequired();

        }
    }


}
