using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.UserEntity;
using Sempi5.Domain.SpecializationEntity;

namespace Sempi5Test.UnitTests.StaffEntityTest
{
    public class StaffTest
    {
        [Fact]
        public void Staff_CanBeInitialized_WithValidParameters()
        {
            // Arrange
            var licenseNumberMock = new Mock<LicenseNumber>("12345");
            licenseNumberMock.Setup(x => x.ToString()).Returns("12345");

            var nameMock = new Mock<Name>("John Doe");
            nameMock.Setup(x => x.ToString()).Returns("John Doe");

            var emailMock = new Mock<Email>("john.doe@example.com");
            emailMock.Setup(x => x.ToString()).Returns("john.doe@example.com");

            var phoneMock = new Mock<Phone>("123-456-7890");
            phoneMock.Setup(x => x.ToString()).Returns("123-456-7890");

            var addressMock = new Mock<Address>("123 Main St", "City", "State");
            addressMock.Setup(x => x.ToString()).Returns("123 Main St, City, State");
            
            var specializationIDMock = new Mock<SpecializationID>("Neurology");
            var specializationMock = new Mock<Specialization>(specializationIDMock.Object);
            // Act
            var staff = new Staff
            {
                LicenseNumber = licenseNumberMock.Object,
                Name = nameMock.Object,
                Email = emailMock.Object,
                Phone = phoneMock.Object,
                Address = addressMock.Object,
                AvailabilitySlots = new List<AvailabilitySlot>(),
                Specialization = specializationMock.Object
            };

            // Assert
            Assert.Equal(licenseNumberMock.Object, staff.LicenseNumber);
            Assert.Equal(nameMock.Object, staff.Name);
            Assert.Equal(emailMock.Object, staff.Email);
            Assert.Equal(phoneMock.Object, staff.Phone);
            Assert.Equal(addressMock.Object, staff.Address);
            Assert.Equal(specializationMock.Object, staff.Specialization);
            Assert.NotNull(staff.AvailabilitySlots);
        }

        [Fact]
        public void Staff_SystemUser_CanBeSetAndRetrieved()
        {
            // Arrange
            var mockSystemUser = new Mock<SystemUser>();
            mockSystemUser.Setup(x => x.ToString()).Returns("SystemUser");

            var staff = new Staff();

            // Act
            staff.SystemUser = mockSystemUser.Object;

            // Assert
            Assert.NotNull(staff.SystemUser);
            Assert.Equal("SystemUser", mockSystemUser.Object.ToString());
        }

        [Fact]
        public void Staff_CanAddAvailabilitySlot()
        {
            // Arrange
            var staff = new Staff
            {
                AvailabilitySlots = new List<AvailabilitySlot>()
            };

            string begin = DateTime.Now.ToString("dd-MM-yyyyTHH:mm:ss");
            string end = DateTime.Now.AddHours(2).ToString("dd-MM-yyyyTHH:mm:ss");

            var availabilitySlot = new Mock<AvailabilitySlot>(begin + " - " + end).Object;

            // Act
            staff.AvailabilitySlots.Add(availabilitySlot);

            // Assert
            Assert.Single(staff.AvailabilitySlots);
            Assert.Contains(availabilitySlot, staff.AvailabilitySlots);
        }
    }
}
