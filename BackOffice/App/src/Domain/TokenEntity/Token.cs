using Org.BouncyCastle.Tls;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.TokenEntity
{
    /// <summary>
    /// Represents a token entity used for authentication, containing properties like email, expiration date, and usage status.
    /// @author Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    public class Token : Entity<TokenID>
    {
        public Email Email { get; set; }    

        private DateTime _expirationDate;

        /// <summary>
        /// Gets or sets the expiration date of the token. It validates that the expiration date is not in the past.
        /// @throws BusinessRuleValidationException if the expiration date is in the past.
        /// </summary>
        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set => ChangeExpirationDate(value);
        }

        /// <summary>
        /// Changes the expiration date, ensuring that it is not in the past.
        /// </summary>
        /// <param name="value">The new expiration date.</param>
        private void ChangeExpirationDate(DateTime value)
        {
            if (value < DateTime.Now)
            {
                throw new BusinessRuleValidationException("Expiration date cannot be in the past.");
            }
            _expirationDate = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the token has been used.
        /// </summary>
        public bool IsUsed { get; set; }
    }
}
