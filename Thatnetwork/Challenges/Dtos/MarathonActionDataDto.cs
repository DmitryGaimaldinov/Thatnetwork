using Thatnetwork.Challenges.Validators;

namespace Thatnetwork.Challenges.Dtos
{
    public class MarathonActionDataDto
    {
        [ExistingMarathonId]
        public required int MarathonId { get; set; }
    }
}
