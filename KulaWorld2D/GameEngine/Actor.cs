using System;
using System.Drawing;

namespace GameEngine
{
    public abstract class Actor
    {
        protected float startingX, startingY;

        protected bool isEnabled = true;
        protected float currentX, currentY;
        protected float perspective = 0;

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
}
