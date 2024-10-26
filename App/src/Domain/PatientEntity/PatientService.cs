using System.Globalization;
using System.Security.Claims;
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

        public PatientService(IPatientRepository repo, IUserRepository userRepo ,IUnitOfWork unitOfWork, EmailService emailService)
        {
            _repo = repo;
            _repoUser = userRepo;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
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

    // Agendar exclusão para 1 minuto a partir da data e hora atuais
    patient.DeletePatientDate = DateTime.UtcNow.AddMinutes(0.25);

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
                DeletePatientDate = patient.DeletePatientDate?.ToString("dd-MM-yyyy")
            };
        }

        public async Task ProcessScheduledDeletions()
        {
        var patientsToDelete = await _repo.GetPatientsForDeletion(DateTime.UtcNow);

        foreach (var patient in patientsToDelete)
        {
            await NotifyDeletionCompletion(patient.Email.ToString());

            patient.Name = new Name("anonymous");
            patient.Gender = Gender.Other;
            patient.Email = new Email("anonymous@email.com");
            patient.Phone = new Phone("000-000-0000");
            patient.DeletePatientDate = null;
            patient.EmergencyContact = new Phone("000-000-0000");
            patient.Address = new Address("anonymos", "anonymos", "anonymos");
            patient.DateOfBirth = DateTime.MinValue; 
            patient.DeletePatientDate = null;
            patient.SystemUser = null;
            await _unitOfWork.CommitAsync();

        }
        }

        private async Task NotifyDeletionCompletion(string email)
        {
            var patient = await _repo.GetPatientByEmail(email);
            var message = "Your account and personal data have been permanently deleted as requested.";
            
            _emailService.sendEmail(patient.Name.ToString(), email, "Account Deletion Complete", message);
        }
    }
}
