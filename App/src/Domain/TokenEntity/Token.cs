using Org.BouncyCastle.Tls;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.TokenEntity
{
    public class Token : Entity<TokenID>
    {
        public Email Email { get; set; }    
        private DateTime _expirationDate;

        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set => ChangeExpirationDate(value);
        }

        private void ChangeExpirationDate(DateTime value)
        {
            if (value < DateTime.Now)
            {
                throw new BusinessRuleValidationException("Expiration date cannot be in the past.");
            }
            _expirationDate = value;
        }

        public bool IsUsed { get; set; }
    }

}