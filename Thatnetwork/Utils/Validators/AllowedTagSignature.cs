using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Thatnetwork.Middlewares;
using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Utils.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AllowedTagSignatureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var logger = validationContext.GetService(typeof(ILogger<AllowedTagSignatureAttribute>)) as ILogger<AllowedTagSignatureAttribute>;

            if (value == null)
            {
                return ValidationResult.Success;
            }

            string tag = value as string;

            var tagRegExp = new Regex(@"^[a-z0-9_]+$");
            if (!tagRegExp.IsMatch(tag))
            {
                return new ValidationResult("Тэг пользователя может содержать только английские символы, цифры и подчёркивания");
            }
            if (tag.Length < 3 || tag.Length > 20)
            {
                return new ValidationResult("Длина тега должна быть от 3 до 20 символов");
            }

            return ValidationResult.Success;
        }
    }
}
