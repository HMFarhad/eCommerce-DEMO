using System.ComponentModel.DataAnnotations;

namespace eCommerce.Infrastructure.Auth.ApiKey;

public class ApiKeySettings : IValidatableObject
{

    internal const string ConfigSectionPath = "SecuritySettings:ApiKey";

    public string Key { get; set; } = string.Empty;
    public string OneRegister { get; set; } = string.Empty;


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Key))
        {
            yield return new ValidationResult("No value defined in Security:SecuritySettings:ApiKey config", new[] { nameof(Key) });
        }
    }
}