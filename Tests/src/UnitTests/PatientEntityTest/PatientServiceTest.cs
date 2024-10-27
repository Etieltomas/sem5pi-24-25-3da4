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
    }
}
