using Sempi5.Domain.OperationRequestEntity;
using System.Collections.Generic;
using System.Linq;

namespace Sempi5.Infrastructure
{
    public class OperationRequestRepository : IOperationRequestRepository
    {
        private readonly List<OperationRequest> _requests = new List<OperationRequest>();

        public OperationRequest GetOperationRequestById(OperationRequestID id)
        {
            return _requests.FirstOrDefault(r => r.Id.Equals(id));
        }

        public void AddOperationRequest(OperationRequest request)
        {
            _requests.Add(request);
        }

        public void UpdateOperationRequest(OperationRequest request)
        {
            var existingRequest = GetOperationRequestById(request.Id);
            if (existingRequest != null)
            {
                existingRequest.Priority = request.Priority;
                existingRequest.Deadline = request.Deadline;
                existingRequest.Status = request.Status; 
            }
        }

        public void DeleteOperationRequest(OperationRequestID id)
        {
            var request = GetOperationRequestById(id);
            if (request != null)
            {
                _requests.Remove(request);
            }
        }

        public List<OperationRequest> SearchOperationRequests(string? patientName, string? operationType, string? priority, string? status)
        {
            return _requests.Where(r =>
                (string.IsNullOrEmpty(patientName) || r.PatientId.ToString().Contains(patientName)) &&
                (string.IsNullOrEmpty(operationType) || r.OperationType.Name.Contains(operationType)) &&
                (string.IsNullOrEmpty(priority) || r.Priority.Value.Equals(priority, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(status) || r.Status.Value.Equals(status, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
    }
}
