namespace Warehouse.Domain.Entities;

public class Supplier
{
    public Guid SupplierId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime LastUpdatedAt { get; private set; }


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
        
        SupplierId = Guid.NewGuid();
        Name =name;
        Country = country;
        ContactEmail = contactEmail;
        PhoneNumber = phoneNumber;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
		LastUpdatedAt = DateTime.UtcNow;
    }
    
    //parameterless constructor
    
    private Supplier()
    {
    }
    
    public void Deactivate()
    {
        if (!IsActive)
            return;
        IsActive = false;
LastUpdatedAt = DateTime.UtcNow;
    }
}