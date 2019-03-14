using System.Drawing;

namespace GameEngine.Placeables.Valuables
{
    #region Classi di tutti i "valuables"
    public class Silver : Valuable
    {
        public Silver(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 500;
            sound = "Coin";
        }
    }
    #endregion
}
