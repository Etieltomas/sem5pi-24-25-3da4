using Sempi5.Domain.OperationRequestEntity;

public interface IOperationTypeRepository
{
    Task<OperationType> GetOperationTypeById(OperationTypeID id);
     Task<IEnumerable<OperationType>> GetAllAsync();

        Task AddAsync(OperationType operationType);
}
