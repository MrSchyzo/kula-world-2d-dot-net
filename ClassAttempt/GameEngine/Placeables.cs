using GameUtils;
using LevelsStructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    #region G_Valuable, superclasse per tutti gli oggetti che aumentano solo il punteggio
    public abstract class G_Valuable : GamePlaceable
    {
        protected int value = 250;
        protected string sound = "";

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ModifyScore(value);
                b.PlaySound(sound);
                isEnabled = false;
            }
        }
    }
    #endregion

    #region G_Key, classe per rappresentare le chiavi
    public class G_Key : GamePlaceable
    {
        public G_Key(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.IncreaseKey();
                b.ModifyScore(1500);
                isEnabled = false;
            }
        }
    }
    #endregion

    #region G_Fruit, classe per rappresentare i frutti
    public class G_Fruit : GamePlaceable
    {
        public G_Fruit(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.IncreaseFruit();
                b.ModifyScore(2000);
                isEnabled = false;
            }
        }
    }
    #endregion

    #region G_Glasshour, classe per rappresentare le clessidre
    public class G_Glasshour : GamePlaceable
    {
        public G_Glasshour(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ModifyScore(1000);
                b.InvertTime(thisTime);
                isEnabled = false;
            }
        }
    }
    #endregion

    #region G_Glasses, classe per rappresentare le clessidre
    public class G_Glasses : GamePlaceable
    {
        public G_Glasses(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ModifyScore(500);
                b.SetState(BallState.ViewMore, true);
                b.PlaySound("Gem");
                isEnabled = false;
            }
        }
    }
    #endregion

    #region G_GravityChanger, classe per rappresentare i mutatori di gravità
    public class G_GravChanger : GamePlaceable
    {
        private float rot;

        public G_GravChanger(int idX, int idY, float persp, Bitmap img, KulaLevel.Orientation o)
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
    #endregion

    #region G_SlowPill, classe per rappresentare le pillole di "rallentamento"
    public class G_SlowPill : GamePlaceable
    {
        public G_SlowPill(int idX, int idY, float persp, Bitmap img)
        {
            initPlaceable(idX, idY, persp, img);
        }

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ModifyScore(-2500);
                b.RemoveTime(10, thisTime);
                b.PlaySound("SlowPill");
                isEnabled = false;
            }
        }
    }
    #endregion

    #region Classi di tutti i "valuables"
    public class G_Bronze : G_Valuable
    {
        public G_Bronze(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 250;
            sound = "Coin";
        }
    }
    public class G_Silver : G_Valuable
    {
        public G_Silver(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 500;
            sound = "Coin";
        }
    }
    public class G_Gold : G_Valuable
    {
        public G_Gold(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 750;
            sound = "Coin";
        }
    }
    public class G_Sapphire : G_Valuable
    {
        public G_Sapphire(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 1250;
            sound = "Gem";
        }
    }
    public class G_Ruby : G_Valuable
    {
        public G_Ruby(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 1750;
            sound = "Gem";
        }
    }
    public class G_Emerald : G_Valuable
    {
        public G_Emerald(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 2250;
            sound = "Gem";
        }
    }
    public class G_Diamond : G_Valuable
    {
        public G_Diamond(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 3000;
            sound = "Gem";
        }
    }
    #endregion
}
