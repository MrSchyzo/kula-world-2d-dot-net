using System.Drawing;

namespace GameEngine.Placeables
{
    public class SlowPill : Placeable
    {
        public SlowPill(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ModifyScore(-2500);
                b.RemoveTime(10, thisTime);
                b.PlaySound("SlowPill");
                isEnabled = false;
            }
        }
    }
}
