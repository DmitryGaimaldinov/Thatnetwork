using Thatnetwork.Hashtags;

namespace Thatnetwork.Challenges
{
    public class ChallengeHashtag : Hashtag
    {
        public int MarathonId { get; set; }
        public required Marathon Marathon { get; set; }
    }
}
