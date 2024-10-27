using System.Globalization;
using System.Security.Claims;
using Microsoft.DotNet.Scaffolding.Shared;
using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;


namespace Sempi5.Domain.PatientEntity
{
    public class PatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _repo;
        private readonly IUserRepository _repoUser;
        private readonly EmailService _emailService;
        private readonly Serilog.ILogger _logger;

        public PatientService(IPatientRepository repo, IUserRepository userRepo ,IUnitOfWork unitOfWork, EmailService emailService, Serilog.ILogger logger)
        {
            _repo = repo;
            _repoUser = userRepo;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<PatientDTO> AssociateAccount(string email, string cookieEmail)
        {

            if (string.IsNullOrEmpty(cookieEmail))
            {
                return null;
            }

            var user = await _repoUser.GetUserByEmail(email);
            var pat = await _repo.GetPatientByEmail(email);

            if (pat == null || user != null)
            {
                return null;
            }

            var newUser = new SystemUser
            {
                Username = cookieEmail,
                Email = new Email(cookieEmail),
                Active = false,
                Role = "Patient"
            };

            pat.SystemUser = newUser;

            await _repoUser.AddAsync(newUser);
            
            await _unitOfWork.CommitAsync();


            user = await _repoUser.GetUserByEmail(email);
            //  email logic
            var message = "<b>Hello,</b><br>" +
                        "Thank you for signing up! Please confirm your account by clicking the link below:<br><br>" +
                        "<a href='http://localhost:5012/api/SystemUser/confirm/" + user.Id.AsLong() + "/true'>Click here to confirm your account</a><br><br>" +
                        "If you didn't sign up, please ignore this email.";

            var subject = "Confirmation of Account";

            _emailService.sendEmail(user.Username, pat.Email.ToString(), subject, message);

            return ConvertToDTO(pat);
        }


        public async Task<PatientDTO> AddPatient(PatientDTO patientDTO)
        {
            var address = patientDTO.Address.Split(", ");
            var patient = new Patient
            {
                Gender = GenderExtensions.FromString(patientDTO.Gender.ToLower()),
                Name = new Name(patientDTO.Name),
                Email = new Email(patientDTO.Email),
                Phone = new Phone(patientDTO.Phone),
                Conditions = patientDTO.Conditions.Select(condition => new Condition(condition)).ToList(), 
                EmergencyContact = new Phone(patientDTO.EmergencyContact),
                Address = new Address(address[0], address[1], address[2]),
                DateOfBirth = DateTime.ParseExact(patientDTO.DateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture)
            };

            await _repo.AddAsync(patient);
            await _unitOfWork.CommitAsync();

            return ConvertToDTO(patient);
        }

        public async Task<PatientDTO> GetPatientByMedicalRecordNumber(PatientID id)
        {
            var patient = await _repo.GetPatientById(id);
            return patient == null ? null : ConvertToDTO(patient);
        }

        public async Task<PatientDTO> GetPatientByEmail(string email)
        {
            var patient = await _repo.GetPatientByEmail(email);
            return patient == null ? null : ConvertToDTO(patient);
        }

        public async Task<PatientDTO> UpdatePatientProfile(string patientId, PatientDTO updateDto)
        {
            //get patient by id
            var patient = await _repo.GetPatientById(new PatientID(patientId));
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }

            var originalEmail = patient.Email.ToString();
            var originalPhone = patient.Phone.ToString();
            var originalAddress = patient.Address.ToString();

            //update patient information
            if (!string.IsNullOrEmpty(updateDto.Name))
                patient.Name = new Name(updateDto.Name);

            if (!string.IsNullOrEmpty(updateDto.Email))
                patient.Email = new Email(updateDto.Email);

            if (!string.IsNullOrEmpty(updateDto.Phone))
                patient.Phone = new Phone(updateDto.Phone);

            if (!string.IsNullOrEmpty(updateDto.Address))
            {
                var addressParts = updateDto.Address.Split(", ");
                patient.Address = new Address(addressParts[0], addressParts[1], addressParts[2]);
            }

            if (updateDto.Conditions != null && updateDto.Conditions.Any())
            {
                patient.Conditions = updateDto.Conditions.Select(c => new Condition(c)).ToList();
            }

            //sensitive data 
            bool isSensitiveDataChanged = (originalEmail != patient.Email.ToString()) ||
                                          (originalPhone != patient.Phone.ToString()) ||
                                          (originalAddress != patient.Address.ToString());

            if (isSensitiveDataChanged)
            {
                //email
                var message = $"<p>Hello, {patient.Name}!</p>" +
                              $"<p>Your contact information has been updated in the system.</p>" +
                              $"<p>Email: {patient.Email}</p>" +
                              $"<p>Phone Number: {patient.Phone}</p>" +
                              $"<p>Address: {patient.Address}</p>" +
                              "<p>Best regards.</p>";

                _emailService.sendEmail(patient.Name.ToString(), originalEmail, "Profile Update", message);
            }

            await _unitOfWork.CommitAsync();

            UpdateAsAdminLog(patientId, updateDto.ToString());

            return ConvertToDTO(patient);
        }

        

        public async Task<List<PatientDTO>> GetAllPatients()
        {
            var list = await _repo.GetAllPatients();  

            if (list == null)
            {
                return null;
            }

            
            List<PatientDTO> listDto = list.ConvertAll(pat => ConvertToDTO(pat));

            return listDto;  
        }

        public async Task<PatientDTO> GetPatientByName(string name)
        {
            var patient = await _repo.GetPatientByName(name);
            return patient == null ? null : ConvertToDTO(patient);
        }

