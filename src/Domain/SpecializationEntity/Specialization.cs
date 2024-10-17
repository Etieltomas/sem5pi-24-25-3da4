using Sempi5.Domain.Shared;
using Sempi5.Domain.User;

namespace Sempi5.Domain.SpecializationEntity
{
    public class Specialization : Entity<SpecializationID>, IAggregateRoot
    {
        public SpecializationID Id { get; set; }
    }
}
