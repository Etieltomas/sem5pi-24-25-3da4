using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Sempi5.Infrastructure
{
    public class OperationRequestRepository : BaseRepository<OperationRequest, OperationRequestID>, IOperationRequestRepository
    {

        private readonly DataBaseContext _context;
        private readonly Serilog.ILogger _logger;
        
        public OperationRequestRepository(DataBaseContext context, Serilog.ILogger logger) : base(context.OperationRequests)
        {
            _context = context;
            _logger = logger;
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
            List<OperationRequest> operationRequests = await _context.OperationRequests
                .Include(r => r.Staff)
                .Include(r => r.Patient)
                .Include(r => r.OperationType)
                .ToListAsync();
            
            List<OperationRequest> operationRequestsNotScheduled = new List<OperationRequest>();

            foreach (OperationRequest operationRequest in operationRequests)
            {
                if (!operationRequest.Status.Value.Equals("scheduled", StringComparison.OrdinalIgnoreCase))
                {
                    operationRequestsNotScheduled.Add(operationRequest);
                }
            }

            return operationRequestsNotScheduled;
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

public async Task<List<OperationRequest>> SearchOperationRequestsByStaff(StaffID id)
{
    return await _context.OperationRequests
        .Include(r => r.Staff)
        .Include(r => r.Patient)
        .Include(r => r.OperationType)
        .Where(r => r.Staff.Id.Equals(id)) // Filtra pelo ID do Staff
        .ToListAsync(); // Obtém a lista assincronamente
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
