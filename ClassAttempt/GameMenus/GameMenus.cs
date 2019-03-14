﻿using GameEngine;
using GameLoadingScreens;
using GameMetadata;
using GameUtils;
using MultimediaClasses;
using ResourcesBasics;
using ResourceItems;
using System;
using System.Collections.Generic;
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

namespace GameMenus
{
    #region GameMainMenu: Classe che rappresenta il menù principale del gioco
    /// <summary>
    /// Classe che rappresenta il menù principale del gioco. Sarà questa classe a caricare automaticamente molte risorse multimediali delle
    /// altre scene.
    /// </summary>
    public class GameMainMenu : GameMenu
    {
        #region Variabile privata
        private static string tryFileName = null;
        private Highscores tabellone;
        #endregion
        #region Metodi privati per l'inizializzazione del menù
        private UIEssentials.MenuItem Initialize1P()
        {
            UIEssentials.MenuItem nuovo = new UIEssentials.MenuItem("1 Player", false, null, null);

            UIEssentials.MenuItem ng = new UIEssentials.MenuItem(nuovo, "New Game", false, null, null);
            //UIEssentials.MenuItem lg = new UIEssentials.MenuItem(nuovo, "Load Game", false, null, null);
            UIEssentials.MenuItem tl = new UIEssentials.MenuItem(nuovo, "Try Level", false, null, null);
            UIEssentials.MenuItem back = new UIEssentials.MenuItem(nuovo, "Back", false, null, null);

            nuovo.appendChild(ng);
            //nuovo.appendChild(lg);
            nuovo.appendChild(tl);
            nuovo.appendChild(back);

            return nuovo;
        }

        private UIEssentials.MenuItem InitializeOPT()
        {
            UIEssentials.MenuItem nuovo = new UIEssentials.MenuItem("Options", false, null, null);
            nuovo.isEditable = false;


            IntSlider fx = new IntSlider(0, 100, 10);
            UIEssentials.MenuItem ng = new UIEssentials.MenuItem(nuovo, "Sound Effects", false, fx, null);
            ng.setValueType(2);

            IntSlider cd = new IntSlider(0, 100, 10);
            UIEssentials.MenuItem lg = new UIEssentials.MenuItem(nuovo, "Music", false, cd, null);
            lg.setValueType(2);

            IntSlider fps = new IntSlider(10, 140, 20);
            UIEssentials.MenuItem tl = new UIEssentials.MenuItem(nuovo, "FPS Ceiling", false, fps, null);
            tl.setValueType(2);

            UIEssentials.MenuItem back = new UIEssentials.MenuItem(nuovo, "Back", false, null, null);

            nuovo.appendChild(ng);
            nuovo.appendChild(lg);
            nuovo.appendChild(tl);
            nuovo.appendChild(back);

            return nuovo;
        }

        private void ItemsSetup()
        {
            rtLevelItems = new List<UIEssentials.MenuItem>();

            UIEssentials.MenuItem OneP = Initialize1P();
            UIEssentials.MenuItem Opts = InitializeOPT();
            UIEssentials.MenuItem Scores = new UIEssentials.MenuItem("High Scores", false, null, null);
            UIEssentials.MenuItem LvlEditor = new UIEssentials.MenuItem("Level Editor", false, null, null);

            rtLevelItems.Add(OneP);
            rtLevelItems.Add(Opts);
            rtLevelItems.Add(Scores);
            rtLevelItems.Add(LvlEditor);
            curLevelItems = rtLevelItems;
            pointer = 0;
            upperItem = null;
        }
        #endregion

        #region Metodi privati per il caricamento delle risorse: PauseMenu, LoadingScreen, GameScreen
        private void LoadingScreenSetup()
        {
            ResourceDirectory root = this.container.getResourceDirectory;
            root.NewDirectory(GameConstraints.OtherPaths.CachedLevelsDir);
            root.NewDirectory(GameConstraints.OtherPaths.BonusLevelsDir);
            root.NewDirectory(GameConstraints.OtherPaths.PermanentLevelsDir);
            root.NewDirectory(GameConstraints.OtherPaths.MetadataLogicDir);
            root.InsertFile(
                GameConstraints.OtherPaths.MetadataLogicDir,
                GameConstraints.OtherPaths.LoaderResourceName,
                new LoadResourceItem(root)
                );
            root.InsertFile(
                GameConstraints.OtherPaths.MetadataLogicDir,
                FileNames.HighscoresFileName,
                new HighScoresResourceItem()
                );
            container.addScene(GameConstraints.LoadingScreen.ID, new LoadingScreen(container));
        }

