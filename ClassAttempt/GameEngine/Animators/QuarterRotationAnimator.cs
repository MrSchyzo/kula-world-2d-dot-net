using System;

namespace GameEngine.Animators
{
    //Fa comodo per il movimento della X e della Y durante un cambio di faccia
    public class QuarterRotationAnimator: Animator
    {
        private long quarterDuration;
        private bool clockWise;
        private bool calculateWithSinus;
        private double radium;

        public QuarterRotationAnimator(double startVal, long timeStamp, long quarterPeriod, bool isClockWise, bool isX, double radium)
        {
            if (quarterPeriod <= 0 || radium <= 0)
                throw new ArgumentException("In QuarterRotationAnimator(...), the period of the quarter or the radium are non-positive.");
            S_Animator_MutualPart(startVal, timeStamp);
            this.radium = radium;
            quarterDuration = quarterPeriod;
            clockWise = isClockWise;
            calculateWithSinus = isX;
        }

        public override double CalculateValue(long thisTime)
        {
            double t =
                Math.Min
                (
                    Math.Max
                    (
                        0,
                        (double)thisTime - lastTimeStamp

                    ),
                    quarterDuration
                );
            double arg = (t / quarterDuration) * (Math.PI / 2.0);
            if (!clockWise)
                arg *= -1;
            if (calculateWithSinus)
                return beginningValue + radium * Math.Sin(arg);
            else
                return beginningValue - radium * Math.Cos(arg);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double t =
                Math.Min
                (
                    Math.Max
                    (
                        0,
                        (double)thisTime - lastTimeStamp

                    ),
                    quarterDuration
                );
            double arg = (t / quarterDuration) * (Math.PI / 2.0);
            if (arg > 90)
                arg = 90;
            if (arg < 0)
                arg = 0;
            if (!clockWise)
                arg *= -1;
            if (calculateWithSinus)
                return radium * Math.Cos(arg);
            else
                return radium * Math.Sin(arg);
        }
    }
}
