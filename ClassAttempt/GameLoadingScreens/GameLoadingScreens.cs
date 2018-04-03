using GameMenus;
using GameUtils;
using MultimediaClasses;
using ResourcesBasics;
using ResourceItems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIEssentials;
using UIMainClasses;

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

    #region LoadingScreen: Classe che rappresenta la schermata di caricamento intragioco: è delegata al caricamento delle risorse necessarie al gioco.
    /// <summary>
    /// Questa classe rappresenta una schermata di caricamento, è questa che si occupa del caricamento delle risorse che non sono presenti.
    /// </summary>
    public class LoadingScreen : IIndipendentlyAnimable
    {
        private byte progress;
        private int remainingLoadingLevels;
        private bool isActive;
        private BackgroundWorker loader;
        private SoundMediaPlayer bgMusic;
        private Bitmap definitiveBg;
        private LoadResourceItem table;
        private long periodX = 5000;
        private long periodY = 6500;
        private double offsetY = 0.0;
        private double offsetX = 0.0;
        private bool inDebug = false;
        private bool allOk = false;
        private long lastMoment = 0;

        #region Metodi di utilità
        /// <summary>
        /// Stampa alla console il messaggio indicato in input. Funziona solo se la variabile interna "inDebug" è true.
        /// </summary>
        /// <param name="s">Stringa da stampare alla console</param>
        protected void PrintToConsole(string s)
        {
            if (inDebug)
                Console.WriteLine(s);
        }

        private void waitTime()
        {
            long remaining = Math.Max(0, 3000 - (timer.ElapsedMilliseconds - lastMoment));
            Thread.Sleep((int)remaining);
            allOk = true;
        }
        #endregion

        #region Metodi privati per gestire gli eventi del worker: Inizia lavoro, lavoro completato
        private void loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread t = new Thread(waitTime);
            t.Start();
        }

        private void loader_DoWork(object sender, DoWorkEventArgs e)
        {
            allOk = false;
            progress = 0;
            if (table.IsBatchLoading)
            {
                table.FlushAllCachedLevelsAndThemes();
                int todo = 10;
                while (todo > 0 && table.ProcessNormalLevel())
                {
                    todo--;
                    progress = (byte)(((float)(remainingLoadingLevels - todo)) / ((float)(remainingLoadingLevels)));
                }
                progress = 100;
            }
            else
            {
                table.ProcessNormalLevel();
                table.ProcessBonusLevel();
            }
        }
        #endregion

        #region Metodi privati di disegno: drawBackground, drawStatusBar
        private void drawBackground(Graphics e)
        {
            if (definitiveBg != null && clipRegion != null)
                e.DrawImage(definitiveBg, clipRegion);
        }

        private void drawStatusBar(Graphics e)
        {
            if (clipRegion != null)
            {
                #region Modo senza il metodo zoomedAndSizedRectangleOf
                /*int clipW = clipRegion.Width;
                int clipH = clipRegion.Height;

                int barW = clipW / 3;
                int barH = clipH / 8;
                int barX = (clipW - barW) / 2;
                int barY = 3 * ((clipH - barH) / 4);

                Rectangle bar = new Rectangle(barX, barY, barW, barH);*/
                #endregion

                Rectangle bar = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.5f, 0.8f, 0.3f, 0.0625f, true);

                int centreX = bar.X + (bar.Width / 2);
                int centreY = bar.Y + (bar.Height / 2);

                float amount = ((float)progress) / 100.0f;
                int insideWidth = (int)(((float)bar.Width) * amount);

                LinearGradientBrush inside =
                    new LinearGradientBrush(
                        new Point(bar.X, centreY),
                        new Point(bar.X + bar.Width, centreY),
                        Color.FromArgb(200, 20, 100, 20),
                        Color.FromArgb(200, 100, 255, 100)
                        );

                Pen outside1 = new Pen(Color.FromArgb(250, Color.AntiqueWhite), 4.0f);
                Pen outside2 = new Pen(Color.FromArgb(250, Color.SandyBrown), 2.0f);

                e.FillRectangle(inside, new Rectangle(bar.X, bar.Y, insideWidth, bar.Height));
                e.DrawRectangle(outside1, bar);
                e.DrawRectangle(outside2, bar);

                inside.Dispose();
                inside = null;
                outside1.Dispose();
                outside2.Dispose();
                GC.Collect();
            }
        }
        #endregion

        #region Metodi privati per le oscillazioni di background
        private double Periodize(long x, double period)
        {
            return GameApp.Periodize(x, period);
        }

        private void SetupPositions(long x)
        {
            offsetX = Math.Cos(Periodize(x, periodX));
            offsetY = -Math.Cos(Periodize(x, periodY));
        }
        #endregion

        #region Metodi ereditati dalla superclasse IIndipendentlyAnimable
        /// <summary>
        /// Aggiorna lo stato della schermata di caricamento.
        /// </summary>
        public override void Update()
        {
            //Oscillazione dello sfondo.
            if (layers.ElementAt<Bitmap>(0) != null)
            {
                Bitmap bg = layers.ElementAt<Bitmap>(0);

                double imgW = ((double)bg.Width);
                double imgH = ((double)bg.Height);

                double recW = (imgW / 2.0) - 1.0;
                double recH = (imgH / 2.0) - 1.0;

                long now = timer.ElapsedMilliseconds;
                SetupPositions(now);

                double baseX = (imgW - recW) / 2.0;
                double baseY = (imgH - recH) / 2.0;

                int outX = (int)(baseX + (baseX * offsetX));
                int outY = (int)(baseY + (baseY * offsetY));
                int outW = (int)recW;
                int outH = (int)recH;

                definitiveBg = GameApp.copyImgPiece(bg, new Rectangle(outX, outY, outW, outH), new Size(400, 300));
                bg = null;
                GC.Collect();
                if (allOk && isActive)
                    container.changeScene(GameConstraints.GameScreen.ID);
            }
        }

        /// <summary>
        /// Avvia l'elaborazione dello stato della schermata di caricamento.
        /// </summary>
        public override void Play()
        {
            timer.Start();
            isActive = true;
            if (bgMusic != null)
                bgMusic.PlayLooping(this.container.VolumeMusic);
            lastMoment = timer.ElapsedMilliseconds;
            loader.RunWorkerAsync(null);
        }

        /// <summary>
        /// Disegna lo stato della schermata di caricamento.
        /// </summary>
        /// <param name="e"></param>
        public override void Draw(Graphics e)
        {
            try
            {
                if (inDebug)
                    Console.Clear();
            }
            catch (Exception e1)
            {
                e1.ToString();
            }
            PrintToConsole("\n==================================");

            long now = timer.ElapsedTicks;
            base.Draw(e);
            long after = timer.ElapsedTicks;
            long total = 0;
            total += (after - now);
            PrintToConsole("Superclass draw: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");

            now = timer.ElapsedTicks;
            drawBackground(e);
            after = timer.ElapsedTicks;
            total += (after - now);
            PrintToConsole("Drawing Background: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");

            now = timer.ElapsedTicks;
            drawStatusBar(e);
            after = timer.ElapsedTicks;
            total += (after - now);
            PrintToConsole("Drawing Bar: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");

            PrintToConsole("All the drawing took: " + GameApp.TicksToMillis(total) + " milliseconds.");
            PrintToConsole("==================================");
        }

        /// <summary>
        /// Gestisce l'evento dispatchato dal contenitore delle scene.
        /// </summary>
        /// <param name="bundle"> </param>
        public override void HandleEvent(KeyboardMouseEventBundle bundle)
        {
            KeyboardMouseEventArgs ev = bundle.extractEvent();
            if (ev != null && ev.isKeyEvent && !ev.isNullKeyEvent() && ev.getEventType == KeyboardMouseEventID.Key_Up)
            {
                if (ev.keyEvent.KeyCode == Keys.F1)
                    inDebug = !inDebug;
            }
        }

        /// <summary>
        /// Blocca lo stato della schermata di caricamento.
        /// </summary>
        public override void Pause()
        {
            isActive = false;
            bgMusic.Stop();
            if (bgMusic != null)
                while (!bgMusic.Stop()) ;
            timer.Stop();
        }

        /// <summary>
        /// Reimposta lo stato della schermata di caricamento.
        /// </summary>
        public override void Reset()
        {
            isActive = false;
            timer.Stop();
            timer.Reset();
        }

        /// <summary>
        /// Ripone tutte le risorse della schermata di caricamento
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region Costruttori
        private void ConstructorsMutualPart(SceneContainer sc)
        {
            Container = sc;
            ResourceDirectory root = sc.getResourceDirectory;
            string itsPath = GameApp.CurDir() + GameConstraints.LoadingScreen.Path;
            string itsName = GameConstraints.LoadingScreen.LogicDir;
            root.NewDirectory(itsName);
            loadMediaFiles(itsName, itsPath + @"\");
            
            remainingLoadingLevels = 10;
            timer = new Stopwatch();
            layers = new List<Bitmap>();
            loader = new BackgroundWorker();
            isActive = false;

            loader.DoWork += loader_DoWork;
            loader.RunWorkerCompleted += loader_RunWorkerCompleted;
            ImageResourceItem a = (ImageResourceItem)Container.getResourceDirectory.GetFile(GameConstraints.LoadingScreen.LogicDir, "Background.bmp");
            if (a != null)
            {
                Bitmap img = a.Content;
                layers.Add(GameApp.ResizeImg(img, 800, 600));
            }

            SoundResourceItem s = (SoundResourceItem)Container.getResourceDirectory.GetFile(GameConstraints.LoadingScreen.LogicDir, "loop.mp3");
            bgMusic = s.Content;
            
            table =
                (LoadResourceItem)sc.getResourceDirectory.GetFile
                (
                    GameConstraints.OtherPaths.MetadataLogicDir,
                    GameConstraints.OtherPaths.LoaderResourceName
                );
            table.IsBatchLoading = true;
            table.IsCaching = false;
            table.SetNextNormalLevelToLoad(FileNames.FirstLevelFilename);

            foreach(string bonLvl in FileNames.AllBonusLevelsFileName)
            {
                table.SetNextBonusLevelToLoad(bonLvl);
                table.ProcessBonusLevel();
            }

            loader_DoWork(null, null);
            table.IsCaching = true;
            progress = 0;
        }

        /// <summary>
        /// Crea una schermata di caricamento.
        /// </summary>
        /// <param name="sc">Contenitore delle scene a cui si deve riferire la schermata.</param>
        public LoadingScreen(SceneContainer sc)
        {
            ConstructorsMutualPart(sc);
        }
        #endregion
    }
    #endregion
}
