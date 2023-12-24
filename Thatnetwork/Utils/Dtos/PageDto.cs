namespace Thatnetwork.Utils.Dtos
{
    public abstract class PageDto<Item, Cursor>
    {
        public required List<Item> Items { get; set; }
        public required Cursor? Next { get; set; }
    }
}
