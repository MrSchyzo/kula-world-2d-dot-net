using GameMetadata;
using GameUtils;
using LevelsStructure;
using MultimediaClasses;
using ResourcesBasics;
using ResourceItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIEssentials;


namespace GameEngine
{
    public enum GameScreenState
    {
        ToSearch,
        ToBuild,
        InGame,
        InPrompt
    }

    public enum PromptType
    {
        Retry, //Premi qualsiasi tasto per riprovare (vinto/perso in prova, perso in partita normale)
        GoNext, //Premi qualsiasi tasto per continuare (vinto/perso in bonus, vinto in partita normale)
        End //Non ci sono più livelli da fare o game over
    }
    
    public class GameScreen : IIndipendentlyAnimable
    {
        #region Variabili private: mappa
        private SortedDictionary<Pair<int>, GameBlock> blocks;
        private SortedDictionary<Pair<int>, SortedDictionary<KulaLevel.Orientation, GameSurface>> surfaces;
        private SortedDictionary<Pair<int>, GamePlaceable> placeables;
        private SortedDictionary<Pair<int>, GameEnemy> enemies;
        private Ball ball;
        private RectangleF lvlBounds;
        #region Variabili di creazione della mappa
        private bool hasExit = false;
        private bool hasSpawn = false;
        private bool isBonus = false;
        private int keys;
        #endregion
        #region Variabili della gestione del gioco: se è un trylevel
        public bool IsTryingLevel { get; set; }
        #endregion
        #endregion
        #region Variabili del profilo: punteggio, frutti, livelli fatti, livello corrente, prossimo livello, livelli bonus
        private long currentScore, maxScore;
        private int fruit;
        private string currentLevel;
        private string nextLevel;
        private List<string> bonuses = new List<string>();
        private bool didBonus = true;
        #endregion
        #region Variabili private: immagini e suoni di gioco
        private SortedDictionary<string, Bitmap> images;
        private SortedDictionary<string, SoundMediaPlayer> sounds;
        #endregion
        #region Variabili private: round corrente
        private int remainingTime;
        private int roundTime;
        private long roundScore;
        private long roundPenalty;
        private bool gotFruit = false;
        private int remainingKeys;
        private S_Animator timeCounter;
        #endregion
        #region Variabili private: gestione degli input
        private Command cmd = Command.Nothing;
        private GameScreenState gsState = GameScreenState.ToSearch;
        private bool inDebug = false;
        private long lastMoment = 0;
        private Highscores tabellone;
        #endregion
        #region Variabili private: prompt intralivello
        private string promptMsg = "";
        private PromptType promptType = PromptType.Retry;
        #endregion
        #region Timer di gioco e musica di sottofondo
        private Stopwatch gameTimer;
        private SoundMediaPlayer bgMusic;
        private Bitmap bgImage;
        private Bitmap promptFrame = new Bitmap(1, 1);
        private bool needToUpdateFrame = true;
        #endregion

        #region Metodi privati di gestione degli input
        private void handleKeyDown(KeyEventArgs e)
        {
            if (gsState == GameScreenState.InGame)
            {
                if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                    cmd = Command.Left;
                else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                    cmd = Command.Right;
                else if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up || e.KeyCode == Keys.Space)
                    cmd = Command.Jump;
                else if (e.KeyCode == Keys.Escape && !ball.IsStateAlso(BallState.ChangingFace)) //Bug: se uno pausa quando cambio faccia, exception!
                    container.changeScene(GameConstraints.PauseMenu.ID);
                else if (e.KeyCode == Keys.F1)
                    inDebug = !inDebug;
            }
        }

        private void handleKeyUp(KeyEventArgs e)
        {
            bool inputLatency = timer.ElapsedMilliseconds - lastMoment >= 1500;
            if (gsState == GameScreenState.InPrompt && inputLatency)
            {
                if (promptType == PromptType.End)
                    quitGame();
                else if (promptType == PromptType.GoNext)
                    toNextLevel();
                else
                    retryLevel();
            }
        }

        /// <summary>
        /// Stampa alla console il messaggio indicato in input. Funziona solo se la variabile interna "inDebug" è true.
        /// </summary>
        /// <param name="s">Stringa da stampare alla console</param>
        protected void PrintToConsole(string s)
        {
            if (inDebug)
                Console.WriteLine(s);
        }
        #endregion

        #region Metodi privati per l'inizializzazione
        private void initSounds()
        {
            ResourceDirectory dir = container.getResourceDirectory;
            foreach(string file in dir.GetDirectoryItemNames(GameConstraints.GameScreen.LogicDir))
            {
                if (file.EndsWith(".wav"))
                {
                    SoundResourceItem ri = (SoundResourceItem) dir.GetFile(GameConstraints.GameScreen.LogicDir, file);
                    sounds.Add(file, ri.Content);
                }
            }

            HighScoresResourceItem hsri = (HighScoresResourceItem)dir.GetFile(GameConstraints.OtherPaths.MetadataLogicDir, FileNames.HighscoresFileName);
            tabellone = hsri.Content;
        }
        private void initImages()
        {
            ResourceDirectory dir = container.getResourceDirectory;
            foreach (string file in dir.GetDirectoryItemNames(GameConstraints.GameScreen.LogicDir))
            {
                if (file.EndsWith(".png") || file.EndsWith(".bmp"))
                {
                    ImageResourceItem ri = (ImageResourceItem)dir.GetFile(GameConstraints.GameScreen.LogicDir, file);
                    images.Add(file, ri.Content);
                }
            }
        }
        private void initVariables()
        {
            fruit = 0;
            maxScore = 0;
            currentScore = 0;
            timer = new Stopwatch();
            ball = new Ball(new Bitmap(64, 64), new Bitmap(64, 64), this);
            gameTimer = new Stopwatch();
            bgMusic = new SoundMediaPlayer();
            bgImage = new Bitmap(1, 1);
            sounds = new SortedDictionary<string, SoundMediaPlayer>();
            images = new SortedDictionary<string, Bitmap>();
            blocks = new SortedDictionary<Pair<int>, GameBlock>();
            surfaces = new SortedDictionary<Pair<int>, SortedDictionary<KulaLevel.Orientation, GameSurface>>();
            placeables = new SortedDictionary<Pair<int>, GamePlaceable>();
            enemies = new SortedDictionary<Pair<int>, GameEnemy>();
            bonuses = FileNames.AllBonusLevelsFileName;
            isBonus = false;
            IsTryingLevel = false;


        }
        private void bindToContainer(SceneContainer sc)
        {
            container = sc;
            this.clipRegion = new Rectangle(0, 0, 800, 600);
        }
        #endregion

