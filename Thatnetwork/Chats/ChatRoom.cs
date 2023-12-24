using Thatnetwork.Challenges;
using Thatnetwork.Users;

namespace Thatnetwork.Chats
{
    public class ChatRoom
    {
        public int Id {  get; set; }
        public string? Name { get; set; }
        //public string? AvatarPath {  get; set; }
        public List<Message> Messages { get; set; } = new();
        public required User Creator {  get; set; }
        public List<User> Participants { get; set; } = new();
        public int? MarathonId { get; set; }
        public Marathon? Marathon { get; set; }
    }
}
