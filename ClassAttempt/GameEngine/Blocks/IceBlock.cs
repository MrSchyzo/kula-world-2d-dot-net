using System.Drawing;

namespace GameEngine.Blocks
{
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
}
