using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Warehouse.Application.Metadata;

public static class ValidationMetadataHelper
{
    public static ValidationMetadataResponse GetMetadata<T>()
    {
        var response = new ValidationMetadataResponse
        {
            DtoName = typeof(T).Name
        };

        foreach (var property in typeof(T).GetProperties())
        {
            var rules = property
                .GetCustomAttributes<ValidationAttribute>()
                .Select(attribute => attribute.GetType().Name.Replace("Attribute", ""))
                .ToList();

            response.Properties.Add(new PropertyValidationMetadata
            {
                PropertyName = property.Name,
                PropertyType = property.PropertyType.Name,
                ValidationRules = rules
            });
        }

        return response;
    }
}