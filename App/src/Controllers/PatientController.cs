using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.TokenEntity;


namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {

        private readonly PatientService _service;
        private readonly EmailService _emailService;
        private readonly Cryptography _cryptography;
        private readonly Serilog.ILogger _logger;

        public PatientController(PatientService service, EmailService emailService, Serilog.ILogger logger, Cryptography cryptography)
        {
            _service = service;
            _emailService = emailService;
            _logger = logger;
            _cryptography = cryptography;
        }

        [HttpPut("associate/{email}")]
        [Authorize]
        public async Task<IActionResult> AssociateAccount(string email)
        {
            var cookie = User.Identity as ClaimsIdentity;
            var emailCookie = cookie?.FindFirst(ClaimTypes.Email)?.Value;

            var patient = await _service.AssociateAccount(email, emailCookie);
            
            if (patient == null)
            {
                return BadRequest();
            }

            return Ok(new { sucess = true });
        }

        // Function to create patient
        [HttpPost("register")]
        [Authorize(Roles = "Admin, Patient")]  
        public async Task<ActionResult<PatientDTO>> RegisterPatient(PatientDTO PatientDTO)
        {
            var patient = await _service.AddPatient(PatientDTO);

            return CreatedAtAction(nameof(GetPatient), new { id = patient.MedicalRecordNumber }, patient);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllPatients()
        {
            return Ok(await _service.GetAllPatients());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(string id)
        {
            var patient = await _service.GetPatientByMedicalRecordNumber(new PatientID(id));

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<PatientDTO>> GetPatientByEmail(string email)
        {
            var patient = await _service.GetPatientByEmail(email);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<PatientDTO>> GetPatientByName(string name)
        {
            var patient = await _service.GetPatientByName(name);

            if (patient == null)
            {
                return NotFound();
            }
            
            return Ok(patient);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PatientDTO>>> SearchPatients(
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] string? dateOfBirth,
            [FromQuery] string? medicalRecordNumber,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var patients = await _service.SearchPatients(name, email, dateOfBirth, medicalRecordNumber, page, pageSize);

            if (patients == null || !patients.Any())
            {
                return NotFound("No patients found with the given criteria");
            }

            return Ok(patients);
        }

        [HttpPut("update")]
        [Authorize (Roles = "Patient")]
        public async Task<ActionResult> UpdatePatient(PatientDTO editPatientDTO)
        {
            var cookie = User.Identity as ClaimsIdentity;
            var emailCookie = cookie?.FindFirst(ClaimTypes.Email)?.Value;

            var myProfile = await _service.GetPatientByEmail(emailCookie);

            if (myProfile == null)
            {
                return NotFound();
            }

            var originalName = myProfile.Name;
            var originalPhone = myProfile.Phone;
            var originalEmail = myProfile.Email;
            var originalAddress = myProfile.Address;
            // Ver se é para adicionar preferências

            var newPatientProfile = await _service.UpdatePatient(emailCookie, editPatientDTO, false);

            if (newPatientProfile == null)
            {
                return NotFound();
            }

            if (!originalEmail.Equals(newPatientProfile.Email) ||
                !originalPhone.Equals(newPatientProfile.Phone) ||
                !originalAddress.Equals(newPatientProfile.Address))
            {
                var confirmationLink = "http://localhost:5012/api/Patient/update/confirm-changes?email=" +
                        Uri.EscapeDataString(_cryptography.EncryptString(JsonSerializer.Serialize(emailCookie))) + 
                        "&json=" +
                        Uri.EscapeDataString(_cryptography.EncryptString(JsonSerializer.Serialize(editPatientDTO)));

                var message = CreatePatientUpdateEmail(originalName, editPatientDTO, confirmationLink);

                _emailService.sendEmail(newPatientProfile.Name, originalEmail, "Contact Information Updated", message);

                return Ok(new { message = "Confirmation sent successfully." });

            } else {
                CreateLog(emailCookie, editPatientDTO);
                
                return Ok(new { message = "Update successful." });
            }
        }

        public string CreatePatientUpdateEmail(string name, PatientDTO editPatientDto, string confirmationLink)
        {
            var message = "<html>";
            message += "<body>";
            message += "<b>Hello,</b><br>";
            message += $"<p>{name} has been updated.</p>"; 

            if (!string.IsNullOrEmpty(editPatientDto.Name))
            {
                message += $"<p>Name: {editPatientDto.Name}</p>";
            }

            if (!string.IsNullOrEmpty(editPatientDto.Email))
            {
                message += $"<p>Email: {editPatientDto.Email}</p>";
            }

            if (!string.IsNullOrEmpty(editPatientDto.Phone))
            {
                message += $"<p>Phone: {editPatientDto.Phone}</p>";
            }

            if (!string.IsNullOrEmpty(editPatientDto.Address))
            {
                message += $"<p>Address: {editPatientDto.Address}</p>";
            }

            message += $"<p>Please <a href='{confirmationLink}'>Click here</a> to confirm the changes.</p>";
            message += "</body>";
            message += "</html>";

            return message;
        }

        [HttpGet("update/confirm-changes")]
        public async Task<ActionResult> EditPatientProfile(
            [FromQuery] string email,
            [FromQuery] string json
        )
        {
            try
            {
                var decryptedEmail = _cryptography.DecryptString(email);
                var decryptedJson = _cryptography.DecryptString(json);

                var editPatientDto = JsonSerializer.Deserialize<PatientDTO>(decryptedJson);
                var formerEmail = JsonSerializer.Deserialize<string>(decryptedEmail);

                if (editPatientDto == null || formerEmail == null)
                {
                    return BadRequest();
                }

                var newPatient = await _service.UpdatePatient(formerEmail.ToString(), editPatientDto, true);

                CreateLog(formerEmail, editPatientDto);
                
                return Ok(new { message = "Changes confirmed." });
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing your request."+ex.Message);
            }
        }

        private void CreateLog(string email, PatientDTO editPatientDTO)
        {
            var text = $"Patiente with email {email} has been updated with the following information:";

            if (editPatientDTO.Name != null)
            {
                text += $" Name: {editPatientDTO.Name},";
            }
            if (editPatientDTO.Email != null)
            {
                text += $" Email: {editPatientDTO.Email},";
            }
            if (editPatientDTO.Phone != null)
            {
                text += $" Phone: {editPatientDTO.Phone},";
            }
            if (editPatientDTO.Address != null)
            {
                text += $" Address: {editPatientDTO.Address},";
            }

            _logger.ForContext("CustomLogLevel", "CustomLevel")
                    .Information(text.Remove(text.Length - 1));
        }

        [HttpPost("request-delete")]
        [Authorize (Roles = "Patient")]
        public async Task<IActionResult> RequestDeleteAccount(){

            var cookie = User.Identity as ClaimsIdentity;
            var emailCookie = cookie?.FindFirst(ClaimTypes.Email)?.Value;

            var patient = await _service.GetPatientByEmail(emailCookie);

            if (patient == null)
            {
                return NotFound();
            }

            var confirmationLink = "http://localhost:5012/api/Patient/request-delete/confirm-deletion?email=" +
                        Uri.EscapeDataString(_cryptography.EncryptString(JsonSerializer.Serialize(emailCookie)));

            var message = CreateDeleteAccountEmail(patient.Name, confirmationLink);

            _emailService.sendEmail(patient.Name, emailCookie, "Account Deletion Request", message);

            return Ok(new { message = "Confirmation sent successfully." });
        }

        public string CreateDeleteAccountEmail(string name, string confirmationLink)
        {
            var message = "<html>";
            message += "<body>";
            message += "<b>Hello,</b><br>";
            message += $"<p>{name} has requested to delete their account.</p>"; 
            message += $"<p>Please <a href='{confirmationLink}'>Click here</a> to confirm the deletion.</p>";
            message += "</body>";
            message += "</html>";

            return message;
        }

        [HttpGet("request-delete/confirm-deletion")]
        public async Task<ActionResult> DeleteAccount(
            [FromQuery] string email
        )
        {
            try
            {
                var decryptedEmail = _cryptography.DecryptString(email);

                var formerEmail = JsonSerializer.Deserialize<string>(decryptedEmail);

                var deleteScheduled = await _service.ScheduleDeletion(formerEmail.ToString());

                if (!deleteScheduled)
                {
                    return BadRequest("Deletion could not be scheduled.");
                }

                //CreateLog(formerEmail, editPatientDto);

                var numberOfDaysUntilDelete = 30;
                
                return Ok(new { message = "Deletion scheduled. Data will be erased in " +  numberOfDaysUntilDelete + "days." });
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing your request."+ex.Message);
            }
        } 

        [HttpPut("{patientId}")]
        [Authorize(Roles = "Admin")]  
        public async Task<IActionResult> UpdatePatientProfile(string patientId, [FromBody] PatientDTO updateDto)
        {
            if (updateDto == null || string.IsNullOrEmpty(patientId))
            {
                return BadRequest("Patient ID and update data are required.");
            }

            try
            {
                var updatedPatient = await _service.UpdatePatientProfile(patientId, updateDto);

                return Ok(updatedPatient);
            }
            catch (Exception ex)
            {
                    return BadRequest("An error occurred while processing your request."+ex.Message);
                }
            }

        private void LogChanges(string email, PatientDTO editPatientDTO)
        {
            var logMessage = $"Patient {email} profile updated: ";
            if (!string.IsNullOrEmpty(editPatientDTO.Email)) logMessage += $"Email: {editPatientDTO.Email}, ";
            if (!string.IsNullOrEmpty(editPatientDTO.Phone)) logMessage += $"Phone: {editPatientDTO.Phone}, ";
            if (!string.IsNullOrEmpty(editPatientDTO.Address)) logMessage += $"Address: {editPatientDTO.Address}, ";
            if (editPatientDTO.Conditions != null) logMessage += $"Conditions: {string.Join(", ", editPatientDTO.Conditions)}, ";
            _logger.Information(logMessage.TrimEnd(','));
        }
    
    }
}