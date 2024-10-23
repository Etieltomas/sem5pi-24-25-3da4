using Sempi5.Domain.OperationRequestEntity;

public interface IOperationRequestRepository
{
    OperationRequest GetOperationRequestById(OperationRequestID id);
    void AddOperationRequest(OperationRequest request);
    void UpdateOperationRequest(OperationRequest request);
    void DeleteOperationRequest(OperationRequestID id);
    List<OperationRequest> SearchOperationRequests(string? patientName, string? operationType, string? priority, string? status);
}
