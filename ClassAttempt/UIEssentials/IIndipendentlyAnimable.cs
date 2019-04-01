using GameUtils;
using ResourcesBasics;
using ResourceItems;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace UIEssentials
{
    #region MenuItem: classe basilare per costruire Menù a gerarchia.
    #endregion

    #region IIndipendentlyAnimable: rappresenta degli oggetti animabili indipendentemente (hanno la loro timeline indipendente).
    /// <summary>
    /// Questa classe rappresenta degli oggetti animabili indipendentemente (hanno la loro timeline indipendente).
    /// </summary>
    public abstract class IIndipendentlyAnimable
    {
        /*COMPLETE THIS SHIT!!!*/
        /// <summary>
        /// Timer per temporizzare la scena
        /// </summary>
        protected Stopwatch timer;
        /// <summary>
        /// Strati di disegno della scena
        /// </summary>
        protected List<Bitmap> layers;
        /// <summary>
        /// Riferimento al contenitore della scena
        /// </summary>
        protected SceneContainer container;
        /// <summary>
        /// Rettangolo di clipping per il disegno della scena
        /// </summary>
        protected Rectangle clipRegion;

        /// <summary>
        /// Avvia la timeline dell'animazione dell'oggetto.
        /// </summary>
        abstract public void Play();

        /// <summary>
        /// Serve per fermare la timeline dell'oggetto.
        /// </summary>
        abstract public void Pause();

        /// <summary>
        /// Serve a resettare lo stato dell'oggetto.
        /// </summary>
        abstract public void Reset();

        /// <summary>
        /// Serve a eliminare le risorse dell'oggetto.
        /// </summary>
        virtual public void Dispose() { }

        /// <summary>
        /// Disegna l'oggetto in un dato momento. (Si consiglia, in caso di subclassing, di chiamare base.Draw(e), se si vuole
        /// ricordare la regione di clipping del contesto grafico in input).
        /// </summary>
        /// <param name="e">PaintEventArgs e: argomenti dell'evento Paint (da cui ricavare il contesto grafico)</param>
        virtual public void Draw(Graphics e)
        {
            RectangleF fpBounds = e.ClipBounds;
            int x = (int)fpBounds.Width;
            int y = (int)fpBounds.Height;
            clipRegion = new Rectangle(0, 0, x, y);
        }

        /// <summary>
        /// Aggiorna lo stato dell'oggetto.
        /// </summary>
        abstract public void Update();

        /// <summary>
        /// Gestisce la coda di eventi passata come parametro, modificandola.
        /// </summary>
        /// <param name="bundle">Coda di eventi passata al gestore</param>
        abstract public void HandleEvent(KeyboardMouseEventBundle bundle);

        /// <summary>
        /// Imposta o restituisce lo scenecontainer a cui si fa riferimento.
        /// </summary>
        public SceneContainer Container
        {
            get { return container; }
            set { container = value; }
        }

        /// <summary>
        /// Carica tutti i file bmp/png e wav/mp3 di una cartella (tale path deve già contenere il backslash finale, prima del nome del file).
        /// </summary>
        /// <param name="logicPathDest">Cartella "logica" di destinazione</param>
        /// <param name="realPathSource">Cartella di sorgente del sistema operativo (deve già contenere il "\" finale)</param>
        protected void loadMediaFiles(string logicPathDest, string realPathSource)
        {
            ResourceDirectory root = container.getResourceDirectory;
            char[] c = new char[] { '\\' };
            string[] s = new string[] { };
            foreach (string f in GameApp.getFileNames(realPathSource))
            {
                if (f.EndsWith(".bmp") || f.EndsWith(".png"))
                    root.InsertFile(logicPathDest, f, new ImageResourceItem(realPathSource + f));
                else if (f.EndsWith(".wav") || f.EndsWith(".mp3"))
                    root.InsertFile(logicPathDest, f, new SoundResourceItem(realPathSource + f));
            }
        }
    }
    #endregion

}
