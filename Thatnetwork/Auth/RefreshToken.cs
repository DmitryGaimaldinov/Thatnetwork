using Thatnetwork.Users;

namespace Thatnetwork.Auth
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}
