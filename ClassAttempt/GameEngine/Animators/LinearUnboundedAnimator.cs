namespace GameEngine.Animators
{
    //Fa comodo per il movimento non limitato con velocità costante
    public class LinearUnboundedAnimator : Animator
    {
        private double c_angolare;

        public LinearUnboundedAnimator(double val, long timeStamp, double slope)
        {
            S_Animator_MutualPart(val, timeStamp);
            c_angolare = slope;
        }

        public override double CalculateValue(long thisTime)
        {
            double arg = ((double)thisTime - lastTimeStamp);
            return beginningValue + arg * c_angolare;
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            return c_angolare;
        }
    }
}
