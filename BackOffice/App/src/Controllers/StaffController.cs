using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.TokenEntity;

namespace Sempi5.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly StaffService _service;   
        private readonly EmailService _emailService;   
        private readonly Cryptography _cryptography;
        private readonly Serilog.ILogger _logger; 
        private readonly string base_url;

        public StaffController(IConfiguration configuration, StaffService service, EmailService emailService, Serilog.ILogger logger, Cryptography cryptography)
        {
            _configuration = configuration;
            _service = service;
            _emailService = emailService; 
            _logger = logger; 
            _cryptography = cryptography;
            base_url = _configuration["IpAddresses:This"] ?? "http://localhost:5012";
        }

        /// <summary>
        /// Registers a new staff member.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="staffDTO">Staff member data to be registered.</param>
        /// <returns>The registered staff member with a CreatedAtAction response.</returns>
        [HttpPost("register")] 
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<StaffDTO>> RegisterStaff(StaffDTO staffDTO)
        {
            var staff = await _service.AddStaffMember(staffDTO); 
            return CreatedAtAction(nameof(GetStaffMember), new { id = staff.Id }, staff);
        }

        /// <summary>
        /// Retrieves all staff members.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <returns>A list of all staff members.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> GetAllStaffMembers()
        {
            return await _service.GetAllStaffMembers();
        }

        /// <summary>
        /// Searches for staff members based on given parameters.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="name">The name of the staff member.</param>
        /// <param name="email">The email of the staff member.</param>
        /// <param name="specialization">The specialization of the staff member.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of results per page.</param>
        /// <returns>A list of staff members matching the search criteria.</returns>
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<StaffDTO>>> Search(
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] string? specialization,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var staff = await _service.SearchStaff(name, email, specialization, page, pageSize);

            if (staff == null || staff.Count == 0)
            {
                return NotFound("No staff members found with the given criteria.");
            }

            return Ok(staff);
        }

        /// <summary>
        /// Confirms staff profile changes.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The encrypted email of the staff member.</param>
        /// <param name="json">The encrypted staff member data in JSON format.</param>
        /// <returns>A confirmation message if changes are processed successfully, otherwise a BadRequest result.</returns>
        [HttpGet("update/confirm-changes")]
        public async Task<ActionResult> EditStaffProfile(
            [FromQuery] string email,
            [FromQuery] string json
        )
        {
            try
            {
                var decryptedEmail = _cryptography.DecryptString(email);
                var decryptedJson = _cryptography.DecryptString(json);

                var editStaffDto = JsonSerializer.Deserialize<StaffDTO>(decryptedJson);
                var formerEmail = JsonSerializer.Deserialize<string>(decryptedEmail);

                if (editStaffDto == null || formerEmail == null)
                {
                    return BadRequest();
                }

                var newStaff = await _service.EditStaff(formerEmail.ToString(), editStaffDto, true);

                CreateLog(formerEmail, editStaffDto);

                return Ok(new { message = "Changes confirmed." });
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing your request." + ex.Message);
            }
        }

        /// <summary>
        /// Requests to edit a staff profile.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the staff member to edit.</param>
        /// <param name="editStaffDto">The staff data to update.</param>
        /// <returns>A confirmation message if the edit is requested successfully, otherwise an error response.</returns>
        [HttpPut("update/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AskEditStaffProfile(string email, [FromBody] StaffDTO editStaffDto)
        {
            var staff = await _service.GetStaffMemberByEmail(email);

            if (staff == null)
            {
                return NotFound();
            }

            var originalEmail = staff.Email;
            var originalPhone = staff.Phone;
            var originalAddress = staff.Address;
            var newStaff = await _service.EditStaff(email, editStaffDto, false);

            if (newStaff == null)
            {
                return NotFound();
            }

            if (!originalEmail.Equals(newStaff.Email) ||
                !originalPhone.Equals(newStaff.Phone) ||
                !originalAddress.Equals(newStaff.Address))
            {
                var confirmationLink = base_url + "/api/Staff/update/confirm-changes?email=" +
                                        Uri.EscapeDataString(_cryptography.EncryptString(JsonSerializer.Serialize(email))) + 
                                        "&json=" +
                                        Uri.EscapeDataString(_cryptography.EncryptString(JsonSerializer.Serialize(editStaffDto)));

                var message = CreateStaffUpdateEmail(staff.Name, editStaffDto, confirmationLink);

                _emailService.sendEmail(newStaff.Name, originalEmail, "Contact Information Updated", message);
            } 
            else 
            {
                CreateLog(email, editStaffDto);
            }

            return Ok(new { message = "Confirmation sent successfully." });
        }

        private string CreateStaffUpdateEmail(string name, StaffDTO editStaffDto, string confirmationLink)
        {
            var message = "<html>";
            message += "<body>";
            message += "<b>Hello,</b><br>";
            message += $"<p>{name} has been updated.</p>"; 

            if (!string.IsNullOrEmpty(editStaffDto.Email))
            {
                message += $"<p>Email: {editStaffDto.Email}</p>";
            }

            if (!string.IsNullOrEmpty(editStaffDto.Phone))
            {
                message += $"<p>Phone: {editStaffDto.Phone}</p>";
            }

            if (!string.IsNullOrEmpty(editStaffDto.Address))
            {
                message += $"<p>Address: {editStaffDto.Address}</p>";
            }

            if (!string.IsNullOrEmpty(editStaffDto.Specialization))
            {
                message += $"<p>Specialization: {editStaffDto.Specialization}</p>";
            }

            if (editStaffDto.AvailabilitySlots != null && editStaffDto.AvailabilitySlots.Count > 0)
            {
                message += "<p>Availability Slots:</p>";
                message += "<ul>";
                foreach (var slot in editStaffDto.AvailabilitySlots)
                {
                    message += $"<li>{slot}</li>";
                }
                message += "</ul>";
            }

            message += $"<p>Please <a href='{confirmationLink}'>Click here</a> to confirm the changes.</p>";
            message += "</body>";
            message += "</html>";

            return message;
        }

        private void CreateLog(string email, StaffDTO editStaffDTO)
        {
            var text = $"Staff member {email} has been updated with the following information:";

            if (editStaffDTO.AvailabilitySlots != null)
            {
                text += $" Availability Slots: {editStaffDTO.AvailabilitySlots},";
            }
            if (editStaffDTO.Specialization != null)
            {
                text += $" Specialization: {editStaffDTO.Specialization},";
            }
            if (editStaffDTO.Address != null)
            {
                text += $" Address: {editStaffDTO.Address},";
            }
            if (editStaffDTO.Phone != null)
            {
                text += $" Phone: {editStaffDTO.Phone},";
            }
            if (editStaffDTO.Email != null)
            {
                text += $" Email: {editStaffDTO.Email},";
            }

            _logger.ForContext("CustomLogLevel", "CustomLevel")
                    .Information(text.Remove(text.Length - 1));
        }

        /// <summary>
        /// Retrieves a specific staff member by their ID.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="id">The ID of the staff member to retrieve.</param>
        /// <returns>The staff member with the given ID or a NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDTO>> GetStaffMember(string id)
        {
            var staff = await _service.GetStaffMember(new StaffID(id));

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        /// <summary>
        /// Retrieves a specific staff member by their email.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the staff member to retrieve.</param>
        /// <returns>The staff member with the given email or a NotFound result.</returns>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<StaffDTO>> GetStaffMemberByEmail(string email)
        {
            var staff = await _service.GetStaffMemberByEmail(email);

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }
    }
}
