using System.Drawing;

namespace GameEngine.Placeables.Valuables
{
    public class Diamond : Valuable
    {
        public Diamond(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 3000;
            sound = "Gem";
        }
    }
}
