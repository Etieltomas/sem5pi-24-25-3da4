using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.Shared;

public interface IOperationTypeRepository : IRepository<OperationType, OperationTypeID>
{
    Task<OperationType> GetOperationTypeById(OperationTypeID id);
    Task<OperationType> GetOperationTypeByName(string name);
    Task<IEnumerable<OperationType>> GetAllAsync();
}
