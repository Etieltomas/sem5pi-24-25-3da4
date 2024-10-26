using System;
using Xunit;
using Moq;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;
using System.Collections.Generic;

public class PatientTests
{ /*
    [Fact]
    public void CanInitializePatient()
    {
        // TODO: Maybe change SystemUser into ISystemUser and all the other ones into their respective interfaces
        var systemUserMock = new Mock<ISystemUser>();
        systemUserMock.SetupGet(su => su.Username).Returns("johndoe");
        systemUserMock.SetupGet(su => su.Email).Returns(new Email("johndoe@example.com"));

        var patient = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("John Doe"),
            Gender = Gender.Male,
            Email = new Email("johndoe@example.com"),
            Phone = new Phone("123-456-7890"),
            Address = new Address("123 Main St", "Springfield", "IL"),
            Conditions = new List<Condition> { new Condition("Asthma") },
            EmergencyContact = new Phone("098-765-4321"),
            SystemUser = (SystemUser) systemUserMock.Object
        };

        Assert.NotNull(patient);
        Assert.Equal("John Doe", patient.Name.ToString());
        Assert.Equal(Gender.Male, patient.Gender);
        Assert.Equal("johndoe@example.com", patient.Email.ToString());
        Assert.Equal("johndoe", patient.SystemUser.Username);
    }

    [Fact]
    public void ChangeBirthDate_ValidDate_UpdatesDateOfBirth()
    {
        var patient = new Patient { DateOfBirth = new DateTime(1990, 1, 1) };
        var newDate = new DateTime(2000, 1, 1);

        patient.DateOfBirth = newDate;

        Assert.Equal(newDate, patient.DateOfBirth);
    }

    [Fact]
    public void ChangeBirthDate_InvalidDate_ThrowsException()
    {
        var patient = new Patient { DateOfBirth = new DateTime(1990, 1, 1) };
        var newDate = DateTime.Now.AddDays(1); // Future date

        Assert.Throws<BusinessRuleValidationException>(() => patient.DateOfBirth = newDate);
    }

    [Fact]
    public void PatientsWithSameValuesShouldBeEqual()
    {
        var systemUserMock = new Mock<SystemUser>();
        systemUserMock.SetupGet(su => su.Username).Returns("johndoe");
        systemUserMock.SetupGet(su => su.Email).Returns(new Email("johndoe@example.com"));

        var patient1 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("John Doe"),
            Gender = Gender.Male,
            Email = new Email("johndoe@example.com"),
            Phone = new Phone("123-456-7890"),
            Address = new Address("123 Main St", "Springfield", "IL"),
            Conditions = new List<Condition> { new Condition("Asthma") },
            EmergencyContact = new Phone("098-765-4321"),
            SystemUser = systemUserMock.Object
        };

        var patient2 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("John Doe"),
            Gender = Gender.Male,
            Email = new Email("johndoe@example.com"),
            Phone = new Phone("123-456-7890"),
            Address = new Address("123 Main St", "Springfield", "IL"),
            Conditions = new List<Condition> { new Condition("Asthma") },
            EmergencyContact = new Phone("098-765-4321"),
            SystemUser = systemUserMock.Object
        };

        Assert.Equal(patient1.DateOfBirth, patient2.DateOfBirth);
        Assert.Equal(patient1.Name.ToString(), patient2.Name.ToString());
        Assert.Equal(patient1.Gender, patient2.Gender);
        Assert.Equal(patient1.Email.ToString(), patient2.Email.ToString());
        Assert.Equal(patient1.Phone.ToString(), patient2.Phone.ToString());
        Assert.Equal(patient1.Address.ToString(), patient2.Address.ToString());
        Assert.Equal(patient1.SystemUser.Username, patient2.SystemUser.Username);
        Assert.Equal(patient1.SystemUser.Email.ToString(), patient2.SystemUser.Email.ToString());
    }

    [Fact]
    public void PatientsWithDifferentValuesShouldNotBeEqual()
    {
        var systemUserMock1 = new Mock<SystemUser>();
        systemUserMock1.SetupGet(su => su.Username).Returns("johndoe");
        systemUserMock1.SetupGet(su => su.Email).Returns(new Email("johndoe@example.com"));

        var systemUserMock2 = new Mock<SystemUser>();
        systemUserMock2.SetupGet(su => su.Username).Returns("janedoe");
        systemUserMock2.SetupGet(su => su.Email).Returns(new Email("janedoe@example.com"));

        var patient1 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("John Doe"),
            Gender = Gender.Male,
            Email = new Email("johndoe@example.com"),
            Phone = new Phone("123-456-7890"),
            Address = new Address("123 Main St", "Springfield", "IL"),
            Conditions = new List<Condition> { new Condition("Asthma") },
            EmergencyContact = new Phone("098-765-4321"),
            SystemUser = systemUserMock1.Object
        };

        var patient2 = new Patient
        {
            DateOfBirth = new DateTime(2000, 1, 1), // Different date
            Name = new Name("Jane Doe"), // Different name
            Gender = Gender.Female, // Different gender
            Email = new Email("janedoe@example.com"), // Different email
            Phone = new Phone("098-765-4321"), // Different phone
            Address = new Address("456 Elm St", "Metropolis", "NY"), // Different address
            Conditions = new List<Condition> { new Condition("Diabetes") }, // Different condition
            EmergencyContact = new Phone("123-456-7890"), // Different emergency contact
            SystemUser = systemUserMock2.Object // Different system user
        };

        Assert.NotEqual(patient1.DateOfBirth, patient2.DateOfBirth);
        Assert.NotEqual(patient1.Name.ToString(), patient2.Name.ToString());
        Assert.NotEqual(patient1.Gender, patient2.Gender);
        Assert.NotEqual(patient1.Email.ToString(), patient2.Email.ToString());
        Assert.NotEqual(patient1.Phone.ToString(), patient2.Phone.ToString());
        Assert.NotEqual(patient1.Address.ToString(), patient2.Address.ToString());
        Assert.NotEqual(patient1.SystemUser.Username, patient2.SystemUser.Username);
        Assert.NotEqual(patient1.SystemUser.Email.ToString(), patient2.SystemUser.Email.ToString());
    } */
}
