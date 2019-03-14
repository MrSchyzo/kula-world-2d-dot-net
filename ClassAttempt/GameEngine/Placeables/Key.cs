using System.Drawing;

namespace GameEngine.Placeables
{
    public class Key : Placeable
    {
        public Key(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.IncreaseKey();
                b.ModifyScore(1500);
                isEnabled = false;
            }
        }
    }
}
