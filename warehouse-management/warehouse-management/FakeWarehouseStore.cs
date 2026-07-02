using warehouse_management.Models;

namespace warehouse_management;

public class FakeWarehouseStore
{
    public static List<Product> DummyProducts = new List<Product>();
    
    //This prevents seeding the products multiple times if the constructor is called more than once.
    private static bool _isSeeded;



    public FakeWarehouseStore()
    {
        
        if (_isSeeded) return;
        
        var laptop1 = new Product("ceccabb0-71c0-4a5b-91de-47791acafbbd", "laptop1", "powerful gaming laptop",
            1299.9, 10, "laptopSupplier1", new DateTime(2029, 2, 7));

        var laptop2 = new Product("bccc8c96-3eda-4c57-95e5-f4a5faf75a7e", "laptop2", "budget laptop",
            500, 12, "laptopSupplier2", new DateTime(2028, 5, 30));
        
        var mouse1 = new Product("e3cd8ddc-f167-417c-92f0-4c0c49ef2374", "mouse1", "budget mouse",
            10, 30, "mouseSupplier1", new DateTime(2028, 1, 1));
        
        var mouse2 = new Product("7a1a1873-ff23-4438-a7ba-8a05dd3134e6", "mouse2", "gaming mouse",
            40, 15, "mouseSupplier2", new DateTime(2028, 1, 1));
        
        var keyboard1 = new Product("de52a77b-2e4c-4021-b32d-29b45cb27f8d", "keyboard1", "gaming keyboard",
            60.99, 20, "keyBoardSupplier1", new DateTime(2029, 1, 1));
        
        var keyboard2 = new Product("49bdd028-d8eb-46b5-bbcc-9986c55db365", "keyboard2", "budget keyboard",
            20, 25, "keyBoardSupplier2", new DateTime(2029, 6, 30));
        
        var scanner1 = new Product("4ff1ca99-4bd5-403d-b6a9-a5025bd354ef", "scanner1", "scanner 1",
            100, 8, "scannerSupplier1", new DateTime(2029, 4, 1));
        
        var printer1 = new Product("b5ae82b5-dc95-4cfd-bcd3-72b532f45948", "printer1", "printer 1",
            120.99, 7, "printerSupplier1", new DateTime(2028, 5, 1));
        
        var monitor1 = new Product("6a2738ac-2e92-4582-9136-70ca49e7adac", "monitor1", "OLED monitor",
            450.99, 5, "monitorSupplier1", new DateTime(2029, 1, 1));
        
        var monitor2 = new Product("ba7d46eb-944f-4095-91f3-ad4d36043791", "monitor2", "budget monitor",
            100.99, 17, "monitorSupplier2", new DateTime(2029, 1, 1));

        
        DummyProducts.Add(laptop1);
        DummyProducts.Add(laptop2);
        DummyProducts.Add(mouse1);
        DummyProducts.Add(mouse2);
        DummyProducts.Add(keyboard1);
        DummyProducts.Add(keyboard2);
        DummyProducts.Add(scanner1);
        DummyProducts.Add(printer1);
        DummyProducts.Add(monitor1);
        DummyProducts.Add(monitor2);

        _isSeeded = true;
    }
    
    public List<Product> GetAll()
    {
        return DummyProducts;
    }
    
    
    //We want to implement the necessary methods that will be used in the controller
    
    //First the Read/Get Methods
    public Product? GetById(string id)
    {
        foreach (var product in DummyProducts)
        {
            if (product.Id == id)
            {
                return product;
            }
        }
        return null;
    }
    
    public List<Product> Search(string? name, string? supplier)
    {
        return DummyProducts.Where(p => (string.IsNullOrWhiteSpace(name) ||
                                         p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                                         (string.IsNullOrWhiteSpace(supplier) ||
                                         p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }
    
    //Second the Write/Set Methods
    
    public void Add(Product product)
    {
        DummyProducts.Add(product);
    }

    public bool UpdateQuantity(string id, int quantity)
    {
        var product = GetById(id);
        if (product == null)
            return false;
        
        product.QuantityInStock = quantity;
        product.LastUpdatedAt = DateTime.UtcNow;
        return true;
    }

    public bool UpdatePrice(string id, double price)
    {
        var product = GetById(id);
        if (product == null)
            return false;
        
        product.Price = price;
        product.LastUpdatedAt = DateTime.UtcNow;
        return true;
    }

    public bool Archive(string id)
    {
        var product =  GetById(id);
        if (product == null)
            return false;
        
        //Marks the product as Archived to make use of the isArchived property in the Product.cs class
        product.IsArchived = true;
        product.LastUpdatedAt = DateTime.UtcNow;
        return true;
    }
    
    public bool Delete(string id)
    {
        var product = GetById(id);
        if (product == null)
            return false;

        DummyProducts.Remove(product);
        return true;
    }
    
}

    