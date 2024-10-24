using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.StaffEntity;
using System;
using System.Collections.Generic;
using System.Globalization;

public class OperationRequestService
{
    private readonly IOperationRequestRepository _operationRequestRepository;
    private readonly IOperationTypeRepository _operationTypeRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IPatientRepository _patientRepository;

    public OperationRequestService(
        IOperationRequestRepository operationRequestRepository,
        IOperationTypeRepository operationTypeRepository,
        IStaffRepository staffRepository,
        IPatientRepository patientRepository)
    {
        _operationRequestRepository = operationRequestRepository;
        _operationTypeRepository = operationTypeRepository;
        _staffRepository = staffRepository;
        _patientRepository = patientRepository;
    }

    //create operation request
    public async Task<OperationRequestCreateDTO> CreateOperationRequest(OperationRequestCreateDTO dto)
    {
        //operation type by id
        var operationType = await _operationTypeRepository.GetOperationTypeById(new OperationTypeID(dto.OperationTypeId.ToString()));
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
        
        return ConvertToDTO(newOperationRequest);
    }


    //update
    public async Task<OperationRequestCreateDTO> UpdateOperationRequest(OperationRequestUpdateDTO dto)
    {
        //get request by id
        var operationRequest = await _operationRequestRepository.GetOperationRequestById(new OperationRequestID(dto.OperationRequestId.ToString()));
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
        operationRequest.UpdateDeadline(new Deadline(dto.NewDeadline));

        //add to repository
        var newOperationRequest = await _operationRequestRepository.AddAsync(operationRequest);

        return ConvertToDTO(newOperationRequest);
    }

    //delete request
    public async void DeleteOperationRequest(int requestId, int staffId)
    {
        //get request by id
        var operationRequest = await _operationRequestRepository.GetOperationRequestById(new OperationRequestID(requestId.ToString()));
        if (operationRequest == null)
        {
            throw new Exception("Operation request not found.");
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
    }

     //list request
    public async Task<List<OperationRequestCreateDTO>> SearchOperationRequests(string? patientName, string? operationType, string? priority, string? status)
    {
        var operationRequests = await _operationRequestRepository.SearchOperationRequests(patientName, operationType, priority, status);

        return operationRequests.Select(ConvertToDTO).ToList();
    }



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
}
