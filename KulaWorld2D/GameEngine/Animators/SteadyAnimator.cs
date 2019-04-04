namespace GameEngine.Animators
{
    //Fa comodo quando la palla è ferma
    public class SteadyAnimator : Animator
    {
        public SteadyAnimator(double val, long timeStamp)
        {
            S_Animator_MutualPart(val, timeStamp);
        }

        public override double CalculateValue(long thisTime)
        {
            return beginningValue;
        }

        public override double GetCurrentSpeed(long thisTime)
        {
            return 0;
        }
        
    }
}
