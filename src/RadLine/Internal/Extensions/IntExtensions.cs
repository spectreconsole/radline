namespace RadLine
{
    internal static class IntExtensions
    {
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        public static int WrapAround(this int value, int min, int max)
        {
            if (value < min)
            {
                return max;
            }

            if (value > max)
            {
                return min;
            }

            return value;
        }
    }
}
