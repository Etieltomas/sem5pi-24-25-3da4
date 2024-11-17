using Sempi5.Domain.Shared;

public enum AppointmentType
{
    Consultation,
    Meeting,
    Surgery,
    Other
}

public static class AppointmentTypeExtensions
    {
        public static AppointmentType FromString(string appointmentTypeString)
        {
            appointmentTypeString = appointmentTypeString?.Replace(" ", string.Empty).ToLower();
            if (string.IsNullOrEmpty(appointmentTypeString))
            {
                throw new BusinessRuleValidationException("Appointment Status cannot be null or empty.");
            }

            switch (appointmentTypeString)
            {
                case "consultation":
                    return AppointmentType.Consultation;
                case "meeting":
                    return AppointmentType.Meeting;
                case "surgery":
                    return AppointmentType.Surgery;
                case "other":
                    return AppointmentType.Other;
                default:
                    throw new BusinessRuleValidationException("Appointment Type does not exist");
            }
        }
    }