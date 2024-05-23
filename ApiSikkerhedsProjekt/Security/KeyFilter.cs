using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiSikkerhedsProjekt.Security
{
  public class KeyFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      if (operation.Parameters is null)
      {
        operation.Parameters = new List<OpenApiParameter>();
      }

      operation.Parameters.Add(new OpenApiParameter
      {
        Name = "Api-Key",
        In = ParameterLocation.Header,
        Required = true,
        Schema = new OpenApiSchema
        {
          Type = "string"
        }
      });
    }
  }
}
