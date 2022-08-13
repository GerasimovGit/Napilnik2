namespace NapilnikTasks
{
    internal class Program
    {
        public static int GetFirstLargestNumber(int a, int b, int c)
        {
            if (a < b)
                return b;

            if (a > c)
                return c;

            return a;
        }
    }
}