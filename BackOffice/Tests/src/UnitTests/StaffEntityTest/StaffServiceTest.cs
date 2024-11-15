using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.StaffEntity;

namespace Sempi5Test.UnitTests.StaffEntityTest
{
    public class StaffServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IStaffRepository> _mockStaffRepo;
        private readonly Mock<ISpecializationRepository> _mockSpecRepo;
        private readonly StaffService _staffService;

        public StaffServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStaffRepo = new Mock<IStaffRepository>();
            _mockSpecRepo = new Mock<ISpecializationRepository>();
            _staffService = new StaffService(
                _mockSpecRepo.Object,
                _mockStaffRepo.Object,
                _mockUnitOfWork.Object
            );
        }

        [Fact]
        public async Task AddStaffMember_ShouldReturnStaffDTO_WhenStaffIsAdded()
        {
            string begin = DateTime.Now.AddHours(1).ToString("dd-MM-yyyyTHH:mm:ss");
            string end = DateTime.Now.AddHours(2).ToString("dd-MM-yyyyTHH:mm:ss");
            // Arrange
            var staffDTO = new StaffDTO
            {
                LicenseNumber = "LN123456",
                Name = "Jane Doe",
                Email = "jane@example.com",
                Phone = "1234567890",
                Address = "123 Main St, City, Country",
                AvailabilitySlots = new List<string> { begin+" - "+end },
                Specialization = "specialization-id"
            };

            var IDmock = new Mock<SpecializationID>(staffDTO.Specialization.ToLower());
            var specialization = new Mock<Specialization>(IDmock.Object);

            _mockSpecRepo.Setup(repo => repo.GetByIdAsync(IDmock.Object))
                .ReturnsAsync(specialization.Object);

            // Act
            var result = await _staffService.AddStaffMember(staffDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staffDTO.Name, result.Name);
            Assert.Equal(staffDTO.Email, result.Email);
            Assert.Equal(staffDTO.Phone, result.Phone);
            _mockStaffRepo.Verify(repo => repo.AddAsync(It.IsAny<Staff>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetStaffMember_ShouldReturnStaffDTO_WhenStaffExists()
        {
            // Arrange
            var staffId = new StaffID("1");
            var staff = new Staff
            {
                Name = new Name("Jane Doe"),
                Email = new Email("jane@example.com"),
                Phone = new Phone("1234567890"),
                LicenseNumber = new LicenseNumber("LN123456"),
                Address = new Address("123 Main St", "City", "Country")
            };

            _mockStaffRepo.Setup(repo => repo.GetStaffMemberById(staffId))
                .ReturnsAsync(staff);

            // Act
            var result = await _staffService.GetStaffMember(staffId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staff.Name.ToString(), result.Name);
            Assert.Equal(staff.Email.ToString(), result.Email);
        }

        [Fact]
        public async Task GetStaffMemberByEmail_ShouldReturnStaffDTO_WhenStaffExists()
        {
            // Arrange
            var email = "jane@example.com";
            var staff = new Staff
            {
                Name = new Name("Jane Doe"),
                Email = new Email(email),
                Phone = new Phone("1234567890"),
                LicenseNumber = new LicenseNumber("LN123456")
            };

            _mockStaffRepo.Setup(repo => repo.GetStaffMemberByEmail(email))
                .ReturnsAsync(staff);

            // Act
            var result = await _staffService.GetStaffMemberByEmail(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staff.Name.ToString(), result.Name);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetAllStaffMembers_ShouldReturnListOfStaffDTOs()
        {
            // Arrange
            var staffList = new List<Staff>
            {
                new Staff { Name = new Name("Jane Doe"), Email = new Email("jane@example.com") },
                new Staff { Name = new Name("John Doe"), Email = new Email("john@example.com") }
            };

            _mockStaffRepo.Setup(repo => repo.GetAllStaffMembers())
                .ReturnsAsync(staffList);

            // Act
            var result = await _staffService.GetAllStaffMembers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staffList.Count, result.Count);
        }

        [Fact]
        public async Task SearchStaff_ShouldReturnListOfStaffDTOs_WhenSearchCriteriaMatches()
        {
            // Arrange
            var name = "Jane";
            var staffList = new List<Staff>
            {
                new Staff { Name = new Name("Jane Doe"), Email = new Email("jane@example.com") }
            };

            _mockStaffRepo.Setup(repo => repo.SearchStaff(name, null, null, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(staffList);

            // Act
            var result = await _staffService.SearchStaff(name, null, null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task EditStaff_ShouldUpdateStaff_WhenStaffExists()
        {
            // Arrange
            var email = "jane@example.com";
            var staff = new Staff
            {
                Email = new Email(email),
                Phone = new Phone("1234567890"),
                Name = new Name("Jane Doe"),
                Address = new Address("123 Main St", "City", "Country"),
                LicenseNumber = new LicenseNumber("LN123456")
            };

            string begin = DateTime.Now.AddHours(1).ToString("dd-MM-yyyyTHH:mm:ss");
            string end = DateTime.Now.AddHours(2).ToString("dd-MM-yyyyTHH:mm:ss");

            var staffDTO = new StaffDTO
            {
                Email = "new-email@example.com",
                Phone = "0987654321",
                Address = "456 Another St, Another City, Another Country",
                AvailabilitySlots = new List<string> { begin+" - "+end },
                Specialization = "new-specialization-id"
            };

            _mockStaffRepo.Setup(repo => repo.GetStaffMemberByEmail(email))
                .ReturnsAsync(staff);

            var IDmock = new Mock<SpecializationID>(staffDTO.Specialization.ToLower());
            var specialization = new Mock<Specialization>(IDmock.Object);

            _mockSpecRepo.Setup(repo => repo.GetByIdAsync(IDmock.Object))
                .ReturnsAsync(specialization.Object);

            // Act
            var result = await _staffService.EditStaff(email, staffDTO, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staffDTO.Email, result.Email);
            Assert.Equal(staffDTO.Phone, result.Phone);
            Assert.Equal(staffDTO.Address, result.Address);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }
    }
}
