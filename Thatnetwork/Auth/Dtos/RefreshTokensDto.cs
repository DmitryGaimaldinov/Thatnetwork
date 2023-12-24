using System.ComponentModel.DataAnnotations;

namespace Thatnetwork.Auth.Dtos
{
    public class RefreshTokensDto
    {
        [Required]
        public required string RefreshToken { get; set; }
    }
}
