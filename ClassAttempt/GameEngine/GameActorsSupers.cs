using GameUtils;
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
    public abstract class GameBlock : Actor
    {
        #region Variabili di istanza di gioco
        protected Ball layingBall = new Ball();
        protected bool isFaceChangeable = true;
        protected bool isTouched;
        protected Ball NullBall = new Ball();
        #endregion
        #region Textures
        protected static Color bonusColor = Color.FromArgb(150, Color.Orange);
        protected static Bitmap mainTex;
        protected static Bitmap destructTex;
        protected static Bitmap fireTex;
        #endregion

        protected void blockPositioning(int idX, int idY)
        {
            float startX = idX * EngineConst.BlockWidth;
            float startY = idY * EngineConst.BlockWidth;
            startingX = startX;
            startingY = startY;
            currentX = startingX;
            currentY = startingY;
            isEnabled = true;
        }

        protected Pen colBorder(Color c)
        {
            return new Pen(c, 2);
        }

        public bool IsFaceChangeable { get { return isFaceChangeable; } }

        public bool IsTouched { get { return isTouched; } }

        public PointF Center
        {
            get
            {
                return new PointF(currentX + EngineConst.BlockWidth / 2, currentY + EngineConst.BlockWidth / 2);
            }
        }

        protected bool doITouchTheBall(Ball b)
        {
            RectangleF block = new RectangleF(currentX - 1f, currentY - 1f, EngineConst.BlockWidth + 2f, EngineConst.BlockWidth + 2f);
            return CollisionUtil.CircleIntersectsRectangle(30, b.Center, b.Radium, block);
        }

        /// <summary>
        /// Questo metodo controlla se ci sono collisioni tra la palla e il blocco corrente: restituisce una coppia di valori tra cui
        /// un enum che indica quale tipo di interazione c'è stata tra il blocco e la palla e l'altra serve per inizializzare il cambiamento
        /// di traiettoria della palla.
        /// </summary>
        /// <param name="thisTime">Tempo in cui cercare la collisione</param>
        /// <param name="b">Palla da testare per le collisioni</param>
        /// <returns></returns>
        protected KeyValuePair<BlockReact, float> ballCollision(long thisTime, Ball b)
        {
            BlockReact res = BlockReact.None;
            float offset = 0;
            if (doITouchTheBall(b) && isEnabled)
            {
                Matrix m = new Matrix();
                m.RotateAt(b.Rotation, Center);
                PointF rotBall = MatrixUtils.TransformPointF(m, b.Center);
                bool underX = rotBall.X < Center.X - EngineConst.BlockWidth / 2;
                bool underY = rotBall.Y < Center.Y - EngineConst.BlockWidth / 2;
                bool overX = rotBall.X > Center.X + EngineConst.BlockWidth / 2;
                bool overY = rotBall.Y > Center.Y + EngineConst.BlockWidth / 2;
                bool inX = !(underX || overX);
                bool inY = !(underY || overY);
                bool moreXthanY = Math.Abs(rotBall.X - Center.X) > Math.Abs(rotBall.Y - Center.Y);
                if (inX && underY)
                {
                    offset = -EngineConst.BlockWidth * 0.75f - (rotBall.Y - Center.Y);
                    if (b.IsStateAlso(BallState.Flying))
                        res = BlockReact.Land;
                    else
                        res = BlockReact.MoveTo;
                    layingBall = b;
                }
                else if (overY && !moreXthanY)
                {
                    if (b.IsStateAlso(BallState.Flying))
                        res = BlockReact.BounceDown;
                    offset = EngineConst.BlockWidth * 0.75f - (rotBall.Y - Center.Y) + 1;
                }
                else if (underX)
                {
                    if (b.IsStateAlso(BallState.Flying))
                        res = BlockReact.BounceLeft;
                    offset = -EngineConst.BlockWidth * 0.75f - (rotBall.X - Center.X) - 1;
                }
                else if (overX)
                {
                    if (b.IsStateAlso(BallState.Flying))
                        res = BlockReact.BounceRight;
                    offset = EngineConst.BlockWidth * 0.75f - (rotBall.X - Center.X) + 1;
                }
                else if (inX && inY)
                {
                    res = BlockReact.Die;
                }
                Console.Write("");//Line("There was an interaction: " + res.ToString() + ", with offset of " + offset.ToString() + " units.");
            }
            else
                layingBall = NullBall;
            
            
            return new KeyValuePair<BlockReact, float>(res, offset);
        }

        public override void Reset(long thisTime)
        {
            currentX = startingX;
            currentY = startingY;
            isEnabled = true;
            isTouched = false;
        }
    }
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

    #region GameSurface: superclasse comune a tutte le superfici del gioco
    public abstract class GameSurface : Actor
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
            
            float d = EngineConst.BlockWidth;
            PointF off = new PointF(0, -d / 2.0f);

            Matrix m = new Matrix();
            float rot = -RotationUtilities.getAngleFromDownOrientation(o);
            m.Rotate(rot);
            off = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, off));

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
            bool touch = CollisionUtil.CircleIntersectsRectangle(360, b.Center, b.Radium, bounds());
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
    #endregion
    
    public abstract class GameEnemy : Actor
    {
        #region Variabili per il "movimento dei nemici"
        protected float rotation;
        #endregion
    }
}
