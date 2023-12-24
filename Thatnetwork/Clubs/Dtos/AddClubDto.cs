using System.ComponentModel.DataAnnotations;

namespace Thatnetwork.Clubs.Dtos
{
    public class AddClubDto
    {
        [Required, StringLength(20, MinimumLength = 1)]
        public required string Name { get; set; }

        private string _tag;
        [Required, StringLength(20, MinimumLength = 1)]
        public required string Tag {
            get => _tag; 
            set => _tag = value.ToLower();
        }

        [StringLength(1000, MinimumLength = 0)]
        public string Description { get; set; } = "";

        public int? AvatarId { get; set; }
    }
}
