using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;
using System;
using System.Collections.Generic;

namespace Sempi5.Domain.PatientEntity
{
    public class Patient : Entity<PatientID>, IAggregateRoot
    {
        private DateTime _dateOfBirth;

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => ChangeBirthDate(value);
        }

        public Name Name { get; set; }
        public Gender Gender { get; set; }
        public Email Email { get; set; }
        public Phone Phone { get; set; }
        public Address Address { get; set; }
        public List<Condition> Conditions { get; set; }
        public Phone EmergencyContact { get; set; }
        public SystemUser? SystemUser { get; set; }

        public void ChangeBirthDate(DateTime newDate)
        {
            if (newDate > DateTime.Now)
            {
                throw new BusinessRuleValidationException("Date of birth cannot be in the future");
            }
            _dateOfBirth = newDate;
        }
    }
}
