using Sempi5.Domain.Shared;

public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Canceled
}

public static class AppointmentStatusExtensions
    {
        public static AppointmentStatus FromString(string appointmentStatusString)
        {
            appointmentStatusString = appointmentStatusString?.Replace(" ", string.Empty).ToLower();
            if (string.IsNullOrEmpty(appointmentStatusString))
            {
                throw new BusinessRuleValidationException("Appointment Status cannot be null or empty.");
            }

            switch (appointmentStatusString)
            {
                case "scheduled":
                    return AppointmentStatus.Scheduled;
                case "completed":
                    return AppointmentStatus.Completed;
                case "canceled":
                    return AppointmentStatus.Canceled;
                default:
                    throw new BusinessRuleValidationException("Appointment Status does not exist");
            }
        }
    }