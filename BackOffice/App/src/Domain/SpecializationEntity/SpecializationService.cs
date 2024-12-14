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

        public async Task<string> AddSpecialization(string name){

            var specialization = new Specialization{Name=name};

            await _repo.AddAsync(specialization);
            
            await _unitOfWork.CommitAsync();

            return specialization.Id.AsString();
        }

        // Listar todas as especializações
        public async Task<List<string>> GetAllSpecialization()
        {
            var result = await _repo.GetAllAsync();
            return result.ConvertAll(cat => cat.Id.AsString());
        }

        // Buscar especialização por ID
        public async Task<string?> GetSpecializationById(long id)
        {
            var specialization = await _repo.GetByIdAsync(new SpecializationID(id));
            return specialization?.Id.AsString();
        }

        // Atualizar uma especialização
        public async Task<bool> UpdateSpecialization(long id, string newName)
        {
            var specialization = await _repo.GetByIdAsync(new SpecializationID(id));
            if (specialization == null) return false;

            specialization.UpdateName(newName); 
            await _repo.UpdateAsync(specialization);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}