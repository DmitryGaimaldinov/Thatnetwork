using System.ComponentModel.DataAnnotations;

namespace Thatnetwork.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; } = null!;
    }
}
