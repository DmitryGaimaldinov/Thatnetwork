using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Thatnetwork.Challenges.Dtos;
using Thatnetwork.Chats;
using Thatnetwork.Entities;
using Thatnetwork.Middlewares;
using Thatnetwork.Users;

namespace Thatnetwork.Challenges.Validators
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedToJoinMarathonAttribute : ValidationAttribute
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

            Marathon marathon = dbContext.Marathons.Single(m => m.Id == marathonId);
            if (marathon.EndDate < DateTime.Now)
            {
                return new ValidationResult("Нельзя присоединиться к марафону, который уже начался");
            }

            ChallengeService challengeService = validationContext.GetService(typeof(ChallengeService)) as ChallengeService;
            bool isJoined = challengeService.IsJoined(marathonId, currUser.Id);
            if (isJoined)
            {
                return new ValidationResult("Вы уже присоединились к марафону");
            }

            return ValidationResult.Success;
        }
    }
}
