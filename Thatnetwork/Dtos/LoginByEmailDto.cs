using System.ComponentModel.DataAnnotations;

namespace Thatnetwork.Dtos
{
    public class LoginByEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
