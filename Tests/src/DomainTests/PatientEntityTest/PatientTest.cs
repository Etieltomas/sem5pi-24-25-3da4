using Xunit;
using Sempi5.Domain.PatientEntity;
using System.Globalization;

namespace Sempi5Test.DomainTests.PatientEntityTest
{
    public class PatientTests
    {
        [Fact]
        public void CanInitializePatient()
        {
            var patient = new Patient
            {
                DateOfBirth = DateTime.ParseExact("20-11-2004", "dd-MM-yyyy", CultureInfo.InvariantCulture),
                Name = new Name("John Doe"),
                Gender = Gender.Male,
                Email = new Email("johndoe@example.com"),
                Phone = new Phone("123-456-7890"),
                Address = new Address("Rua das Flores", "Porto", "Portugal"),
                Conditions = new List<Condition> { new Condition("Scoliosis"), new Condition("Carpal Tunnel") },
                EmergencyContact = new Phone("098-765-4321"),
            };

            Assert.NotNull(patient);
            Assert.Equal("John", patient.Name.FirstName());
            Assert.Equal("Doe", patient.Name.LastName());
            Assert.Equal("Male", patient.Gender.ToString());
            Assert.Equal("johndoe@example.com", patient.Email.ToString());
        }

        [Theory]
        [InlineData("John Doe", "johndoe@example.com", "123-456-7890", "Rua das Palmeiras", "Trofa", "Portugal", "098-765-4321", true)]
        [InlineData("", "johndoe@example.com", "123-456-7890", "Rua das Tábuas", "Trofa", "Portugal", "098-765-4321", false)]
        [InlineData("Maria Doe", "", "123-456-7890", "Rua das Calças", "Trofa", "Portugal", "098-765-4321", false)]
        [InlineData("David Doe", "daviddoe@example.com", "", "Rua das Flores", "Trofa", "Portugal", "098-765-4321", false)]
        public void ValidateNonEmptyFields(string name, string email, string phone, string street,
                                        string city ,string state, string emergencyContact, bool expected)
        {
            bool result;
            try
            {
                var patient = new Patient
                {
                    Name = new Name(name),
                    Email = new Email(email),
                    Phone = new Phone(phone),
                    Address = new Address(street, city, state),
                    EmergencyContact = new Phone(emergencyContact)
                };
                result = true;
            }
            catch
            {
                result = false;
            }

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1990-01-01", true)]
        [InlineData("2025-01-01", false)]
        public void DateOfBirthIsValid(string dateOfBirth, bool expected)
        {
            bool result;
            try
            {
                var patient = new Patient
                {
                    DateOfBirth = DateTime.Parse(dateOfBirth)
                };
                result = patient.DateOfBirth <= DateTime.Now;
            }
            catch
            {
                result = false;
            }

            Assert.Equal(expected, result);
        }
    }
}
