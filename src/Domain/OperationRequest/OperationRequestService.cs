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

}
