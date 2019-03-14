using System;
using System.Drawing;

namespace GameEngine.Utils
{
    public class MiscUtilities
    {
        public static bool IncludesAllTheOnesOf(int thisInt, int also)
        {
            return (thisInt | also) == thisInt;
        }

        public static double Distance(PointF a, PointF b)
        {
            float bX = b.X;
            float bY = b.Y;
            float aX = a.X;
            float aY = a.Y;
            return Math.Sqrt((aX - bX) * (aX - bX) + (aY - bY) * (aY - bY));
        }
    }
}
