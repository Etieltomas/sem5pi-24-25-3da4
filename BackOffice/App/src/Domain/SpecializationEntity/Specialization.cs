using Sempi5.Domain.Shared;

namespace Sempi5.Domain.SpecializationEntity
{
    public class Specialization : Entity<SpecializationID>, IAggregateRoot
    {
        public string Name { get; set; }

        public void UpdateName(string newName)
        {
            Name = newName;
        }
    }
}
