using System;

namespace NapilnikTasks
{
    internal class Program
    {
        public static void Main(string[] args)
        {
        }
        
        public static int IndexOf(int[] array, int element)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == element)
                    return i;

            return -1;
        }
    }
}