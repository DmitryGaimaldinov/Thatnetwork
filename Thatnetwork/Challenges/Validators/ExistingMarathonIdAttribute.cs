using System.ComponentModel.DataAnnotations;
using Thatnetwork.Entities;
using Thatnetwork.Middlewares;
using Thatnetwork.Users;

namespace Thatnetwork.Challenges.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ExistingMarathonIdAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Укажите id марафона");
            }
            int marathonId = (int)value;

            var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            User currUser = httpContextAccessor.HttpContext.GetCurrentUser();

            Marathon? marathon = dbContext.Marathons.SingleOrDefault(m => m.Id == marathonId);
            if (marathon == null)
            {
                return new ValidationResult("Марафон не найден");
            }

            return ValidationResult.Success;
        }
    }
}
