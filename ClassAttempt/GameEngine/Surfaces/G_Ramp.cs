using LevelsStructure;
using System.Drawing;

namespace GameEngine.Surfaces
{
    public class G_Ramp : GameSurface
    {
        public G_Ramp(int idxX, int idxY, KulaLevel.Orientation o, Bitmap img)
        {
            surfacesPhysicInit(EngineConst.BlockWidth * 0.5f, 32, -RotationUtilities.getAngleFromDownOrientation(o));
            surfacesDrawInit(img, (int)EngineConst.BlockWidth, 32);
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
