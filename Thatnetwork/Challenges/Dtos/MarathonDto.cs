using Thatnetwork.Chats;
using Thatnetwork.Photos.Dtos;
using Thatnetwork.Users;
using Thatnetwork.Users.Dtos;

namespace Thatnetwork.Challenges.Dtos
{
    public class MarathonDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Tag { get; set; }
        public required UserDto Creator { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public bool isJoined { get; set; } = false;
        public required List<string> Hashtags { get; set; }
        public PhotoDto? Avatar { get; set; }
        public required int ChatRoomId { get; set; }

        public required int ParticipantsCount { get; set; }
        // TODO: ChatRoom ChatRoom
    }
}
