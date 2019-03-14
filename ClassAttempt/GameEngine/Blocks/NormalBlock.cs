using GameUtils;
using System;
using System.Drawing;

namespace GameEngine.Blocks
{
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
        }

        public override void Draw(Graphics e, Ball b)
        {
            e.DrawImage(mainTex, new RectangleF(currentX, currentY, Constants.BlockWidth, Constants.BlockWidth));
            if (isTouched && b.IsStateAlso(BallState.Bonused))
                e.FillRectangle(new SolidBrush(bonusColor), currentX, currentY, Constants.BlockWidth, Constants.BlockWidth);
            e.DrawRectangle(colBorder(Color.Black), currentX, currentY, Constants.BlockWidth, Constants.BlockWidth);
        }

        public override void Update(long thisTime, Ball b)
        {
            //Non faccio nulla
        }
    }
}
