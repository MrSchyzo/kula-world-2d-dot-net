using GameEngine.Utils;
using GameUtils;
using LevelsStructure;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngine.Surfaces
{
    public abstract class Surface : Actor
    {
        protected Bitmap toDraw;
        protected float dimX = 64;
        protected float dimY = 8;
        protected bool isLandingNeeded = true;

        protected void surfacesPhysicInit(float dimenX, float dimenY, float persp)
        {
            if (dimenX <= 0 || dimenY <= 0)
                throw new ArgumentException("The dimension of the surface mustn't be negative!");
            perspective = -persp;
            while (perspective < 0) perspective += 360f;
            dimX = dimenX;
            dimY = dimenY;
            isEnabled = true;
        }

        protected void surfacesDrawInit(Bitmap image, int _x, int _y)
        {
            if (image == null || _x <= 0 || _y <= 0)
                throw new ArgumentException("Creating a surface via surfacesDrawInit(...) without a valid image is unallowed!");
            toDraw = GameApp.ResizeImg(image, (int)Math.Max(_x, _y), (int)Math.Max(_x, _y));
            float persp = perspective;
            if (Math.Round(persp) % 360 == 0)
                toDraw.RotateFlip(RotateFlipType.RotateNoneFlipNone);
            else if (Math.Round(persp) % 360 == 270)
                toDraw.RotateFlip(RotateFlipType.Rotate90FlipNone);
            else if (Math.Round(persp) % 360 == 180)
                toDraw.RotateFlip(RotateFlipType.Rotate180FlipNone);
            else if (Math.Round(persp) % 360 == 90)
                toDraw.RotateFlip(RotateFlipType.Rotate270FlipNone);
            else
                toDraw.RotateFlip(RotateFlipType.RotateNoneFlipNone);
        }

        public PointF Center
        {
            get
            {
                return new PointF(currentX, currentY);
            }
        }

        protected PointF suggestedCenter(int idxX, int idxY, KulaLevel.Orientation o)
        {
            
            float d = Constants.BlockWidth;
            PointF off = new PointF(0, -d / 2.0f);

            Matrix m = new Matrix();
            float rot = -RotationUtilities.getAngleFromDownOrientation(o);
            m.Rotate(rot);
            off = m.TransformAndThenRound(off);

            float cX = d / 2.0f + d * idxX;
            float cY = d / 2.0f + d * idxY;

            off = new PointF((float)Math.Round(cX + off.X), (float)Math.Round(cY + off.Y));
            return off;
        }

        protected RectangleF bounds()
        {
            float ulX = Center.X - dimX / 2.0f;
            float ulY = Center.Y - dimY / 2.0f;
            Matrix m = new Matrix();
            RectangleF rec = new RectangleF(ulX, ulY, dimX, dimY);
            m.RotateAt(-perspective, GameApp.RectangleFCentre(rec));
            return RotationUtilities.TransformRectangle(rec, m);
        }

        protected bool doITouchTheBall(Ball b)
        {
            bool landImpliesFly = !isLandingNeeded || (Math.Round(b.Rotation).Equals(Math.Round(perspective)));
            bool touch = CollisionUtilities.CircleIntersectsRectangle(360, b.Center, b.Radium, bounds());
            return isEnabled && touch && landImpliesFly;
        }

        public override void Update(long thisTime, Ball b)
        {
            //TODO: c'è davvero da fare qualcosa per le superfici?
        }

        public override void Reset(long thisTime)
        {
            isEnabled = true;
            currentX = startingX;
            currentY = startingY;
        }

        public override void Draw(Graphics e, Ball b)
        {
            RectangleF r = bounds();
            if (isEnabled)
                e.DrawImage(toDraw, bounds());
        }
    }
}
