using System.Collections.Generic;

namespace GameEngine.Blocks
{
    public abstract class IndestructibleBlock : Block
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
}
