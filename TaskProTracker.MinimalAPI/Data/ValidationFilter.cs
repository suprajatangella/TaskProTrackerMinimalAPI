
using System.ComponentModel.DataAnnotations;

namespace TaskProTracker.MinimalAPI.Data
{
    public class ValidationFilter<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dto = context.GetArgument<T>(0);

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage);
                return Results.BadRequest(errors);
            }

            return await next(context);
        }
    }
}
