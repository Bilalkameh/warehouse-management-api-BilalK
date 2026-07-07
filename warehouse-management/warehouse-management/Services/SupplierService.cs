using warehouse_management.Models;
using warehouse_management.Contracts;
namespace warehouse_management;



public class SupplierService
{
    private readonly FakeSupplierStore _supplierStore;
    
    public SupplierService(FakeSupplierStore supplierStore)
    {
        _supplierStore = supplierStore;
    }
    
    //1. Get All Suppliers
    public List<Supplier> GetAllSuppliers()
    {
        var suppliers = _supplierStore.GetAllSuppliers()
            .Where(s => s.IsActive)
            .ToList();
        
        return suppliers;
    }
    
    //2. Get Supplier by Id
    public Supplier? GetSupplierById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var supplier = _supplierStore.GetSupplierById(id);

        if (supplier == null || !supplier.IsActive)
            return null;

        return supplier;
    }
    
    //3. Create Supplier
    public Supplier? CreateSupplier(CreateSupplierRequest request)
    {
        if (request == null)
            return null;

        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Country) ||
            string.IsNullOrWhiteSpace(request.ContactEmail) ||
            string.IsNullOrWhiteSpace(request.PhoneNumber))
            return null;

        var newSupplier = new Supplier(
            Guid.NewGuid().ToString(),
            request.Name,
            request.Country,
            request.ContactEmail,
            request.PhoneNumber
        );

        _supplierStore.AddSupplier(newSupplier);

        return newSupplier;
    }
    //4. Deactivate Supplier
    public bool DeactivateSupplier(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        var supplier = _supplierStore.GetSupplierById(id);

        if (supplier == null || !supplier.IsActive)
            return false;

        return _supplierStore.Deactivate(id);
    }
    
    
}