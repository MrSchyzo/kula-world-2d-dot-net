using GameLoadingScreens;
using GameUtils;
using System;
using System.Drawing;
using System.Windows.Forms;
using UIEssentials;

public class KulaGame
{
    private Form containingForm;
    private GameViewport gameViewport;

    private void formSetup(Form f)
    {
        gameViewport = new GameViewport(144, null);
        this.containingForm = f;
        f.ClientSize = new Size(600, 450);
        f.FormBorderStyle = FormBorderStyle.Sizable;
        f.MaximizeBox = false;
        f.MinimizeBox = false;
        f.MaximumSize = new Size(1200, 900);
        f.MinimumSize = new Size(480, 320);
        f.Text = "K2D 0.0.b";
        f.Resize += f_Resized;
        f.FormClosing += f_FormClosing;
        gameViewport.Dock = DockStyle.Fill;
        f.Controls.Add(gameViewport);
        f.Show();
    }

    void f_Resized(object sender, EventArgs e)
    {
        double w = containingForm.ClientSize.Width;
        double h = containingForm.ClientSize.Height;

        if (h != (0.75) * w)
            containingForm.ClientSize = new Size((int)w, (int)(0.75 * w));
    }

    void f_FormClosing(object sender, FormClosingEventArgs e)
    {
        gameViewport.Freeze();
    }

    /// <summary>
    /// Inizializza il gioco impostando gli FPS massimi a 45.
    /// </summary>
    /// <param name="viewport">La form che conterrà il gioco.</param>
    public KulaGame(Form viewport)
    {
        formSetup(viewport);
        gameViewport.FramesPerSecondCap = 75;
        gameViewport.sceneList.addScene(GameConstraints.BeginScreen.ID, new BeginScreen(GameConstraints.GameMainMenu.ID));
    }

    /// <summary>
    /// Inizia l'esecuzione del gioco.
    /// </summary>
    public void Begin()
    {
        gameViewport.Focus();
        gameViewport.Go();
        gameViewport.Focus();
    }

    /// <summary>
    /// Termina l'esecuzione del gioco.
    /// </summary>
    public void End()
    {
        gameViewport.Freeze();
    }
}
