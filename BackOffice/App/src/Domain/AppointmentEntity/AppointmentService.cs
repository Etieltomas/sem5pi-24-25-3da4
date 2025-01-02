using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;
using Sempi5.Infrastructure.Shared;
using Sempi5.Domain.OperationRequestEntity;
using Mono.TextTemplating;


namespace Sempi5.Domain.AppointmentEntity
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _repo;
        private readonly IOperationRequestRepository _operationRequestRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly Serilog.ILogger _logger;


        public AppointmentService(IAppointmentRepository repo, IUnitOfWork unitOfWork, IStaffRepository staffRepo, IRoomRepository roomRepo, IOperationRequestRepository operationRequestRepository, Serilog.ILogger logger)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._staffRepo = staffRepo;
            this._roomRepo = roomRepo;
            this._operationRequestRepo = operationRequestRepository;
            _logger = logger;
        }

        public async Task<List<AppointmentDTO>> GetAppointmentsByDoctor(string doctorEmail)
        {
            var appointments = await _repo.GetAppointmentsByDoctor(doctorEmail);
            
            if (appointments == null)
            {
                return null;
            }

            List<AppointmentDTO> appointmentDTOs = new List<AppointmentDTO>();
            foreach (var appointment in appointments)
            {
                appointmentDTOs.Add(await MapToDTO(appointment));
            }

            return appointmentDTOs;
        }

        public async Task<AppointmentDTO> EditAppointment(AppointmentDTO appointmentDTO, long id)
        {
            Appointment appointment = await _repo.GetAppointmentsByID(new AppointmentID(id));
            Room oldRoom = await _roomRepo.GetByIdAsync(appointment.Room.Id);
            if (appointment == null)
            {
                return null;
            }

            if(appointmentDTO.Room != null)
            {
                RemoveOldSlots(appointment, null, oldRoom);
                appointment.Room = appointmentDTO.Room.HasValue ? await _roomRepo.GetByIdAsync(new RoomID(appointmentDTO.Room.Value)) : null;
            }

            List<Staff> staffs = new List<Staff>();
            if(appointmentDTO.Team != null)
            {
                foreach (var staffEmail in appointmentDTO.Team)
                {
                    staffs.Add(await _staffRepo.GetStaffMemberByEmail(staffEmail));
                }
                RemoveOldSlots(appointment, staffs, null);
                appointment.OperationRequest.Staffs = staffs.Select(s => s.Id).ToList();
            }

            if(appointmentDTO.DateOperation != null)
            {
                if (staffs.Count == 0)
                {
                    foreach (var staffID in appointment.OperationRequest.Staffs)
                    {
                        staffs.Add(await _staffRepo.GetStaffMemberById(staffID));
                    }               
                }
                
                RemoveOldSlots(appointment, staffs, appointment.Room);
                appointment.DateOperation = new DateOperation(DateTime.ParseExact(appointmentDTO.DateOperation, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture));
            }

            bool available = await CheckAppointmentAvailability(appointment, staffs, appointment.Room);
            
            if (available) {
                foreach (var staff in staffs)
                {
                    _unitOfWork.MarkAsModified(staff);
                }
                
                if (appointmentDTO.Room != null)
                    _unitOfWork.MarkAsModified(oldRoom);

               _unitOfWork.MarkAsModified(appointment.Room);

                await _unitOfWork.CommitAsync();
                return await MapToDTO(appointment);
            }

            return null;
        }

        public async Task<AppointmentDTO> CreateAppointment(AppointmentDTO appointmentDTO)
        {

            _logger.Information("Creating appointment {@AppointmentDTO}", appointmentDTO);

            List<Staff> staffs = new List<Staff>();
            if(appointmentDTO.Team != null)
            {
                foreach (var staffEmail in appointmentDTO.Team)
                {
                    staffs.Add(await _staffRepo.GetStaffMemberByEmail(staffEmail));
                }
            }

            Appointment appointment = MapFromDTO(appointmentDTO);

            bool available = await CheckAppointmentAvailability(appointment, staffs, appointment.Room);

            if (available)
            {
                foreach (var staff in staffs)
                {
                    _unitOfWork.MarkAsModified(staff);
                }
                _unitOfWork.MarkAsModified(appointment.Room);

                appointment.OperationRequest.Staffs = staffs.Select(s => s.Id).ToList();

                await _repo.AddAsync(appointment);
            
                await _unitOfWork.CommitAsync();
                return await MapToDTO(appointment);
            }

            return null;
        }

        private void RemoveOldSlots(Appointment appointment, List<Staff> staffs, Room room)
        {
            if (room != null)
            {
                var slotToRemove = appointment.DateOperation.Value.ToString("dd-MM-yyyyTHH:mm");
                var slots = room.Slots.Where(s => s.ToString().StartsWith(slotToRemove)).ToList();
                foreach (var slot in slots)
                {
                    room.Slots.Remove(slot);
                }
            }

            if (staffs != null)
            {
                foreach (var staffMember in staffs)
                {
                    DateTime surgeryStart = appointment.DateOperation.Value;
                    var slotToRemove = appointment.DateOperation.Value.ToString("dd-MM-yyyyTHH:mm");

                    if (staffMember.Specialization.Name.ToLower().Equals("anaesthetist")) {
                        slotToRemove = surgeryStart.ToString("dd-MM-yyyyTHH:mm");
                    } else if (staffMember.SystemUser.Role.ToLower().Equals("assistant")) {
                        slotToRemove = surgeryStart.AddMinutes(appointment.OperationRequest.OperationType.Anesthesia_Duration+appointment.OperationRequest.OperationType.Surgery_Duration-1).ToString("dd-MM-yyyyTHH:mm");
                    } else {
                        slotToRemove = surgeryStart.AddMinutes(appointment.OperationRequest.OperationType.Anesthesia_Duration).ToString("dd-MM-yyyyTHH:mm");
                    }

                    var slots = staffMember.AvailabilitySlots.Where(s => s.ToString().StartsWith(slotToRemove)).ToList();
                    foreach (var slot in slots)
                    {
                        staffMember.AvailabilitySlots.Remove(slot);
                    }
                }
            }
        }

        public async Task<bool> CheckAppointmentAvailability(Appointment appointment, List<Staff> staffs, Room room)
        {
            // Calculate the total duration of the surgery
            int surgeryDuration = appointment.OperationRequest.OperationType.Anesthesia_Duration +
                                appointment.OperationRequest.OperationType.Surgery_Duration +
                                appointment.OperationRequest.OperationType.Cleaning_Duration;

            DateTime surgeryStart = appointment.DateOperation.Value;
            DateTime surgeryEnd = surgeryStart.AddMinutes(surgeryDuration);

            // Create lists to store new slots and old slots to remove
            List<Slot> slotsToAdd = new List<Slot>();

            // Check the availability of the room
            foreach (var slot in room.Slots)
            {
                var slotParts = slot.ToString().Split(" - ");
                if (slotParts.Length != 2)
                    throw new FormatException("Slot format is invalid. Expected format: 'dd-MM-yyyyTHH:mm:ss - dd-MM-yyyyTHH:mm:ss'.");

                DateTime startSlot = DateTime.ParseExact(slotParts[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
                DateTime endSlot = DateTime.ParseExact(slotParts[1], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

                if (!((startSlot < surgeryStart && endSlot < surgeryStart) ||
                    (startSlot > surgeryEnd && endSlot > surgeryEnd)))
                {
                    return false; 
                }
            }
                
            // Collect new slot to add
            var newSlotString = surgeryStart.ToString("dd-MM-yyyyTHH:mm:ss") + " - " + surgeryEnd.ToString("dd-MM-yyyyTHH:mm:ss");
            slotsToAdd.Add(new Slot(newSlotString));
        
            // After iterating, update the room's slots
            foreach (var slot in slotsToAdd)
            {
                room.Slots.Add(slot);  
            }

            if (!room.Slots.Any(s => s.ToString() == newSlotString))
            {
                return false;
            }

            // Create lists to store new staff member slots and old slots to remove
            List<AvailabilitySlot> staffSlotsToAdd = new List<AvailabilitySlot>();

            // Check the availability of each staff member
            foreach (var staffMember in staffs)
            {
                DateTime staffStart = surgeryStart;
                DateTime staffEnd = surgeryEnd;

                var busySlots = staffMember.AvailabilitySlots.Where(s => s.ToString().StartsWith(appointment.DateOperation.Value.ToString("dd-MM-yyyyT"))).ToList();

                if (staffMember.Specialization.Name.ToLower().Equals("anaesthetist")) {
                    staffEnd = surgeryEnd.AddMinutes(-appointment.OperationRequest.OperationType.Cleaning_Duration);
                } else if (staffMember.SystemUser.Role.ToLower().Equals("assistant")) {
                    staffStart = surgeryEnd.AddMinutes(-appointment.OperationRequest.OperationType.Cleaning_Duration);
                } else {
                    staffStart = surgeryStart.AddMinutes(appointment.OperationRequest.OperationType.Anesthesia_Duration);
                    staffEnd = surgeryEnd.AddMinutes(-appointment.OperationRequest.OperationType.Cleaning_Duration);
                }

                foreach (var slot in busySlots)
                {
                    var busySlotParts = slot.ToString().Split(" - ");
                    DateTime startSlot = DateTime.ParseExact(busySlotParts[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
                    DateTime endSlot = DateTime.ParseExact(busySlotParts[1], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

                    if (!((startSlot < staffStart && endSlot < staffStart) ||
                        (startSlot > staffEnd && endSlot > staffEnd)))
                    {
                        return false;  
                    }
                }

                var newStaffSlotString = staffStart.ToString("dd-MM-yyyyTHH:mm:ss") + " - " + staffEnd.ToString("dd-MM-yyyyTHH:mm:ss");
                //staffSlotsToAdd.Add(new AvailabilitySlot(newStaffSlotString));
                staffMember.AvailabilitySlots.Add(new AvailabilitySlot(newStaffSlotString));

                if (!staffMember.AvailabilitySlots.Any(s => s.ToString() == newStaffSlotString))
                {
                    return false;
                }
            }

            await _unitOfWork.CommitAsync();
            return true;
        }


        private async Task<AppointmentDTO> MapToDTO(Appointment appointment)
        {
            List<Staff> staffs = new List<Staff>();
            foreach (var staffId in appointment.OperationRequest.Staffs)
            {
                staffs.Add(await _staffRepo.GetStaffMemberById(staffId));
            }

            return new AppointmentDTO
            {
                Id = appointment.Id.AsLong(),
                AppointmentStatus = appointment.AppointmentStatus.ToString(),
                DateOperation = appointment.DateOperation.Value.ToString("dd-MM-yyyy HH:mm"),
                AppointmentType = appointment.AppointmentType.ToString(),
                OperationRequest = appointment.OperationRequest.Id.AsLong(),
                Room = appointment.Room.Id.AsLong(),
                Team = staffs.Select(s => s.Email.ToString()).ToList()
            };
        }

        private Appointment MapFromDTO(AppointmentDTO appointmentDTO)
        {
            var operationRequest = _operationRequestRepo.GetOperationRequestById(new OperationRequestID(appointmentDTO.OperationRequest.Value)).Result;
            var room = _roomRepo.GetByIdAsync(new RoomID(appointmentDTO.Room.Value)).Result;

            _logger.Information("Mapping appointmentDTO to Appointment {@AppointmentDTO}", appointmentDTO);

            DateTime dateOperation;
            // Corrigir o formato da string de data/hora
            string correctedDateOperation = appointmentDTO.DateOperation.Replace("T:", ":");

            if (!DateTime.TryParseExact(correctedDateOperation, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOperation))
            {
                _logger.Error("Invalid date format for DateOperation: {DateOperation}", appointmentDTO.DateOperation);
                throw new FormatException($"Invalid date format for DateOperation: {appointmentDTO.DateOperation}");
            }

            return new Appointment
            {
                AppointmentStatus = AppointmentStatusExtensions.FromString(appointmentDTO.AppointmentStatus),
                DateOperation = new DateOperation(dateOperation), 
                AppointmentType = AppointmentTypeExtensions.FromString(appointmentDTO.AppointmentType),
                OperationRequest = operationRequest,
                Room = room
            };
        }
    }
}