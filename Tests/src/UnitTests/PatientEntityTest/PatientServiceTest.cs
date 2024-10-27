using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;
using Sempi5;

namespace Sempi5Test.UnitTests.PatientEntityTest
{
    public class PatientServiceTest
    {
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<EmailService> _mockEmailService;
        private readonly Mock<Serilog.ILogger> _mockLogger;
        private readonly PatientService _patientService;

        public PatientServiceTest()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEmailService = new Mock<EmailService>();
            _mockLogger = new Mock<Serilog.ILogger>();
            _mockLogger.Setup(logger => logger.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Returns(_mockLogger.Object);

            _patientService = new PatientService(
                _mockPatientRepo.Object,
                _mockUserRepo.Object,
                _mockUnitOfWork.Object,
                _mockEmailService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task AddPatient_ShouldReturnPatientDTO_WhenPatientIsAdded()
        {
            // Arrange
            var patientDTO = new PatientDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Address = "123 Main St, City, Country",
                Gender = "male",
                Conditions = new List<string> { "Condition1" },
                EmergencyContact = "9876543210",
                DateOfBirth = "01-01-1990"
            };

            _mockUnitOfWork.Setup(uow => uow.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _patientService.AddPatient(patientDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientDTO.Name, result.Name);
            Assert.Equal(patientDTO.Email, result.Email);
            Assert.Equal(patientDTO.Phone, result.Phone);
            Assert.Equal(patientDTO.Address, result.Address);
        }

        [Fact]
        public async Task GetPatientByEmail_ShouldReturnPatientDTO_WhenPatientExists()
        {
            // Arrange
            var email = "john@example.com";
            var patient = new Patient
            {
                Name = new Name("John Doe"),
                Email = new Email(email),
                Phone = new Phone("1234567890"),
                Address = new Address("123 Main St", "City", "Country"),
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _mockPatientRepo.Setup(repo => repo.GetPatientByEmail(email))
                .ReturnsAsync(patient);

            // Act
            var result = await _patientService.GetPatientByEmail(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patient.Name.ToString(), result.Name);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task UpdatePatientProfile_ShouldUpdatePatient_WhenPatientExists()
        {
            // Arrange
            var patientId = "123";
            var updateDto = new PatientDTO
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                Phone = "0987654321",
                Address = "456 Another St, Another City, Another Country"
            };

            var existingPatient = new Patient
            {
                Name = new Name("John Doe"),
                Email = new Email("john@example.com"),
                Phone = new Phone("1234567890"),
                Address = new Address("123 Main St", "City", "Country")
            };

            _mockPatientRepo.Setup(repo => repo.GetPatientById(It.IsAny<PatientID>()))
                .ReturnsAsync(existingPatient);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _patientService.UpdatePatientProfile(patientId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Name, result.Name);
            Assert.Equal(updateDto.Email, result.Email);
            Assert.Equal(updateDto.Phone, result.Phone);
            Assert.Equal(updateDto.Address, result.Address);
        }

        [Fact]
        public async Task UpdatePatientProfile_ShouldThrowException_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = "invalid_id";
            var updateDto = new PatientDTO();
            _mockPatientRepo.Setup(repo => repo.GetPatientById(It.IsAny<PatientID>())).ReturnsAsync((Patient)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
                await _patientService.UpdatePatientProfile(patientId, updateDto));
        }

        [Fact]
        public async Task UpdatePatientProfile_ShouldSendEmail_WhenSensitiveDataChanges()
        {
            // Arrange
            var patientId = "valid_id";
            var originalEmail = "john@example.com";
            var updatedEmail = "john.doe@example.com";
            var patient = new Patient
            {
                Name = new Name("John Doe"), // Definimos o nome aqui para evitar NullReferenceException
                Email = new Email(originalEmail),
                Phone = new Phone("1234567890"),
                Address = new Address("123 Main St", "City", "Country")
            };

            var updateDto = new PatientDTO { Email = updatedEmail };
            _mockPatientRepo.Setup(repo => repo.GetPatientById(It.IsAny<PatientID>())).ReturnsAsync(patient);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _patientService.UpdatePatientProfile(patientId, updateDto);

            // Assert
            _mockEmailService.Verify(service => service.sendEmail(
                patient.Name.ToString(), originalEmail, "Profile Update", It.IsAny<string>()), Times.Once);
            Assert.Equal(updatedEmail, result.Email);
        }

        [Fact]
        public async Task AssociateAccount_ShouldReturnNull_WhenCookieEmailIsNull()
        {
            // Arrange
            var email = "user@example.com";
            var cookieEmail = string.Empty;

            // Act
            var result = await _patientService.AssociateAccount(email, cookieEmail);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task AssociateAccount_ShouldReturnNull_WhenPatientAlreadyAssociated()
        {
            // Arrange
            var email = "user@example.com";
            _mockUserRepo.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(new SystemUser());

            // Act
            var result = await _patientService.AssociateAccount(email, "cookieEmail");

            // Assert
            Assert.Null(result);
        }    

        [Fact]
        public async Task GetAllPatients_ShouldReturnListOfPatientDTOs()
        {
            // Arrange
            var patients = new List<Patient>
            {
                new Patient { Name = new Name("John Doe"), Email = new Email("john@example.com") },
                new Patient { Name = new Name("Jane Doe"), Email = new Email("jane@example.com") }
            };

            _mockPatientRepo.Setup(repo => repo.GetAllPatients())
                .ReturnsAsync(patients);

            // Act
            var result = await _patientService.GetAllPatients();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patients.Count, result.Count);
        }



        [Fact]
        public async Task ScheduleDeletion_ShouldSchedulePatientForDeletion_WhenPatientExists()
        {
            // Arrange
            var email = "john@example.com";
            var patient = new Patient { Email = new Email(email), DeletePatientDate = null };

            _mockPatientRepo.Setup(repo => repo.GetPatientByEmail(email)).ReturnsAsync(patient);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _patientService.ScheduleDeletion(email);

            // Assert
            Assert.True(result);
            Assert.NotNull(patient.DeletePatientDate);
        }

        [Fact]
        public async Task ScheduleDeletion_ShouldReturnFalse_WhenPatientDoesNotExist()
        {
            // Arrange
            var email = "notfound@example.com";
            _mockPatientRepo.Setup(repo => repo.GetPatientByEmail(email)).ReturnsAsync((Patient)null);

            // Act
            var result = await _patientService.ScheduleDeletion(email);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ScheduleDeletion_ShouldUpdatePatientAndCommit_WhenPatientExists()
        {
            // Arrange
            var email = "found@example.com";
            var patient = new Patient { Email = new Email(email), DeletePatientDate = null };
            _mockPatientRepo.Setup(repo => repo.GetPatientByEmail(email)).ReturnsAsync(patient);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _patientService.ScheduleDeletion(email);

            // Assert
            Assert.True(result);
            Assert.NotNull(patient.DeletePatientDate);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task SearchPatients_ShouldReturnPatientDTOList_WhenPatientsExist()
        {
            // Arrange
            string name = "John";
            string email = "john@example.com";
            string dateOfBirth = "1990-01-01";
            string medicalRecordNumber = "MR123";
            int page = 1;
            int pageSize = 10;

            var patients = new List<Patient>
            {
                new Patient { Name = new Name(name), Email = new Email(email), DateOfBirth = DateTime.Parse(dateOfBirth) },
                new Patient { Name = new Name("Jane Doe"), Email = new Email("jane@example.com"), DateOfBirth = DateTime.Parse("1995-05-05") }
            };

            _mockPatientRepo.Setup(repo => repo.GetPatientsFiltered(name, email, dateOfBirth, medicalRecordNumber, page, pageSize)).ReturnsAsync(patients);

            // Act
            var result = await _patientService.SearchPatients(name, email, dateOfBirth, medicalRecordNumber, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(name, result[0].Name);
            Assert.Equal(email, result[0].Email);
        }

        [Fact]
        public async Task SearchPatients_ShouldReturnEmptyList_WhenNoPatientsMatchCriteria()
        {
            // Arrange
            string name = "NonExistent";
            string email = "noone@example.com";
            string dateOfBirth = "2000-01-01";
            string medicalRecordNumber = "MR999";
            int page = 1;
            int pageSize = 10;

            _mockPatientRepo.Setup(repo => repo.GetPatientsFiltered(name, email, dateOfBirth, medicalRecordNumber, page, pageSize)).ReturnsAsync(new List<Patient>());

            // Act
            var result = await _patientService.SearchPatients(name, email, dateOfBirth, medicalRecordNumber, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchPatients_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            _mockPatientRepo.Setup(repo => repo.GetPatientsFiltered(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((List<Patient>)null);

            // Act
            var result = await _patientService.SearchPatients("any", "any", "any", "any", 1, 10);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SearchPatients_ShouldReturnPagedResults()
        {
            // Arrange
            int page = 1;
            int pageSize = 2;
            var patients = new List<Patient>
            {
                new Patient { Name = new Name("Patient1"), Email = new Email("patient1@example.com"), DateOfBirth = DateTime.Parse("1990-01-01") },
                new Patient { Name = new Name("Patient2"), Email = new Email("patient2@example.com"), DateOfBirth = DateTime.Parse("1991-02-02") }
            };

            _mockPatientRepo.Setup(repo => repo.GetPatientsFiltered(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), page, pageSize))
                .ReturnsAsync(patients);

            // Act
            var result = await _patientService.SearchPatients(null, null, null, null, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageSize, result.Count);
            Assert.Equal("Patient1", result[0].Name);
            Assert.Equal("patient1@example.com", result[0].Email);
        }
    }        
}
