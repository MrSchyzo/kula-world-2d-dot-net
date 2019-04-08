using System;

namespace GameEngine.Animators
{
    //Fa comodo per il movimento sottoposto a gravità, ma con limiti di velocità
    public class ParabolicToLinearAnimator : Animator
    {
        private double maxAbsoluteValue;
        private double a_parabola;
        private double b_parabola;

        private double linearUpper(double arg)
        {
            double chi = (maxAbsoluteValue - b_parabola);
            return
                -(chi * chi / (4 * a_parabola)) +
                maxAbsoluteValue * arg;
        }

        private double linearLower(double arg)
        {
            double chi = -(maxAbsoluteValue + b_parabola);
            return
               -(chi * chi / (4 * a_parabola)) -
               maxAbsoluteValue * arg;
        }

        public ParabolicToLinearAnimator(double val, long timeStamp, double gravity, double initialVelocity, double maxAbsValue)
        {
            S_Animator_MutualPart(val, timeStamp);
            maxAbsoluteValue = Math.Abs(maxAbsValue);
            a_parabola = gravity / 2.0;
            b_parabola = initialVelocity;
        }

        public override double CalculateValue(long thisTime)
        {
            double t = (double)thisTime - lastTimeStamp;
            double cSpeed = 2 * a_parabola * t + b_parabola;
            if (cSpeed > maxAbsoluteValue)
                return beginningValue + linearUpper(t);
            else if (cSpeed < -maxAbsoluteValue)
                return beginningValue + linearLower(t);
            else
                return beginningValue + a_parabola * t * t + b_parabola * t;
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double t = (double)thisTime - lastTimeStamp;
            return Math.Max(-maxAbsoluteValue, Math.Min(maxAbsoluteValue, 2 * a_parabola * t + b_parabola));
        }

    }
}
