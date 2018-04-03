using GameUtils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public abstract class IndestructibleBlock : GameBlock
    {
        protected bool mustSound = true;

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            KeyValuePair<BlockReact, float> hit = ballCollision(thisTime, b);
            if (hit.Key == BlockReact.Land)
            {
                b.Land(hit.Value, thisTime);
                if (b.IsStateAlso(BallState.Bonused) && mustSound && !isTouched)
                {
                    b.PlaySound("BonusBlock");
                    b.ModifyScore(50);
                }
                    
                isTouched = true;
            }
            else if (hit.Key == BlockReact.BounceDown)
                b.BounceDown(hit.Value, thisTime);
            else if (hit.Key == BlockReact.BounceLeft || hit.Key == BlockReact.BounceRight)
                b.BounceLateral(hit.Value, thisTime);
            else if (hit.Key == BlockReact.Die)
                b.Die(DeathType.Captured, thisTime);
            else if (hit.Key == BlockReact.MoveTo)
            {
                if (b.IsStateAlso(BallState.Bonused) && mustSound && !isTouched)
                {
                    b.PlaySound("BonusBlock");
                    b.ModifyScore(50);
                }
                    
                isTouched = true;
            }
                
        }
    }

    public class NormalBlock : IndestructibleBlock
    {
        public NormalBlock()
        {

        }
        
        public NormalBlock(Bitmap texture, int idX, int idY)
        {
            blockPositioning(idX, idY);
            isTouched = false;
            mustSound = true;
            if (texture == null)
                throw new ArgumentNullException("Nobody wants a void block. Load a good texture!!");
            mainTex = GameApp.ResizeImg(texture, 48, 48);
            if (mainTex == null)
                throw new Exception("Error during the block's texture loading.");
            //overlayTex = new Bitmap(1, 1);
            //overlayTex.SetPixel(0, 0, Color.FromArgb(0, 0, 0, 0));
        }

        public override void Draw(Graphics e, Ball b)
        {
            e.DrawImage(mainTex, new RectangleF(currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth));
            if (isTouched && b.IsStateAlso(BallState.Bonused))
                e.FillRectangle(new SolidBrush(bonusColor), currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth);
            e.DrawRectangle(colBorder(Color.Black), currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth);
        }

        public override void Update(long thisTime, Ball b)
        {
            //Non faccio nulla
        }
    }

    public class TransparentBlock : IndestructibleBlock
    {
        
        public TransparentBlock(int idX, int idY)
        {
            blockPositioning(idX, idY);
            mustSound = false;
            isTouched = true; //I blocchi trasparenti, nei bonus, vengono contati come toccati
        }

        public override void Draw(Graphics e, Ball b)
        {
            int alpha;
            float dist = (float)EngUtils.Distance(Center, b.Center);
            if (dist > 192)
                alpha = 0;
            else
                alpha = (int)((1.0f - (dist / 192.0f)) * 120.0f);
            if (b.IsStateAlso(BallState.ViewMore))
                alpha = 125;
            using (Pen p = colBorder(Color.FromArgb(alpha, 255, 255, 255)))
            {
                float bw = EngineConst.BlockWidth;
                e.FillRectangle(new SolidBrush(Color.FromArgb(alpha, 200, 200, 200)), currentX, currentY, bw, bw);
                e.DrawRectangle(p, currentX, currentY, bw, bw);
            }
        }

        public override void Update(long thisTime, Ball b)
        {
            //Non faccio nulla
        }

        public override void Reset(long thisTime)
        {
            base.Reset(thisTime);
            isTouched = true;
        }
    }

    public class IceBlock : NormalBlock
    {
        public IceBlock(Bitmap texture, int idX, int idY) : base(texture, idX, idY)
        {
        }

        public override void Draw(Graphics e, Ball b)
        {
            base.Draw(e, b);
            e.FillRectangle(new SolidBrush(Color.FromArgb(200, 100, 200, 255)), currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth);
            e.DrawRectangle(colBorder(Color.Azure), currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth);
        }
    }

    public class FireBlock : NormalBlock
    {
        public FireBlock(Bitmap tex, Bitmap overlay, int idX, int idY) : base(tex, idX, idY)
        {
            if (overlay == null)
                throw new ArgumentNullException("Initializing a fireblock without a burning overlay");
            fireTex = GameApp.ResizeImg(overlay, 48, 48);
        }
        public override void Draw(Graphics e, Ball b)
        {
            base.Draw(e, b);
            e.DrawImage(fireTex, new RectangleF(currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth));
            e.DrawRectangle(colBorder(Color.DarkRed), currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth);
        }
    }

    public class IntermittentBlock : IndestructibleBlock
    {
        private float periodo;
        private float beginTime;
        private S_Animator alphaAnimator;
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
                Rectangle block = new Rectangle((int)currentX, (int)currentY, (int)EngineConst.BlockWidth, (int)EngineConst.BlockWidth);
                GameApp.DrawAlphaImage(e, alpha, mainTex, block);
                e.FillRectangle(new SolidBrush(c), currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth);
                if (isTouched && b.IsStateAlso(BallState.Bonused))
                    e.FillRectangle(new SolidBrush(bonusColor), currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth);
                e.DrawRectangle(colBorder(Color.Gray), currentX, currentY, EngineConst.BlockWidth, EngineConst.BlockWidth);
            }
        }
    }

    public class DestructibleBlock : GameBlock
    {
        private float scale;
        private bool isDestroying = false;
        private S_Animator scaleAnim;
        
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
            KeyValuePair<BlockReact, float> hit = ballCollision(thisTime, b);
            if (hit.Key == BlockReact.Land)
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
                    //scaleAnim = new LinearBoundedAnimator(scale, thisTime, -0.001f / EngineConst.DestructionTime, 1, 0);
                    scaleAnim = new ParabolicUnboundedAnimator(scale, thisTime, 1f / 750000f, -7f / 3000f);
                }
                
            }
            else if (hit.Key == BlockReact.BounceDown)
                b.BounceDown(hit.Value, thisTime);
            else if (hit.Key == BlockReact.BounceLeft || hit.Key == BlockReact.BounceRight)
                b.BounceLateral(hit.Value, thisTime);
            else if (hit.Key == BlockReact.Die)
                b.Die(DeathType.Captured, thisTime);
            else if (hit.Key == BlockReact.MoveTo)
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
                    //scaleAnim = new LinearBoundedAnimator(scale, thisTime, -0.001f / EngineConst.DestructionTime, 1, 0);
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
                SizeF sz = new SizeF(scale*EngineConst.BlockWidth, scale*EngineConst.BlockWidth);
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
