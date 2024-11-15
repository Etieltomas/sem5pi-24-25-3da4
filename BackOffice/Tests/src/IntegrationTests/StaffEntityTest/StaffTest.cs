using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.UserEntity;
using Sempi5.Domain.SpecializationEntity;

namespace Sempi5Test.IntegrationTests.StaffEntityTest
{
    public class StaffTest
    {
        [Fact]
        public void Staff_CanBeInitialized_WithValidParameters()
        {
            // Arrange
            var licenseNumber = new LicenseNumber("12345");

            var name = new Name("John Doe");

            var email = new Email("john.doe@example.com");

            var phone = new Phone("123-456-7890");

            var address = new Address("123 Main St", "City", "State");
            
            var specialization = new Specialization(new SpecializationID("Neurology"));

            // Act
            var staff = new Staff
            {
                LicenseNumber = licenseNumber,
                Name = name,
                Email = email,
                Phone = phone,
                Address = address,
                AvailabilitySlots = new List<AvailabilitySlot>(),
                Specialization = specialization
            };

            // Assert
            Assert.Equal(licenseNumber, staff.LicenseNumber);
            Assert.Equal(name, staff.Name);
            Assert.Equal(email, staff.Email);
            Assert.Equal(phone, staff.Phone);
            Assert.Equal(address, staff.Address);
            Assert.Equal(specialization, staff.Specialization);
            Assert.NotNull(staff.AvailabilitySlots);
        }

        [Fact]
        public void Staff_SystemUser_CanBeSetAndRetrieved()
        {
            // Arrange
            var systemUser = new SystemUser();

            var staff = new Staff();

            // Act
            staff.SystemUser = systemUser;

            // Assert
            Assert.NotNull(staff.SystemUser);
            Assert.Equal(staff.SystemUser, systemUser);
        }

        [Fact]
        public void Staff_CanAddAvailabilitySlot()
        {
            // Arrange
            var staff = new Staff
            {
                AvailabilitySlots = new List<AvailabilitySlot>()
            };

            string begin = DateTime.Now.AddHours(1).ToString("dd-MM-yyyyTHH:mm:ss");
            string end = DateTime.Now.AddHours(2).ToString("dd-MM-yyyyTHH:mm:ss");

            var availabilitySlot = new AvailabilitySlot(begin + " - " + end);

            // Act
            staff.AvailabilitySlots.Add(availabilitySlot);

            // Assert
            Assert.Single(staff.AvailabilitySlots);
            Assert.Contains(availabilitySlot, staff.AvailabilitySlots);
        }
    }
}
