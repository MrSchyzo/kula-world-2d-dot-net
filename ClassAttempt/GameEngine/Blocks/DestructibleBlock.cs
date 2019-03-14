using GameUtils;
using System;
using System.Drawing;
using System.Collections.Generic;
using GameEngine.Animators;
using GameEngine.Enumerations;

namespace GameEngine.Blocks
{
    public class DestructibleBlock : Block
    {
        private float scale;
        private bool isDestroying = false;
        private Animator scaleAnim;
        
        public DestructibleBlock(Bitmap tex, Bitmap overlay, int idX, int idY)
        {
            isFaceChangeable = false;
            isDestroying = false;
            scale = 1;
            scaleAnim = new SteadyAnimator(1, 0);
            blockPositioning(idX, idY);
            isTouched = false;
            if (tex == null || overlay == null)
                throw new ArgumentNullException("A destructible block MUST have a texture!");
            mainTex = GameApp.ResizeImg(tex, 48, 48);
            destructTex = GameApp.ResizeImg(overlay, 48, 48);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            KeyValuePair<BlockReaction, float> hit = ballCollision(thisTime, b);
            if (hit.Key == BlockReaction.Land)
            {
                b.Land(hit.Value, thisTime);
                if (b.IsStateAlso(BallState.Bonused) && !isTouched)
                {
                    b.PlaySound("BonusBlock");
                    b.ModifyScore(50);
                }
                isTouched = true;
                if (!isDestroying)
                {
                    b.PlaySound("DestructibleBlock");
                    isDestroying = true;
                    //scaleAnim = new LinearBoundedAnimator(scale, thisTime, -0.001f / Constants.DestructionTime, 1, 0);
                    scaleAnim = new ParabolicUnboundedAnimator(scale, thisTime, 1f / 750000f, -7f / 3000f);
                }
                
            }
            else if (hit.Key == BlockReaction.BounceDown)
                b.BounceDown(hit.Value, thisTime);
            else if (hit.Key == BlockReaction.BounceLeft || hit.Key == BlockReaction.BounceRight)
                b.BounceLateral(hit.Value, thisTime);
            else if (hit.Key == BlockReaction.Die)
                b.Die(DeathType.Captured, thisTime);
            else if (hit.Key == BlockReaction.MoveTo)
            {
                if (b.IsStateAlso(BallState.Bonused) && !isTouched)
                {
                    b.PlaySound("BonusBlock");
                    b.ModifyScore(50);
                }
                isTouched = true;
                if (!isDestroying)
                {
                    b.PlaySound("DestructibleBlock");
                    isDestroying = true;
                    //scaleAnim = new LinearBoundedAnimator(scale, thisTime, -0.001f / Constants.DestructionTime, 1, 0);
                    scaleAnim = new ParabolicUnboundedAnimator(scale, thisTime, 1f / 750000f, -7f / 3000f);
                }
            }
        }

        public override void Update(long thisTime, Ball b)
        {
            scale = Math.Min((float)scaleAnim.CalculateValue(thisTime), 1f);
            if (scale < 0f)
            {
                isEnabled = false;
                layingBall.StartFalling(thisTime);
            }  
        }

        public override void Reset(long thisTime)
        {
            base.Reset(thisTime);
            isDestroying = false;
            isFaceChangeable = false;
            isDestroying = false;
            scale = 1;
            scaleAnim = new SteadyAnimator(1, thisTime);
        }

        public override void Draw(Graphics e, Ball b)
        {
            if (isEnabled && scale > 0)
            {
                PointF ul = new PointF(scale*(currentX - Center.X) + Center.X, scale*(currentY - Center.Y) + Center.Y);
                SizeF sz = new SizeF(scale*Constants.BlockWidth, scale*Constants.BlockWidth);
                RectangleF block = new RectangleF(ul, sz);
                e.DrawImage(mainTex, block);
                e.DrawImage(destructTex, block);
                if (isTouched && b.IsStateAlso(BallState.Bonused))
                    e.FillRectangle(new SolidBrush(bonusColor), block);
                e.DrawRectangle(colBorder(Color.Black), Rectangle.Round(block));
            }
            
        }
    }
}
