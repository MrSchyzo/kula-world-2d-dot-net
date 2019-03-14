using LevelsStructure;
using System.Drawing;

namespace GameEngine.Surfaces
{
    public class Ramp : Surface
    {
        public Ramp(int idxX, int idxY, KulaLevel.Orientation o, Bitmap img)
        {
            surfacesPhysicInit(Constants.BlockWidth * 0.5f, 32, -RotationUtilities.getAngleFromDownOrientation(o));
            surfacesDrawInit(img, (int)Constants.BlockWidth, 32);
            PointF newCenter = suggestedCenter(idxX, idxY, o);
            startingX = newCenter.X;
            startingY = newCenter.Y;
            currentX = startingX;
            currentY = startingY;
            isLandingNeeded = false;
            isEnabled = true;
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchTheBall(b))
                b.RampJump(thisTime);
        }
    }
}
