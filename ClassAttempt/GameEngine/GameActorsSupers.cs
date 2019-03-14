using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public enum BlockReact
    {
        Land,
        BounceLeft,
        BounceRight,
        MoveTo,
        BounceDown,
        None,
        Die
    }

    #region Superclasse Actor: da cui discendono i blocchi, i piazzabili, le superfici e i nemici
    public abstract class Actor
    {
        #region Variabili di istanza iniziali
        protected float startingX, startingY;
        #endregion
        #region Variabili di istanza di gioco
        protected bool isEnabled = true;
        protected float currentX, currentY;
        protected float perspective = 0;
        #endregion

        protected void setEnabled(bool val)
        {
            isEnabled = val;
        }

        public abstract void Update(long thisTime, Ball b);

        public virtual void Draw(Graphics e, Ball b)
        {
            Console.WriteLine("I'm drawn @(x:" + currentX + ", y:" +currentY + ")");
        }

        public abstract void CollidesWithBall(long thisTime, Ball b);

        public abstract void Reset(long thisTime);

        public bool IsEnabled
        {
            get { return isEnabled; }
        }
    }

#endregion
#region Superclasse Placeable, ogni piazzabile discende da tale classe

#endregion
    
    public abstract class GameEnemy : Actor
    {
        #region Variabili per il "movimento dei nemici"
        protected float rotation;
        #endregion
    }
}
