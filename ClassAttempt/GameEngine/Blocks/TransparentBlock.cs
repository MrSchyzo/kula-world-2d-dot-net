using System.Drawing;

namespace GameEngine.Blocks
{
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
}
