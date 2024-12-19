using Sempi5.Domain.Shared;

namespace Sempi5.Domain.RoomEntity
{
    public class RoomType : Entity<RoomTypeID>, IAggregateRoot
    {
        public string Name { get; private set; }

        public RoomType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Room type name cannot be null or empty.", nameof(name));
            }

            Name = name;
        }
    }

    public class RoomTypeID : EntityId
    {

        public RoomTypeID(long value) : base(value)
        {
        }

        public override string AsString()
        {
            return (string)base.ObjValue.ToString();
        }

        protected override object createFromString(string text)
        {
            return long.Parse(text);
        }

        public long AsLong()
        {
            return (long)base.ObjValue;
        }
    }
}
