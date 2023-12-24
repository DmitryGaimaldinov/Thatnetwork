using Thatnetwork.Challenges.Validators;

namespace Thatnetwork.Challenges.Dtos
{
    public class JoinMarathonDto
    {
        [ExistingMarathonId, AllowedToJoinMarathon]
        public required int MarathonId { get; set; }
    }
}
