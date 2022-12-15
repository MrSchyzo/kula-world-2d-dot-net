using System.Windows.Forms;
using System.Diagnostics;
using System;
using System.IO;

/// <summary>
/// Classe per il test del gioco
/// </summary>
public class App
{
    static void Main()
    {
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        Console.WriteLine(Directory.GetCurrentDirectory());
        KulaGame g = new KulaGame(new Form());
        try 
        {
            g.Begin();
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message + "\n\nStack trace:\n" + e.StackTrace, e.Source + " exception!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
    }
}
