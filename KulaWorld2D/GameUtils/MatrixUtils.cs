using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameUtils
{
    public static class MatrixUtils
    {

        public static PointF TransformAndThenRound(this Matrix m, PointF p)
            => m.TransformPointF(p).Round();

        public static PointF TransformPointF(this Matrix m, PointF p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");

            PointF[] pt = new PointF[] { new PointF(p.X, p.Y) };
            m.TransformPoints(pt);
            return pt[0];
        }

        public static PointF Round(this PointF a)
            => new PointF((float)Math.Round(a.X), (float)Math.Round(a.Y));

        public static float ScalarProduct(PointF a, PointF b)
            => a.X * b.X + a.Y * b.Y;
        
    }
    
}
