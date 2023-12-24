using System.ComponentModel.DataAnnotations;
using Thatnetwork.Chats.Validators;
using Thatnetwork.Photos.Validation;

namespace Thatnetwork.Chats.Dtos
{
    public class EditMessageDto
    {
        [AllowedToEditMessage]
        public required int MessageId { get; set; }

        [MaxLength(3000)]
        public string Text { get; set; } = string.Empty;

        [AllowedPhotoIds]
        public List<int> PhotoIds { get; set; } = new();

        public bool IsEmpty { get => Text.Trim() == string.Empty && !PhotoIds.Any(); }
    }
}
