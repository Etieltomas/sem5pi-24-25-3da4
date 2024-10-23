
using Microsoft.AspNetCore.Mvc;
using Sempi5;
using Sempi5.Domain;
using Sempi5.Domain.Shared;
using Sempi5.Domain.Staff;

namespace Sempi5.Domain.Token
{
    public interface ITokenRepository : IRepository<Token, TokenID>
    {
        public Task<Token> GetTokenByEmail(string email);
        public Task<List<Token>> GetAllTokens();
        public Task<Token> GetTokenByValue(TokenID id);
    }
}