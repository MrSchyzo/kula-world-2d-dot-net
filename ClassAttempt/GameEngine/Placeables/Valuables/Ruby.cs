using System.Drawing;

namespace GameEngine.Placeables.Valuables
{
    
    public class Ruby : Valuable
    {
        public Ruby(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 1750;
            sound = "Gem";
        }
    }
    
}
