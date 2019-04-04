using System.Drawing;

namespace GameEngine.Placeables.Valuables
{
    
    public class Emerald : Valuable
    {
        public Emerald(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 2250;
            sound = "Gem";
        }
    }
    
}
