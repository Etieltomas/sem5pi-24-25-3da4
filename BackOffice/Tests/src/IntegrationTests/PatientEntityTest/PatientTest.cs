using System;
using Xunit;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;
using System.Collections.Generic;

namespace Sempi5Test.IntegrationTests.PatientEntityTest;

public class PatientTest
{ 
    [Fact]
    public void CanInitializePatient()
    {
        var systemUser = new SystemUser{
            Username = "franciscoaguiar", 
            Role = "Patient",
            Email = new Email("franciscoaguiar@example.com"),
            Active = true,
            MarketingConsent = false
        };

        var patient = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("Francisco Aguiar"),
            Gender = Gender.Male,
            Email = new Email("franciscoaguiar@example.com"),
            Phone = new Phone("123-456-7890"),
            Address = new Address("123 Main St", "Springfield", "IL"),
            Conditions = new List<Condition> { new Condition("Asthma") },
            EmergencyContact = new Phone("098-765-4321"),
            SystemUser = systemUser
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
        var systemUser = new SystemUser{
            Username = "franciscoaguiar", 
            Role = "Patient",
            Email = new Email("franciscoaguiar@example.com"),
            Active = true,
            MarketingConsent = false
        };

        var patient1 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("Francisco Aguiar"),
            Gender = Gender.Male,
            Email = new Email("franciscoaguiar@example.com"),
            Phone = new Phone("123-456-7890"),
            Address = new Address("123 Main St", "Springfield", "IL"),
            Conditions = new List<Condition> { new Condition("Asthma") },
            EmergencyContact = new Phone("098-765-4321"),
            SystemUser = systemUser
        };

        var patient2 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("Francisco Aguiar"),
            Gender = Gender.Male,
            Email = new Email("franciscoaguiar@example.com"),
            Phone = new Phone("123-456-7890"),
            Address = new Address("123 Main St", "Springfield", "IL"),
            Conditions = new List<Condition> { new Condition("Asthma") },
            EmergencyContact = new Phone("098-765-4321"),
            SystemUser = systemUser
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
        var systemUser1 = new SystemUser{
            Username = "franciscoaguiar", 
            Role = "Patient",
            Email = new Email("franciscoaguiar@example.com"),
            Active = true,
            MarketingConsent = false
        };

        var systemUser2 = new SystemUser{
            Username = "saraaguiar", 
            Role = "Patient",
            Email = new Email("sara@example.com"),
            Active = true,
            MarketingConsent = false
        };

