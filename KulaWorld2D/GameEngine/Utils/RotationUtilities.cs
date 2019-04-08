using LevelsStructure;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngine.Utils
{
    /// <summary>
    /// Classe utilità per le rotazioni
    /// </summary>
    public class RotationUtilities
    {
        /// <summary>
        /// Restituisce la matrice di rotazione rispetto all'orientamento "down".
        /// </summary>
        /// <param name="o">Orientamento dato</param>
        /// <returns></returns>
        public static Matrix getRotationFromDownOrientation(KulaLevel.Orientation o)
        {
            Matrix m = new Matrix();
            switch(o)
            {
                
                case (KulaLevel.Orientation.Up):
                    {
                        m.Rotate(180.0f, MatrixOrder.Append);
                        break;
                    }
                
                
                case (KulaLevel.Orientation.Left):
                    {
                        m.Rotate(90.0f, MatrixOrder.Append);
                        break;
                    }
                
                
                case (KulaLevel.Orientation.Right):
                    {
                        m.Rotate(-90.0f, MatrixOrder.Append);
                        break;
                    }
                
                
                default:
                    {
                        break;
                    }
                
            }
            return m;
        }

        /// <summary>
        /// Restituisce un rettangolo contenente i 4 vertici trasformati di un dato rettangolo.
        /// </summary>
        /// <param name="input">Rettangolo in input</param>
        /// <param name="m">Matrice di trasformazione</param>
        /// <returns></returns>
        public static RectangleF TransformRectangle(RectangleF input, Matrix m)
        {
            RectangleF res = new RectangleF();
            if (input != null)
            {
                PointF[] coords = new PointF[4];
                coords[0] = new PointF(input.Location.X, input.Location.Y);
                coords[1] = new PointF(coords[0].X + input.Width, coords[0].Y);
                coords[2] = new PointF(coords[0].X + input.Width, coords[0].Y + input.Height);
                coords[3] = new PointF(coords[0].X, coords[0].Y + input.Height);
                m.TransformPoints(coords);
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                foreach(PointF p in coords)
                {
                    if (p.X < minX)
                        minX = p.X;
                    else if (p.X > maxX)
                        maxX = p.X;

                    if (p.Y < minY)
                        minY = p.Y;
                    else if (p.Y > maxY)
                        maxY = p.Y;
                }
                res = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }
            return res;
        }

        /// <summary>
        /// Restituisce l'angolo assoluto di rotazione rispetto all'orientamento "down".
        /// </summary>
        /// <param name="o">Orientamento dato</param>
        /// <returns></returns>
        public static float getAngleFromDownOrientation(KulaLevel.Orientation o)
        {
            if (o == KulaLevel.Orientation.Left)
                return -90f;
            else if (o == KulaLevel.Orientation.Right)
                return 90f;
            else if (o == KulaLevel.Orientation.Up)
                return 180f;
            else
                return 0f;
        }
    }
}
