using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameUtils
{
    
    

    
    public static class MatrixUtils
    {
        
        private static PointF[] pointf(PointF p)
        {
            return new PointF[] { new PointF(p.X, p.Y) };
        }

        private static Point[] point(Point p)
        {
            return new Point[] { new Point(p.X, p.Y) };
        }
        private static PointF[] pointf(Point p)
        {
            return new PointF[] { new PointF(p.X, p.Y) };
        }

        private static Point[] point(PointF p)
        {
            return new Point[] { new Point((int)p.X, (int)p.Y) };
        }
        

        
        public static PointF TransformPointF(Matrix m, PointF p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");
            PointF[] pt = pointf(p);
            m.TransformPoints(pt);
            return pt[0];
        }
        public static PointF TransformPointF(Matrix m, Point p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");
            PointF[] pt = pointf(p);
            m.TransformPoints(pt);
            return pt[0];
        }
        public static Point TransformPoint(Matrix m, PointF p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");
            Point[] pt = point(p);
            m.TransformPoints(pt);
            return pt[0];
        }
        public static Point TransformPoint(Matrix m, Point p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");
            Point[] pt = point(p);
            m.TransformPoints(pt);
            return pt[0];
        }
        

        public static PointF RoundPoint(PointF a)
        {
            return new PointF((float)Math.Round(a.X), (float)Math.Round(a.Y));
        }

        public static float ScalarProduct(PointF a, PointF b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        
    }
    
}
