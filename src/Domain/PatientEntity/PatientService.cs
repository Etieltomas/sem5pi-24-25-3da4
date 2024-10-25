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
            _repoUser = userRepo;
            _emailService = emailService;
            _repo = repo;
            _unitOfWork = unitOfWork;
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
                DateOfBirth = patient.DateOfBirth.ToString("dd-MM-yyyy")
            };
        }
    
    }
}
