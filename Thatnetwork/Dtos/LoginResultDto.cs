namespace Thatnetwork.Dtos
{
    public class LoginResultDto
    {
        public required string accessToken { get; set; }
        public required string refreshToken { get; set; }
    }
}