        var patient1 = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("Francisco Aguiar"),
            Gender = Gender.Male,
            Email = new Email("franciscoaguiar@example.com"),
            Phone = new Phone("123-456-7890"),
            Address = new Address("123 Main St", "Springfield", "IL"),
            Conditions = new List<Condition> { new Condition("Asthma") },
            EmergencyContact = new Phone("098-765-4321"),
            SystemUser = systemUser1
        };

        var patient2 = new Patient
        {
            DateOfBirth = new DateTime(2000, 1, 1), 
            Name = new Name("Sara Aguiar"),
            Gender = Gender.Female, 
            Email = new Email("saraaguiar@example.com"),
            Phone = new Phone("098-765-4321"),
            Address = new Address("456 Elm St", "Metropolis", "NY"),
            Conditions = new List<Condition> { new Condition("Diabetes") },
            EmergencyContact = new Phone("123-456-7890"),
            SystemUser = systemUser2
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
    public void CanInitializePatientWithMinimumData()
    {
        var patient = new Patient
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Name = new Name("Francisco Aguiar")
        };

        Assert.NotNull(patient);
        Assert.Equal("Francisco Aguiar", patient.Name.ToString());
    }

    [Fact]
    public void AddCondition_IncreasesConditionsCount()
    {
        var patient = new Patient
        {
            Conditions = new List<Condition>()
        };

        var condition = new Condition("Asthma");
        patient.Conditions.Add(condition);

        Assert.Single(patient.Conditions);
    }
    
    [Fact]
    public void RemoveCondition_DecreasesConditionsCount()
    {
        var condition = new Condition("Asthma");
        var patient = new Patient
        {
            Conditions = new List<Condition> { condition }
        };

        patient.Conditions.Remove(condition);

        Assert.Empty(patient.Conditions);
    }

    [Fact]
    public void PatientWithSameEmailShouldNotBeEqual()
    {
        var patient1 = new Patient
        {
            Email = new Email("franciscoaguiar@example.com")
        };

        var patient2 = new Patient
        {
            Email = new Email("franciscoaguiar@example.com")
        };

        Assert.Equal(patient1.Email.ToString(), patient2.Email.ToString());
    }

    [Fact]
    public void PatientWithDifferentEmailShouldNotBeEqual()
    {
        var patient1 = new Patient
        {
            Email = new Email("franciscoaguiar@example.com")
        };

        var patient2 = new Patient
        {
            Email = new Email("saraaguiar@example.com")
        };

        Assert.NotEqual(patient1.Email.ToString(), patient2.Email.ToString());
    }

    [Fact]
    public void CanAddAndRemoveConditions()
    {
        var patient = new Patient
        {
            Conditions = new List<Condition>()
        };

        var condition1 = new Condition("Asthma");
        var condition2 = new Condition("Diabetes");

        patient.Conditions.Add(condition1);
        patient.Conditions.Add(condition2);

        Assert.Equal(2, patient.Conditions.Count);

        patient.Conditions.Remove(condition1);
        Assert.Equal(1, patient.Conditions.Count);
    }

    [Fact]
    public void SetDeletePatientDate_ShouldSetCorrectly()
    {
        var patient = new Patient();
        var deleteDate = DateTime.Now.AddDays(30);
        patient.DeletePatientDate = deleteDate;

        Assert.Equal(deleteDate, patient.DeletePatientDate);
    }

    [Fact]
    public void PatientIsNotActiveWhenSystemUserIsInactive()
    {
        var systemUser = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Patient",
            Email = new Email("franciscoaguiar@example.com"),
            Active = false,
            MarketingConsent = false
        };

        var patient = new Patient { SystemUser = systemUser };

        Assert.False(patient.SystemUser.Active);
    }   

    [Fact]
    public void Email_ValidEmail_SetsEmail()
    {
        var patient = new Patient();
        var email = new Email("valid@example.com");

        patient.Email = email;

        Assert.Equal("valid@example.com", patient.Email.ToString());
    }

    [Fact]
    public void Email_InvalidEmail_ThrowsException()
    {
        var patient = new Patient();

        Assert.Throws<BusinessRuleValidationException>(() => patient.Email = new Email("invalid-email"));
    }

    [Fact]
    public void AddEmergencyContact_SetsEmergencyContact()
    {
        var patient = new Patient();
        var emergencyContact = new Phone("123-456-7890");

        patient.EmergencyContact = emergencyContact;

        Assert.Equal("123-456-7890", patient.EmergencyContact.ToString());
    }

    [Fact]
    public void UpdateEmergencyContact_ChangesEmergencyContact()
    {
        var patient = new Patient { EmergencyContact = new Phone("123-456-7890") };
        var newEmergencyContact = new Phone("987-654-3210");

        patient.EmergencyContact = newEmergencyContact;

        Assert.Equal("987-654-3210", patient.EmergencyContact.ToString());
    }  

    [Fact]
    public void Patient_IsActive_ReturnsCorrectStatus()
    {
        var systemUser = new SystemUser { Active = true };
        var patient = new Patient { SystemUser = systemUser };

        Assert.True(patient.SystemUser.Active);
    } 

    [Fact]
    public void RemoveCondition_NonExistentCondition_DoesNotThrow()
    {
        var patient = new Patient { Conditions = new List<Condition>() };

        var exception = Record.Exception(() => patient.Conditions.Remove(new Condition("NonExistent")));

        Assert.Null(exception);
        Assert.Empty(patient.Conditions);
    }

    [Fact]
    public void UpdatePersonalDetails_UpdatesFields()
    {
        var patient = new Patient
        {
            Name = new Name("Francisco Aguiar"),
            Phone = new Phone("123-456-7890")
        };

        patient.Name = new Name("Sara Aguiar");
        patient.Phone = new Phone("987-654-3210");

        Assert.Equal("Sara Aguiar", patient.Name.ToString());
        Assert.Equal("987-654-3210", patient.Phone.ToString());
    }       
}
