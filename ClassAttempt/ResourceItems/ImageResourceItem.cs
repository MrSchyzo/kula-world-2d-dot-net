using GameUtils;
using ResourcesBasics;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ResourceItems
{
    #region ImageResourceItem: classe che incorpora un'immagine nei file "logici".
    /// <summary>
    /// ResourceItem specializzato nel trattare Bitmaps: è possibile caricare e scaricare un'immagine, oltre che a consultare il contenuto.
    /// Il caricamento della risorsa è sincrono!
    /// </summary>
    public class ImageResourceItem : ResourceItem
    {
        private Bitmap img;
        private Rectangle bounds;

        private void load(string Path)
        {
            try
            {
                img = new Bitmap(Path);
                bounds = new Rectangle(0, 0, img.Size.Width, img.Size.Height);
            }
            catch (Exception e)
            {
                Console.WriteLine("Minchia! L'immagine " + Path + " non è stata caricata.\n" + e.ToString());
                Console.WriteLine();
                MessageBox.Show("Minchia! L'immagine " + Path + " non è stata caricata.\n" + e.ToString());
                img = null;
            }
        }

        private void unload()
        {
            if (img != null)
                img.Dispose();
            img = null;
        }

        private bool IsEmpty()
        {
            return (img == null);
        }

        /// <summary>
        /// Inizializza una risorsa di immagine a partire dal path del file da rappresentare.
        /// </summary>
        /// <param name="Path">Path del file</param>
        public ImageResourceItem(string Path)
        {
            if (Path != null)
                load(Path);
            else
                img = null;
        }

        /// <summary>
        /// Carica un'altra immagine nella risorsa a partire dal path, restituisce true se e solo se il caricamento è andato a buon fine
        /// </summary>
        /// <param name="Path">Path del file</param>
        /// <returns></returns>
        public override bool Load(string Path)
        {
            if (img != null)
                unload();
            if (Path != null)
                load(Path);
            return !IsEmpty();
        }

        /// <summary>
        /// Scarica l'immagine contenuta dalla risorsa
        /// </summary>
        public override void Unload()
        {
            unload();
        }

        /// <summary>
        /// Restituisce il contenuto dell'image resource item.
        /// </summary>
        public Bitmap Content
        {
            get { return img; }
        }

        /// <summary>
        /// Ridimensiona il contenuto ad altezza e larghezza scelta.
        /// </summary>
        /// <param name="w">Larghezza in pixel</param>
        /// <param name="h">Altezza in pixel</param>
        public void resizeContent(int w, int h)
        {
            if (w > 0 && h > 0 && img != null)
            {
                Bitmap nuovo = GameApp.ResizeImg(img, w, h);
                img.Dispose();
                img = nuovo;
            }
        }

        /// <summary>
        /// Ridimensiona il contenuto secondo uno scalare, che manterrà l'aspect ratio.
        /// </summary>
        /// <param name="scale">Scalare che modifica le dimensioni.</param>
        public void resizeContent(float scale)
        {
            if (scale > 0 && img != null)
            {
                int w = (int)(scale * ((float)img.Width));
                int h = (int)(scale * ((float)img.Height));
                Bitmap nuovo = GameApp.ResizeImg(img, w, h);
                img.Dispose();
                img = nuovo;
            }
        }
    }
    #endregion
}
