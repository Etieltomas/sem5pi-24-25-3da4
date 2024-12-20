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

        // Adicionar uma nova especialização
        public async Task<string> AddSpecialization(string name, string code, string? description)
        {
            var specialization = new Specialization
            {
                Name = name,
                Code = code,
                Description = description
            };

            await _repo.AddAsync(specialization);
            await _unitOfWork.CommitAsync();

            return specialization.Name;
        }

        // Listar todas as especializações
        public async Task<List<string>> GetAllSpecializations()
        {
            var result = await _repo.GetAllAsync();
            return result.ConvertAll(cat => cat.Name);
        }

        // Buscar especialização por ID
        public async Task<string?> GetSpecializationById(long id)
        {
            var specialization = await _repo.GetByIdAsync(new SpecializationID(id));
            return specialization?.Name;
        }

        // Obtter especialização por Nome
        public async Task<List<SpecializationDTO>> SearchSpecializations(string? name, string? code,
                                                                string? description, int page, int pageSize)
        {
            var result = await _repo.SearchSpecializations(name, code, description, page, pageSize);
            return result.ConvertAll(MapToDTO);
        }

        // Atualizar uma especialização
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
