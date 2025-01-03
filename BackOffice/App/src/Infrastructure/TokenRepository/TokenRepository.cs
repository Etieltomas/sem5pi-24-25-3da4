using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.TokenEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.TokenRepository
{
    /// <summary>
    /// Repository class responsible for managing Token entities in the database.
    /// @author Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    public class TokenRepository : BaseRepository<Token, TokenID>, ITokenRepository
    {
        private readonly DataBaseContext _context;

        /// <summary>
        /// Initializes a new instance of the TokenRepository with the given database context.
        /// </summary>
        /// <param name="context">The database context used to interact with the Tokens table.</param>
        public TokenRepository(DataBaseContext context) : base(context.Tokens)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a token by its associated email.
        /// Returns null if the email is null or empty.
        /// </summary>
        /// <param name="email">The email to search for the token.</param>
        /// <returns>The token associated with the email, or null if not found.</returns>
        public async Task<Token> GetTokenByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var token = await _context.Tokens
                .FirstOrDefaultAsync(s => s.Email.Equals(new Email(email)));
                    
            return token;
        }

        /// <summary>
        /// Retrieves all tokens from the database.
        /// </summary>
        /// <returns>A list of all tokens.</returns>
        public async Task<List<Token>> GetAllTokens()
        {
            return await _context.Tokens
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a token by its unique value (TokenID).
        /// Returns null if the value is null.
        /// </summary>
        /// <param name="value">The TokenID to search for the token.</param>
        /// <returns>The token associated with the given TokenID, or null if not found.</returns>
        public async Task<Token> GetTokenByValue(TokenID value)
        {
            if (value == null)
            {
                return null;
            }

            var token = await _context.Tokens
                .FirstOrDefaultAsync(s => s.Id == value); 

            return token;
        }
    }
}
