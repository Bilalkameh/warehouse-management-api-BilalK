namespace Warehouse.Domain.Entities;
public class Supplier
{
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string Country { get; set; } = string.Empty;
    
    public string ContactEmail { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;

    public Supplier(string id, string name, string country, string contactEmail, string phoneNumber)
    {
        Id = id;
        Name = name;
        Country = country;
        ContactEmail = contactEmail;
        PhoneNumber = phoneNumber;
        IsActive = true;
    }

    public Supplier()
    {
        
    }



}