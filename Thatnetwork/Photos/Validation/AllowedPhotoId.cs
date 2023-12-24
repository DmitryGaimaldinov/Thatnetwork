using System.ComponentModel.DataAnnotations;
using Thatnetwork.Middlewares;
using Thatnetwork.Users;

namespace Thatnetwork.Photos.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AllowedPhotoIdAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var logger = validationContext.GetService(typeof(ILogger<AllowedPhotoIdAttribute>)) as ILogger<AllowedPhotoIdAttribute>;

            if (value == null)
            {
                return ValidationResult.Success;
            }

            int photoId = int.TryParse(value.ToString(), out int id) ? id : -1;
            if (photoId == -1)
            {
                return new ValidationResult("Указан некорректный id фотографии");
            }

            String? errorText = PhotoIdValidator.Validate(photoId, validationContext);
            if (errorText != null)
            {
                return new ValidationResult(errorText);
            }

            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AllowedPhotoIdsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var logger = validationContext.GetService(typeof(ILogger<AllowedPhotoIdAttribute>)) as ILogger<AllowedPhotoIdAttribute>;

            if (value == null)
            {
                return ValidationResult.Success;
            }

            List<int> photoIds = value as List<int>;
            foreach (int photoId in photoIds)
            {
                String? errorText = PhotoIdValidator.Validate(photoId, validationContext);
                if (errorText != null)
                {
                    return new ValidationResult(errorText);
                }
            }

            return ValidationResult.Success;
        }
    }

    internal class PhotoIdValidator
    {
        public static String? Validate(int photoId, System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            User user = httpContextAccessor.HttpContext.GetCurrentUser();

            var photoService = validationContext.GetService(typeof(PhotoService)) as PhotoService;
            Photo? photo = photoService.GetPhotoById(photoId);
            if (photo == null)
            {
                return "Фотография не найдена";
            }
            if (photo.Owner.Id != user.Id)
            {
                return "Нельзя установить чужую фотографию";
            }

            return null;
        }
    }
}
