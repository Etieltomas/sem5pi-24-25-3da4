using System;
using Xunit;
using Moq;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;
using System.Collections.Generic;

namespace Sempi5Test.UnitTests.PatientEntityTest;

public class PatientTest
{ 
    [Fact]
    public void CanInitializePatient()
    {
        var systemUserMock = new Mock<SystemUser>();
        systemUserMock.Setup(su => su.Username).Returns("franciscoaguiar");
        systemUserMock.Setup(su => su.Email).Returns(new Email("franciscoaguiar@example.com"));

        var nameMock = new Mock<Name>("Francisco Aguiar");
        nameMock.Setup(n => n.ToString()).Returns("Francisco Aguiar");

        var emailMock = new Mock<Email>("franciscoaguiar@example.com");
        emailMock.Setup(e => e.ToString()).Returns("franciscoaguiar@example.com");

        var phoneMock = new Mock<Phone>("123-456-7890");
        phoneMock.Setup(p => p.ToString()).Returns("123-456-7890");

        var addressMock = new Mock<Address>("123 Main St", "Springfield", "IL");
        addressMock.Setup(a => a.ToString()).Returns("123 Main St, Springfield, IL");

        var conditionMock = new Mock<Condition>("Asthma");
        conditionMock.Setup(c => c.ToString()).Returns("Asthma");

        var emergencyContactMock = new Mock<Phone>("098-765-4321");
        emergencyContactMock.Setup(ec => ec.ToString()).Returns("098-765-4321");

        var patient = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = nameMock.Object,
            Gender = Gender.Male,
            Email = emailMock.Object,
            Phone = phoneMock.Object,
            Address = addressMock.Object,
            Conditions = new List<Condition> { conditionMock.Object },
            EmergencyContact = emergencyContactMock.Object,
            SystemUser = systemUserMock.Object
        };

        Assert.NotNull(patient);
        Assert.Equal("Francisco Aguiar", patient.Name.ToString());
        Assert.Equal(Gender.Male, patient.Gender);
        Assert.Equal("franciscoaguiar@example.com", patient.Email.ToString());
        Assert.Equal("franciscoaguiar", patient.SystemUser.Username);
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
        var newDate = DateTime.Now.AddDays(1); 

