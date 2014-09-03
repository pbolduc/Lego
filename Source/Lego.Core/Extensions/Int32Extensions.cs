namespace Lego.Extensions
{
    public static class Int32Extensions
    {
        public static bool IsPowerOfTwo(this int value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }
    }
}