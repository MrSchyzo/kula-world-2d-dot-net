using System;
using System.Drawing;

namespace GameEngine.Utils
{
    /// <summary>
    /// Classe utilità per gestire le collisioni
    /// </summary>
    public class CollisionUtilities
    {
        /// <summary>
        /// Cerca di approssimare in maniera primitiva la collisione tra un cerchio ed un rettangolo: richiede che il rettangolo ed
        /// il cerchio siano nello stesso spazio di coordinate.
        /// </summary>
        /// <param name="dots">Numero di approssimazioni</param>
        /// <param name="circleCenter">Centro del cerchio</param>
        /// <param name="radium">Raggio del cerchio</param>
        /// <param name="rect">Rettangolo con cui fare il test delle collisioni</param>
        /// <returns></returns>
        public static bool CircleIntersectsRectangle(int dots, PointF circleCenter, float radium, RectangleF rect)
        {
            if (circleCenter != null && rect != null && dots > 0)
            {
                float cX = circleCenter.X;
                float cY = circleCenter.Y;
                float delta = 360f / ((float)dots);
                float angle = 0f;
                bool isHit = false;
                while (!isHit && angle < 360f)
                {
                    float dx = (float)Math.Sin(angle) * radium;
                    float dy = (float)Math.Cos(angle) * radium;
                    isHit = rect.Contains(cX + dx, cY + dy);
                    angle += delta;
                    if (isHit)
                        Console.Write("");//Line("Collision at: (" + (cX + dx) + ", " + (cY + dy) + ")");
                }
                return isHit;
            }
            else if (dots <= 0)
                throw new ArgumentException();
            else
                throw new NullReferenceException();
        }
    }
}