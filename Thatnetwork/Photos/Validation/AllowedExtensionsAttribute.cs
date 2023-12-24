using System.ComponentModel.DataAnnotations;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    private ILogger<AllowedExtensionsAttribute> _logger;

    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
        
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        bool isValid;

        if (value is IFormFile)
        {
            var file = value as IFormFile;
            isValid = IsFileValid(file);
        }
        else if (value is List<IFormFile>)
        {
            var files = value as List<IFormFile>;
            isValid = files.All(f => IsFileValid(f));
        } else
        {
            isValid = false;
        }

        if (isValid)
        {
            return ValidationResult.Success;
        }
        return new ValidationResult(GetErrorMessage());
    }

    private bool IsFileValid(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        
        if (!_extensions.Contains(extension.ToLower()))
        {
            return false;
        }
        return true;
    }

    public string GetErrorMessage()
    {
        return $"This file extension is not allowed! Only files with extension: {string.Join(", ", _extensions)} are allowed";
    }
}