namespace GameEngine.Animators
{
    /// <summary>
    /// Classe che rappresenta un animatore singolo, senza problemi di rotazione annessi
    /// </summary>
    public abstract class Animator
    {
        protected double beginningValue;
        protected long lastTimeStamp;

        protected void S_Animator_MutualPart(double val, long timeStamp)
        {
            beginningValue = val;
            lastTimeStamp = timeStamp;
        }

        public Animator()
        {
        }

        public Animator(double val, long timeStamp)
        {
            S_Animator_MutualPart(val, timeStamp);
        }

        public abstract double GetCurrentSpeed(long thisTime);

        public abstract double CalculateValue(long thisTime);

        public long GetStartingTime()
        {
            return lastTimeStamp;
        }

        public double GetBeginningValue()
        {
            return beginningValue;
        }
        
        public void ResynchronizeAnimation(long thisTime)
        {
            beginningValue = CalculateValue(thisTime);
            lastTimeStamp = thisTime;
        }

        public void ResynchronizeValue(long thisTime, double val)
        {
            beginningValue = val;
            lastTimeStamp = thisTime;
        }
    }
}
