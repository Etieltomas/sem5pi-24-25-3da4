namespace Sempi5.Domain.Shared
{
    /// <summary>
    /// Base class for entities.
    /// </summary>
    public abstract class Entity<TEntityId>
    where TEntityId: EntityId
    {
         public virtual TEntityId Id { get;  protected set; }
    }
}