        private void GameScreenLoading()
        {
            ResourceDirectory root = this.container.getResourceDirectory;
            string itsPath = GameApp.CurDir() + GameConstraints.GameScreen.Path;
            string itsName = GameConstraints.GameScreen.LogicDir;
            root.NewDirectory(itsName);
            loadMediaFiles(itsName, itsPath + @"\");
            game = new GameScreen(container);
            container.addScene(GameConstraints.GameScreen.ID, game);
        }

        private void PauseMenuLoading()
        {
            PauseMenu pm = new PauseMenu(container);
            pm.BindToGameScreen(game);
            container.addScene(GameConstraints.PauseMenu.ID, pm);
        }
        #endregion

        #region Metodi privati per le azioni generiche del menù, tipo cambio di scena, cambio volume, e così via
        private void changePixelsHue(Bitmap img, bool variation)
        {
            if (img != null)
            {
                Color c;
                for (int i = 0; i < img.Height; i++)
                    for (int j = 0; j < img.Width; j++)
                    {
                        c = img.GetPixel(j, i);
                        if (variation)
                            img.SetPixel(j, i, Color.FromArgb(c.A, c.G, c.B, c.R));
                        else
                            img.SetPixel(j, i, Color.FromArgb(c.A, c.B, c.R, c.G));
                    }
            }
        }

        private static void pickLevel()
        {
            tryFileName = null;
            #region Creo un file dialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = false;
            ofd.DefaultExt = ".bin";
            ofd.Filter = "binary files (*.bin) | *.bin";
            ofd.FilterIndex = 1;
            ofd.ReadOnlyChecked = true;
            ofd.ValidateNames = true;
            ofd.Title = "Pick a level";
            ofd.RestoreDirectory = true;
            ofd.InitialDirectory = GameApp.CurDir() + GameConstraints.OtherPaths.Levels + @"\";
            ofd.Multiselect = false;
            #endregion
            #region Controllo se il filename è buono
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string bonDir = GameApp.CurDir() + GameConstraints.OtherPaths.Bonuses + @"\";
                string lvlDir = GameApp.CurDir() + GameConstraints.OtherPaths.Levels + @"\";
                if ((ofd.FileName.StartsWith(bonDir) || ofd.FileName.StartsWith(lvlDir)) && ofd.FileName.EndsWith(".bin"))
                {
                    int lastSlash = ofd.FileName.LastIndexOf(@"\");
                    tryFileName = ofd.FileName.Substring(lastSlash + 1);
                }
                else
                    MessageBox.Show("Il file deve essere in formato .bin e deve stare nei seguenti path:\n- " + bonDir + "\n- " + lvlDir);
            }
            #endregion
        }
        #endregion

        #region Metodi per la gestione degli eventi e delle scelte del menù
        /// <summary>
        /// Gestisce il movimento della rotellina.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            changePixelsHue(cursor, e.Delta > 0);
        }

