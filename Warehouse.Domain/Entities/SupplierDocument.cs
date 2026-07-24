using Warehouse.Domain.Exceptions;

namespace Warehouse.Domain.Entities;

public class SupplierDocument
{
    public Guid Id { get; private set; }
    public Guid SupplierId { get; private set; }
    public Supplier Supplier { get; private set; } = null!;

    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;

    public SupplierDocument(Supplier supplier, string fileName, string filePath)
    {
        if (supplier == null)
            throw new ArgumentNullException(nameof(supplier));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new BusinessRuleException("File name is required.");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new BusinessRuleException("File path is required.");

        Id = Guid.NewGuid();
        Supplier = supplier;
        SupplierId = supplier.SupplierId;
        FileName = fileName;
        FilePath = filePath;
    }

    private SupplierDocument()
    {
    }
}