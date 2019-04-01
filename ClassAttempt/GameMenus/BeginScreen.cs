using GameMenus;
using GameUtils;
using MultimediaClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UIEssentials;

namespace GameLoadingScreens
{
    #region BeginScreen: Classe che rappresenta la schermata iniziale del gioco.
    /// <summary>
    /// Questa classe rappresenta la schermata iniziale del gioco, prima del menù principale. È necessario specificare l'ID del menù principale nel container.
    /// </summary>
    public class BeginScreen : IIndipendentlyAnimable
    {
        private SoundMediaPlayer loopingAudio;
        private ulong times = 0;
        private bool audioReady = false;
        private byte mainMenuID;
        private BackgroundWorker loader;

        private void LoadStuff(object sender, DoWorkEventArgs e)
        {
            long now = timer.ElapsedMilliseconds;
            GameMainMenu m = new GameMainMenu(this.container);
            container.addScene(mainMenuID, m);
            long after = timer.ElapsedMilliseconds;
            long wait = 2000 - (after - now);
            Thread.Sleep(Math.Max((int)wait, 0));
        }

        void LoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loader.Dispose();
            goToMainMenu();
        }

        private bool validImage()
        {
            return (this.layers.Count != 0);
        }

        private bool validAudio()
        {
            return this.audioReady;
        }

        private void goToMainMenu()
        {
            this.container.changeScene(mainMenuID);
        }

        private void loadAudio(string Path)
        {
            try
            {
                this.loopingAudio = new SoundMediaPlayer(Path);
                this.loopingAudio.Load();
                this.audioReady = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Minchia! Non è stato caricato l'audio " + Path + "! \n" + e.ToString());
                Console.WriteLine();
                this.audioReady = false;
            }
        }

        private void ConstructorsMutualPart(string Path, byte id)
        {
            this.layers = new List<Bitmap>();
            this.timer = new Stopwatch();
            this.loader = new BackgroundWorker();
            this.mainMenuID = id;
            this.clipRegion = new Rectangle(0, 0, 800, 600);
            try
            {
                layers.Add(new Bitmap(Image.FromFile(Path)));
            }
            catch (Exception e)
            {
                Console.WriteLine("Minchia! Non è stata caricata l'immagine " + Path + "!! \n" + e.ToString());
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Inizializza una schermata iniziale a partire dall'id da attribuire
        /// </summary>
        /// <param name="ID">byte da attribuire alla schermata iniziale</param>
        public BeginScreen(byte ID)
        {
            string ImgPath = GameApp.CurDir() + GameConstraints.BeginScreen.Path;
            this.ConstructorsMutualPart(ImgPath + @"\Background.bmp", ID);
            this.loadAudio(ImgPath + @"\loop.mp3");
        }

        /// <summary>
        /// Inizializza una schermata iniziale a partire dall'id da attribuire e il path dell'immagine di sfondo
        /// </summary>
        /// <param name="ImgPath">Path dell'immagine di sfondo</param>
        /// <param name="ID">byte da attribuire alla schermata iniziale</param>
        public BeginScreen(string ImgPath, byte ID)
        {
            this.ConstructorsMutualPart(ImgPath, ID);
        }

        /// <summary>
        /// Inizializza una schermata iniziale a partire dall'id da attribuire, il path dell'immagine di sfondo, il path dell'audio del loop
        /// </summary>
        /// <param name="ImgPath">Path dell'immagine di sfondo</param>
        /// <param name="ID">byte da attribuire alla schermata iniziale</param>
        /// <param name="AudioPath">Path del suono del loop</param>
        public BeginScreen(string ImgPath, string AudioPath, byte ID)
        {
            this.ConstructorsMutualPart(ImgPath, ID);
            this.loadAudio(AudioPath);
        }

        /// <summary>
        /// Inizia l'elaborazione dello stato della scena corrente.
        /// </summary>
        public override void Play()
        {
            /*Fai partire l'audio*/
            if (validAudio())
                this.loopingAudio.PlayLooping();
            this.timer.Start();
            /*Istruisci il worker*/
            loader.DoWork += LoadStuff;
            loader.RunWorkerCompleted += LoadCompleted;
            /*Avvia il worker*/
            loader.RunWorkerAsync();
        }

        /// <summary>
        /// Ferma l'elaborazione dello stato della scena corrente.
        /// </summary>
        public override void Pause()
        {
            if (validAudio())
                while (!this.loopingAudio.Stop()) ;
            //TODO Altro
            this.timer.Stop();
        }

        /// <summary>
        /// Gestisce l'evento dispatchato dal contenitore padre
        /// </summary>
        /// <param name="bundle"></param>
        public override void HandleEvent(KeyboardMouseEventBundle bundle)
        {
            KeyboardMouseEventArgs received = bundle.extractEvent();
            if (received == null)
                Console.WriteLine("Nessun evento ricevuto.");
            else
            {
                if (received.isKeyEvent)
                    Console.WriteLine(received.getEventType.ToString() + " ricevuto.");
                else
                {
                    MouseEventArgs e = received.mouseEvent;
                    int x = e.X;
                    int y = e.Y;
                    Console.WriteLine(received.getEventType.ToString() + @" (x: " + x + @", y: " + y + ")" + " ricevuto.");
                }
            }
        }

        /// <summary>
        /// Aggiorna lo stato della scena corrente.
        /// </summary>
        public override void Update()
        {
            (this.times)++;
            //Console.WriteLine("Ricevuta la " + (this.times).ToString() + "a richiesta di aggiornamento");
        }

        /// <summary>
        /// Reimposta lo stato della scena corrente.
        /// </summary>
        public override void Reset()
        {
            this.times = 0;
            //Console.WriteLine("Ricevuta una richiesta di reset.");
        }

        /// <summary>
        /// Disegna lo stato della scena corrente.
        /// </summary>
        /// <param name="e">Contesto grafico dove disegnare la scena</param>
        public override void Draw(Graphics e)
        {
            if (validImage())
            {
                //Console.WriteLine("Ricevuta una richiesta di disegno.");
                base.Draw(e);
                e.DrawImage(this.layers.ElementAt<Bitmap>(0), clipRegion);
            }
        }

        /// <summary>
        /// Ripone tutte le risorse dalla scena corrente
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.layers.ElementAt<Bitmap>(0).Dispose();
            this.layers.Clear();
            this.loopingAudio.Stop();
            this.loopingAudio.Dispose();
            this.loopingAudio = null;
            GC.Collect();
        }
    }
    #endregion
}
