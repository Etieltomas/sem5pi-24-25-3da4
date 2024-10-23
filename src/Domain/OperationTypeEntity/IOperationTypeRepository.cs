using Sempi5.Domain.OperationRequestEntity;

public interface IOperationTypeRepository
{
    OperationType GetOperationTypeById(OperationTypeID id);
}