        /// <summary>
        /// Gestisce la scelta di un item del menù
        /// </summary>
        /// <param name="chosen">Menuitem scelto</param>
        protected override void HandleChoice(UIEssentials.MenuItem chosen)
        {
            base.HandleChoice(chosen);
            if (chosen != null)
            {
                string dscr = chosen.description;
                switch (dscr)
                {
                    case ("Back"):
                        {
                            this.AscendMenu();
                            break;
                        }
                    case ("FPS Ceiling"):
                        {
                            this.container.ChangeFPSCap((byte)chosen.int32Value.Value);
                            break;
                        }
                    case ("Music"):
                        {
                            this.container.VolumeMusic = chosen.int32Value.Value;
                            if (bgMusic != null)
                                bgMusic.PlayLooping(this.container.VolumeMusic);
                            break;
                        }
                    case ("Sound Effects"):
                        {
                            this.container.VolumeFX = chosen.int32Value.Value;
                            if (selectSound != null)
                                selectSound.SetVolume(this.container.VolumeFX);
                            break;
                        }
                    case ("Try Level"):
                        {
                            //TOCHECK
                            Thread t = new Thread(pickLevel); //Creo un thread in sta, sarà lui a gestirmi il dialog
                            t.SetApartmentState(ApartmentState.STA);
                            t.Start();
                            t.Join();
                            string lvl = tryFileName;
                            if (lvl != null)
                            {
                                game.NewGame(lvl, true);
                                this.Container.changeScene(GameConstraints.GameScreen.ID);
                            }
                            break;
                        }
                    case ("Level Editor"):
                        {
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.FileName = GameApp.CurDir() + "\\LvlEditor.exe";
                            psi.WindowStyle = ProcessWindowStyle.Normal;
                            psi.CreateNoWindow = false;
                            MessageBox.Show("Now starting the level editor: the game is now shutting down.");
                            System.Diagnostics.Process.Start(psi);
                            Application.Exit();
                            break;
                        }
                    case ("New Game"):
                        {
                            game.NewGame();
                            this.container.changeScene(GameConstraints.GameScreen.ID);
                            break;
                        }
                    case ("High Scores"):
                        {
                            tabellone.ShowHighscores();
                            break;
                        }
                }
            }
        }
        #endregion

        #region Costruttori
        /// <summary>
        /// Crea un menù principale di gioco a partire dal contenitore di scene
        /// </summary>
        /// <param name="sc">Contenitore di scene a cui collegare la scena corrente</param>
        public GameMainMenu(SceneContainer sc)
        {
            curPath = GameApp.CurDir() + GameConstraints.GameMainMenu.Path;
            curName = GameConstraints.GameMainMenu.LogicDir;
            GameMenuMutualPart(sc);
            ItemsSetup();
            updateText();
            LoadAndGatherResources();
            LoadingScreenSetup();
            GameScreenLoading();
            PauseMenuLoading();
            ResourceDirectory root = this.container.getResourceDirectory;
            HighScoresResourceItem hsri = (HighScoresResourceItem)root.GetFile(GameConstraints.OtherPaths.MetadataLogicDir, FileNames.HighscoresFileName);

            tabellone = hsri.Content;
        }
        #endregion
    }
    #endregion

    #region PauseMenu: Classe che rappresenta il menù di pausa durante il gioco.
    /// <summary>
    /// Questa classe rappresenta il menù di pausa.
    /// </summary>
    public class PauseMenu : GameMenu
    {
        #region Metodi privati per l'impostazione del menù
        private UIEssentials.MenuItem InitializeOPT()
        {
            UIEssentials.MenuItem nuovo = new UIEssentials.MenuItem("Options", false, null, null);
            nuovo.isEditable = false;


            IntSlider fx = new IntSlider(0, 100, 10);
            UIEssentials.MenuItem ng = new UIEssentials.MenuItem(nuovo, "Sound Effects", false, fx, null);
            ng.setValueType(2);

            IntSlider cd = new IntSlider(0, 100, 10);
            UIEssentials.MenuItem lg = new UIEssentials.MenuItem(nuovo, "Music", false, cd, null);
            lg.setValueType(2);

            IntSlider fps = new IntSlider(10, 140, 20);
            UIEssentials.MenuItem tl = new UIEssentials.MenuItem(nuovo, "FPS Ceiling", false, fps, null);
            tl.setValueType(2);

            UIEssentials.MenuItem back = new UIEssentials.MenuItem(nuovo, "Back", false, null, null);

            nuovo.appendChild(ng);
            nuovo.appendChild(lg);
            nuovo.appendChild(tl);
            nuovo.appendChild(back);

            return nuovo;
        }

