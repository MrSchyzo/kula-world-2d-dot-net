using GameEngine.Animators;
using GameEngine.Blocks;
using GameEngine.Enumerations;
using GameEngine.Placeables;
using GameEngine.Placeables.Valuables;
using GameEngine.Surfaces;
using GameEngine.Utils;
using GameMetadata;
using GameUtils;
using LevelsStructure;
using MultimediaClasses;
using ResourceItems;
using ResourcesBasics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UIEssentials;

namespace GameEngine
{
    /* Marco (4y after) fainted */
    public class GameScreen : IIndipendentlyAnimable
    {
        private SortedDictionary<Pair<int>, Block> blocks;
        private SortedDictionary<Pair<int>, SortedDictionary<KulaLevel.Orientation, Surface>> surfaces;
        private SortedDictionary<Pair<int>, Placeable> placeables;
        private SortedDictionary<Pair<int>, GameEnemy> enemies;
        private Ball ball;
        private RectangleF lvlBounds;
        private bool hasExit = false;
        private bool hasSpawn = false;
        private bool isBonus = false;
        private int keys;
        public bool IsTryingLevel { get; set; }
        private long currentScore, maxScore;
        private int fruit;
        private string currentLevel;
        private string nextLevel;
        private List<string> bonuses = new List<string>();
        private bool didBonus = true;
        private SortedDictionary<string, Bitmap> images;
        private SortedDictionary<string, SoundMediaPlayer> sounds;
        private int remainingTime;
        private int roundTime;
        private long roundScore;
        private long roundPenalty;
        private bool gotFruit = false;
        private int remainingKeys;
        private Animator timeCounter;
        private Command cmd = Command.Nothing;
        private GameScreenState gsState = GameScreenState.ToSearch;
        private bool inDebug = false;
        private long lastMoment = 0;
        private Highscores tabellone;
        private string promptMsg = "";
        private PromptType promptType = PromptType.Retry;
        private Stopwatch gameTimer;
        private SoundMediaPlayer bgMusic;
        private Bitmap bgImage;
        private Bitmap promptFrame = new Bitmap(1, 1);
        private bool needToUpdateFrame = true;
        private void handleKeyDown(KeyEventArgs e)
        {
            if (gsState != GameScreenState.InGame) return;

            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                cmd = Command.Left;
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                cmd = Command.Right;
            else if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up || e.KeyCode == Keys.Space)
                cmd = Command.Jump;
            else if (e.KeyCode == Keys.Q)
                cmd = Command.JumpLeft;
            else if (e.KeyCode == Keys.E)
                cmd = Command.JumpRight;
            else if (e.KeyCode == Keys.Escape && !ball.IsStateAlso(BallState.ChangingFace)) //Pausing and unpausing during rotation breaks stuff, I'll find out why
                container.changeScene(GameConstraints.PauseMenu.ID);
            else if (e.KeyCode == Keys.F1)
                inDebug = !inDebug;
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
        private void initSounds()
        {
            ResourceDirectory dir = container.getResourceDirectory;
            foreach (string file in dir.GetDirectoryItemNames(GameConstraints.GameScreen.LogicDir))
            {
                if (file.EndsWith(".wav"))
                {
                    SoundResourceItem ri = (SoundResourceItem)dir.GetFile(GameConstraints.GameScreen.LogicDir, file);
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
            blocks = new SortedDictionary<Pair<int>, Block>();
            surfaces = new SortedDictionary<Pair<int>, SortedDictionary<KulaLevel.Orientation, Surface>>();
            placeables = new SortedDictionary<Pair<int>, Placeable>();
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

            e.DrawLine(new Pen(Color.Black, 2), center.X, center.Y, center.X + (float)Math.Sin(time) * length, center.Y - (float)Math.Cos(time) * length);
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
            for (int i = 0; i < n; i++)
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
            int k = blocks.Values.ToList().Count(x => !x.IsTouched);
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
            Rectangle box = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.5f, 1f / 12f, 0.5f, 1f / 6f, true);
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
        private void drawWorld(Graphics e, bool enhanced, float tilesRange)
        {
            if (tilesRange <= 0)
                tilesRange = 1;
            tilesRange += 1;

            float maxDis = tilesRange * Constants.BlockWidth;

            foreach (SortedDictionary<KulaLevel.Orientation, Surface> a in surfaces.Values)
            {
                foreach (Surface gs in a.Values)
                {
                    if (gs != null)
                    {
                        if (!enhanced || MiscUtilities.Distance(gs.Center, ball.Center) <= maxDis)
                            gs.Draw(e, ball);
                    }
                }
            }
            foreach (Block b in blocks.Values)
            {
                if (b != null)
                    if (!enhanced || MiscUtilities.Distance(b.Center, ball.Center) <= maxDis)
                        b.Draw(e, ball);
            }
            foreach (Placeable p in placeables.Values)
            {
                if (p != null)
                    if (!enhanced || MiscUtilities.Distance(p.Center, ball.Center) <= maxDis)
                        p.Draw(e, ball);
            }
            foreach (GameEnemy en in enemies.Values)
            {
                if (en != null)
                    if (!enhanced)// or MiscUtilities.Distance(en.Center, ball.Center) <= maxDis)
                        en.Draw(e, ball);
            }
        }

        private void drawScene(Graphics e)
        {
            float
                vpWidth = this.clipRegion.Width,
                vpHeight = this.clipRegion.Height,
                worldvpWidth = Constants.ViewportXTiles * Constants.BlockWidth,
                worldvpHeight = Constants.ViewportYTiles * Constants.BlockWidth,
                ratioX = (vpWidth / worldvpWidth),
                ratioY = (vpHeight / worldvpHeight),
                imageTilesDimension = (float)Math.Sqrt(Constants.ViewportXTiles * Constants.ViewportXTiles + Constants.ViewportYTiles * Constants.ViewportYTiles),
                imageTileX = -(imageTilesDimension - Constants.ViewportXTiles) / 2.0f,
                imageTileY = -(imageTilesDimension - Constants.ViewportYTiles) / 2.0f,
                worldCenterX = worldvpWidth * Constants.ViewportBallXRatio,
                worldCenterY = worldvpHeight * Constants.ViewportBallYRatio,
                offX = ball.Center.X - worldCenterX,
                offY = ball.Center.Y - worldCenterY;
            Matrix m = new Matrix();
            m.Scale(ratioX, ratioY);
            m.RotateAt(ball.Rotation, new PointF(worldvpWidth / 2f, worldvpHeight / 2f));
            e.Transform = m;
            e.DrawImage(
                bgImage,
                new RectangleF(
                    imageTileX * Constants.BlockWidth,
                    imageTileY * Constants.BlockWidth,
                    imageTilesDimension * Constants.BlockWidth,
                    imageTilesDimension * Constants.BlockWidth
                    )
                    );
            m.Reset();
            m.Scale(ratioX, ratioY);
            m.RotateAt(ball.Rotation, new PointF(worldCenterX, worldCenterY));
            m.Translate(-offX, -offY);
            e.Transform = m;
            drawWorld(e, true, imageTilesDimension);
            m.Reset();
            m.Scale(ratioX, ratioY);
            e.Transform = m;
            ball.Draw(e);
            m.Reset();
            e.Transform = m;
        }

        private void drawPrompt(Graphics e)
        {
            Rectangle msgBounds = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.5f, 1f / 10f, 0.95f, 1f / 5f, true);
            TextFormatFlags tff = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis;

            bool inputLatency = timer.ElapsedMilliseconds - lastMoment >= 1500;

            GameApp.DrawPromptTextInBox(e, promptMsg, msgBounds, StringAlignment.Center, tff);
            msgBounds = GameApp.zoomedAndSizedRectangleOf(clipRegion, 0.5f, 1f / 2f, 0.75f, 1f / 3f, true);
            string scoreboard = "";
            scoreboard += "Current Score: " + (currentScore - roundScore) + "\n";
            scoreboard += "+ Level Score: " + roundScore + "\n";
            scoreboard += "= New Total: " + (currentScore);
            GameApp.DrawPromptTextInBox(e, scoreboard, msgBounds, StringAlignment.Near, tff);
            if (inputLatency)
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
        }
        private List<Pair<int>> collisionControlArea(Ball b)
        {
            Point ballTilePos = new Point((int)(b.Center.X / Constants.BlockWidth), (int)(b.Center.Y / Constants.BlockWidth));
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

            ball.Update(thisTime, cmd);
            if (isBonus && !ball.IsStateAlso(BallState.Bonused))
                ball.SetState(BallState.Bonused, true);
            else if (!isBonus)
                ball.SetState(BallState.Bonused, false);

            foreach (Block b in blocks.Values)
                b.Update(thisTime, ball);

            foreach (SortedDictionary<KulaLevel.Orientation, Surface> a in surfaces.Values)
                foreach (Surface b in a.Values)
                    b.Update(thisTime, ball);

            foreach (Placeable b in placeables.Values)
                b.Update(thisTime, ball);

            foreach (GameEnemy b in enemies.Values)
                b.Update(thisTime, ball);
            foreach (Pair<int> tile in collisionControlArea(ball))
            {
                Block gb;
                if (blocks.TryGetValue(tile, out gb))
                    gb.CollidesWithBall(thisTime, ball);
            }

            foreach (SurfType st in Enum.GetValues(typeof(SurfType)))
                ball.SetFoundSurface(st, false);
            foreach (Pair<int> tile in collisionControlArea(ball))
            {
                SortedDictionary<KulaLevel.Orientation, Surface> surfs;
                if (surfaces.TryGetValue(tile, out surfs))
                {
                    foreach (Surface gs in surfs.Values)
                        gs.CollidesWithBall(thisTime, ball);
                }
            }
            ball.SetState(BallState.Burning, ball.IsFoundSurface(SurfType.Fire));
            ball.SetState(BallState.GroundForced, ball.IsFoundSurface(SurfType.Forced));
            ball.SetState(BallState.Sliding, ball.IsFoundSurface(SurfType.Ice));
            foreach (Pair<int> tile in collisionControlArea(ball))
            {
                Placeable gp;
                GameEnemy ge;
                if (placeables.TryGetValue(tile, out gp))
                    gp.CollidesWithBall(thisTime, ball);
                if (enemies.TryGetValue(tile, out ge))
                    ge.CollidesWithBall(thisTime, ball);
            }
            elapsedTicks = gameTimer.ElapsedTicks;
            elapsedMs = elapsedTicks * 1000.0 / ((double)GameApp.TicksPerSecond);
            thisTime = (long)Math.Round(elapsedMs, 0);
            ball.Update(thisTime, cmd);
            int before = remainingTime / 1000;
            remainingTime = (int)timeCounter.CalculateValue(thisTime);
            if (before - remainingTime / 1000 >= 1 && remainingTime <= 20000)
                if (((int)remainingTime / 1000) % 2 == 0)
                    playSound("Tick");
                else
                    playSound("Tock");
            if (!lvlBounds.Contains(ball.Center))
                levelLoss(DeathType.Fell, "You fell off!");
            else if (remainingTime == 0)
                levelLoss(DeathType.TimeOut, "Time out!");
            else
            {
                List<Block> gbList = blocks.Values.ToList<Block>();
                if (!gbList.Exists(x => !x.IsTouched) && isBonus)
                    levelWin();
            }

        }
        public GameScreen(SceneContainer sc)
        {
            initVariables();
            bindToContainer(sc);
            initSounds();
            initImages();
        }
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
            if (gsState == GameScreenState.InGame)
            {
                now = timer.ElapsedTicks;
                drawScene(e);
                after = timer.ElapsedTicks;
                total += (after - now);
                PrintToConsole("Drawing Actors: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
                now = timer.ElapsedTicks;
                drawHUD(e);
                after = timer.ElapsedTicks;
                total += (after - now);
                PrintToConsole("Drawing HUD: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
                PrintToConsole("All the drawing took: " + GameApp.TicksToMillis(total) + " milliseconds.");
                PrintToConsole("==================================");
                return;
            }
            if (gsState == GameScreenState.InPrompt)
            {
                if (needToUpdateFrame)
                {
                    promptFrame = new Bitmap(clipRegion.Width, clipRegion.Height);
                    drawScene(Graphics.FromImage(promptFrame));
                    promptFrame = GameApp.ResizeImg(promptFrame, 180, 120);
                    needToUpdateFrame = false;
                }

                now = timer.ElapsedTicks;
                e.DrawImage(promptFrame, clipRegion);
                after = timer.ElapsedTicks;
                total += (after - now);
                PrintToConsole("Drawing Background: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
                now = timer.ElapsedTicks;
                drawPrompt(e);
                after = timer.ElapsedTicks;
                total += (after - now);
                PrintToConsole("Drawing Prompt: " + GameApp.TicksToMillis(after - now) + " milliseconds,\n");
                PrintToConsole("All the drawing took: " + GameApp.TicksToMillis(total) + " milliseconds.");
                PrintToConsole("==================================");
                return;
            }
        }
        public override void Play()
        {
            //TOCHECK: metodo play
            ResourceDirectory rootDir = container.getResourceDirectory;
            string
                permaDir = GameConstraints.OtherPaths.PermanentLevelsDir,
                cachedDir = GameConstraints.OtherPaths.CachedLevelsDir,
                bonusDir = GameConstraints.OtherPaths.BonusLevelsDir,
                toFind;
            timer.Start();

            if (gsState == GameScreenState.InGame)
            {
                gameTimer.Start();
                bgMusic.PlayLooping(container.VolumeMusic);
            }
            else if (gsState == GameScreenState.ToSearch)
            {
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
            }
            else if (gsState == GameScreenState.ToBuild)
            {
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
                gsState = GameScreenState.InGame;
                restartLevel();
            }
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

            int ballX = ((int)ball.Center.X) / ((int)Constants.BlockWidth);
            int ballY = ((int)ball.Center.Y) / ((int)Constants.BlockWidth);

            if (ballX <= 0 && ball.Center.X < 0) ballX -= 1;
            if (ballY <= 0 && ball.Center.Y < 0) ballY -= 1;

            if (toLeft)
                checkOffsetX *= -1;

            Matrix m = new Matrix();
            m.Rotate(-ball.Rotation);

            //Console.WriteLine("Ball @" + ball.Center);

            PointF offH = m.TransformAndThenRound(new PointF(checkOffsetX, 0));
            PointF offL = m.TransformAndThenRound(new PointF(checkOffsetX, checkOffsetY));

            Pair<int> high = new Pair<int>(ballX + (int)(offH.X), ballY + (int)(offH.Y));
            Pair<int> low = new Pair<int>(ballX + (int)(offL.X), ballY + (int)(offL.Y));
            //Console.WriteLine("HighBlock + " + offH +" @" + highBlock);
            //Console.WriteLine("LowBlock + " + offL + " @" + lowBlock);

            //Pair<int> high = new Pair<int>((int)(highBlock.X / Constants.BlockWidth), (int)(highBlock.Y / Constants.BlockWidth));
            //Pair<int> low = new Pair<int>((int)(lowBlock.X / Constants.BlockWidth), (int)(lowBlock.Y / Constants.BlockWidth));

            Block gb;
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

            int ballX = ((int)ball.Center.X) / ((int)Constants.BlockWidth);
            int ballY = ((int)ball.Center.Y) / ((int)Constants.BlockWidth);

            if (ballX <= 0 && ball.Center.X < 0) ballX -= 1;
            if (ballY <= 0 && ball.Center.Y < 0) ballY -= 1;

            Matrix m = new Matrix();
            m.Rotate(-ball.Rotation);

            //Console.WriteLine("Ball @" + ball.Center);

            PointF off = m.TransformAndThenRound(new PointF(checkOffsetX, checkOffsetY));
            ;
            Pair<int> block = new Pair<int>(ballX + (int)(off.X), ballY + (int)(off.Y));

            Block gb;
            return blocks.TryGetValue(block, out gb) && gb.IsEnabled;
        }
        public void NewGame()
        {
            NewGame(FileNames.FirstLevelFilename, false);
        }
        //WARNING: e se esistesse un nome di un livello normale uguale ad un bonus?!
        public void NewGame(string lvlName, bool isTryLevel)
        {
            //TOCHECK: new game ricevuto dal menù principale
            fruit = 0;
            didBonus = true;
            currentScore = 0;
            maxScore = 0;
            IsTryingLevel = isTryLevel;
            if (bonuses.Exists(x => x.Equals(lvlName)))
            {
                didBonus = false;
                fruit = (bonuses.IndexOf(lvlName) + 1) * 5; //Così forzo la scelta del bonus
            }
            else
                currentLevel = lvlName;
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
        private T element<T>(int i, List<T> l)
        {
            return l.ElementAt(i);
        }
        private List<Bitmap> allTheImagesAreLoaded(List<string> imgs)
        {
            int i = 0;
            List<Bitmap> ret = new List<Bitmap>();
            while (i < imgs.Count)
            {
                Bitmap target;
                if (!images.TryGetValue(imgs.ElementAt<string>(i), out target))
                    throw new Exception(imgs.ElementAt<string>(i) + " is not loaded.");
                i++;
                ret.Add(target);
            }
            return ret;
        }
        private Surface createSurface(KulaLevel.Surface surface, int idX, int idY)
        {
            Bitmap spikes, ramp;
            if (!images.TryGetValue("Spikes.png", out spikes) || !images.TryGetValue("Ramp.png", out ramp))
                throw new Exception("During creating surface, ramp or spike texture are missing.");
            if (surface.Type == KulaLevel.SurfaceType.Spikes)
                return new Spikes(idX, idY, surface.Orientation, spikes);
            else if (surface.Type == KulaLevel.SurfaceType.Ramp)
                return new Ramp(idX, idY, surface.Orientation, ramp);
            else if (surface.Type == KulaLevel.SurfaceType.Exit)
            {
                if (hasExit)
                    return null;
                else
                {
                    hasExit = true;
                    return new Exit(idX, idY, surface.Orientation);
                }
            }
            else if (surface.Type == KulaLevel.SurfaceType.NoJump)
                return new GroundForced(idX, idY, surface.Orientation);
            else if (surface.Type == KulaLevel.SurfaceType.Ice)
                return new Ice(idX, idY, surface.Orientation);
            else if (surface.Type == KulaLevel.SurfaceType.Fire)
                return new Fire(idX, idY, surface.Orientation);
            else
                return null;
        }
        private void surfaceInsertion(KulaLevel.Block block, int idX, int idY)
        {
            if (TileConverter.FromByteSpecificType(KulaLevel.TileType.Block, block.Type) == "Normal")
            {
                SortedDictionary<KulaLevel.Orientation, Surface> surfs = new SortedDictionary<KulaLevel.Orientation, Surface>();
                foreach (KulaLevel.Surface s in block.Surfaces)
                {
                    if (!isBonus || !(s.Type == KulaLevel.SurfaceType.Exit))
                    {
                        Surface ss = createSurface(s, idX, idY);
                        if (ss != null)
                            surfs.Add(s.Orientation, ss);
                    }
                }
                surfaces.Add(new Pair<int>(block.X, block.Y), surfs);
            }
            else if (TileConverter.FromByteSpecificType(KulaLevel.TileType.Block, block.Type) == "Ice")
            {
                SortedDictionary<KulaLevel.Orientation, Surface> surfs = new SortedDictionary<KulaLevel.Orientation, Surface>();
                foreach (KulaLevel.Orientation o in Enum.GetValues(typeof(KulaLevel.Orientation)))
                    surfs.Add(o, new Ice(idX, idY, o));
                surfaces.Add(new Pair<int>(block.X, block.Y), surfs);
            }
            else if (TileConverter.FromByteSpecificType(KulaLevel.TileType.Block, block.Type) == "Fire")
            {
                SortedDictionary<KulaLevel.Orientation, Surface> surfs = new SortedDictionary<KulaLevel.Orientation, Surface>();
                foreach (KulaLevel.Orientation o in Enum.GetValues(typeof(KulaLevel.Orientation)))
                    surfs.Add(o, new Fire(idX, idY, o));
                surfaces.Add(new Pair<int>(block.X, block.Y), surfs);
            }
        }
        private void blockTreatment(Bitmap blockTx, KulaLevel lvl)
        {
            Bitmap des, fire;
            if (!images.TryGetValue("Fire.png", out fire) || !images.TryGetValue("Cracks.png", out des))
                throw new Exception("In creating level, block overlays weren't found");
            foreach (KulaLevel.Block b in lvl.Blocks)
            {
                float bw = Constants.BlockWidth;
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
            List<Bitmap> texs = new List<Bitmap>();
            List<string> names = new List<string>();

            names.Add("B_Coin.png");
            names.Add("S_Coin.png");
            names.Add("G_Coin.png");
            names.Add("Key.png");
            names.Add("Fruit" + (fruit % 5).ToString() + ".png");
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
            hasSpawn = false;

            foreach (KulaLevel.Placeable p in lvl.Placeables)
            {
                float bw = Constants.BlockWidth;
                float persp = -RotationUtilities.getAngleFromDownOrientation(p.Orientation);
                if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Bronze Coin")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Bronze(p.X, p.Y, persp, element<Bitmap>(0, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Silver Coin")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Silver(p.X, p.Y, persp, element<Bitmap>(1, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Golden Coin")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Gold(p.X, p.Y, persp, element<Bitmap>(2, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Key")
                {
                    if (!isBonus)
                    {
                        placeables.Add(new Pair<int>(p.X, p.Y), new Key(p.X, p.Y, persp, element<Bitmap>(3, texs)));
                        keys++;
                    }
                }
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Fruit")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Fruit(p.X, p.Y, persp, element<Bitmap>(4, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Glasshour")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Glasshour(p.X, p.Y, persp, element<Bitmap>(5, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Gravity Changer")
                    placeables.Add(new Pair<int>(p.X, p.Y), new GravityChanger(p.X, p.Y, persp, element<Bitmap>(6, texs), p.GChangerDirection));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Glasses")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Glasses(p.X, p.Y, persp, element<Bitmap>(7, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Slow Pill")
                    placeables.Add(new Pair<int>(p.X, p.Y), new SlowPill(p.X, p.Y, persp, element<Bitmap>(8, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Sapphire")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Sapphire(p.X, p.Y, persp, element<Bitmap>(9, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Ruby")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Ruby(p.X, p.Y, persp, element<Bitmap>(10, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Emerald")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Emerald(p.X, p.Y, persp, element<Bitmap>(11, texs)));
                else if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Diamond")
                    placeables.Add(new Pair<int>(p.X, p.Y), new Diamond(p.X, p.Y, persp, element<Bitmap>(12, texs)));
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
            ball.ResetState(thisTime);

            foreach (Actor a in blocks.Values)
                a.Reset(thisTime);

            foreach (SortedDictionary<KulaLevel.Orientation, Surface> b in surfaces.Values)
                foreach (Actor a in b.Values)
                    a.Reset(thisTime);

            foreach (Actor a in enemies.Values)
                a.Reset(thisTime);

            foreach (Actor a in placeables.Values)
                a.Reset(thisTime);
            roundScore = 0;
            remainingKeys = keys;
            remainingTime = roundTime;
            gotFruit = false;
            timeCounter = new LinearBoundedAnimator(remainingTime, thisTime, -1, 120000, 0);
        }
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
            keys = 0;
            hasExit = false;
            gotFruit = false;
            gameTimer.Reset();
            blocks.Clear();
            enemies.Clear();
            surfaces.Clear();
            placeables.Clear();
            GC.Collect();
            ResourceDirectory dir = container.getResourceDirectory;

            if (lvl == null)
                throw new ArgumentNullException("In CreateLevel(...) the level is null");
            if (!dir.ContainsDirectory(lvl.Theme))
                throw new Exception("In CreateLevel(...) the level theme was not loaded before!");
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
            blockTreatment(block.Content, lvl);
            placeableTreatment(lvl);
            enemyTreatment(lvl);
            resetAll(0);
            ball.SetState(BallState.Bonused, isBonus);
            roundTime = (int)lvl.StartingSeconds * 1000;
            remainingTime = (int)lvl.StartingSeconds * 1000;
            roundPenalty = lvl.LossPenalty;
            lvlBounds = new RectangleF(-320, -320, (lvl.Width + 10) * Constants.BlockWidth, (lvl.Height + 10) * Constants.BlockWidth);
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
            if (currentScore - Math.Abs(roundScore) - roundPenalty < 0 && !IsTryingLevel)
            {
                //Hai proprio finito
                createPrompt("Game Over", PromptType.End);
                updateProfile(false);
            }
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
        public void createPrompt(string msg, PromptType pt)
        {
            promptMsg = msg;
            promptType = pt;
            needToUpdateFrame = true;
            gsState = GameScreenState.InPrompt;
            lastMoment = timer.ElapsedMilliseconds;
        }
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
            blocks.Clear();
            enemies.Clear();
            surfaces.Clear();
            placeables.Clear();
            GC.Collect();
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
                if (Interaction.InputBox("Enter your name", "Press OK if you want to save the score (" + maxScore + "pts)", ref input) == DialogResult.OK)
                    if (input != null || !input.Trim().Equals(""))
                        if (tabellone.AddRecord(input.Trim(), maxScore))
                            MessageBox.Show("Well done, " + input + ". Your score was good enough to be saved.");
                        else
                            MessageBox.Show("Your score was not good enough to be saved, sorry.");
                tabellone.SaveHighscores();
            }
        }
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
    }
}