        Assert.Throws<BusinessRuleValidationException>(() => patient.DateOfBirth = newDate);
    }

    [Fact]
    public void PatientsWithSameValuesShouldBeEqual()
    {
        var systemUserMock = new Mock<SystemUser>();
        systemUserMock.Setup(su => su.Username).Returns("franciscoaguiar");
        systemUserMock.Setup(su => su.Email).Returns(new Email("franciscoaguiar@example.com"));

        var nameMock = new Mock<Name>("Francisco Aguiar");
        nameMock.Setup(n => n.ToString()).Returns("Francisco Aguiar");

        var emailMock = new Mock<Email>("franciscoaguiar@example.com");
        emailMock.Setup(e => e.ToString()).Returns("franciscoaguiar@example.com");

        var phoneMock = new Mock<Phone>("123-456-7890");
        phoneMock.Setup(p => p.ToString()).Returns("123-456-7890");

        var addressMock = new Mock<Address>("123 Main St", "Springfield", "IL");
        addressMock.Setup(a => a.ToString()).Returns("123 Main St, Springfield, IL");

        var conditionMock = new Mock<Condition>("Asthma");
        conditionMock.Setup(c => c.ToString()).Returns("Asthma");

        var emergencyContactMock = new Mock<Phone>("098-765-4321");
        emergencyContactMock.Setup(ec => ec.ToString()).Returns("098-765-4321");

        var patient1 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = nameMock.Object,
            Gender = Gender.Male,
            Email = emailMock.Object,
            Phone = phoneMock.Object,
            Address = addressMock.Object,
            Conditions = new List<Condition> { conditionMock.Object },
            EmergencyContact = emergencyContactMock.Object,
            SystemUser = systemUserMock.Object
        };

        var patient2 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = nameMock.Object,
            Gender = Gender.Male,
            Email = emailMock.Object,
            Phone = phoneMock.Object,
            Address = addressMock.Object,
            Conditions = new List<Condition> { conditionMock.Object },
            EmergencyContact = emergencyContactMock.Object,
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
        systemUserMock1.Setup(su => su.Username).Returns("franciscoaguiar");
        systemUserMock1.Setup(su => su.Email).Returns(new Email("franciscoaguiar@example.com"));

        var systemUserMock2 = new Mock<SystemUser>();
        systemUserMock2.Setup(su => su.Username).Returns("saraaguiar");
        systemUserMock2.Setup(su => su.Email).Returns(new Email("saraaguiar@example.com"));

        var nameMock1 = new Mock<Name>("Francisco Aguiar");
        nameMock1.Setup(n => n.ToString()).Returns("Francisco Aguiar");

        var nameMock2 = new Mock<Name>("Sara Aguiar");
        nameMock2.Setup(n => n.ToString()).Returns("Sara Aguiar");

        var emailMock1 = new Mock<Email>("franciscoaguiar@example.com");
        emailMock1.Setup(e => e.ToString()).Returns("franciscoaguiar@example.com");

        var emailMock2 = new Mock<Email>("saraaguiar@example.com");
        emailMock2.Setup(e => e.ToString()).Returns("saraaguiar@example.com");

        var phoneMock1 = new Mock<Phone>("123-456-7890");
        phoneMock1.Setup(p => p.ToString()).Returns("123-456-7890");

        var phoneMock2 = new Mock<Phone>("098-765-4321");
        phoneMock2.Setup(p => p.ToString()).Returns("098-765-4321");

        var addressMock1 = new Mock<Address>("123 Main St", "Springfield", "IL");
        addressMock1.Setup(a => a.ToString()).Returns("123 Main St, Springfield, IL");

        var addressMock2 = new Mock<Address>("456 Elm St", "Metropolis", "NY");
        addressMock2.Setup(a => a.ToString()).Returns("456 Elm St, Metropolis, NY");

        var conditionMock1 = new Mock<Condition>("Asthma");
        conditionMock1.Setup(c => c.ToString()).Returns("Asthma");

        var conditionMock2 = new Mock<Condition>("Diabetes");
        conditionMock2.Setup(c => c.ToString()).Returns("Diabetes");

        var patient1 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = nameMock1.Object,
            Gender = Gender.Male,
            Email = emailMock1.Object,
            Phone = phoneMock1.Object,
            Address = addressMock1.Object,
            Conditions = new List<Condition> { conditionMock1.Object },
            EmergencyContact = phoneMock2.Object,
            SystemUser = systemUserMock1.Object
        };

        var patient2 = new Patient
        {
            DateOfBirth = new DateTime(2000, 1, 1),
            Name = nameMock2.Object,
            Gender = Gender.Female,
            Email = emailMock2.Object,
            Phone = phoneMock2.Object,
            Address = addressMock2.Object,
            Conditions = new List<Condition> { conditionMock2.Object },
            EmergencyContact = phoneMock1.Object,
            SystemUser = systemUserMock2.Object
        };

        Assert.NotEqual(patient1.DateOfBirth, patient2.DateOfBirth);
        Assert.NotEqual(patient1.Name.ToString(), patient2.Name.ToString());
        Assert.NotEqual(patient1.Gender, patient2.Gender);
        Assert.NotEqual(patient1.Email.ToString(), patient2.Email.ToString());
        Assert.NotEqual(patient1.Phone.ToString(), patient2.Phone.ToString());
        Assert.NotEqual(patient1.Address.ToString(), patient2.Address.ToString());
        Assert.NotEqual(patient1.SystemUser.Username, patient2.SystemUser.Username);
        Assert.NotEqual(patient1.SystemUser.Email.ToString(), patient2.SystemUser.Email.ToString());
    }

    [Fact]
    public void CanSetDeletePatientDate()
    {
        var patient = new Patient();
        var deleteDate = DateTime.Now;

        patient.DeletePatientDate = deleteDate;

        Assert.Equal(deleteDate, patient.DeletePatientDate);
    }

    [Fact]
    public void CanAddMultipleConditions()
    {
        var patient = new Patient();
        var condition1 = new Mock<Condition>("Asthma").Object;
        var condition2 = new Mock<Condition>("Diabetes").Object;

        patient.Conditions = new List<Condition> { condition1, condition2 };

        Assert.Contains(condition1, patient.Conditions);
        Assert.Contains(condition2, patient.Conditions);
        Assert.Equal(2, patient.Conditions.Count);
    }

    [Fact]
    public void CanSetEmergencyContact()
    {
        var patient = new Patient();
        var emergencyContact = new Mock<Phone>("098-765-4321");

        emergencyContact.Setup(ec => ec.ToString()).Returns("098-765-4321");

        patient.EmergencyContact = emergencyContact.Object;

        Assert.Equal("098-765-4321", patient.EmergencyContact.ToString());
    }

    [Fact]
    public void CanChangeSystemUser()
    {
        var patient = new Patient();
        var initialUser = new Mock<SystemUser>();
        initialUser.Setup(u => u.Username).Returns("user1");

        var newUser = new Mock<SystemUser>();
        newUser.Setup(u => u.Username).Returns("user2");

        patient.SystemUser = initialUser.Object;
        Assert.Equal("user1", patient.SystemUser.Username);

        patient.SystemUser = newUser.Object;
        Assert.Equal("user2", patient.SystemUser.Username);
    }   

    [Fact]
    public void ChangeBirthDate_ExactCurrentDate_ValidDate()
    {
        var patient = new Patient();
        var currentDate = DateTime.Now;

        patient.DateOfBirth = currentDate;

        Assert.Equal(currentDate, patient.DateOfBirth);
    }

    [Fact]
    public void ChangeBirthDate_AfterDate_ThrowsException()
    {
        var patient = new Patient();
        var oldDate = new DateTime(2025, 1, 1);

        Assert.Throws<BusinessRuleValidationException>(() => patient.DateOfBirth = oldDate);
    }
}
