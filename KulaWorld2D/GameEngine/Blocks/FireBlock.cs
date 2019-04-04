using GameUtils;
using System;
using System.Drawing;

namespace GameEngine.Blocks
{
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
            e.DrawImage(fireTex, new RectangleF(currentX, currentY, Constants.BlockWidth, Constants.BlockWidth));
            e.DrawRectangle(colBorder(Color.DarkRed), currentX, currentY, Constants.BlockWidth, Constants.BlockWidth);
        }
    }
}
