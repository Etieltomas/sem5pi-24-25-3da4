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
        
        public async Task<List<OperationRequest>> GetAllOperationRequests()
        {
            return await _context.OperationRequests
            .Include(r => r.Staff)
            .Include(r => r.Patient)
            .Include(r => r.OperationType).ToListAsync();
        }

        public async Task<List<OperationRequest>> GetAllOperationRequestsNotScheduled()
        {
            return await _context.OperationRequests
            .Include(r => r.Staff)
            .Include(r => r.Patient)
            .Include(r => r.OperationType)
            .Where(r => !r.Status.Value.ToLower().Equals("scheduled"))
            .ToListAsync();
        }

        public async Task<OperationRequest> GetOperationRequestById(OperationRequestID id)
        {
            
            return _context.OperationRequests
            .Include(r => r.Staff)
            .Include(r => r.Patient)
            .Include(r => r.OperationType)
            .FirstOrDefault(r => r.Id.Equals(id));

        }


        public async Task<List<OperationRequest>> SearchOperationRequests(string? patientName, string? operationType, string? priority, string? status, int page, int pageSize) 
        {
            return await _context.OperationRequests
            .Include(r => r.Staff)
            .Include(r => r.Patient)
            .Include(r => r.OperationType)
            .Where(r =>
                (string.IsNullOrEmpty(patientName) || r.Patient.Id.ToString().Contains(patientName)) &&
                (string.IsNullOrEmpty(operationType) || r.OperationType.Name.Contains(operationType)) &&
                (string.IsNullOrEmpty(priority) || r.Priority.Value.Equals(priority, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(status) || r.Status.Value.Equals(status, StringComparison.OrdinalIgnoreCase))
            ).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

    
        
        /*
        public async Task RemoveAsync(OperationRequest operationRequest)
        {
            operationRequest.MarkAsDeleted();
            _context.OperationRequests.Update(operationRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OperationRequest operationRequest)
        {
            _context.OperationRequests.Update(operationRequest); 
            await _context.SaveChangesAsync(); 
        }
        */

    }
}
