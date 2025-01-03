using Sempi5.Domain.Shared;

namespace Sempi5.Domain.SpecializationEntity
{
    public class Specialization : Entity<SpecializationID>, IAggregateRoot
    {
        /**
         * Specialization.cs created by Ricardo Guimarães on 10/12/2024
         */
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

