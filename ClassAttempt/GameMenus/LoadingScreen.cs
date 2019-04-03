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
using System.Threading;
using System.Windows.Forms;
using UIEssentials;

namespace GameLoadingScreens
{

    
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
        

        
        private void drawBackground(Graphics e)
        {
            if (definitiveBg != null && clipRegion != null)
                e.DrawImage(definitiveBg, clipRegion);
        }

        private void drawStatusBar(Graphics e)
        {
            if (clipRegion != null)
            {
                
                /*int clipW = clipRegion.Width;
                int clipH = clipRegion.Height;

                int barW = clipW / 3;
                int barH = clipH / 8;
                int barX = (clipW - barW) / 2;
                int barY = 3 * ((clipH - barH) / 4);

                Rectangle bar = new Rectangle(barX, barY, barW, barH);*/
                

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
        

        
        private double Periodize(long x, double period)
        {
            return GameApp.Periodize(x, period);
        }

        private void SetupPositions(long x)
        {
            offsetX = Math.Cos(Periodize(x, periodX));
            offsetY = -Math.Cos(Periodize(x, periodY));
        }
        

        
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
        
    }
    
}
