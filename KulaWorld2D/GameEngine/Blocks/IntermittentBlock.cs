using GameEngine.Animators;
using GameEngine.Enumerations;
using GameUtils;
using System;
using System.Drawing;

namespace GameEngine.Blocks
{
    public class IntermittentBlock : IndestructibleBlock
    {
        private float periodo;
        private float beginTime;
        private Animator alphaAnimator;
        private int alpha;

        public IntermittentBlock(Bitmap texture, int idX, int idY, float beginTime, float period)
        {
            blockPositioning(idX, idY);
            periodo = period;
            mustSound = true;
            this.beginTime = beginTime;
            isTouched = false;
            if (texture == null)
                throw new ArgumentNullException("Nobody wants a void block. Load a good texture!!");
            mainTex = GameApp.ResizeImg(texture, 48, 48);
            if (mainTex == null)
                throw new Exception("Error during the block's texture loading.");
            alphaAnimator = new AlternateAnimator(0.0, 0, (long)beginTime, 180.0, 0.0, 500, (long)period);
            alpha = 180;
        }

        public override void Update(long thisTime, Ball b)
        {
            alpha = Math.Max(Math.Min((int)Math.Round(alphaAnimator.CalculateValue(thisTime), 0), 180), 0);
            if (alpha == 0)
            {
                isEnabled = false;
                layingBall.StartFalling(thisTime);
            }
            else
                isEnabled = true;
        }

        public override void Reset(long thisTime)
        {
            base.Reset(thisTime);
            alphaAnimator = new AlternateAnimator(0.0, thisTime, (long)beginTime, 180.0, 0.0, 500, (long)periodo);
            alpha = 180;
        }

        public override void Draw(Graphics e, Ball b)
        {
            if (isEnabled)
            {
                Color c = Color.FromArgb(180 - alpha, Color.White);
                Rectangle block = new Rectangle((int)currentX, (int)currentY, (int)Constants.BlockWidth, (int)Constants.BlockWidth);
                GameApp.DrawAlphaImage(e, alpha, mainTex, block);
                e.FillRectangle(new SolidBrush(c), currentX, currentY, Constants.BlockWidth, Constants.BlockWidth);
                if (isTouched && b.IsStateAlso(BallState.Bonused))
                    e.FillRectangle(new SolidBrush(bonusColor), currentX, currentY, Constants.BlockWidth, Constants.BlockWidth);
                e.DrawRectangle(colBorder(Color.Gray), currentX, currentY, Constants.BlockWidth, Constants.BlockWidth);
            }
        }
    }
}
