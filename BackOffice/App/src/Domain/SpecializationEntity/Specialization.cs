using Sempi5.Domain.Shared;

namespace Sempi5.Domain.SpecializationEntity
{
    public class Specialization : Entity<SpecializationID>, IAggregateRoot
    {
        public string Name { get; set; }
        public string Code { get; set; } 
        public string? Description { get; set; }  
        public void Update(string newName, string newCode, string? newDescription)
        {
            Name = newName;
            Code = newCode;
            Description = newDescription;
        }
    }
}

