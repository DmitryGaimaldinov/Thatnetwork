using Thatnetwork.Chats;
using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Challenges
{
    public class Marathon
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required string Tag { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public Photo? Avatar {  get; set; }
        public int CreatorId { get; set; }
        public required User Creator { get; set; }
        public List<User> Participants { get; set; } = new();
        public List<ChallengeHashtag> Hashtags { get; set; } = new();
        public int ChatRoomId {  get; set; }
        public required ChatRoom ChatRoom { get; set; }
    }

    //public enum ChallengeType
    //{
    //    Challenge = 0,
    //    Maraphone = 1,
    //    KingWar = 2,
    //}
}
