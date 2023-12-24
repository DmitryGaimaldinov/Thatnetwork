namespace Thatnetwork.Dtos
{
    public class NoteDto
    {
        public int Id { get; set; }
        public required UserDto creator;
        public required string Text { get; set; }
        public bool ISDTO = true;
    }
}
