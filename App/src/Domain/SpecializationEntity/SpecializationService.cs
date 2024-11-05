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

            var specialization = new Specialization(new SpecializationID(name));

            await _repo.AddAsync(specialization);
            
            await _unitOfWork.CommitAsync();

            return specialization.Id.AsString();
        }

        public async Task<List<string>> GetAllSpecialization(){
            var result = await _repo.GetAllAsync();

            List<string> listDto = result.ConvertAll(cat => cat.Id.AsString());

            return listDto;
        }
    }
}