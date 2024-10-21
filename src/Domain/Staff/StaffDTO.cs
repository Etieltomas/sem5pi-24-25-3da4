using Sempi5;

namespace Sempi5.Domain.Staff
{
    public class StaffDTO
    {
        public string? Id { get; set; }
        public string LicenseNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }  
        public string Address { get; set; }
        public List<string> AvailabilitySlots { get; set; }  
        public string Specialization { get; set; }
    }
}