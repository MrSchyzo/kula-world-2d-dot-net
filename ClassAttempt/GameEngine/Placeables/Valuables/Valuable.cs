namespace GameEngine.Placeables.Valuables
{
    public abstract class Valuable : Placeable
    {
        protected int value = 250;
        protected string sound = "";

        public override void CollidesWithBall(long thisTime, Ball b)
        {
            if (doITouchBall(b) && isEnabled)
            {
                b.ModifyScore(value);
                b.PlaySound(sound);
                isEnabled = false;
            }
        }
    }
}
