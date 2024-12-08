using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;
using System;
using System.Collections.Generic;

namespace Sempi5.Domain.PatientEntity
{
    public class Patient : Entity<PatientID>, IAggregateRoot
    {
        private DateTime _dateOfBirth;

        public virtual DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => ChangeBirthDate(value);
        }

        public virtual Name Name { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Email Email { get; set; }
        public virtual Phone Phone { get; set; }
        public virtual Address Address { get; set; }
        public virtual Phone EmergencyContact { get; set; }
        public virtual SystemUser? SystemUser { get; set; }

        public virtual DateTime? DeletePatientDate { get; set; }

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
