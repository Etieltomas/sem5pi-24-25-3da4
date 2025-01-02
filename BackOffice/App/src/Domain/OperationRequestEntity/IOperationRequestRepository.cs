using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

public interface IOperationRequestRepository : IRepository<OperationRequest, OperationRequestID>
{
    Task<OperationRequest> GetOperationRequestById(OperationRequestID id);
    Task<List<OperationRequest>> SearchOperationRequests(string? patientName, string? operationType, string? priority, string? status, int page, int pageSize);
    Task<List<OperationRequest>> GetAllOperationRequests();
    Task<List<OperationRequest>> GetAllOperationRequestsNotScheduled();
    Task<List<OperationRequest>> SearchOperationRequestsByStaff(StaffID staffId);

    //Task RemoveAsync(OperationRequest operationRequest);
    //Task UpdateAsync(OperationRequest operationRequest);
}
