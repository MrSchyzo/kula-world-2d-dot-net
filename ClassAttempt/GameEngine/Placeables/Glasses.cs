using GameEngine.Enumerations;
using System.Drawing;

namespace GameEngine.Placeables
{
    public class Glasses : Placeable
    {
        public Glasses(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ModifyScore(500);
                b.SetState(BallState.ViewMore, true);
                b.PlaySound("Gem");
                isEnabled = false;
            }
        }
    }
}
