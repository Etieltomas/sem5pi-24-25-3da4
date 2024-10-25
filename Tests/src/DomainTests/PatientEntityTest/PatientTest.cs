using System;
using Xunit;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.UserEntity;

namespace DomainTests.PatientEntityTest
{
    public class PatientTest
    {
        [Fact]
        public void CanCreatePatient()
        {
            // Arrange
            var dateOfBirth = new DateTime(1990, 1, 1);
            var name = new Name("John", "Doe");
            var gender = Gender.Male;
            var email = new Email("john.doe@example.com");
            var phone = new Phone("1234567890");
            var address = new Address("123 Main St", "City", "State", "12345");
            var conditions = new List<Condition>();
            var emergencyContact = new Phone("9876543210");
            var systemUser = new SystemUser();

            // Act
            var patient = new Patient
            {
                Id = patientId,
                DateOfBirth = dateOfBirth,
                Name = name,
                Gender = gender,
                Email = email,
                Phone = phone,
                Address = address,
                Conditions = conditions,
                EmergencyContact = emergencyContact,
                SystemUser = systemUser
            };

            // Assert
            Assert.Equal(patientId, patient.Id);
            Assert.Equal(dateOfBirth, patient.DateOfBirth);
            Assert.Equal(name, patient.Name);
            Assert.Equal(gender, patient.Gender);
            Assert.Equal(email, patient.Email);
            Assert.Equal(phone, patient.Phone);
            Assert.Equal(address, patient.Address);
            Assert.Equal(conditions, patient.Conditions);
            Assert.Equal(emergencyContact, patient.EmergencyContact);
            Assert.Equal(systemUser, patient.SystemUser);
        }
    }
}
