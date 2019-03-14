using System.Drawing;

namespace GameEngine.Placeables.Valuables
{
    #region Classi di tutti i "valuables"
    public class Gold : Valuable
    {
        public Gold(int idX, int idY, float persp, Bitmap tex)
        {
            initPlaceable(idX, idY, persp, tex);
            value = 750;
            sound = "Coin";
        }
    }
    #endregion
}
