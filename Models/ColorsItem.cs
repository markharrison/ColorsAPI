using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColorsAPI.Models
{
    [SwaggerSchema(Description = "An object representing a color with its identifier, name, hexadecimal code, and additional data.")]
    public class ColorsItem
    {
        [Required]
        [SwaggerSchema(Description = "Represents a numeric identifier for the color. It is required and must be an integer.")]
        public int Id { get; set; }

        [Required]
        [SwaggerSchema(Description = "Represents the name of the color. It is required and must be a string.")]
        public string Name { get; set; }

        [SwaggerSchema(Description = "Represents the hexadecimal code for the color. It is optional and can be an empty string.")]
        public string Hexcode { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Represents additional data or metadata related to the color. It is optional and can be an empty string.")]
        public string Data { get; set; } = string.Empty;
    }
}
