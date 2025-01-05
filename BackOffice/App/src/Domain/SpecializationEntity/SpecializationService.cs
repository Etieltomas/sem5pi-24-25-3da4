using Microsoft.AspNetCore.Http.HttpResults;
using Sempi5.Domain.Shared;
using Sempi5.Domain.TokenEntity;
using Sempi5.Domain.UserEntity;

namespace Sempi5.Domain.SpecializationEntity
{
    public class SpecializationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpecializationRepository _repo;

        public SpecializationService(ISpecializationRepository repo, IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        /**
         * Adicionar uma nova especialização
         * @param name Nome da especialização
         * @param code Código da especialização
         * @param description Descrição da especialização
         * @return Task<string> - O nome da especialização adicionada
         * @author Ricardo Guimarães
         * @date 10/12/2024
         */
        public async Task<SpecializationDTO> AddSpecialization(string name, string code, string? description)
        {
            var specialization = new Specialization
            {
                Name = name,
                Code = code,
                Description = description
            };

            await _repo.AddAsync(specialization);
            await _unitOfWork.CommitAsync();

            return MapToDTO(specialization);
        }

        /**
        * Listar todas as especializações
        * @return Task<List<string>> - Lista de todas as especializações
        * @author Ricardo Guimarães
        * @date 10/12/2024
        */
        public async Task<List<string>> GetAllSpecializations()
        {
            var result = await _repo.GetAllAsync();
            return result.ConvertAll(cat => cat.Name);
        }

        /**
        * Obter especialização por ID
        * @param id long - ID da especialização
        * @return Task<string> - O nome da especialização
        * author Ricardo Guimarães
        * date 10/12/2024
        */
        public async Task<string?> GetSpecializationById(long id)
        {
            var specialization = await _repo.GetByIdAsync(new SpecializationID(id));
            return specialization?.Name;
        }

        /**
        * Pesquisar especializações
        * @param name Nome da especialização
        * @param code Código da especialização
        * @param description Descrição da especialização
        * @param page Página
        * @param pageSize Tamanho da página
        * @return Task<List<SpecializationDTO>> - Lista de especializações
        * author Ricardo Guimarães
        * date 10/12/2024
        */
        public async Task<List<SpecializationDTO>> SearchSpecializations(string? name, string? code,
                                                                string? description, int page, int pageSize)
        {
            var result = await _repo.SearchSpecializations(name, code, description, page, pageSize);
            return result.ConvertAll(MapToDTO);
        }

        /**
        * Atualizar especialização
        * @param id long - ID da especialização
        * @param dto SpecializationUpdateDTO - DTO com os dados atualizados da especialização
        * @return Task<bool> - True se a especialização foi atualizada com sucesso, False caso contrário
        * author Ricardo Guimarães
        * date 10/12/2024
        */
        public async Task<bool> UpdateSpecialization(long id, SpecializationUpdateDTO dto)
        {
            var specialization = await _repo.GetByIdAsync(new SpecializationID(id));
            if (specialization == null) return false;

            if (!string.IsNullOrEmpty(dto.Name))
            {
                specialization.Name = dto.Name;
            }
            if (!string.IsNullOrEmpty(dto.Description))
            {
                specialization.Description = dto.Description;
            }

            await _unitOfWork.CommitAsync();
            return true;
        }

        /**
        * Mapear especialização para DTO
        * @param specialization Especialização
        * @return SpecializationDTO - DTO da especialização
        * author Ricardo Guimarães
        * date 10/12/2024
        */
        private SpecializationDTO MapToDTO(Specialization specialization)
        {
            return new SpecializationDTO
            {
                Id = specialization.Id.AsLong(),
                Name = specialization.Name,
                Code = specialization.Code,
                Description = specialization.Description
            };
        }
    }
}
