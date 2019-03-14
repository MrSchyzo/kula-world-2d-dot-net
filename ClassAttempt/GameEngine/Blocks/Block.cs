using GameEngine.Enumerations;
using GameUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngine.Blocks
{
    public abstract class Block : Actor
    {
        protected Ball layingBall = new Ball();
        protected bool isFaceChangeable = true;
        protected bool isTouched;
        protected Ball NullBall = new Ball();

        protected static Color bonusColor = Color.FromArgb(150, Color.Orange);
        protected static Bitmap mainTex;
        protected static Bitmap destructTex;
        protected static Bitmap fireTex;

        protected void blockPositioning(int idX, int idY)
        {
            float startX = idX * Constants.BlockWidth;
            float startY = idY * Constants.BlockWidth;
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
                return new PointF(currentX + Constants.BlockWidth / 2, currentY + Constants.BlockWidth / 2);
            }
        }

        protected bool doITouchTheBall(Ball b)
        {
            RectangleF block = new RectangleF(currentX - 1f, currentY - 1f, Constants.BlockWidth + 2f, Constants.BlockWidth + 2f);
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
        protected KeyValuePair<BlockReaction, float> ballCollision(long thisTime, Ball b)
        {
            BlockReaction res = BlockReaction.None;
            float offset = 0;
            if (doITouchTheBall(b) && isEnabled)
            {
                Matrix m = new Matrix();
                m.RotateAt(b.Rotation, Center);
                PointF rotBall = MatrixUtils.TransformPointF(m, b.Center);
                bool underX = rotBall.X < Center.X - Constants.BlockWidth / 2;
                bool underY = rotBall.Y < Center.Y - Constants.BlockWidth / 2;
                bool overX = rotBall.X > Center.X + Constants.BlockWidth / 2;
                bool overY = rotBall.Y > Center.Y + Constants.BlockWidth / 2;
                bool inX = !(underX || overX);
                bool inY = !(underY || overY);
                bool moreXthanY = Math.Abs(rotBall.X - Center.X) > Math.Abs(rotBall.Y - Center.Y);
                if (inX && underY)
                {
                    offset = -Constants.BlockWidth * 0.75f - (rotBall.Y - Center.Y);
                    if (b.IsStateAlso(BallState.Flying))
                        res = BlockReaction.Land;
                    else
                        res = BlockReaction.MoveTo;
                    layingBall = b;
                }
                else if (overY && !moreXthanY)
                {
                    if (b.IsStateAlso(BallState.Flying))
                        res = BlockReaction.BounceDown;
                    offset = Constants.BlockWidth * 0.75f - (rotBall.Y - Center.Y) + 1;
                }
                else if (underX)
                {
                    if (b.IsStateAlso(BallState.Flying))
                        res = BlockReaction.BounceLeft;
                    offset = -Constants.BlockWidth * 0.75f - (rotBall.X - Center.X) - 1;
                }
                else if (overX)
                {
                    if (b.IsStateAlso(BallState.Flying))
                        res = BlockReaction.BounceRight;
                    offset = Constants.BlockWidth * 0.75f - (rotBall.X - Center.X) + 1;
                }
                else if (inX && inY)
                {
                    res = BlockReaction.Die;
                }
                Console.Write("");//Line("There was an interaction: " + res.ToString() + ", with offset of " + offset.ToString() + " units.");
            }
            else
                layingBall = NullBall;
            
            
            return new KeyValuePair<BlockReaction, float>(res, offset);
        }

        public override void Reset(long thisTime)
        {
            currentX = startingX;
            currentY = startingY;
            isEnabled = true;
            isTouched = false;
        }
    }
}
