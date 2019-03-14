using System;

namespace GameEngine.Animators
{
    //Fa comodo per l'apparizione e la sparizione dei blocchi intermittenti
    public class AlternateAnimator : Animator
    {
        private long timeOffset;
        private double firstValue;
        private double secondValue;
        private long transitionPeriod;
        private long steadyPeriod;

        public AlternateAnimator(double startVal, long timeStamp, long offset, double firstVal, double secondVal, long transPeriod, long standPeriod)
        {
            if (transPeriod < 0 && standPeriod < 0)
                throw new ArgumentException("In AlternateAnimator(...) at least one of the two last args is incorrect.");
            beginningValue = startVal;
            lastTimeStamp = timeStamp;
            timeOffset = offset;
            firstValue = firstVal;
            secondValue = secondVal;
            transitionPeriod = transPeriod;
            steadyPeriod = standPeriod;
        }

        private double derivative(long time)
        {
            if (time <= transitionPeriod)
            {
                double m = (firstValue - secondValue) / ((double)transitionPeriod);
                return m;
            }
            else if (time > transitionPeriod && time <= transitionPeriod + steadyPeriod)
            {
                return 0;
            }
            else if (time > transitionPeriod + steadyPeriod && time <= 2 * transitionPeriod + steadyPeriod)
            {
                double m = (secondValue - firstValue) / ((double)transitionPeriod);
                return m;
            }
            else if (time < 2 * transitionPeriod + 2 * steadyPeriod)
            {
                return 0;
            }
            else
                throw new Exception("Calculating the alternator's value went wrong.");
        }

        private double calculator(long time)
        {
            if (time <= transitionPeriod)
            {
                double m = (secondValue - firstValue) / ((double)transitionPeriod);
                double x = time;
                double y0 = firstValue;
                return y0 + m * x;
            }
            else if (time > transitionPeriod && time <= transitionPeriod + steadyPeriod)
            {
                return secondValue;
            }
            else if (time > transitionPeriod + steadyPeriod && time <= 2 * transitionPeriod + steadyPeriod)
            {
                double m = (firstValue - secondValue) / ((double)transitionPeriod);
                double x = time;
                double y0 = secondValue;
                double x0 = transitionPeriod + steadyPeriod;
                return m * x - m * x0 + y0;
            }
            else if (time < 2 * transitionPeriod + 2 * steadyPeriod)
            {
                return firstValue;
            }
            else
                throw new Exception("Calculating the alternator's value went wrong.");
        }

        public override double CalculateValue(long thisTime)
        {
            long totalPeriod = 2 * transitionPeriod + 2 * steadyPeriod;
            long newTime = (thisTime + timeOffset) % totalPeriod;
            return calculator(newTime);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            long totalPeriod = 2 * transitionPeriod + 2 * steadyPeriod;
            long newTime = (thisTime + timeOffset) % totalPeriod;
            return derivative(newTime);
        }
    }
}
