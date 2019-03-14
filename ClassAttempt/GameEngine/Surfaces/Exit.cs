using LevelsStructure;
using System.Drawing;

namespace GameEngine.Surfaces
{
    public class Exit : Surface
    {
        private static Color noExit = Color.FromArgb(200, Color.Red);
        private static Color yesExit = Color.FromArgb(200, Color.LimeGreen);

        public Exit(int idxX, int idxY, KulaLevel.Orientation o)
        {
            surfacesPhysicInit(Constants.BlockWidth, 8, -RotationUtilities.getAngleFromDownOrientation(o));
            PointF newCenter = suggestedCenter(idxX, idxY, o);
            startingX = newCenter.X;
            startingY = newCenter.Y;
            currentX = startingX;
            currentY = startingY;
            isLandingNeeded = true;
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchTheBall(b) && b.IsStateAlso(BallState.Exiting))
                b.Exit();
        }

        public override void Draw(Graphics e, Ball b)
        {
            RectangleF r = bounds();
            if (b.IsStateAlso(BallState.Exiting))
            {
                //TOCHECK: disegno la superficie attiva
                e.FillRectangle(new SolidBrush(yesExit), bounds());
            }
            else
            {
                //TOCHECK: disegno la superficie inattiva
                e.FillRectangle(new SolidBrush(noExit), bounds());
            }
        }
    }
}
