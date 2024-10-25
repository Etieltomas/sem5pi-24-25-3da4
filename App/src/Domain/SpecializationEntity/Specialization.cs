using Sempi5.Domain.Shared;

namespace Sempi5.Domain.SpecializationEntity
{
    public class Specialization : Entity<SpecializationID>, IAggregateRoot
    {
        public Specialization(SpecializationID id)
        {
            Id = id;
        }
    }
}