        #region Metodi privati per i disegni
        
        #region Metodi per il disegno dello HUD
        private void drawClock(Rectangle bounds, Graphics e)
        {
            Rectangle circle = GameApp.zoomedAndSizedRectangleOf(bounds, 0.5f, 3f / 8f, 0.75f, 0.75f, true);
            Rectangle text = GameApp.zoomedAndSizedRectangleOf(bounds, 0.5f, 7f / 8f, 0.5f, 0.25f, true);

            Bitmap clock;
            if (!images.TryGetValue("Clock.png", out clock))
                clock = new Bitmap(1, 1);
            clock = GameApp.ResizeImg(clock, 100, 100);

            e.DrawImage(clock, circle);

            int remTime = remainingTime / 1000;

            //if (remTime < 20)
                GameApp.DrawPromptTextInBox(e, remTime.ToString(), text, StringAlignment.Center, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            float length = (circle.Height * 0.4f);
            Point center = GameApp.RectangleCentre(circle);
            float time = (float)GameApp.Periodize(remainingTime, 120000);

            e.DrawLine(new Pen(Color.Black, 2), center.X, center.Y, center.X + (float)Math.Sin(time)*length, center.Y - (float)Math.Cos(time)*length);
        }

        private void drawScore(Rectangle bounds, Graphics e)
        {
            GameApp.DrawPromptTextInBox(
                e, 
                "Current: " + currentScore + "\nRound: " + roundScore, 
                bounds, 
                StringAlignment.Center, 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis
                );
        }

        private void drawKeys(Rectangle bounds, Graphics e)
        {
            //TOCHECK: draw keys
            if (keys > 10) //scrivo solo quante chiavi sono rimaste
                GameApp.DrawPromptTextInBox(
                    e, 
                    "Remaining keys: " + remainingKeys, 
                    bounds, 
                    StringAlignment.Center, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    );
            else //Disegno le chiavi centrate
            {
                Bitmap kSlot, keyImg;
                if (!images.TryGetValue("KeySlot.png", out kSlot) || !images.TryGetValue("Key.png", out keyImg))
                    throw new Exception("At least one of the following files {'KeySlot.png', 'Key.png'} is not loaded.");
                Rectangle box;
                float relWidth = Math.Min(0.125f, 1f / (float)keys);
                //float relX = (1f - (relWidth * (float)keys)) / 2f;
                for (int i = 0; i < keys; i++)
                {
                    box = GameApp.zoomedAndSizedRectangleOf(bounds, i * relWidth, 0f, relWidth, 1f, false);
                    e.DrawImage(kSlot, box);
                    if (i < keys - remainingKeys)
                        e.DrawImage(keyImg, box);
                }
            }
        }

        private void drawFruits(Rectangle bounds, Graphics e)
        {
            //TOCHECK: draw fruits
            int n = fruit % 5;
            if (gotFruit)
                n++;
            
            Rectangle box;
            Bitmap frutto;
            for(int i = 0; i < n; i++)
            {
                box = GameApp.zoomedAndSizedRectangleOf(bounds, i * 0.2f, 0f, 0.2f, 1f, false);
                if (!images.TryGetValue("Fruit" + i + ".png", out frutto))
                    throw new Exception("The file 'Fruit" + i + ".png' was not loaded.");
                e.DrawImage(frutto, box);
            }
        }

        private void drawRemainingBlocks(Rectangle bounds, Graphics e)
        {
            //TOCHECK and REDO: draw blocks
            int k = blocks.Values.ToList<GameBlock>().Count<GameBlock>(x => !x.IsTouched);
            GameApp.DrawPromptTextInBox(
                e,
                "Blocks to go: \n" + k,
                bounds, 
                StringAlignment.Center, 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
                );
        }

        private void drawHUD(Graphics e)
        {
            //TOCHECK: hud
            Rectangle box = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.5f, 1f/12f, 0.5f, 1f/6f, true);
            drawScore(box, e);

            box = GameApp.zoomedAndSizedRectangleOf(clipRegion, 7f / 8f, 0.125f, 0.25f, 0.25f, true);
            drawClock(box, e);

            if (!isBonus)
            {
                box = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.825f, 11f / 12f, 0.35f, 1f / 10f, true);
                drawFruits(box, e);

                box = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.25f, 11f / 12f, 0.5f, 1f / 10f, true);
                drawKeys(box, e);
            }
            else
            {
                box = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.125f, 0.125f, 0.25f, 0.25f, true);
                drawRemainingBlocks(box, e);
            }
        }
        #endregion

        private void drawWorld(Graphics e, bool enhanced, float tilesRange)
        {
            if (tilesRange <= 0)
                tilesRange = 1;
            tilesRange += 1;

            float maxDis = tilesRange * EngineConst.BlockWidth;

            #region Superfici
            foreach (SortedDictionary<KulaLevel.Orientation, GameSurface> a in surfaces.Values)
            {
                foreach (GameSurface gs in a.Values)
                {
                    if (gs != null)
                    {
                        if (!enhanced || EngUtils.Distance(gs.Center, ball.Center) <= maxDis)
                            gs.Draw(e, ball);
                    }
                }
            }
            #endregion

            #region Blocchi
            foreach (GameBlock b in blocks.Values)
            {
                if (b != null)
                    if (!enhanced || EngUtils.Distance(b.Center, ball.Center) <= maxDis)
                        b.Draw(e, ball);
            }
            #endregion

            #region Piazzabili
            foreach (GamePlaceable p in placeables.Values)
            {
                if (p != null)
                    if (!enhanced || EngUtils.Distance(p.Center, ball.Center) <= maxDis)
                        p.Draw(e, ball);
            }
            #endregion

            #region TODO: nemici
            foreach (GameEnemy en in enemies.Values)
            {
                if (en != null)
                    if (!enhanced)// or EngUtils.Distance(en.Center, ball.Center) <= maxDis)
                        en.Draw(e, ball);
            }
            #endregion
        }

        private void drawScene(Graphics e)
        {
            #region Definizione delle variabili utili per la misura del disegno
            float
                vpWidth = this.clipRegion.Width,
                vpHeight = this.clipRegion.Height,
                worldvpWidth = EngineConst.ViewportXTiles * EngineConst.BlockWidth,
                worldvpHeight = EngineConst.ViewportYTiles * EngineConst.BlockWidth,
                ratioX = (vpWidth / worldvpWidth),
                ratioY = (vpHeight / worldvpHeight),
                imageTilesDimension = (float)Math.Sqrt(EngineConst.ViewportXTiles * EngineConst.ViewportXTiles + EngineConst.ViewportYTiles * EngineConst.ViewportYTiles),
                imageTileX = -(imageTilesDimension - EngineConst.ViewportXTiles) / 2.0f,
                imageTileY = -(imageTilesDimension - EngineConst.ViewportYTiles) / 2.0f,
                worldCenterX = worldvpWidth * EngineConst.ViewportBallXRatio,
                worldCenterY = worldvpHeight * EngineConst.ViewportBallYRatio,
                offX = ball.Center.X - worldCenterX,
                offY = ball.Center.Y - worldCenterY;
            #endregion

            #region Definizione della matrice di trasformazione
            Matrix m = new Matrix();
            #endregion

            #region Disegno dello sfondo: preparazione della matrice, disegno e reset della matrice
            m.Scale(ratioX, ratioY);
            m.RotateAt(ball.Rotation, new PointF(worldvpWidth / 2f, worldvpHeight / 2f));
            e.Transform = m;
            e.DrawImage(
                bgImage,
                new RectangleF(
                    imageTileX * EngineConst.BlockWidth,
                    imageTileY * EngineConst.BlockWidth,
                    imageTilesDimension * EngineConst.BlockWidth,
                    imageTilesDimension * EngineConst.BlockWidth
                    )
                    );
            m.Reset();
            #endregion

            #region Disegno tutti gli altri attori eccetto la palla: preparo la matrice, disegno e reset della matrice
            m.Scale(ratioX, ratioY);
            m.RotateAt(ball.Rotation, new PointF(worldCenterX, worldCenterY));
            m.Translate(-offX, -offY);
            e.Transform = m;
            drawWorld(e, true, imageTilesDimension);
            m.Reset();
            #endregion

            #region Disegno la palla: preparo la matrice, disegno e reset della matrice
            m.Scale(ratioX, ratioY);
            e.Transform = m;
            ball.Draw(e);
            m.Reset();
            e.Transform = m;
            #endregion
        }

        private void drawPrompt(Graphics e)
        {
            Rectangle msgBounds = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.5f, 1f/10f, 0.95f, 1f/5f, true);
            TextFormatFlags tff = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis;

            bool inputLatency = timer.ElapsedMilliseconds - lastMoment >= 1500;

            #region Disegno il testo di prompt
            GameApp.DrawPromptTextInBox(e, promptMsg, msgBounds, StringAlignment.Center, tff);
            #endregion

            #region Disegno il testo intermedio del punteggio
            msgBounds = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.5f, 1f / 2f, 0.75f, 1f / 3f, true);
            string scoreboard = "";
            scoreboard += "Current Score: " + (currentScore - roundScore) + "\n";
            scoreboard += "+ Level Score: " + roundScore + "\n";
            scoreboard += "= New Total: " + (currentScore);
            GameApp.DrawPromptTextInBox(e, scoreboard, msgBounds, StringAlignment.Near, tff);
            #endregion

            if (inputLatency) 
            #region Disegno il testo del prompt di bottone
            { 
                msgBounds = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.5f, 7f / 8f, 0.9f, 1f / 8f, true);
                scoreboard = "Press any key to ";
                if (PromptType.End == promptType)
                    scoreboard += "go back to main menu.";
                else if (PromptType.Retry == promptType)
                    scoreboard += "play this level again.";
                else if (PromptType.GoNext == promptType)
                    scoreboard += "play the next level.";
                GameApp.DrawPromptTextInBox(e, scoreboard, msgBounds, StringAlignment.Center, tff);
            }
            #endregion
            
        }
        #endregion

        #region Metodi privati per gli aggiornamenti
        private List<Pair<int>> collisionControlArea(Ball b)
        {
            Point ballTilePos = new Point((int)(b.Center.X / EngineConst.BlockWidth), (int)(b.Center.Y / EngineConst.BlockWidth));
            List<Pair<int>> ret = new List<Pair<int>>();
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    ret.Add(new Pair<int>(i + ballTilePos.X, j + ballTilePos.Y));
            return ret;
        }
        
        private void updateWorld()
        {
            double elapsedTicks = gameTimer.ElapsedTicks;
            double elapsedMs = elapsedTicks * 1000.0 / ((double)GameApp.TicksPerSecond);
            long thisTime = (long)Math.Round(elapsedMs, 0);

            #region Aggiornamenti dello stato di tutti gli attori
		    ball.Update(thisTime, cmd);
            if (isBonus && !ball.IsStateAlso(BallState.Bonused))
                ball.SetState(BallState.Bonused, true);
            else if (!isBonus)
                ball.SetState(BallState.Bonused, false);

            foreach(GameBlock b in blocks.Values)
                b.Update(thisTime, ball);

            foreach(SortedDictionary<KulaLevel.Orientation, GameSurface> a in surfaces.Values)
                foreach(GameSurface b in a.Values)
                    b.Update(thisTime, ball);

            foreach(GamePlaceable b in placeables.Values)
                b.Update(thisTime, ball);

            foreach(GameEnemy b in enemies.Values)
                b.Update(thisTime, ball);
	        #endregion

            #region Collisione con il resto degli attori
            foreach(Pair<int> tile in collisionControlArea(ball))
            {
                GameBlock gb;
                if (blocks.TryGetValue(tile, out gb))
                    gb.CollidesWithBall(thisTime, ball);
            }

            #region Collisioni con le superfici
            #region Setto a false ogni superficie
            foreach (SurfType st in Enum.GetValues(typeof(SurfType)))
                ball.SetFoundSurface(st, false);
            #endregion
            foreach (Pair<int> tile in collisionControlArea(ball))
            {
                #region Controllo la collisione con le superfici
                SortedDictionary<KulaLevel.Orientation, GameSurface> surfs;
                if (surfaces.TryGetValue(tile, out surfs))
                {
                    foreach (GameSurface gs in surfs.Values)
                        gs.CollidesWithBall(thisTime, ball);
                }
                #endregion
            }
            #region Modifico lo stato "continuo" nella seguente maniera: stato continuo <=> superficie che tocca
            ball.SetState(BallState.Burning, ball.IsFoundSurface(SurfType.Fire));
            ball.SetState(BallState.GroundForced, ball.IsFoundSurface(SurfType.Forced));
            ball.SetState(BallState.Sliding, ball.IsFoundSurface(SurfType.Ice));
            #endregion
            #endregion
            

            foreach(Pair<int> tile in collisionControlArea(ball))
            {
                GamePlaceable gp;
                GameEnemy ge;
                if (placeables.TryGetValue(tile, out gp))
                    gp.CollidesWithBall(thisTime, ball);
                if (enemies.TryGetValue(tile, out ge))
                    ge.CollidesWithBall(thisTime, ball);
            }
            #endregion

            #region Aggiorno di nuovo la palla
            elapsedTicks = gameTimer.ElapsedTicks;
            elapsedMs = elapsedTicks * 1000.0 / ((double)GameApp.TicksPerSecond);
            thisTime = (long)Math.Round(elapsedMs, 0);
            ball.Update(thisTime, cmd); 
            #endregion

            #region Aggiorno lo HUD: il tempo, per esempio
            int before = remainingTime / 1000;
            remainingTime = (int)timeCounter.CalculateValue(thisTime);
            if (before - remainingTime / 1000 >= 1 && remainingTime <= 20000)
                if (((int)remainingTime / 1000) % 2 == 0)
                    playSound("Tick");
                else
                    playSound("Tock");
            #endregion

            #region Controlli della palla che solo il gamescreen può fare (O.O.B., Time's up, EndBonus)
            if (!lvlBounds.Contains(ball.Center))
                levelLoss(DeathType.Fell, "You fell off!");
            else if (remainingTime == 0)
                levelLoss(DeathType.TimeOut, "Time out!");
            else
            {
                List<GameBlock> gbList = blocks.Values.ToList<GameBlock>();
                if (!gbList.Exists(x => !x.IsTouched) && isBonus)
                    levelWin();
            }
            
            #endregion
        }
        #endregion

        #region Costruttori
        public GameScreen(SceneContainer sc)
        {
            initVariables();
            bindToContainer(sc);
            initSounds();
            initImages();
        }
        #endregion

        #region Metodi ereditati dalla superclasse: Dispose, Pause, Play, Reset, Draw, HandleEvent
        public override void Dispose()
        {
            base.Dispose();
        }
        public override void Pause()
        {
            //TOCHECK: metodo pause
            timer.Stop();
            gameTimer.Stop();
            bgMusic.Pause();
        }
        public override void Draw(Graphics e)
        {
            #region Ripristino la console
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
            #endregion

            #region Chiamata al draw della superclasse
            long now = timer.ElapsedTicks;
            base.Draw(e);
            long after = timer.ElapsedTicks;
            long total = 0;
            total += (after - now);
            PrintToConsole("Superclass draw: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
            #endregion

            #region In caso di non prompt
		    if (gsState == GameScreenState.InGame)
            {
                #region Disegno la scena
                now = timer.ElapsedTicks;
                drawScene(e);
                after = timer.ElapsedTicks;
                total += (after - now);
                PrintToConsole("Drawing Actors: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
                #endregion

                #region Disegno lo hud
                now = timer.ElapsedTicks;
                drawHUD(e);
                after = timer.ElapsedTicks;
                total += (after - now);
                PrintToConsole("Drawing HUD: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
                #endregion

                PrintToConsole("All the drawing took: " + GameApp.TicksToMillis(total) + " milliseconds.");
                PrintToConsole("==================================");
                return;
            }
            #endregion
            #region In caso di prompt
            if (gsState == GameScreenState.InPrompt)
            {
                if (needToUpdateFrame)
                {
                    promptFrame = new Bitmap(clipRegion.Width, clipRegion.Height);
                    drawScene(Graphics.FromImage(promptFrame));
                    promptFrame = GameApp.ResizeImg(promptFrame, 180, 120);
                    needToUpdateFrame = false;
                }

                #region Disegno lo sfondo
                now = timer.ElapsedTicks;
                e.DrawImage(promptFrame, clipRegion);
                after = timer.ElapsedTicks;
                total += (after - now);
                PrintToConsole("Drawing Background: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
                #endregion

                #region Disegno le scritte
                now = timer.ElapsedTicks;
                drawPrompt(e); ;
                after = timer.ElapsedTicks;
                total += (after - now);
                PrintToConsole("Drawing Prompt: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
                #endregion

                PrintToConsole("All the drawing took: " + GameApp.TicksToMillis(total) + " milliseconds.");
                PrintToConsole("==================================");
                return;
            }
            #endregion
        }
        public override void Play()
        {
            //TOCHECK: metodo play
            #region Definisco variabili utili
            ResourceDirectory rootDir = container.getResourceDirectory;
            string
                permaDir = GameConstraints.OtherPaths.PermanentLevelsDir,
                cachedDir = GameConstraints.OtherPaths.CachedLevelsDir,
                bonusDir = GameConstraints.OtherPaths.BonusLevelsDir,
                toFind;
            #endregion

            timer.Start();

            #region Caso in cui sono in gioco
		    if (gsState == GameScreenState.InGame)
            {
                gameTimer.Start();
                bgMusic.PlayLooping(container.VolumeMusic);
            }
	        #endregion
            #region Caso in cui devo cercare il livello
		    else if (gsState == GameScreenState.ToSearch)
            {
                #region Primo controllo: se non c'è provo a caricarlo
                if (fruit == 0 || fruit % 5 != 0 || didBonus)
                    toFind = currentLevel;
                else
                    toFind = bonuses.ElementAt<string>(fruit / 5 - 1);
                gsState = GameScreenState.ToBuild;
                if (!rootDir.ContainsFile(permaDir, toFind) && !rootDir.ContainsFile(cachedDir, toFind) && !rootDir.ContainsFile(bonusDir, toFind))
                {
                    string metadataDir = GameConstraints.OtherPaths.MetadataLogicDir;
                    string loaderItem = GameConstraints.OtherPaths.LoaderResourceName;

                    LoadResourceItem lri = (LoadResourceItem)rootDir.GetFile(metadataDir, loaderItem);
                    if (lri == null)
                        throw new Exception("Warning, the loader item is not present, the loader cannot work.");

                    lri.IsBatchLoading = true;
                    lri.IsCaching = true;
                    if (fruit == 0 || fruit % 5 != 0 || didBonus)
                        lri.SetNextNormalLevelToLoad(toFind);
                    else
                        lri.SetNextBonusLevelToLoad(toFind);
                    container.changeScene(GameConstraints.LoadingScreen.ID);
                }
                else 
                    Play();
                #endregion
            }
            #endregion
            #region Caso in cui ho già provato a cercare
            else if (gsState == GameScreenState.ToBuild)
            {
                #region Secondo controllo: se non c'è butto eccezione.
                LevelResourceItem lvl;
                if (fruit == 0 || fruit % 5 != 0 || didBonus)
                    toFind = currentLevel;
                else
                    toFind = bonuses.ElementAt<string>(fruit / 5 - 1);

                if ((lvl = (LevelResourceItem)rootDir.GetFile(permaDir, toFind)) != null)
                    createLevel(lvl.Content);
                else if ((lvl = (LevelResourceItem)rootDir.GetFile(cachedDir, toFind)) != null)
                    createLevel(lvl.Content);
                else if ((lvl = (LevelResourceItem)rootDir.GetFile(bonusDir, toFind)) != null)
                    createLevel(lvl.Content);
                else
                    throw new Exception("Level loading failed, there was something wrong.");
                #endregion
                gsState = GameScreenState.InGame;
                restartLevel();
            }
	        #endregion  
            
        }
        public override void Reset()
        {
            //TODO: reset, serve?
            throw new NotImplementedException();
        }
        public override void Update()
        {
            //TOCHECK: Metodo di update per il gioco
            PrintToConsole("\n==================================");

            long now = timer.ElapsedTicks;
            if (gsState == GameScreenState.InGame)
                updateWorld();
            long after = timer.ElapsedTicks;
            long total = 0;
            total += (after - now);
            PrintToConsole("All the update took: " + GameApp.TicksToMillis(total) + " milliseconds.");
            PrintToConsole("==================================");
        }
        public override void HandleEvent(KeyboardMouseEventBundle bundle)
        {
            KeyboardMouseEventArgs ev;
            cmd = Command.Nothing;
            if ((ev = bundle.extractEvent()) != null)
            {
                if (ev.getEventType == KeyboardMouseEventID.Key_Down)
                {
                    KeyEventArgs e = ev.keyEvent;
                    handleKeyDown(e);
                }
                else if (ev.getEventType == KeyboardMouseEventID.Key_Up)
                {
                    KeyEventArgs e = ev.keyEvent;
                    handleKeyUp(e);
                }
            }
        }
        #endregion

        #region Metodi pubblici chiamati dalla palla, quindi dal gioco
        public void PlaySound(string sound)
        {
            playSound(sound);
        }
        public void Die(DeathType dt)
        {
            if (dt == DeathType.Captured)
                playSound("Captured");
            else if (dt == DeathType.Spiked)
                playSound("Spiked");

            levelLoss(dt, "");
        }
        public void ModScore(long amount)
        {
            roundScore += amount;
        }
        public void AddKey()
        {
            if (--remainingKeys <= 0)
            {
                remainingKeys = 0;
                playSound("AllKeys");
                ball.StartEscaping();
            }
            else
                playSound("Key");
        }
        public void AddFruit()
        {
            gotFruit = true;
            playSound("Fruit");
        }
        public void InvertTime(long thisTime)
        {
            timeCounter = new LinearBoundedAnimator(120000 - remainingTime, thisTime, -1, 120000, 0);
            playSound("Glasshour");
        }
        public void RemoveTime(int amount, long thisTime)
        {
            timeCounter = new LinearBoundedAnimator(Math.Max(remainingTime - 10000, 1000), thisTime, -1, 120000, 0);
        }
        public void EndLevel()
        {
            levelWin();
        }
        /// <summary>
        /// Restituisce 1 se c'è un blocco accanto alla palla, 0 se c'è il blocco sotto la palla, -1 se la palla non ha blocchi accanto
        /// </summary>
        /// <param name="toLeft">Lato da controllare</param>
        /// <returns></returns>
        public int CheckBlocks(bool toLeft)
        {
            float checkOffsetX = 1f;
            float checkOffsetY = 1f;

            int ballX = ((int)ball.Center.X) / ((int)EngineConst.BlockWidth);
            int ballY = ((int)ball.Center.Y) / ((int)EngineConst.BlockWidth);

            if (ballX <= 0 && ball.Center.X < 0) ballX -= 1;
            if (ballY <= 0 && ball.Center.Y < 0) ballY -= 1;

            if (toLeft)
                checkOffsetX *= -1;

            Matrix m = new Matrix();
            m.Rotate(-ball.Rotation);

            //Console.WriteLine("Ball @" + ball.Center);

            PointF offH = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, new PointF(checkOffsetX, 0)));
            PointF offL = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, new PointF(checkOffsetX, checkOffsetY)));

            Pair<int> high = new Pair<int>(ballX + (int)(offH.X), ballY + (int)(offH.Y));
            Pair<int> low = new Pair<int>(ballX + (int)(offL.X), ballY + (int)(offL.Y));
            //Console.WriteLine("HighBlock + " + offH +" @" + highBlock);
            //Console.WriteLine("LowBlock + " + offL + " @" + lowBlock);

            //Pair<int> high = new Pair<int>((int)(highBlock.X / EngineConst.BlockWidth), (int)(highBlock.Y / EngineConst.BlockWidth));
            //Pair<int> low = new Pair<int>((int)(lowBlock.X / EngineConst.BlockWidth), (int)(lowBlock.Y / EngineConst.BlockWidth));

            GameBlock gb;
            if (blocks.TryGetValue(high, out gb) && gb.IsEnabled)
                return 1;
            else if (blocks.TryGetValue(low, out gb) && gb.IsEnabled)
                return 0;
            else
                return -1;
        }

        /// <summary>
        /// Controlla se è presente un singolo blocco posizionato nella griglia rispetto al centro attuale della palla:
        /// viene tenuta di conto pure la rotazione, quindi la griglia viene modificata pure in base a questo parametro
        /// </summary>
        /// <param name="fromBallX">Numero di celle a sinistra del centro della palla</param>
        /// <param name="fromBallY">Numero di celle a destra del centro della palla</param>
        /// <returns></returns>
        public bool CheckBlock(int fromBallX, int fromBallY)
        {
            float checkOffsetX = fromBallX;
            float checkOffsetY = fromBallY;

            int ballX = ((int)ball.Center.X) / ((int)EngineConst.BlockWidth);
            int ballY = ((int)ball.Center.Y) / ((int)EngineConst.BlockWidth);

            if (ballX <= 0 && ball.Center.X < 0) ballX -= 1;
            if (ballY <= 0 && ball.Center.Y < 0) ballY -= 1;

            Matrix m = new Matrix();
            m.Rotate(-ball.Rotation);

            //Console.WriteLine("Ball @" + ball.Center);

            PointF off = MatrixUtils.RoundPoint(MatrixUtils.TransformPointF(m, new PointF(checkOffsetX, checkOffsetY)));
;
            Pair<int> block = new Pair<int>(ballX + (int)(off.X), ballY + (int)(off.Y));

            GameBlock gb;
            return blocks.TryGetValue(block, out gb) && gb.IsEnabled;
        }
        #endregion

        #region Metodi pubblici chiamati dai menù
        public void NewGame()
        {
            NewGame(FileNames.FirstLevelFilename, false);
        }
        //WARNING: e se esistesse un nome di un livello normale uguale ad un bonus?!
        public void NewGame(string lvlName, bool isTryLevel)
        {
            //TOCHECK: new game ricevuto dal menù principale
            #region Imposto le variabili del profilo corrente
            fruit = 0;
            didBonus = true;
            currentScore = 0;
            maxScore = 0;
            IsTryingLevel = isTryLevel;
            #endregion

            #region Controllo se il livello richiesto è un bonus o meno
            if (bonuses.Exists(x => x.Equals(lvlName)))
            {
                didBonus = false;
                fruit = (bonuses.IndexOf(lvlName) + 1) * 5; //Così forzo la scelta del bonus
            }
            else
                currentLevel = lvlName;
            #endregion

            gsState = GameScreenState.ToSearch;
        }
        public void PromptRetry()
        {
            //TOCHECK: retry ricevuto dalla pausa
            levelLoss(DeathType.Retry, "Level restarted.");
        }
        public void PromptQuit()
        {
            //TOCHECK: quit ricevuto dalla pausa
            createPrompt("You quitted", PromptType.End);
        }
        #endregion

        #region Metodi di utilità per la creazione del livello
        private T element<T>(int i, List<T> l)
        {
            return l.ElementAt<T>(i);
        }
        private List<Bitmap> allTheImagesAreLoaded(List<string> imgs)
        {
            int i = 0;
            List<Bitmap> ret = new List<Bitmap>();
            while(i < imgs.Count)
            {
                Bitmap target;
                if (!images.TryGetValue(imgs.ElementAt<string>(i), out target))
                    throw new Exception(imgs.ElementAt<string>(i) + " is not loaded.");
                i++;
                ret.Add(target);
            }
            return ret;
        }
        private GameSurface createSurface(KulaLevel.Surface surface, int idX, int idY)
        {
            Bitmap spikes, ramp;
            if (!images.TryGetValue("Spikes.png", out spikes) || !images.TryGetValue("Ramp.png", out ramp))
                throw new Exception("During creating surface, ramp or spike texture are missing.");
            if (surface.Type == KulaLevel.SurfaceType.Spikes)
                return new G_Spikes(idX, idY, surface.Orientation, spikes);
            else if (surface.Type == KulaLevel.SurfaceType.Ramp)
                return new G_Ramp(idX, idY, surface.Orientation, ramp);
            else if (surface.Type == KulaLevel.SurfaceType.Exit)
            {
                if (hasExit)
                    return null;
                else
                {
                    hasExit = true;
                    return new G_Exit(idX, idY, surface.Orientation);
                }
            }
            else if (surface.Type == KulaLevel.SurfaceType.NoJump)
                return new G_GroundForced(idX, idY, surface.Orientation);
            else if (surface.Type == KulaLevel.SurfaceType.Ice)
                return new G_Ice(idX, idY, surface.Orientation);
            else if (surface.Type == KulaLevel.SurfaceType.Fire)
                return new G_Fire(idX, idY, surface.Orientation);
            else
                return null;
        }
        private void surfaceInsertion(KulaLevel.Block block, int idX, int idY)
        {
            #region Blocco normale
            if (TileConverter.FromByteSpecificType(KulaLevel.TileType.Block, block.Type) == "Normal")
            {
                SortedDictionary<KulaLevel.Orientation, GameSurface> surfs = new SortedDictionary<KulaLevel.Orientation, GameSurface>();
                foreach(KulaLevel.Surface s in block.Surfaces)
                {
                    if (!isBonus || !(s.Type == KulaLevel.SurfaceType.Exit))
                    {
                        GameSurface ss = createSurface(s, idX, idY);
                        if (ss != null)
                            surfs.Add(s.Orientation, ss);
                    }
                }
                surfaces.Add(new Pair<int>(block.X, block.Y), surfs);
            }
            #endregion
            #region Blocco ghiaccio
            else if (TileConverter.FromByteSpecificType(KulaLevel.TileType.Block, block.Type) == "Ice")
            {
                SortedDictionary<KulaLevel.Orientation, GameSurface> surfs = new SortedDictionary<KulaLevel.Orientation, GameSurface>();
                foreach (KulaLevel.Orientation o in Enum.GetValues(typeof(KulaLevel.Orientation)))
                    surfs.Add(o, new G_Ice(idX, idY, o));
                surfaces.Add(new Pair<int>(block.X, block.Y), surfs);
            }
            #endregion
            #region Blocco fuoco
            else if (TileConverter.FromByteSpecificType(KulaLevel.TileType.Block, block.Type) == "Fire")
            {
                SortedDictionary<KulaLevel.Orientation, GameSurface> surfs = new SortedDictionary<KulaLevel.Orientation, GameSurface>();
                foreach (KulaLevel.Orientation o in Enum.GetValues(typeof(KulaLevel.Orientation)))
                    surfs.Add(o, new G_Fire(idX, idY, o));
                surfaces.Add(new Pair<int>(block.X, block.Y), surfs);
            }
            #endregion
        }
        private void blockTreatment(Bitmap blockTx, KulaLevel lvl)
        {
            Bitmap des, fire;
            if (!images.TryGetValue("Fire.png", out fire) || !images.TryGetValue("Cracks.png", out des))
                throw new Exception("In creating level, block overlays weren't found");
            foreach(KulaLevel.Block b in lvl.Blocks)
            {
                float bw = EngineConst.BlockWidth;
                string tipo = TileConverter.FromByteSpecificType(KulaLevel.TileType.Block, b.Type);
                if (compareType(tipo, "Normal"))
                {
                    blocks.Add(new Pair<int>(b.X, b.Y), new NormalBlock(blockTx, b.X, b.Y));
                    surfaceInsertion(b, b.X, b.Y);
                }
                else if (compareType(tipo, "Transparent"))
                    blocks.Add(new Pair<int>(b.X, b.Y), new TransparentBlock(b.X, b.Y));
                else if (compareType(tipo, "Intermittent"))
                    blocks.Add(new Pair<int>(b.X, b.Y), new IntermittentBlock(blockTx, b.X, b.Y, b.DisappearBegin, b.DisappearPeriod));
                else if (compareType(tipo, "Destructible"))
                    blocks.Add(new Pair<int>(b.X, b.Y), new DestructibleBlock(blockTx, des, b.X, b.Y));
                else if (compareType(tipo, "Ice"))
                {
                    blocks.Add(new Pair<int>(b.X, b.Y), new IceBlock(blockTx, b.X, b.Y));
                    surfaceInsertion(b, b.X, b.Y);
                }
                    
                else if (compareType(tipo, "Fire"))
                {
                    blocks.Add(new Pair<int>(b.X, b.Y), new FireBlock(blockTx, fire, b.X, b.Y));
                    surfaceInsertion(b, b.X, b.Y);
                } 
                else
                    throw new Exception("During level creation, a block had an unappropriate blocktype: what is " + tipo + "?");
            }
            //TODO: ricorda di "ripulire le superfici"
        }
        private void placeableTreatment(KulaLevel lvl)
        {
            #region Preparo la lista di immagini da usare
            List<Bitmap> texs = new List<Bitmap>();
            List<string> names = new List<string>();
            
            names.Add("B_Coin.png");
            names.Add("S_Coin.png");
            names.Add("G_Coin.png");
            names.Add("Key.png");
            names.Add("Fruit" + (fruit%5).ToString() + ".png");
            names.Add("Glasshour.png");
            names.Add("G_Changer.png");
            names.Add("Glasses.png");
            names.Add("SlowPill.png");
            names.Add("Sapphire.png");
            names.Add("Ruby.png");
            names.Add("Emerald.png");
            names.Add("Diamond.png");

            if ((texs = allTheImagesAreLoaded(names)) == null)
                throw new Exception("In creating placeables, some textures were not loaded.");
	        #endregion

            hasSpawn = false;

            foreach(KulaLevel.Placeable p in lvl.Placeables)
            {
                float bw = EngineConst.BlockWidth;
                float persp = -RotationUtilities.getAngleFromDownOrientation(p.Orientation);
                if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Bronze Coin")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Bronze(p.X, p.Y, persp, element<Bitmap>(0, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Silver Coin")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Silver(p.X, p.Y, persp, element<Bitmap>(1, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Golden Coin")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Gold(p.X, p.Y, persp, element<Bitmap>(2, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Key")
                {
                    if (!isBonus)
                    {
                        placeables.Add(new Pair<int>(p.X, p.Y), new G_Key(p.X, p.Y, persp, element<Bitmap>(3, texs)));
                        keys++;
                    }
                }
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Fruit")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Fruit(p.X, p.Y, persp, element<Bitmap>(4, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Glasshour")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Glasshour(p.X, p.Y, persp, element<Bitmap>(5, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Gravity Changer")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_GravChanger(p.X, p.Y, persp, element<Bitmap>(6, texs), p.GChangerDirection));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Glasses")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Glasses(p.X, p.Y, persp, element<Bitmap>(7, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Slow Pill")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_SlowPill(p.X, p.Y, persp, element<Bitmap>(8, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Sapphire")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Sapphire(p.X, p.Y, persp, element<Bitmap>(9, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Ruby")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Ruby(p.X, p.Y, persp, element<Bitmap>(10, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Emerald")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Emerald(p.X, p.Y, persp, element<Bitmap>(11, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Diamond")
                    placeables.Add(new Pair<int>(p.X, p.Y), new G_Diamond(p.X, p.Y, persp, element<Bitmap>(12, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Spawn Point")
                {
                    hasSpawn = true;
                    PointF start = new PointF(p.X * bw + bw / 2.0f, p.Y * bw + bw / 2.0f);
                    ball.SetStartingPoint(start.X, start.Y, RotationUtilities.getAngleFromDownOrientation(p.Orientation), 0);
                }
                else
                    Console.WriteLine(TileConverter.FromByteSpecificType(p.TileType, p.Type) + " is a strange type");
                    
                
            }
        }
        private void enemyTreatment(KulaLevel lvl)
        {
            //TODO: nemici.
        }
        private void resetAll(long thisTime)
        {
            #region Reimposta lo stato di tutti gli attori
            ball.ResetState(thisTime);

            foreach (Actor a in blocks.Values)
                a.Reset(thisTime);

            foreach (SortedDictionary<KulaLevel.Orientation, GameSurface> b in surfaces.Values)
                foreach (Actor a in b.Values)
                    a.Reset(thisTime);

            foreach (Actor a in enemies.Values)
                a.Reset(thisTime);

            foreach (Actor a in placeables.Values)
                a.Reset(thisTime);
            #endregion

            #region Resetto lo stato del livello corrente
            roundScore = 0;
            remainingKeys = keys;
            remainingTime = roundTime;
            gotFruit = false;
            timeCounter = new LinearBoundedAnimator(remainingTime, thisTime, -1, 120000, 0);
            #endregion
        }
        #endregion

        #region Metodi privati per l'interazione con il livello
        private void updateProfile(bool win)
        {
            if (!IsTryingLevel)
            {
                currentScore += roundScore;

                if (currentScore > maxScore)
                    maxScore = currentScore;

                if (win && gotFruit && !isBonus)
                    fruit++;

                didBonus = isBonus || !gotFruit;
            }
        }
        private bool compareType(string type1, string type2)
        {
            return String.Equals(type1, type2, StringComparison.InvariantCultureIgnoreCase);
        }
        private void createLevel(KulaLevel lvl)
        {
            #region Reimposto lo stato del gamescreen
            keys = 0;
            hasExit = false;
            gotFruit = false;
            gameTimer.Reset();
            #endregion

            #region Ripulisco la mappa
            blocks.Clear();
            enemies.Clear();
            surfaces.Clear();
            placeables.Clear();
            GC.Collect();
            #endregion

            ResourceDirectory dir = container.getResourceDirectory;

            #region Controllo di validità dello stato: tema caricato e livello non vuoto
            if (lvl == null)
                throw new ArgumentNullException("In CreateLevel(...) the level is null");
            if (!dir.ContainsDirectory(lvl.Theme))
                throw new Exception("In CreateLevel(...) the level theme was not loaded before!");
            #endregion

            #region Carico il "tema" e modifico l'aspetto (e imposto se il livello è un bonus o meno)
            string theme = lvl.Theme;
            string myDir = GameConstraints.GameScreen.LogicDir;
            SoundResourceItem bg = (SoundResourceItem)dir.GetFile(theme, "BackgroundMusic.mp3");
            ImageResourceItem block = (ImageResourceItem)dir.GetFile(theme, "Block.bmp");
            ImageResourceItem boll = (ImageResourceItem)dir.GetFile(theme, "Ball.png");
            ImageResourceItem bgImg = (ImageResourceItem)dir.GetFile(theme, "Background.bmp");
            ImageResourceItem lightmap = (ImageResourceItem)dir.GetFile(myDir, "LightBall.png");

            bgImage = GameApp.ResizeImg(bgImg.Content, 500, 500);
            bgMusic.Stop();
            bgMusic = bg.Content;
            ball.SetTexture(boll.Content);
            ball.SetLightMap(lightmap.Content);
            isBonus = lvl.IsBonus;
            #endregion

            #region Inserisco gli attori e resetto il loro stato
            blockTreatment(block.Content, lvl);
            placeableTreatment(lvl);
            enemyTreatment(lvl);
            resetAll(0);
            ball.SetState(BallState.Bonused, isBonus);
            #endregion

            #region Modifico le proprietà della partita: chiavi da raccogliere, tempo rimasto
            roundTime = (int)lvl.StartingSeconds * 1000;
            remainingTime = (int)lvl.StartingSeconds * 1000;
            roundPenalty = lvl.LossPenalty;
            lvlBounds = new RectangleF(-320, -320, (lvl.Width + 10) * EngineConst.BlockWidth, (lvl.Height + 10) * EngineConst.BlockWidth);
            remainingKeys = keys;
            timeCounter = new LinearBoundedAnimator(remainingTime, 0, -1, 120 * 1000, 0);
            if (!isBonus)
            {
                if (lvl.HasNext)
                    nextLevel = lvl.NextLevel;
                else
                    nextLevel = null;
            }
            if (!hasSpawn || (!isBonus && !hasExit))
                throw new Exception("Bad level design: there isn't any spawn point or any exit.");
            #endregion 
        }
        private void restartLevel()
        {
            gameTimer.Reset();
            resetAll(0);
            gameTimer.Start();
            playSound("Begin");
            bgMusic.PlayLooping(container.VolumeMusic);
            ball.StartFalling(0);
            gsState = GameScreenState.InGame;
        }
        private void levelLoss(DeathType dt, string prompt)
        {
            gameTimer.Stop();
            roundScore = -Math.Abs(roundScore) - roundPenalty;
            if (isBonus)
                roundScore = 0;
            //TOCHECK: livello perso, preparo il prompt
            #region Caso in cui non posso più giocare (quando il punteggio è negativo)
            if (currentScore - Math.Abs(roundScore) - roundPenalty < 0 && !IsTryingLevel)
            {
                //Hai proprio finito
                createPrompt("Game Over", PromptType.End);
                updateProfile(false);
            }
            #endregion
            #region Caso in cui posso riprovare a fare il livello (se è un bonus e non sono a fare una prova, proseguo ugualmente)
            else
            {
                PromptType pt = PromptType.GoNext;
                if (isBonus && !IsTryingLevel)
                    pt = PromptType.GoNext;
                else
                    pt = PromptType.Retry;

                if (dt == DeathType.Captured)
                    createPrompt("You have been captured!", pt);
                else if (dt == DeathType.Fell)
                    createPrompt("You fell out!", pt);
                else if (dt == DeathType.Fire)
                    createPrompt("You burnt!", pt);
                else if (dt == DeathType.Retry)
                    createPrompt("You wanted to retry the level.", pt);
                else if (dt == DeathType.Spiked)
                    createPrompt("You have been spiked!", pt);
                else if (dt == DeathType.TimeOut)
                    createPrompt("Time out!", pt);

                updateProfile(false);
            }
            #endregion
        }
        private void levelWin()
        {
            //TOCHECK: livello vinto, preparo il prompt
            gameTimer.Stop();
            if ((nextLevel == null || nextLevel == "" || bonuses.Count <= (fruit / 5)) && !IsTryingLevel)
            {
                createPrompt("Game ended!", PromptType.End);
                updateProfile(true);
            }
            else
            {
                PromptType pt = PromptType.GoNext;

                if (!IsTryingLevel)
                    pt = PromptType.GoNext;
                else
                    pt = PromptType.Retry;

                createPrompt("Well done!", pt);
                updateProfile(!IsTryingLevel);
            }
        }
        #endregion

        #region Metodi privati per la creazione dei prompt
        public void createPrompt(string msg, PromptType pt)
        {
            promptMsg = msg;
            promptType = pt;
            needToUpdateFrame = true;
            gsState = GameScreenState.InPrompt;
            lastMoment = timer.ElapsedMilliseconds;
        }
        #endregion

        #region Metodi privati per il cambio di stato del gamescreen
        private void toNextLevel()
        {
            //TOCHECK: prossimo livello
            currentLevel = nextLevel;
            gsState = GameScreenState.ToSearch;
            Play();
        }
        private void quitGame()
        {
            //TODO: esci dal gioco, salva il punteggio massimo
            #region Ripulisci la mappa
            blocks.Clear();
            enemies.Clear();
            surfaces.Clear();
            placeables.Clear();
            GC.Collect();
	        #endregion

            Thread t = new Thread(saveScores);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            container.changeScene(GameConstraints.GameMainMenu.ID);
        }
        private void retryLevel()
        {
            //TOCHECK: rifai il livello
            restartLevel();
            gsState = GameScreenState.InGame;
            Play();
        }
        private void saveScores()
        {
            if (!IsTryingLevel && maxScore > 0)
            {
                string input = "My name";
                if (Interaction.InputBox("Enter your name", "Press OK if you want to save the score (" + maxScore +"pts)", ref input) == DialogResult.OK)
                    if (input != null || !input.Trim().Equals(""))
                        if (tabellone.AddRecord(input.Trim(), maxScore))
                            MessageBox.Show("Well done, " + input + ". Your score was good enough to be saved.");
                        else
                            MessageBox.Show("Your score was not good enough to be saved, sorry.");
                tabellone.SaveHighscores();
            }
        }
        #endregion

        #region Metodi privati per i suoni: si può indicare o meno l'estensione wav
        private void playSound(string name)
        {
            
            if (!name.EndsWith(".wav"))
                if (name.Contains("."))
                    throw new Exception("The sound is not in wav format!");
                else
                    name += ".wav";
            SoundMediaPlayer smp;
            if (!sounds.TryGetValue(name, out smp))
                throw new Exception(name + " sound was not found!");
            smp.Play(container.VolumeFX);
        }
        #endregion
    }
}

