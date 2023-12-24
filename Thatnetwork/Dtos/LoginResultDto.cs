using Thatnetwork.Users.Dtos;

namespace Thatnetwork.Dtos
{
    public class LoginResultDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required UserDto UserDto { get; set; }
    }
}
