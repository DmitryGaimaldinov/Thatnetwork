using Thatnetwork.Auth;
using Thatnetwork.Challenges;
using Thatnetwork.Chats;
using Thatnetwork.Clubs;
using Thatnetwork.Notes;
using Thatnetwork.Photos;

namespace Thatnetwork.Users
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public string? Tag { get; set; }
        public string Description { get; set; } = "";
        public string? AvatarPath {  get; set; }
        public string? BackgroundPath { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<Club> CreatedClubs { get; set; } = new List<Club>();
        public List<Marathon> CreatedMaraphons { get; set; } = new();
        public List<Marathon> TakenMaraphones { get; set; } = new();
        public List<ChatRoom> ChatRooms { get; set; } = new();
        public List<ChatRoom> CreateChatRooms { get; set; } = new();
    }
}
