using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyMovieList.Business.Configuration;

public class EnumAsStringSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        // Check if the type is an enum
        if (type.IsEnum)
        {
            // Update the schema type to string
            schema.Type = "string";
            schema.Enum.Clear();

            // Add enum values as string to the schema
            foreach (var enumValue in Enum.GetValues(type))
            {
                var field = type.GetField(enumValue.ToString()!);
                var enumMemberAttribute = field?.GetCustomAttribute<EnumMemberAttribute>();
                var enumValueAsString = enumMemberAttribute?.Value ?? enumValue.ToString();
                schema.Enum.Add(new OpenApiString(enumValueAsString));
            }
        }
    }
}