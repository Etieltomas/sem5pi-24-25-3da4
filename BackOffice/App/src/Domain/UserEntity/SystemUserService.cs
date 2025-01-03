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

        /// <summary>
        /// Updates the active status of a system user based on a token.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="token">The token used to identify the user.</param>
        /// <param name="active">The active status to set.</param>
        /// <returns>The updated system user or null if the token is invalid or expired.</returns>
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

        /// <summary>
        /// Updates the active status of a system user by email.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the user to update.</param>
        /// <param name="activate">The active status to set.</param>
        /// <returns>The updated system user or null if the user does not exist.</returns>
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

        /// <summary>
        /// Adds a new system user.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="systemUserDTO">The data transfer object representing the system user to create.</param>
        /// <returns>The created system user.</returns>
        public async Task<SystemUserDTO> AddUser(SystemUserDTO systemUserDTO)
        {

            var user = new SystemUser{
                Username = systemUserDTO.Username,
                Email = new Email(systemUserDTO.Email),
                Role = systemUserDTO.Role,
                Active = systemUserDTO.Active,
                MarketingConsent = systemUserDTO.MarketingConsent
            };

            await _repo.AddAsync(user);
            
            await _unitOfWork.CommitAsync();

            return ConvertToDTO(user);
        }

        /// <summary>
        /// Retrieves a system user by their ID.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="id">The ID of the system user to retrieve.</param>
        /// <returns>The system user or null if the user does not exist.</returns>
        public async Task<SystemUserDTO> GetSystemUser(SystemUserId id)
        {

            var user = await _repo.GetByIdAsync(id);

            return ConvertToDTO(user);
        }
    
        /// <summary>
        /// Retrieves all system users.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <returns>A list of all system users.</returns>
        public async Task<List<SystemUserDTO>> GetAllSystemUsers()
        {
            var result = await _repo.GetAllAsync();

            List<SystemUserDTO> listDto = result.ConvertAll(cat => ConvertToDTO(cat));

            return listDto;
        }

        /// <summary>
        /// Retrieves a system user by their email.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the system user to retrieve.</param>
        /// <returns>The system user or null if the user does not exist.</returns>
        public async Task<SystemUserDTO> GetUserByEmail(string email)
        {
            var patient = await _repo.GetUserByEmail(email);
            return patient == null ? null : ConvertToDTO(patient);
        }

        /// <summary>
        /// Converts a system user to a data transfer object (DTO).
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="user">The system user to convert.</param>
        /// <returns>The converted system user DTO.</returns>
        private SystemUserDTO ConvertToDTO(SystemUser user)
        {
            return new SystemUserDTO
            {
                Id = user.Id.AsString(),
                Email = user.Email.ToString(),
                Username = user.Username,
                Role = user.Role,
                Active = user.Active,
                MarketingConsent = user.MarketingConsent
            };
        }
    }
}
