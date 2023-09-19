using System.ComponentModel.DataAnnotations;

namespace Thatnetwork.Dtos
{
    public class LoginByTagDto
    {
        [Required]
        public string Tag { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