        private void ItemsSetup()
        {
            rtLevelItems = new List<UIEssentials.MenuItem>();

            UIEssentials.MenuItem back = new UIEssentials.MenuItem("Back To Game", false, null, null);
            UIEssentials.MenuItem Opts = InitializeOPT();
            UIEssentials.MenuItem ret = new UIEssentials.MenuItem("Retry", false, null, null);
            UIEssentials.MenuItem quit = new UIEssentials.MenuItem("Quit Game", false, null, null);

            rtLevelItems.Add(back);
            rtLevelItems.Add(Opts);
            rtLevelItems.Add(ret);
            rtLevelItems.Add(quit);
            curLevelItems = rtLevelItems;
            pointer = 0;
            upperItem = null;
        }
        #endregion

        #region Metodi privati per le azioni generiche del menù, tipo cambio di scena, cambio volume, e così via
        private void changePixelsHue(Bitmap img, bool variation)
        {
            if (img != null)
            {
                Color c;
                for (int i = 0; i < img.Height; i++)
                    for (int j = 0; j < img.Width; j++)
                    {
                        c = img.GetPixel(j, i);
                        if (variation)
                            img.SetPixel(j, i, Color.FromArgb(c.A, c.G, c.B, c.R));
                        else
                            img.SetPixel(j, i, Color.FromArgb(c.A, c.B, c.R, c.G));
                    }
            }
        }

        #endregion

        #region Metodi per la gestione degli eventi e delle scelte del menù
        /// <summary>
        /// Gestisce il movimento della rotellina.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            changePixelsHue(cursor, e.Delta > 0);
        }

        /// <summary>
        /// Gestisce la scelta di un item del menù
        /// </summary>
        /// <param name="chosen">Menuitem scelto</param>
        protected override void HandleChoice(UIEssentials.MenuItem chosen)
        {
            base.HandleChoice(chosen);
            if (chosen != null)
            {
                string dscr = chosen.description;
                switch (dscr)
                {
                    case ("Back"):
                        {
                            this.AscendMenu();
                            break;
                        }
                    case ("FPS Ceiling"):
                        {
                            this.container.ChangeFPSCap((byte)chosen.int32Value.Value);
                            break;
                        }
                    case ("Music"):
                        {
                            this.container.VolumeMusic = chosen.int32Value.Value;
                            if (bgMusic != null)
                                bgMusic.PlayLooping(this.container.VolumeMusic);
                            break;
                        }
                    case ("Sound Effects"):
                        {
                            this.container.VolumeFX = chosen.int32Value.Value;
                            if (selectSound != null)
                                selectSound.SetVolume(this.container.VolumeFX);
                            break;
                        }
                    case ("Back To Game"):
                        {
                            this.container.changeScene(GameConstraints.GameScreen.ID);
                            break;
                        }
                    case ("Retry"):
                        {
                            game.PromptRetry();
                            this.container.changeScene(GameConstraints.GameScreen.ID);
                            break;
                        }
                    case ("Quit Game"):
                        {
                            game.PromptQuit();
                            this.container.changeScene(GameConstraints.GameScreen.ID);
                            break;
                        }
                }
            }
        }
        #endregion

        #region Costruttori.
        /// <summary>
        /// Restituisce un menù di pausa che sarà collegato al contenitore di scene.
        /// </summary>
        /// <param name="sc">Contenitore di scene da collegare al menù di pausa.</param>
        public PauseMenu(SceneContainer sc)
        {
            curPath = GameApp.CurDir() + GameConstraints.PauseMenu.Path;
            curName = GameConstraints.PauseMenu.LogicDir;
            GameMenuMutualPart(sc);
            ItemsSetup();
            updateText();
            LoadAndGatherResources();
        }
        #endregion

        /// <summary>
        /// Disegna lo stato del menù di pausa
        /// </summary>
        /// <param name="e"></param>
        public override void Draw(Graphics e)
        {
            base.Draw(e);
        }

        /// <summary>
        /// Questo metodo collega il menù di pausa con la schermata di gioco, così da farle interagire.
        /// </summary>
        /// <param name="gs">Schermata di gioco da collegare, assicurarsi che non sia NULL.</param>
        public void BindToGameScreen(GameScreen gs)
        {
            if (gs != null)
                game = gs;
            else
                throw new ArgumentNullException("The input gamescreen is a null pointer reference.");
        }
    }
    #endregion
}