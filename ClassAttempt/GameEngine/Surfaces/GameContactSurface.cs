using System;
using System.Drawing;

namespace GameEngine.Surfaces
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
}
