using GameEngine.Enumerations;
using System.Collections.Generic;

namespace GameEngine.Blocks
{
    public abstract class IndestructibleBlock : Block
    {
        protected bool mustSound = true;

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            KeyValuePair<BlockReaction, float> hit = ballCollision(thisTime, b);
            if (hit.Key == BlockReaction.Land)
            {
                b.Land(hit.Value, thisTime);
                if (b.IsStateAlso(BallState.Bonused) && mustSound && !isTouched)
                {
                    b.PlaySound("BonusBlock");
                    b.ModifyScore(50);
                }
                    
                isTouched = true;
            }
            else if (hit.Key == BlockReaction.BounceDown)
                b.BounceDown(hit.Value, thisTime);
            else if (hit.Key == BlockReaction.BounceLeft || hit.Key == BlockReaction.BounceRight)
                b.BounceLateral(hit.Value, thisTime);
            else if (hit.Key == BlockReaction.Die)
                b.Die(DeathType.Captured, thisTime);
            else if (hit.Key == BlockReaction.MoveTo)
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
