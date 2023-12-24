using System.ComponentModel.DataAnnotations;
using Thatnetwork.Photos.Validation;
using Thatnetwork.Users.Dtos;
using Thatnetwork.Users.Validation;
using Thatnetwork.Utils.Validators;

namespace Thatnetwork.Challenges.Dtos
{
    public class AddMarathonDto
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        
        [AllowedTagSignature]
        public required string Tag { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        [MaxLength(3)]
        public List<string> Hashtags { get; set; } = new();

        [AllowedPhotoId]
        public int? AvatarPhotoId {  get; set; }
    }
}