        public async Task<List<PatientDTO>> SearchPatients(string name, string email, string dateOfBirth, string medicalRecordNumber, int page, int pageSize){
            
            var list = await _repo.GetPatientsFiltered(name, email, dateOfBirth, medicalRecordNumber, page, pageSize);  

            if (list == null)
            {
                return null;
            }

            List<PatientDTO> listDto = list.ConvertAll(pat => ConvertToDTO(pat));

            return listDto;
        }

        public async Task<PatientDTO> UpdatePatient(string email, PatientDTO patientDTO, bool isEmailComfirmed)
        {
            var patient = await _repo.GetPatientByEmail(email);

            var originalEmail = patient.Email;

            if (patient == null)
            {
                return null;
            }

            bool confirmationEmailNeeded = false;
            if (patientDTO.Name != null)
            {
                patient.Name = new Name(patientDTO.Name);
            }

            if(patientDTO.Phone != null)
            {
                patient.Phone = new Phone(patientDTO.Phone);
                confirmationEmailNeeded = true;
            }

            if(patientDTO.Email != null)
            {
                patient.Email = new Email(patientDTO.Email);
                confirmationEmailNeeded = true;
            }

            if(patientDTO.Address != null)
            {
                var address = patientDTO.Address.Split(", ");
                patient.Address = new Address(address[0], address[1], address[2]);
                confirmationEmailNeeded = true;
            }

            if (!confirmationEmailNeeded || isEmailComfirmed)
            {
                await _unitOfWork.CommitAsync();
            }

            return ConvertToDTO(patient);
        }

        public async Task<bool> ScheduleDeletion(string email)
        {
            var patient = await _repo.GetPatientByEmail(email);
            if (patient == null) return false;

            //var daysUntilExclude = 30;
            
            patient.DeletePatientDate = DateTime.Now.AddMinutes(0.25);

            if(patient.SystemUser != null){
                patient.SystemUser.Active = false;
            }

            await _unitOfWork.CommitAsync();
            return true;
        }

        private PatientDTO ConvertToDTO(Patient patient)
        {
            return new PatientDTO
            {
                Gender = patient.Gender.ToString(),
                MedicalRecordNumber = patient.Id.Value,
                Name = patient.Name.ToString(),
                Email = patient.Email.ToString(),
                Phone = patient.Phone.ToString(), 
                Conditions = patient.Conditions.Select(condition => condition.ToString()).ToList(), 
                EmergencyContact = patient.EmergencyContact.ToString(),
                Address = patient.Address.ToString(),
                DateOfBirth = patient.DateOfBirth.ToString("dd-MM-yyyy"),
                PatientID = patient.Id.ToString(),
                DeletePatientDate = patient.DeletePatientDate?.ToString("dd-MM-yyyy")
            };
        }

        public async Task ProcessScheduledDeletions()
        {
            var patientsToDelete = await _repo.GetPatientsForDeletion(DateTime.Now);
            patientsToDelete.Select(ConvertToDTO).ToList();

            foreach (var patient in patientsToDelete)
            {
                var warningEmailBackup = patient.Email.ToString();
                var warningNameBackup = patient.Name.ToString();

                try
                {
                    if (patient.SystemUser != null)
                    {
                        _repoUser.Remove(patient.SystemUser);
                        await _unitOfWork.CommitAsync();
                    }

                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Fail to exclude SystemUser of Patient.", ex);
                }

                string emailSuffix = patient.Id.AsString();

                var phoneFirstSuffix = patient.Id.AsString().Substring(patient.Id.AsString().Length - 9, 2);
                var phoneMiddleSuffix = patient.Id.AsString().Substring(patient.Id.AsString().Length - 7, 3);
                var phoneLastSuffix = patient.Id.AsString().Substring(patient.Id.AsString().Length - 4);

                patient.Name = new Name("anonymous");
                patient.Email = new Email("anonymous_" + emailSuffix + "@anonymous.anonymous");
                patient.Phone = new Phone("000-" + phoneMiddleSuffix + "-" + phoneLastSuffix);
                patient.EmergencyContact = new Phone("0" + phoneFirstSuffix + "-" + phoneMiddleSuffix + "-" + phoneLastSuffix);
                var address = patient.Address.ToString().Split(", ");
                patient.Address = new Address("anonymos", "anonymos", address[2]);
                patient.SystemUser = null;

                await _unitOfWork.CommitAsync();

                await NotifyDeletionCompletion(warningEmailBackup, warningNameBackup);

                CreateLogDelete();
            }
        }

        private async Task NotifyDeletionCompletion(string email, string name)
        {
            var message = "Your account and personal data have been permanently deleted from our system as requested, respecting GDPR.";
            
            _emailService.sendEmail(name, email, "Patient Account Deletion Complete", message);
        }

        private void CreateLogDelete()
        {
            var text = $"\n - Patient account deleted: All identifiable data associated with the patient has been permanently removed from the system as requested, in compliance with GDPR guidelines. Anonymized data has been retained for legal and/or research purposes.";

            _logger.ForContext("CustomLogLevel", "CustomLevel")
                    .Information(text.Remove(text.Length - 1));
        }

        private void UpdateAsAdminLog(string patientId, string updatedInfo)
        {
            var text = $"\n - PatientID: {patientId}, Updated Information: {updatedInfo}.";

            _logger.ForContext("CustomLogLevel", "CustomLevel")
                    .Information(text.Remove(text.Length - 1));
        }
    }
}
