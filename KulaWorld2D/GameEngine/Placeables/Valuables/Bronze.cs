using System.Drawing;

namespace GameEngine.Placeables.Valuables
{
    
    public class Bronze : Valuable
    {
        public Bronze(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 250;
            sound = "Coin";
        }
    }
    
}
