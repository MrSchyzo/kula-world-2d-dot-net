using GameUtils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public enum BlockReact
    {
        Land,
        BounceLeft,
        BounceRight,
        MoveTo,
        BounceDown,
        None,
        Die
    }

    #region Superclasse Actor: da cui discendono i blocchi, i piazzabili, le superfici e i nemici
    public abstract class Actor
    {
        #region Variabili di istanza iniziali
        protected float startingX, startingY;
        #endregion
        #region Variabili di istanza di gioco
        protected bool isEnabled = true;
        protected float currentX, currentY;
        protected float perspective = 0;
        #endregion

        protected void setEnabled(bool val)
        {
            isEnabled = val;
        }

        public abstract void Update(long thisTime, Ball b);

        public virtual void Draw(Graphics e, Ball b)
        {
            Console.WriteLine("I'm drawn @(x:" + currentX + ", y:" +currentY + ")");
        }

        public abstract void CollidesWithBall(long thisTime, Ball b);

        public abstract void Reset(long thisTime);

        public bool IsEnabled
        {
            get { return isEnabled; }
        }
    }

#endregion
#region Superclasse GameBlock: ogni blocco discende da tale classe
    #endregion

    #region Superclasse Placeable, ogni piazzabile discende da tale classe
    public abstract class GamePlaceable : Actor
    {
        protected S_Animator offsAnim = new SinusoidalAnimator(0, 0, 4000, 4);
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
            float d = EngineConst.BlockWidth;
            PointF off = new PointF(0, (d - dimX)/2.0f - 4.0f);
            Matrix m = new Matrix();
            m.Rotate(-perspective);
            off = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, off));
            float cX = d / 2.0f + d*idxX;
            float cY = d / 2.0f + d*idxY;
            return new PointF((float)Math.Round(cX + off.X), (float)Math.Round(cY + off.Y));
        }
        protected bool doITouchBall(Ball b)
        {
            return CollisionUtil.CircleIntersectsRectangle(30, b.Center, b.Radium, getBounds());
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

#endregion
    
    public abstract class GameEnemy : Actor
    {
        #region Variabili per il "movimento dei nemici"
        protected float rotation;
        #endregion
    }
}
