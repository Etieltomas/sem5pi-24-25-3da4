using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.Patient;
using Sempi5.Domain.Staff;
using System;
using System.Collections.Generic;

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
    public OperationRequest CreateOperationRequest(OperationRequestCreateDTO dto)
    {
        //operation type by id
        var operationType = _operationTypeRepository.GetOperationTypeById(new OperationTypeID(dto.OperationTypeId.ToString()));
        if (operationType == null)
        {
            throw new Exception("Operation Type not found.");
        }

        //verify specialization
        var staff = _staffRepository.GetStaffById(new StaffID(dto.StaffId.ToString()));
        if (staff.Specialization != operationType.Specialization)
        {
            throw new Exception("The staff does not have the necessary specialization for this type of operation.");
        }

        var operationRequest = new OperationRequest(
            new PatientID(dto.PatientId.ToString()),
            new StaffID(dto.StaffId.ToString()),
            operationType,
            Priority.FromString(dto.Priority),
            new Deadline(dto.Deadline),
            Status.Pending //initial status
        );

        _operationRequestRepository.AddOperationRequest(operationRequest);
        
        return operationRequest;
    }

    //update
    public OperationRequest UpdateOperationRequest(int operationRequestId, OperationRequestUpdateDTO dto)
    {
        //get request by id
        var operationRequest = _operationRequestRepository.GetOperationRequestById(new OperationRequestID(dto.OperationRequestId.ToString()));
        if (operationRequest == null)
        {
            throw new Exception("Operation request not found.");
        }

        //verify staff
        if (operationRequest.StaffId != new StaffID(dto.StaffId.ToString()))
        {
            throw new Exception("Only the creator of the request can update it.");
        }

        //update
        operationRequest.UpdatePriority(Priority.FromString(dto.NewPriority));
        operationRequest.UpdateDeadline(new Deadline(dto.NewDeadline));

        //add to repository
        _operationRequestRepository.UpdateOperationRequest(operationRequest);
        
        return operationRequest;
    }

    //delete request
    public void DeleteOperationRequest(int requestId, int staffId)
    {
        //get request by id
        var operationRequest = _operationRequestRepository.GetOperationRequestById(new OperationRequestID(requestId.ToString()));
        if (operationRequest == null)
        {
            throw new Exception("Operation request not found.");
        }

        //verification 
        if (operationRequest.StaffId != new StaffID(staffId.ToString()))
        {
            throw new Exception("Only the creator of the request can delete it.");
        }

        //verify status
        if (operationRequest.Status == Status.Pending)
        {
            throw new Exception("Already scheduled operation requests cannot be removed.");
        }

        //delete request
        _operationRequestRepository.DeleteOperationRequest(new OperationRequestID(requestId.ToString()));
    }



}
