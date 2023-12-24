using System.ComponentModel.DataAnnotations;

namespace Thatnetwork.Notes.Dtos
{
    public class AddNoteDto
    {
        public required string Text { get; set; }

        [MaxLength(4)]
        public List<int> PhotoIds { get; set; } = new();

        public int? ParentNoteId { get; set; }

        public bool IsEmpty { get => Text.Trim().Length == 0 && PhotoIds.Count == 0; }
    }
}
