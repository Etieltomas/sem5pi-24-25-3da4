using System.Globalization;
using System.Text;
using Sempi5.Domain.AppointmentEntity;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.StaffEntity;

namespace Sempi5.Domain.Planning
{
    public class PlanningService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly string _baseUrl;
        public PlanningService(HttpClient httpClient, IConfiguration configuration, IAppointmentRepository appointmentRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _appointmentRepository = appointmentRepository;
            _baseUrl = _configuration["IpAddresses:Planning"] ?? "http://localhost:2000";
        }

        public async Task ScheduleOperations(string day, Room room,
                                List<OperationRequest> operationRequests)
        {
            string json = await GetJson(day, room, operationRequests);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/obtain_better", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error while scheduling operations");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetJson(string day, Room room, List<OperationRequest> operationRequests)
        {
            List<Staff> staff = new List<Staff>();
            foreach (var operationRequest in operationRequests)
            {        
                foreach (var staffMember in operationRequest.Staffs)
                {
                    if (!staff.Contains(staffMember))
                    {
                        staff.Add(staffMember);
                    }
                }  
            }

            StringBuilder bigJson = new StringBuilder();
            bigJson.Append(await FormatAgenda(day, staff));
            bigJson.Append(await FormatTimetables(day, staff));
            bigJson.Append(await FormatStaff(staff));
            bigJson.Append(await FormatSurgery(operationRequests));
            bigJson.Append(await FormatSurgeryID(operationRequests));
            bigJson.Append(await FormatAssignment(operationRequests));
            bigJson.Append(await FormatRoom(room, day));

            return bigJson.ToString();
        }

        private async Task<string> FormatAgenda(string day, List<Staff> staff)
        {
            //agenda_staff(d001,20241028,[(720,790,m01),(1080,1140,c01)]).
            StringBuilder agenda = new StringBuilder();

            foreach (var staffMember in staff)
            {
                List<Appointment> appointments = await _appointmentRepository.GetAppointmentsByStaff(staffMember);
                agenda.Append($"agenda_staff({staffMember.Id},{day},[");
                foreach (var appointment in appointments)
                {
                    var startMinutes = appointment.DateOperation.Value.Hour * 60 + appointment.DateOperation.Value.Minute;
                    var endMinutes = startMinutes + appointment.OperationRequest.OperationType.Anesthesia_Duration +
                                                    appointment.OperationRequest.OperationType.Surgery_Duration +
                                                    appointment.OperationRequest.OperationType.Cleaning_Duration;   

                    agenda.Append($"({startMinutes},{endMinutes},{appointment.Id}),");
                }
                agenda.Append("]).");
            }

            return agenda.ToString();
        }

        private async Task<string> FormatTimetables(string day, List<Staff> staff)
        {
            //timetable(d001,20241028,(480,1200)).
            StringBuilder timetable = new StringBuilder();
            foreach (var staffMember in staff)
            {
                List<AvailabilitySlot> slotDay = new List<AvailabilitySlot>();
                foreach (var availabilitySlot in staffMember.AvailabilitySlots)
                {
                    // 21-10-2024T09:00:00 - 21-10-2024T11:00:00
                    DateTime start = DateTime.ParseExact(availabilitySlot.ToString().Split(" - ")[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
                    DateTime startDay = DateTime.ParseExact(day, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    if (start.Day == startDay.Day)
                    {
                        slotDay.Add(availabilitySlot);
                    }
                }

                DateTime startShift = DateTime.ParseExact(slotDay.First().ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime endShift = DateTime.ParseExact(slotDay.Last().ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                int startMinutes = startShift.Hour * 60 + startShift.Minute;
                int endMinutes = endShift.Hour * 60 +endShift.Minute;

                timetable.Append($"timetable({staffMember.Id},{day},({startMinutes},{endMinutes})).");
            }

            return timetable.ToString();
        }

        private async Task<string> FormatStaff(List<Staff> staff)
        {
            // staff(d001,doctor,orthopaedist).

            StringBuilder staffJson = new StringBuilder();
            foreach (var staffMember in staff)
            {
                // TODO - CHECK THE SPECIALIZATION
                staffJson.Append($"staff({staffMember.Id},{staffMember.SystemUser.Role},{staffMember.Specialization.ToString()}).");
            }

            return staffJson.ToString();
        }

        private async Task<string> FormatSurgery(List<OperationRequest> operationRequests)
        {
            //surgery(so2,15,20,15).
            StringBuilder surgeryJson = new StringBuilder();
            foreach (var operationRequest in operationRequests)
            {
                surgeryJson.Append($"surgery({operationRequest.Id},{operationRequest.OperationType.Anesthesia_Duration},{operationRequest.OperationType.Surgery_Duration},{operationRequest.OperationType.Cleaning_Duration}).");
            }

            return surgeryJson.ToString();
        }

        private async Task<string> FormatSurgeryID(List<OperationRequest> operationRequests)
        {
            //surgery_id(so100001,so2).
            StringBuilder surgeryIdJson = new StringBuilder();
            foreach (var operationRequest in operationRequests)
            {
                surgeryIdJson.Append($"surgery_id({operationRequest.Id},{operationRequest.OperationType.Id}).");
            }
            return surgeryIdJson.ToString();
        }

        private async Task<string> FormatAssignment(List<OperationRequest> operationRequests)
        {
            //assignment_surgery(so100001,d001).
            StringBuilder assignmentJson = new StringBuilder();
            foreach (var operationRequest in operationRequests)
            {
                foreach (var staffMember in operationRequest.Staffs)
                {
                    assignmentJson.Append($"assignment_surgery({operationRequest.Id},{staffMember.Id}).");
                }
            }

            return assignmentJson.ToString();
        }

        /*private async Task<string> FormatRoom(Room room, string day)
        {
            //agenda_operation_room(or1,20241028,[(520,579,so100000),(1000,1059,so099999)]).
            StringBuilder roomJson = new StringBuilder();
            List<Slot> slots = room.Slots;
            roomJson.Append($"agenda_operation_room({room.Id},{day},[");
            foreach (var slot in slots)
            {
                DateTime start = DateTime.ParseExact(slot.ToString().Split(" - ")[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(slot.ToString().Split(" - ")[1], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

                var startMinutes = start.Hour * 60 + start.Minute;
                var endMinutes = end.Hour * 60 + end.Minute; 
                roomJson.Append($"({startMinutes},{endMinutes}),");
            }
            roomJson.Append("]).");

            return roomJson.ToString();
        }*/

        private async Task<string> FormatRoom(Room room, string day)
        {
            //agenda_operation_room(or1,20241028,[(520,579,so100000),(1000,1059,so099999)]).
            StringBuilder roomJson = new StringBuilder();
            List<Slot> slots = room.Slots;
            roomJson.Append($"agenda_operation_room({room.Id},{day},[");
            foreach (var appointent in await _appointmentRepository.GetAppointmentsByRoom(room))
            {
                DateTime start = appointent.DateOperation.Value;
                DateTime end = appointent.DateOperation.Value.AddMinutes(appointent.OperationRequest.OperationType.Anesthesia_Duration +
                                                    appointent.OperationRequest.OperationType.Surgery_Duration +
                                                    appointent.OperationRequest.OperationType.Cleaning_Duration);

                var startMinutes = start.Hour * 60 + start.Minute;
                var endMinutes = end.Hour * 60 + end.Minute; 
                roomJson.Append($"({startMinutes},{endMinutes},{appointent.Id}),");
            }
            roomJson.Append("]).");

            return roomJson.ToString();
        }
    }
}
