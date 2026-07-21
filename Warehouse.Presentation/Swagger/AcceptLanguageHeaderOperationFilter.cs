using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Warehouse.Presentation.Swagger;

public class AcceptLanguageHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Supported cultures: en-US or fr-FR",
            Schema = new OpenApiSchema
            {
                Type = "string"
            } 
        });
    }
}