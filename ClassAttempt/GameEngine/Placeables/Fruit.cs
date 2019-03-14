using System.Drawing;

namespace GameEngine.Placeables
{
    public class Fruit : Placeable
    {
        public Fruit(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.IncreaseFruit();
                b.ModifyScore(2000);
                isEnabled = false;
            }
        }
    }
}
