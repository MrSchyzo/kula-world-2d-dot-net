using System;

namespace GameEngine.Animators
{
    //Fa comodo per tutti i movimenti periodici che partono dal centro
    public class SinusoidalAnimator : Animator
    {
        private double periodo;
        private double ampiezza;

        public SinusoidalAnimator(double val, long timeStamp, double period, double width)
        {
            S_Animator_MutualPart(val, timeStamp);
            periodo = period;
            ampiezza = width;
        }

        public override double CalculateValue(long thisTime)
        {
            double argToSin = (((double)thisTime - lastTimeStamp) / periodo) * 2.0 * Math.PI;
            return beginningValue + (Math.Sin(argToSin) * ampiezza);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double argToSin = (((double)thisTime - lastTimeStamp) / periodo) * 2.0 * Math.PI;
            return (Math.Cos(argToSin) * ampiezza);
        }
    }
}
