using System;

namespace GameEngine.Animators
{
    //Fa comodo per l'accentramento automatico della palla
    public class LinearBoundedAnimator : Animator
    {
        private double c_angolare;
        private double maxValue;
        private double minValue;
        private long timeCap;
        private bool isTimeBounded;

        public LinearBoundedAnimator(double val, long timeStamp, double slope, double maxVal, double minVal)
        {
            if (minVal > maxVal)
                throw new ArgumentException("In LinearBoundedAnimator(...), minVal MUST BE less than or equal maxVal!!");
            S_Animator_MutualPart(val, timeStamp);
            isTimeBounded = false;
            c_angolare = slope;
            maxValue = maxVal;
            minValue = minVal;
        }

        public LinearBoundedAnimator(double val, long timeStamp, double slope, long timeBeforeCap)
        {
            if (timeBeforeCap < 0)
                throw new ArgumentException("In LinearBoundedAnimator(...), timeBeforeCap MUST BE more than or equal 0");
            S_Animator_MutualPart(val, timeStamp);
            isTimeBounded = true;
            timeCap = timeBeforeCap;
            c_angolare = slope;
        }

        public override double CalculateValue(long thisTime)
        {
            double arg = ((double)thisTime - lastTimeStamp);
            double ret = beginningValue + arg * c_angolare;
            if (isTimeBounded)
                return beginningValue + (Math.Min(arg, (double)timeCap) * c_angolare);
            else
                return Math.Min(Math.Max(minValue, ret), maxValue);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double arg = ((double)thisTime - lastTimeStamp);
            double ret = beginningValue + arg * c_angolare;
            if (isTimeBounded && (arg < 0 || arg > timeCap))
                return 0;
            if (maxValue > ret || ret < minValue)
                return 0;
            else
                return c_angolare;
                
        }
    }
}
