using LevelsStructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public abstract class GameContactSurface : GameSurface
    {
        protected SurfType type;
        protected Color surfColor;

        private BallState surface2state(SurfType type)
        {
            if (type == SurfType.Fire)
                return BallState.Burning;
            else if (type == SurfType.Forced)
                return BallState.GroundForced;
            else if (type == SurfType.Ice)
                return BallState.Sliding;
            else
                throw new Exception("In GameContactSurface.surface2state(...) the input type is unexpected.");
        }

        protected void affectBall(Ball b, long thisTime)
        {
            b.StartBallModify(type, thisTime);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            isLandingNeeded = true;
            if (doITouchTheBall(b))
            {
                if (!b.IsFoundSurface(type))
                {
                    if (!b.IsStateAlso(surface2state(type)))
                        affectBall(b, thisTime);
                }
                b.SetFoundSurface(type, true);
            }
        }

        public override void Draw(Graphics e, Ball b)
        {
            RectangleF r = bounds();
            e.FillRectangle(new SolidBrush(surfColor), bounds());
        }
    }
    
    public class G_Exit : GameSurface
    {
        private static Color noExit = Color.FromArgb(200, Color.Red);
        private static Color yesExit = Color.FromArgb(200, Color.LimeGreen);

        public G_Exit(int idxX, int idxY, KulaLevel.Orientation o)
        {
            surfacesPhysicInit(EngineConst.BlockWidth, 8, -RotationUtilities.getAngleFromDownOrientation(o));
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

    public class G_Spikes : GameSurface
    {
        public G_Spikes(int idxX, int idxY, KulaLevel.Orientation o, Bitmap img)
        {
            surfacesPhysicInit(EngineConst.BlockWidth * 0.875f, 24, -RotationUtilities.getAngleFromDownOrientation(o));
            surfacesDrawInit(img, (int)(EngineConst.BlockWidth * 0.875f), 24);
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
                b.Die(DeathType.Spiked, thisTime);
        }
    }

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

    public class G_Ice : GameContactSurface
    {
        public G_Ice(int idxX, int idxY, KulaLevel.Orientation o)
        {
            surfacesPhysicInit(EngineConst.BlockWidth, 8, -RotationUtilities.getAngleFromDownOrientation(o));
            surfColor = Color.FromArgb(200, Color.Azure);
            type = SurfType.Ice;
            PointF newCenter = suggestedCenter(idxX, idxY, o);
            startingX = newCenter.X;
            startingY = newCenter.Y;
            currentX = startingX;
            currentY = startingY;
            isLandingNeeded = true;
        }
    }

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

    public class G_GroundForced : GameContactSurface
    {
        public G_GroundForced(int idxX, int idxY, KulaLevel.Orientation o)
        {
            surfacesPhysicInit(EngineConst.BlockWidth, 8, -RotationUtilities.getAngleFromDownOrientation(o));
            surfColor = Color.FromArgb(200, Color.Violet);
            type = SurfType.Forced;
            PointF newCenter = suggestedCenter(idxX, idxY, o);
            startingX = newCenter.X;
            startingY = newCenter.Y;
            currentX = startingX;
            currentY = startingY;
            isLandingNeeded = true;
        }
    }
}
