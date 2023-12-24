using Thatnetwork.Hashtags;

namespace Thatnetwork.Clubs
{
    public class ClubHashtag : Hashtag
    {
        public int ClubId {  get; set; }
        public required Club Club {  get; set; }
    }
}
