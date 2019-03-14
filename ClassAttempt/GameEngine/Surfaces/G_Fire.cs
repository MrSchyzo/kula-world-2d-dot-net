using LevelsStructure;
using System.Drawing;

namespace GameEngine.Surfaces
{
    public class G_Fire : GameContactSurface
    {
        public G_Fire(int idxX, int idxY, KulaLevel.Orientation o)
        {
            surfacesPhysicInit(EngineConst.BlockWidth * 0.875f, 6, -RotationUtilities.getAngleFromDownOrientation(o));
            surfColor = Color.FromArgb(200, Color.Firebrick);
            type = SurfType.Fire;
            PointF newCenter = suggestedCenter(idxX, idxY, o);
            startingX = newCenter.X;
            startingY = newCenter.Y;
            currentX = startingX;
            currentY = startingY;
            isLandingNeeded = true;
        }
    }
}
