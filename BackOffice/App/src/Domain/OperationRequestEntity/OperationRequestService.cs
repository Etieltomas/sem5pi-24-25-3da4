using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

public class OperationRequestService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IOperationRequestRepository _operationRequestRepository;
    private readonly IOperationTypeRepository _operationTypeRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly Serilog.ILogger _logger;

    public OperationRequestService(
        IOperationRequestRepository operationRequestRepository,
        IOperationTypeRepository operationTypeRepository,
        IStaffRepository staffRepository,
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        Serilog.ILogger logger)
    {
        _operationRequestRepository = operationRequestRepository;
        _operationTypeRepository = operationTypeRepository;
        _staffRepository = staffRepository;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /**
        * Create a new operation request
        * @param dto OperationRequestCreateDTO
        * @return OperationRequestCreateDTO
        * author Ricardo Guimarães
        * date 10/12/2024
        */
    public async Task<OperationRequestCreateDTO> CreateOperationRequest(OperationRequestCreateDTO dto)
    {
        //operation type by id
        var operationType = await _operationTypeRepository.GetOperationTypeByName(dto.OperationTypeId.ToString());
        if (operationType == null)
        {
            throw new Exception("Operation Type not found.");
        }

        //verify specialization
        var staff = await _staffRepository.GetStaffMemberById(new StaffID(dto.StaffId.ToString()));
        if (staff.Specialization != operationType.Specialization)
        {
            throw new Exception("The staff does not have the necessary specialization for this type of operation.");
        }

        var patient = await _patientRepository.GetPatientById(new PatientID(dto.PatientId.ToString()));
        if (patient == null)
        {   
            // patient not found
            throw new Exception("Patient not found.");
        }

        var operationRequest = new OperationRequest{
            Patient = patient,
            Staff = staff,
            OperationType = operationType,
            Priority = Priority.FromString(dto.Priority),
            Deadline = new Deadline(DateTime.ParseExact(dto.Deadline, "dd-MM-yyyy", CultureInfo.InvariantCulture)),
            Status = Status.Pending //initial status
        };

        var newOperationRequest = await _operationRequestRepository.AddAsync(operationRequest);
        
        await _unitOfWork.CommitAsync();

        return ConvertToDTO(newOperationRequest);
    }


    /**
        * Update an existing operation request
        * @param dto OperationRequestUpdateDTO
        * @return OperationRequestCreateDTO
        * author Ricardo Guimarães
        * date 10/12/2024
        */
    public async Task<OperationRequestCreateDTO> UpdateOperationRequest(OperationRequestUpdateDTO dto)
    {
        //get request by id
        var operationRequest = await _operationRequestRepository.GetOperationRequestById(new OperationRequestID(long.Parse(dto.OperationRequestId.ToString())));
        if (operationRequest == null)
        {
            throw new Exception("Operation request not found.");
        }

        //verify staff
        if (operationRequest.Staff.Id != new StaffID(dto.StaffId.ToString()))
        {
            throw new Exception("Only the creator of the request can update it.");
        }

        //update
        operationRequest.UpdatePriority(Priority.FromString(dto.NewPriority));
        operationRequest.UpdateDeadline(new Deadline(DateTime.ParseExact(dto.NewDeadline, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

        //add to repository
        //var newOperationRequest = await _operationRequestRepository.AddAsync(operationRequest);
        
        await _unitOfWork.CommitAsync();

        UpdateOperationRequestLog(dto.OperationRequestId, $"Priority: {dto.NewPriority}, Deadline: {dto.NewDeadline}");

        return ConvertToDTO(operationRequest);
    }

    /**
        * Delete an existing operation request
        * @param requestId string
        * @param staffId string
        * author Ricardo Guimarães
        * date 10/12/2024
        */
    public async void DeleteOperationRequest(string requestId, string staffId)
    {
        //get request by id
        var operationRequest = await _operationRequestRepository.GetOperationRequestById(new OperationRequestID(long.Parse(requestId)));
        if (operationRequest == null)
        {
            throw new BusinessRuleValidationException("Operation request not found.");
        }

        //verification 
        if (operationRequest.Staff.Id != new StaffID(staffId.ToString()))
        {
            throw new Exception("Only the creator of the request can delete it.");
        }

        //verify status
        if (operationRequest.Status == Status.Pending)
        {
            throw new Exception("Already scheduled operation requests cannot be removed.");
        }

        //delete request
         _operationRequestRepository.Remove(operationRequest);

         await _unitOfWork.CommitAsync();
    }

     /**
        * Search operation requests
        * @param patientName string
        * @param operationType string
        * @param priority string
        * @param status string
        * @param page int
        * @param pageSize int
        * @return List<OperationRequestCreateDTO>
        * author Ricardo Guimarães
        * date 10/12/2024
        */
    public async Task<List<OperationRequestCreateDTO>> SearchOperationRequests(string? patientName, string? operationType, string? priority, string? status,int page, int pageSize)
    {
        var operationRequests = await _operationRequestRepository.SearchOperationRequests(patientName, operationType, priority, status, page, pageSize);

        return operationRequests.Select(ConvertToDTO).ToList();
    }

    public async Task<List<OperationRequestDto>> GetOperationRequestByStaff(string staffEmail)
{
    var staff = await _staffRepository.GetStaffMemberByEmail(staffEmail);

    var staffId = staff.Id.Value;

    var result = await _operationRequestRepository.SearchOperationRequestsByStaff(new StaffID(staffId));

    var operationRequestDtos = new List<OperationRequestDto>();

    try
    {
        foreach (var request in result)
        {
            // Convert each request to DTO and add to the list
            var dto = ConvertToOperationRequestDto(request);
            operationRequestDtos.Add(dto);
        }

        _logger.Information("Converted operation requests to DTOs: {@OperationRequestDtos}", operationRequestDtos);
        return operationRequestDtos;
    }
    catch (Exception ex)
    {
        _logger.Error("Error converting operation requests to DTOs: {message}", ex.Message);
        throw;
    }
}

/**
    * Search for operation requests based on various criteria.
    * @param patientName string?
    * @param operationType string?
    * @param priority string?
    * @param status string?
    * @param page int
    * @param pageSize int
    * @return List<OperationRequestDto>
    * author Ricardo Guimarães
    * date 10/12/2024
    */
    private OperationRequestCreateDTO ConvertToDTO(OperationRequest newOperationRequest)
    {
        return new OperationRequestCreateDTO
        {
            PatientId = newOperationRequest.Patient.Id.Value,
            StaffId = newOperationRequest.Staff.Id.Value,
            OperationTypeId = newOperationRequest.OperationType.Id.Value,
            Priority = newOperationRequest.Priority.Value,
            Deadline = newOperationRequest.Deadline.Value.ToString("dd-MM-yyyy")
        };
    }
    
    /**
    * Convert an OperationRequest to an OperationRequestDto.
    * @param operationRequest OperationRequest
    * @return OperationRequestDto
    * author Ricardo Guimarães
    * date 10/12/2024
    */
    private OperationRequestDto ConvertToOperationRequestDto(OperationRequest operationRequest)
    {
        return new OperationRequestDto
        {
            Id = int.Parse(operationRequest.Id.Value),
        PatientName = operationRequest.Patient?.Name?.ToString() ?? null, 
        OperationType = operationRequest.OperationType?.Name?.ToString() ?? null, 
        Priority = operationRequest.Priority?.ToString() ?? null, 
        Deadline = operationRequest.Deadline?.Value.ToString("dd-MM-yyyy") ?? null, 
        StartDate = operationRequest.Deadline?.Value.ToString("dd-MM-yyyy") ?? null, 
        EndDate = operationRequest.Deadline?.Value.ToString("dd-MM-yyyy") ?? null, 
        Status = operationRequest.Status?.Value ?? null 
        };
    }

/**
    * Update the operation request log.
    * @param operationRequestId string
    * @param updatedInfo string
    * author Ricardo Guimarães
    * date 10/12/2024
    */
    private void UpdateOperationRequestLog(string operationRequestId, string updatedInfo)
    {
    var logMessage = $"\n - OperationRequestID: {operationRequestId}, Updated Information: {updatedInfo}.";

    _logger.ForContext("CustomLogLevel", "CustomLevel")
           .Information(logMessage.Trim());
    }

}
