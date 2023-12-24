using Thatnetwork.Users.Validation;

namespace Thatnetwork.Users.Dtos
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }

        private string _tag;
        [AllowedUserTag()]
        public string? Tag {
            get => _tag;
            set => _tag = value?.ToLower();
        }
        public string? Description { get; set; }

        public bool IsNotEmpty { get => Name != null; }
    }
}
