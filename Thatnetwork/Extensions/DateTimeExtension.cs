namespace Thatnetwork.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime TrimSeconds(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, 0, dt.Kind);
        }
    }
}
