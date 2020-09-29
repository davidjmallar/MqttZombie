using System;

namespace System
{
    public static class RandomExtensions
    {
        public static double Next(this Random random, double minNumber, double maxNumber)
        {
            return random.NextDouble() * (maxNumber - minNumber) + minNumber;
        }
        public static string NextString(this Random random)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
