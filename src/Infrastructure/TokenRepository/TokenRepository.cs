
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Staff;
using Sempi5.Domain.Token;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.TokenRepository
{
    public class TokenRepository : BaseRepository<Token, TokenID>, ITokenRepository
    {
        private readonly DataBaseContext _context;

        public TokenRepository(DataBaseContext context) : base(context.Tokens)
        {
            _context = context;
        }

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

        public async Task<List<Token>> GetAllTokens()
        {
            return await _context.Tokens
                .ToListAsync();
        }

        public async Task<Token> GetTokenByValue(string value)
        {
            if (value == null)
            {
                return null;
            }

            var token = await _context.Tokens
                .FirstOrDefaultAsync(s => s.TokenValue.Equals(value)); 
            
            return token;
        }
    }

}