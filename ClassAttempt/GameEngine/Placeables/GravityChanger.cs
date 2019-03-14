using GameEngine.Utils;
using LevelsStructure;
using System;
using System.Drawing;

namespace GameEngine.Placeables
{
    public class GravityChanger : Placeable
    {
        private float rot;

        public GravityChanger(int idX, int idY, float persp, Bitmap img, KulaLevel.Orientation o)
        {
            initPlaceable(idX, idY, persp, img);
            rot = RotationUtilities.getAngleFromDownOrientation(o);

            float angle = (float)Math.Round(rot - perspective) % 360;
            if (angle < 0f)
                while ((angle += 360f) < 0f) ;

            if (angle % 360 == 0)
                toDraw.RotateFlip(RotateFlipType.RotateNoneFlipNone);
            else if (angle % 360 == 270)
                toDraw.RotateFlip(RotateFlipType.Rotate90FlipNone);
            else if (angle % 360 == 180)
                toDraw.RotateFlip(RotateFlipType.Rotate180FlipNone);
            else if (angle % 360 == 90)
                toDraw.RotateFlip(RotateFlipType.Rotate270FlipNone);
            else
                throw new Exception("What is " + angle + "deg?");
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ChangeGravity(rot, thisTime);
                b.PlaySound("Gem");
                b.StartFalling(thisTime);
                b.ModifyScore(500);
                isEnabled = false;
            }
        }
    }
}
