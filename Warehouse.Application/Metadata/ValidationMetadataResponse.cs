namespace Warehouse.Application.Metadata;

public class ValidationMetadataResponse
{
    public string DtoName { get; set; } = string.Empty;

    public List<PropertyValidationMetadata> Properties { get; set; } = new List<PropertyValidationMetadata>();
}

public class PropertyValidationMetadata
{
    public string PropertyName { get; set; } = string.Empty;

    public string PropertyType { get; set; } = string.Empty;

    public List<string> ValidationRules { get; set; } = new List<string>();
}