using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sempi5.Domain.AppointmentEntity;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

public class PlanningService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IOperationRequestRepository _operationRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _baseUrl;
    public PlanningService(IOperationRequestRepository operationRequestRepository, IUnitOfWork unitOfWork, IStaffRepository staffRepository, IRoomRepository roomRepository, HttpClient httpClient, IConfiguration configuration, IAppointmentRepository appointmentRepository)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _appointmentRepository = appointmentRepository;
        _roomRepository = roomRepository;
        _staffRepository = staffRepository;
        _unitOfWork = unitOfWork;
        _operationRequestRepository = operationRequestRepository;
        _baseUrl = _configuration["IpAddresses:Planning"] ?? "http://localhost:2000";
    }

    public async Task<Planning> ScheduleOperations(string day, long roomId,
                                List<OperationRequest> operationRequests)
    {
        Room room = await _roomRepository.GetByIdAsync(new RoomID(roomId));
        string json = await GetJson(day, room, operationRequests);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/obtain_better", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error while scheduling operations. Error code: " + response.StatusCode);
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        
        var planning = JsonSerializer.Deserialize<Planning>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return planning;
    }


    public async Task CreateAppointments(string day, List<List<int>> agendaRoom, long roomID, List<OperationRequest> operationRequests)
    {
        Room room = await _roomRepository.GetByIdAsync(new RoomID(roomID));
        foreach (var request in agendaRoom)
        {
            int startMinutes = request[0];
            int requestID = request[2];

            int year = int.Parse(day.Substring(0, 4));
            int month = int.Parse(day.Substring(4, 2));
            int dayOfMonth = int.Parse(day.Substring(6, 2));

            DateTime start = new DateTime(year, month, dayOfMonth, startMinutes / 60, startMinutes % 60, 0);
            var operationRequest = await _operationRequestRepository.GetOperationRequestById(new OperationRequestID(requestID));
            operationRequest.Status = Status.scheduled;
            var appointent = new Appointment {
                Room = room,
                AppointmentStatus = AppointmentStatus.Scheduled,
                DateOperation = new DateOperation(start),
                AppointmentType = AppointmentType.Surgery,
                OperationRequest = operationRequest
            };

            await _appointmentRepository.AddAsync(appointent);
            await _unitOfWork.CommitAsync();
        }

    }

    public async Task UpdateScheduleDoctors(List<AgendaDoctor> agendaDoctors, string day)
    {
        foreach (var agendaDoctor in agendaDoctors)
        {
            Staff staff = await _staffRepository.GetStaffMemberById(new StaffID(agendaDoctor.Id));
            List<Appointment> appointments = await _appointmentRepository.GetAppointmentsByStaff(staff);

            List<AvailabilitySlot> slots = new List<AvailabilitySlot>();

            int year = int.Parse(day.Substring(0, 4));
            int month = int.Parse(day.Substring(4, 2));
            int dayOfMonth = int.Parse(day.Substring(6, 2));

            DateTime targetDate = new DateTime(year, month, dayOfMonth);
            foreach (var slot in agendaDoctor.Slots)
            {
                DateTime start = new DateTime(year, month, dayOfMonth, slot[0] / 60, slot[0] % 60, 0);
                DateTime end = new DateTime(year, month, dayOfMonth, slot[1] / 60, slot[1] % 60, 0);

                slots.Add(new AvailabilitySlot($"{start:dd-MM-yyyyTHH:mm:ss} - {end:dd-MM-yyyyTHH:mm:ss}"));
            }

            staff.AvailabilitySlots = staff.AvailabilitySlots
                .Where(s => !s.ToString().StartsWith(targetDate.ToString("dd-MM-yyyyT"))) 
                .ToList();

            staff.AvailabilitySlots.AddRange(slots);
        }

        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateScheduleRoom(List<List<int>> agendaRoom, long roomId, string day)
    {
        Room room = await _roomRepository.GetByIdAsync(new RoomID(roomId));

        int year = int.Parse(day.Substring(0, 4));
        int month = int.Parse(day.Substring(4, 2));
        int dayOfMonth = int.Parse(day.Substring(6, 2));
        DateTime targetDate = new DateTime(year, month, dayOfMonth);

        List<Slot> slots = new List<Slot>();
        foreach (var slot in agendaRoom)
        {
            DateTime start = new DateTime(year, month, dayOfMonth, slot[0] / 60, slot[0] % 60, 0);
            DateTime end = new DateTime(year, month, dayOfMonth, slot[1] / 60, slot[1] % 60, 0);

            slots.Add(new Slot($"{start:dd-MM-yyyyTHH:mm:ss} - {end:dd-MM-yyyyTHH:mm:ss}"));
        }

        room.Slots = room.Slots
            .Where(s => !s.ToString().StartsWith(targetDate.ToString("dd-MM-yyyyT"))) 
            .ToList();

        room.Slots.AddRange(slots);

        await _unitOfWork.CommitAsync();
    }


    private async Task<string> GetJson(string day, Room room, List<OperationRequest> operationRequests)
    {
        List<StaffID> staffID = new List<StaffID>();
        foreach (var operationRequest in operationRequests)
        {
            foreach (var staffMember in operationRequest.Staffs)
            {
                if (!staffID.Contains(staffMember))
                {
                    staffID.Add(staffMember);
                }
            }
        }

        List<Staff> staff = new List<Staff>();
        foreach (var staffMember in staffID)
        {
            staff.Add(await _staffRepository.GetStaffMemberById(staffMember));
        }

        var facts = new List<string>();

        foreach (var fact in await FormatAgenda(day, staff))
        {
            facts.Add(fact);
        }
        foreach (var fact in await FormatTimetables(day, staff))
        {
            facts.Add(fact);
        }
        foreach (var fact in await FormatSurgery(operationRequests))
        {
            facts.Add(fact);
        }
        foreach (var fact in await FormatSurgeryID(operationRequests))
        {
            facts.Add(fact);
        }
        foreach (var fact in await FormatAssignment(operationRequests))
        {
            facts.Add(fact);
        }
        foreach (var fact in await FormatStaff(staff))
        {
            facts.Add(fact);
        }
        foreach (var fact in await FormatRoom(room, day))
        {
            facts.Add(fact);
        }

        var json = new
        {
            day = int.Parse(day),
            room = room.Id.AsLong(),
            facts
        };

        return JsonSerializer.Serialize(json).ToLower();
    }

    private async Task<List<string>> FormatAgenda(string day, List<Staff> staff)
    {
        //agenda_staff(d001,20241028,[(720,790,m01),(1080,1140,c01)]).
        List<string> agendas = new List<string>();

        foreach (var staffMember in staff)
        {
            StringBuilder agenda = new StringBuilder();

            List<Appointment> appointments = await _appointmentRepository.GetAppointmentsByStaff(staffMember);
            agenda.Append($"agenda_staff({staffMember.Id.AsString()},{day},[");
            foreach (var appointment in appointments)
            {
                var startMinutes = appointment.DateOperation.Value.Hour * 60 + appointment.DateOperation.Value.Minute;
                var endMinutes = startMinutes + appointment.OperationRequest.OperationType.Anesthesia_Duration +
                                                appointment.OperationRequest.OperationType.Surgery_Duration +
                                                appointment.OperationRequest.OperationType.Cleaning_Duration;

                agenda.Append($"({startMinutes},{endMinutes},{appointment.Id}),");
                if (appointment == appointments.Last())
                {
                    agenda.Remove(agenda.Length - 1, 1);
                }
            }
            agenda.Append("])");
            agendas.Add(agenda.ToString());
        }

        return agendas;
    }

    private async Task<List<string>> FormatTimetables(string day, List<Staff> staff)
    {
        List<string> timetables = new List<string>();

        //timetable(d001,20241028,(480,1200)).
        foreach (var staffMember in staff)
        {        
            List<AvailabilitySlot> slotDay = new List<AvailabilitySlot>();
            foreach (var availabilitySlot in staffMember.AvailabilitySlots)
            {
                // 21-10-2024T09:00:00 - 21-10-2024T11:00:00
                DateTime start = DateTime.ParseExact(availabilitySlot.ToString().Split(" - ")[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
                DateTime startDay = DateTime.ParseExact(day, "yyyyMMdd", CultureInfo.InvariantCulture);
                if (start.Day == startDay.Day)
                {
                    slotDay.Add(availabilitySlot);
                }
            }

            DateTime startShift = DateTime.ParseExact(slotDay.First().ToString().Split(" - ")[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endShift = DateTime.ParseExact(slotDay.Last().ToString().Split(" - ")[1], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

            int startMinutes = startShift.Hour * 60 + startShift.Minute;
            int endMinutes = endShift.Hour * 60 + endShift.Minute;

            timetables.Add($"timetable({staffMember.Id.AsString()},{day},({startMinutes},{endMinutes}))");
        }

        return timetables;
    }

    private async Task<List<string>> FormatStaff(List<Staff> staff)
    {
        // staff(d001,doctor,orthopaedist).
        List<string> staffJson = new List<string>();
        foreach (var staffMember in staff)
        {
            // TODO - CHECK THE SPECIALIZATION
            //staffJson.Add($"staff({staffMember.Id.AsString()},teste,{staffMember.Specialization.Name})");
          staffJson.Add($"staff({staffMember.Id.AsString()},{staffMember.SystemUser.Role},{staffMember.Specialization.Name})");

        }

        return staffJson;
    }

    private async Task<List<string>> FormatSurgery(List<OperationRequest> operationRequests)
    {
        //surgery(so2,15,20,15).
        List<string> surgeryJson = new List<string>();
        foreach (var operationRequest in operationRequests)
        {
            surgeryJson.Add($"surgery({operationRequest.OperationType.Id.AsLong()},{operationRequest.OperationType.Anesthesia_Duration},{operationRequest.OperationType.Surgery_Duration},{operationRequest.OperationType.Cleaning_Duration})");
        }

        return surgeryJson;
    }

    private async Task<List<string>> FormatSurgeryID(List<OperationRequest> operationRequests)
    {
        //surgery_id(so100001,so2).
        List<string> surgeryIdJson = new List<string>();
        foreach (var operationRequest in operationRequests)
        {
            surgeryIdJson.Add($"surgery_id({operationRequest.Id.AsString()},{operationRequest.OperationType.Id.AsLong()})");
        }
        return surgeryIdJson;
    }

    private async Task<List<string>> FormatAssignment(List<OperationRequest> operationRequests)
    {
        //assignment_surgery(so100001,d001).
        List<string> assignmentJson = new List<string>();
        foreach (var operationRequest in operationRequests)
        {
            foreach (var staffMember in operationRequest.Staffs)
            {
                assignmentJson.Add($"assignment_surgery({operationRequest.Id.AsString()},{staffMember.AsString()})");
            }
        }

        return assignmentJson;
    }

    /*private async Task<List<string>> FormatRoom(Room room, string day)
    {
        //agenda_operation_room(or1,20241028,[(520,579,so100000),(1000,1059,so099999)]).
        StringBuilder roomJson = new StringBuilder();
        List<Slot> slots = room.Slots;
        roomJson.Append($"agenda_operation_room({room.Id.AsLong()},{day},[");
        foreach (var slot in slots)
        {
            DateTime start = DateTime.ParseExact(slot.ToString().Split(" - ")[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(slot.ToString().Split(" - ")[1], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

            var startMinutes = start.Hour * 60 + start.Minute;
            var endMinutes = end.Hour * 60 + end.Minute; 
            roomJson.Append($"({startMinutes},{endMinutes}),");

            if (slot == slots.Last())
            {
                roomJson.Remove(roomJson.Length - 1, 1);
            }
        }
        roomJson.Append("])");

        return new List<string> { roomJson.ToString() };
    }*/

    private async Task<List<string>> FormatRoom(Room room, string day)
    {
        //agenda_operation_room(or1,20241028,[(520,579,so100000),(1000,1059,so099999)]).
        StringBuilder roomJson = new StringBuilder();

        roomJson.Append($"agenda_operation_room({room.Id.AsLong()},{day},[");
        foreach (var appointent in await _appointmentRepository.GetAppointmentsByRoom(room))
        {
            DateTime start = appointent.DateOperation.Value;
            DateTime end = appointent.DateOperation.Value.AddMinutes(appointent.OperationRequest.OperationType.Anesthesia_Duration +
                                                appointent.OperationRequest.OperationType.Surgery_Duration +
                                                appointent.OperationRequest.OperationType.Cleaning_Duration);

            var startMinutes = start.Hour * 60 + start.Minute;
            var endMinutes = end.Hour * 60 + end.Minute;
            roomJson.Append($"({startMinutes},{endMinutes},{appointent.Id.AsString()}),");
        }
        roomJson.Append("])");

        return new List<string> { roomJson.ToString() };
    }

    public async Task<List<StaffDTO>> GetAvailableStaffAsync()
    {
        var response = await _httpClient.GetAsync("http://localhost:5012/api/Staff/Available");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<StaffDTO>>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<StaffDTO>();
    }

    public async Task<List<OperationRequestDto>> GetScheduledOperationsAsync()
    {
        var response = await _httpClient.GetAsync("http://localhost:5012/api/OperationRequest");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var operations = JsonSerializer.Deserialize<List<OperationRequestDto>>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return operations?.Where(o => o.Status.Equals("scheduled", StringComparison.OrdinalIgnoreCase)).ToList()
               ?? new List<OperationRequestDto>();
    }

    public async Task<PlanningDto> GetPlanningAsync()
    {
        var availableStaffTask = GetAvailableStaffAsync();
        var scheduledOperationsTask = GetScheduledOperationsAsync();

        await Task.WhenAll(availableStaffTask, scheduledOperationsTask);

        return new PlanningDto
        {
            AvailableStaffs = availableStaffTask.Result ?? new List<StaffDTO>(),
            ScheduledOperations = scheduledOperationsTask.Result ?? new List<OperationRequestDto>()
        };
    }
}