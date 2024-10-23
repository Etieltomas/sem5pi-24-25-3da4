using Sempi5.Domain.OperationRequestEntity;

public interface IOperationTypeRepository
{
    Task<OperationType> GetOperationTypeById(OperationTypeID id);
}
