using Microsoft.AspNetCore.Http.HttpResults;
using Sempi5.Domain.Shared;
using Sempi5.Domain.TokenEntity;
using Sempi5.Domain.UserEntity;

namespace Sempi5.Domain.UserEntity
{
    public class SystemUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;
        private readonly ITokenRepository _tokenRepository;

        public SystemUserService(IUserRepository repo, ITokenRepository tokenRepository, IUnitOfWork unitOfWork)
        {
            this._tokenRepository = tokenRepository;
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<SystemUserDTO> Update(Guid token, bool active)
        {
            var tokenUsed = await _tokenRepository.GetTokenByValue(new TokenID(token));
            if (token == null) {
                return null;
            }

            var user = await _repo.GetUserByEmail(tokenUsed.Email.ToString());
            if (user == null)
                return null;

            if (tokenUsed.ExpirationDate < DateTime.UtcNow)
            {
                return null; 
            }   

            user.Active = active;

            tokenUsed.IsUsed = true;

            await this._unitOfWork.CommitAsync();

            return ConvertToDTO(user);
        }


        public async Task<SystemUserDTO> UpdateActive(string email, bool activate)
        {
            var user = await _repo.GetUserByEmail(email);
            if (user == null) {
                return null;
            }

            user.Active = activate;

            await _unitOfWork.CommitAsync();

            return ConvertToDTO(user);
        }
        public async Task<SystemUserDTO> AddUser(SystemUserDTO systemUserDTO){

            var user = new SystemUser{
                Username = systemUserDTO.Username,
                Email = new Email(systemUserDTO.Email),
                Role = systemUserDTO.Role,
                Active = systemUserDTO.Active,
                MarktingConsent = systemUserDTO.MarktingConsent
            };

            await _repo.AddAsync(user);
            
            await _unitOfWork.CommitAsync();

            return ConvertToDTO(user);
        }

        public async Task<SystemUserDTO> GetSystemUser(SystemUserId id){

            var user = await _repo.GetByIdAsync(id);

            return ConvertToDTO(user);
        }
    
        public async Task<List<SystemUserDTO>> GetAllSystemUsers(){
            var result = await _repo.GetAllAsync();

            List<SystemUserDTO> listDto = result.ConvertAll(cat => ConvertToDTO(cat));

            return listDto;
        }

        public async Task<SystemUserDTO> GetUserByEmail(string email)
        {
            var patient = await _repo.GetUserByEmail(email);
            return patient == null ? null : ConvertToDTO(patient);
        }

        private SystemUserDTO ConvertToDTO(SystemUser user)
        {
            return new SystemUserDTO
            {
                Id = user.Id.AsString(),
                Email = user.Email.ToString(),
                Username = user.Username,
                Role = user.Role,
                Active = user.Active,
                MarktingConsent = user.MarktingConsent
            };
        }

    }
}