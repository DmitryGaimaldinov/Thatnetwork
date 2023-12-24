using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Thatnetwork.Entities;
using Thatnetwork.Middlewares;

namespace Thatnetwork.Users.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AllowedUserTagAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var logger = validationContext.GetService(typeof(ILogger<AllowedUserTagAttribute>)) as ILogger<AllowedUserTagAttribute>;
            logger.LogInformation($"validation attribute value: {value}");

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

            if (tag.StartsWith("id") && char.IsNumber(tag[2]))
            {
                var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                User user = httpContextAccessor.HttpContext.GetCurrentUser();

                int idFromTag = int.TryParse(tag.Substring(2), out idFromTag) ? idFromTag : -1;

                if (user.Id != idFromTag)
                {
                    return new ValidationResult($"Недопустимый идентификационный тэг пользователя. Можно только id{user.Id}");
                }
            }

            var _dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            bool tagIsBusy = _dbContext.Users.Any(u => u.Tag == tag);
            if (tagIsBusy)
            {
                return new ValidationResult("Извините, данный тег занят");
            }

            return ValidationResult.Success;
        }
    }
}
