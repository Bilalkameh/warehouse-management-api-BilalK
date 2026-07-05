using warehouse_management.Models;
namespace warehouse_management;

public class FakeSupplierStore
{
    public List<Supplier> DummySuppliers = new List<Supplier>();
    
    //Adapted the store to use a Singleton and resolve it via Dependency Injection.

    public FakeSupplierStore()
    {   
        var sup1 = new Supplier("74d3e479-2ef4-4c82-a4e0-b4b8d102fd9a", "sup1",
            "lebanon", "sup1@mail.com","+961-81-123-456");
        
        var sup2 = new Supplier("34dc5cc9-0ef7-481a-9f9d-b3bcb66239d1", "sup2",
            "lebanon", "sup2@mail.com","+961-71-654-123");
        
        var sup3 = new Supplier("48528e3f-2f7d-495c-bef5-30ca183abfb7", "sup3",
            "usa", "sup3@mail.com","+1-123-456");
        
        var sup4 = new Supplier("ca740890-385c-4f4a-9d5f-9a35bee02ab7", "sup4",
            "germany", "sup4@mail.com","+49-123-333");
        
        DummySuppliers.Add(sup1);
        DummySuppliers.Add(sup2);
        DummySuppliers.Add(sup3);
        DummySuppliers.Add(sup4);
    }

    public List<Supplier> GetAllSuppliers()
    {
        return DummySuppliers;
    }
    

    public Supplier? GetSupplierById(string id)
    {
        foreach (var supplier in DummySuppliers)
        {
            if (supplier.Id == id)
                return supplier;
        }
        return null;
    }

    public void AddSupplier(Supplier supplier)
    {
        DummySuppliers.Add(supplier);
    }
    
    
    // This one marks the supplier as inactive instead of removing it completely from the system
    public bool Deactivate(string id)
    {
        var supplier = GetSupplierById(id);
        
        if (supplier == null)
            return false;
        
        supplier.IsActive = false;
        return true;
    }
    

}