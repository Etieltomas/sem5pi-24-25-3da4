using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Sempi5.Infrastructure
{
    public class OperationRequestRepository : BaseRepository<OperationRequest, OperationRequestID>, IOperationRequestRepository
    {

        private readonly DataBaseContext _context;
        
        public OperationRequestRepository(DataBaseContext context) : base(context.OperationRequests)
        {
            _context = context;
        }

        public async Task<OperationRequest> GetOperationRequestById(OperationRequestID id)
        {
            return _context.OperationRequests.FirstOrDefault(r => r.Id.Equals(id));
        }


        public async Task<List<OperationRequest>> SearchOperationRequests(string? patientName, string? operationType, string? priority, string? status)
        {
            return await _context.OperationRequests.Where(r =>
                (string.IsNullOrEmpty(patientName) || r.Patient.Id.ToString().Contains(patientName)) &&
                (string.IsNullOrEmpty(operationType) || r.OperationType.Name.Contains(operationType)) &&
                (string.IsNullOrEmpty(priority) || r.Priority.Value.Equals(priority, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(status) || r.Status.Value.Equals(status, StringComparison.OrdinalIgnoreCase))
            ).ToListAsync();
        }
    }
}
