using GameEngine;
using GameUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UIEssentials;
using UIMainClasses;

namespace GameMenus
{
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
