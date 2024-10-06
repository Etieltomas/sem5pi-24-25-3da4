using Sempi5;

namespace Sempi5.Domain.Staff
{
    public class StaffDTO
    {
        public long LicenseNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }  
        public List<string> AvailabilitySlots { get; set; }  
        public string Specialization { get; set; }
    }
}