using System.Drawing;
using System.Windows.Forms;
using GameCore;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;

/// <summary>
/// Classe per il test del gioco
/// </summary>
public class Sandbox
{
    static void Main()
    {
        Form f = new Form();
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        Console.WriteLine(Directory.GetCurrentDirectory());
        KulaGame g = new KulaGame(f);
        /*try 
        {
            g.Begin();
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message + "\n\nStack trace:\n" + e.StackTrace, e.Source + " exception!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }*/
        g.Begin();
        
    }
}
