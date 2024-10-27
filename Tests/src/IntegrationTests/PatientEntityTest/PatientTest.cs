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
}
