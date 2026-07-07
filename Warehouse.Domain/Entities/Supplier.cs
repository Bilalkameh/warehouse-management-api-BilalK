namespace Warehouse.Domain.Entities;

public class Supplier
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }


    public Supplier(
        string name,
        string country,
        string contactEmail,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Supplier name is required.");

        if (string.IsNullOrWhiteSpace(country))
            throw new Exception("Supplier country is required.");

        if (string.IsNullOrWhiteSpace(contactEmail))
            throw new Exception("Supplier email is required.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new Exception("Supplier phone number is required.");
        
        Id = Guid.NewGuid();
        Name =name;
        Country = country;
        ContactEmail = contactEmail;
        PhoneNumber = phoneNumber;
        IsActive = true;
    }



    public void Deactivate()
    {
        if (!IsActive)
            return;
        IsActive = false;
    }
}