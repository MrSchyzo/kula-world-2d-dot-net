using System.Drawing;

namespace GameEngine.Placeables
{
    public class Glasshour : Placeable
    {
        public Glasshour(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ModifyScore(1000);
                b.InvertTime(thisTime);
                isEnabled = false;
            }
        }
    }
}
