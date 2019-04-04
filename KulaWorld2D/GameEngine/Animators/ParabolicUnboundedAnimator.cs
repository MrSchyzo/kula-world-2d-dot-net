namespace GameEngine.Animators
{
    //Fa comodo per il movimento sottoposto a gravità, senza limiti di velocità
    public class ParabolicUnboundedAnimator : Animator
    {
        private double b_parabola;
        private double a_parabola;

        public ParabolicUnboundedAnimator(double val, long timeStamp, double gravity, double initialSpeed)
        {
            S_Animator_MutualPart(val, timeStamp);
            b_parabola = initialSpeed;
            a_parabola = gravity / 2.0;
        }

        public double getCurrentVelocity(long thisTime)
        {
            double arg = (double)thisTime - lastTimeStamp;
            return b_parabola + 2 * a_parabola * arg;
        }

        public override double CalculateValue(long thisTime)
        {
            double arg = (double)thisTime - lastTimeStamp;
            return beginningValue + (a_parabola * (arg * arg) + b_parabola * arg);
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            double arg = (double)thisTime - lastTimeStamp;
            return 2 * a_parabola * arg + b_parabola;
        }
    }
}
