using System.Drawing;

namespace GameEngine.Placeables.Valuables
{
    #region Classi di tutti i "valuables"
    public class Sapphire : Valuable
    {
        public Sapphire(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 1250;
            sound = "Gem";
        }
    }
    #endregion
}
