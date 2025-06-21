using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace TaskProTracker.MinimalAPI.Transformers
{
    public class AddExternalDocsTransformer(IConfiguration configuration) : IOpenApiOperationTransformer, IOpenApiSchemaTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (operation.OperationId is { Length: > 0 } id &&
                Uri.TryCreate(configuration["DocumentationBaseUrl"], UriKind.Absolute, out var baseUri))
            {
                var url = new Uri(baseUri, $"/api/docs/operations/{Uri.EscapeDataString(id)}");

                operation.ExternalDocs = new OpenApiExternalDocs
                {
                    Description = "Documentation for this OpenAPI endpoint",
                    Url = url
                };
            }

            return Task.CompletedTask;
        }

        public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
        {
            if (Uri.TryCreate(configuration["DocumentationBaseUrl"], UriKind.Absolute, out var baseUri))
            {
                var url = new Uri(baseUri, $"/api/docs/schemas/{Uri.EscapeDataString(schema.Type.ToString()!.ToLowerInvariant())}");

                schema.ExternalDocs = new OpenApiExternalDocs
                {
                    Description = "Documentation for this OpenAPI schema",
                    Url = url
                };
            }
            return Task.CompletedTask;
        }
    }
}
