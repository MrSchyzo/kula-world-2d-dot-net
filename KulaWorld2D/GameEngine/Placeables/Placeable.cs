using GameEngine.Animators;
using GameEngine.Utils;
using GameUtils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngine.Placeables
{
    public abstract class Placeable : Actor
    {
        protected Animator offsAnim = new SinusoidalAnimator(0, 0, 4000, 4);
        protected float offset = 0;
        protected const float dimX = 24;
        protected const float dimY = 24;
        protected Bitmap toDraw = new Bitmap(1, 1);

        protected void initPlaceable(int idxX, int idxY, float persp, Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException("The image used to draw the placeable is null!");

            PointF newCenter = suggestedCenter(idxX, idxY, -persp);
            startingX = newCenter.X;
            startingY = newCenter.Y;

            perspective = -persp;
            toDraw = GameApp.ResizeImg(image, 36, 36);

            currentX = startingX;
            currentY = startingY;

            isEnabled = true;
            offset = 0;

            float angle = (float)Math.Round(persp) % 360;
            if (angle < 0f)
                while ((angle += 360f) < 0f) ;

            if (angle % 360 == 0)
                toDraw.RotateFlip(RotateFlipType.RotateNoneFlipNone);
            else if (angle % 360 == 90)
                toDraw.RotateFlip(RotateFlipType.Rotate90FlipNone);
            else if (angle % 360 == 180)
                toDraw.RotateFlip(RotateFlipType.Rotate180FlipNone);
            else if (angle % 360 == 270)
                toDraw.RotateFlip(RotateFlipType.Rotate270FlipNone);
            else
                throw new Exception("What is " + angle + "deg?");
        }

        public override void Reset(long thisTime)
        {
            currentX = startingX;
            currentY = startingY;
            isEnabled = true;
            offset = 0;
            offsAnim = new SinusoidalAnimator(0, thisTime, 4000, 4);
        }

        public PointF Center
        {
            get
            {
                return new PointF(currentX, currentY);
            }
        }

        protected RectangleF getBounds()
        {
            PointF off = new PointF(0, offset);
            Matrix m = new Matrix();
            m.Rotate(-perspective);
            off = MatrixUtils.TransformPointF(m, off);
            return new RectangleF(Center.X + off.X - dimX / 2, Center.Y + off.Y - dimY / 2, dimX, dimY);
        }

        protected PointF suggestedCenter(int idxX, int idxY, float perspective)
        {
            float d = Constants.BlockWidth;
            PointF off = new PointF(0, (d - dimX)/2.0f - 4.0f);
            Matrix m = new Matrix();
            m.Rotate(-perspective);
            off = m.TransformAndThenRound(off);

            float cX = d / 2.0f + d*idxX;
            float cY = d / 2.0f + d*idxY;
            return new PointF((float)Math.Round(cX + off.X), (float)Math.Round(cY + off.Y));
        }

        protected bool doITouchBall(Ball b)
        {
            return CollisionUtilities.CircleIntersectsRectangle(30, b.Center, b.Radium, getBounds());
        }

        public override void Update(long thisTime, Ball b)
        {
            offset = (float)offsAnim.CalculateValue(thisTime);
        }

        public override void Draw(Graphics e, Ball b)
        {
            RectangleF r = getBounds();
            if (isEnabled)
                e.DrawImage(toDraw, r);
        }
    }
}
