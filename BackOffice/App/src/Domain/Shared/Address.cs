using Sempi5.Domain.Shared;

public class Address : IValueObject
{
    private string city;
    private string street;
    private string state;

    public Address(string street, string city, string state)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrEmpty(street) || string.IsNullOrEmpty(state))
        {
            throw new BusinessRuleValidationException("Address cannot be null or empty.");
        }


        this.city = city;
        this.street = street;
        this.state = state;
    }

    public bool Equals(Address obj)
    {
        return obj.city == city && obj.street == street && obj.state == state;
    }

    public override string ToString()
    {
        return street+", "+city+", "+state;
    }
}