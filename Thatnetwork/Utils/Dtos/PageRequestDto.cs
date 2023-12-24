namespace Thatnetwork.Utils.Dtos
{
    public abstract class PageRequestDto<Cursor>
    {
        public Cursor? start;
        public abstract int Count { get; set; }
    }
}
