using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GameUtils
{
    
    

    
    /// <summary>
    /// Contiene metodi utili a tutte le classi
    /// </summary>
    public static class GameApp
    {
        /// <summary>
        /// Restituisce il numero di ticks per secondo.
        /// </summary>
        public static double TicksPerSecond
        {
            get
            {
                return Stopwatch.Frequency;
            }
        }

        /// <summary>
        /// Converte il numero di ticks dato in millisecondi
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static double TicksToMillis(long ticks)
        {
            double n = ticks;
            return Math.Round(n * 1000 / TicksPerSecond, 3);
        }
        
        /// <summary>
        /// Restituisce la directory di gioco, quella dove è presente il file eseguibile del gioco
        /// </summary>
        /// <returns></returns>
        public static string CurDir()
        {
            return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Restituisce una copia dell'immagine in input, ridimensionata secondo i parametri dati. Restituisce null in caso di errore.
        /// </summary>
        /// <param name="input">Immagine bitmap da cui copiare le informazioni.</param>
        /// <param name="width">Larghezza desiderata dell'immagine</param>
        /// <param name="height">Altezza desiderata dell'immagine</param>
        /// <returns>L'immagine bitmap, se l'esito è positivo; altrimenti restituisce null</returns>
        public static Bitmap ResizeImg(Bitmap input, int width, int height)
        {
            if (input != null && width > 0 && height > 0)
            {
                Bitmap a = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(a);

                g.DrawImage(input, new Rectangle(0, 0, a.Width, a.Height));
                g.Dispose();
                return a;
            }
            return null;
        }

        /// <summary>
        /// Restituisce una parte dell'immagine presa in input, ridimensionato in una bitmap di dimensioni specificate.
        /// Il rettangolo di copia è espresso in coordinate immagine e dev'essere contenuto nei limiti della bitmap.
        /// </summary>
        /// <param name="input">Immagine da cui copiare la parte</param>
        /// <param name="piece">Coordinate del pezzo da copiare</param>
        /// <param name="outputSize">Dimensioni dell'immagine di output: il pezzo copiato viene ridimensionato da questo parametro.</param>
        /// <returns></returns>
        public static Bitmap copyImgPiece(Bitmap input, Rectangle piece, Size outputSize)
        {
            if (input != null && piece != null && outputSize != null)
            {
                Rectangle imgR = new Rectangle(0, 0, input.Width, input.Height);
                if (imgR.Contains(piece) && (!outputSize.IsEmpty))
                {
                    Bitmap output = new Bitmap(piece.Width, piece.Height);

                    /*uso drawimage per scrivere sopra l'immagine che sarà data come output*/
                    Graphics g = Graphics.FromImage(output);
                    g.DrawImage(input, new Point(-piece.X, -piece.Y));
                    g.Dispose();

                    Bitmap output2 = GameApp.ResizeImg(output, outputSize.Width, outputSize.Height);
                    output.Dispose();
                    output = null;
                    return output2;
                }
            }
            return null;
        }

        /// <summary>
        /// Restituisce la lista dei nomi dei file, esclusa la parte del path che li contiene (quindi non inizierà mai con \).
        /// </summary>
        /// <param name="dirPath">Cartella del path assoluto di cui si vuole fare il listing, deve già contenere il backslash finale</param>
        /// <returns>Lista che contiene i nomi dei file contenuti in una cartella (path escluso)</returns>
        public static List<string> getFileNames(string dirPath)
        {
            char[] c = new char[] { '\\' };
            List<string> s = new List<string>();
            foreach (string f in Directory.GetFiles(dirPath))
            {
                string[] temp = f.Split(c);
                string name = temp.ElementAt<string>(temp.Length - 1);
                if (name.CompareTo("") != 0)
                    s.Add(name);
            }
            return s;
        }

        /// <summary>
        /// Converte un numero long in radianti moltiplicati per una costante double per indicare la lunghezza del periodo.
        /// </summary>
        /// <param name="x">Numero da convertire.</param>
        /// <param name="period">Lunghezza del periodo (esempio: period = 1000 => 1000 è un giro intero)</param>
        /// <returns></returns>
        public static double Periodize(long x, double period)
        {
            return ((double)x) * 2.0 * Math.PI / period;
        }

        /// <summary>
        /// Restituisce il centro di un rettangolo se non è vuoto.
        /// </summary>
        /// <param name="input">Rettangolo in input.</param>
        /// <returns></returns>
        public static Point RectangleCentre(Rectangle input)
        {
            if (input != null && (!input.IsEmpty))
                return new Point(input.X + (input.Width / 2), input.Y + (input.Height / 2));
            return new Point();
        }

        /// <summary>
        /// Restituisce il centro di un rettangolo se non è vuoto.
        /// </summary>
        /// <param name="input">Rettangolo in input.</param>
        /// <returns></returns>
        public static PointF RectangleFCentre(RectangleF input)
        {
            if (input != null && (!input.IsEmpty))
                return new PointF(input.X + (input.Width / 2), input.Y + (input.Height / 2));
            return new PointF();
        }

        /// <summary>
        /// Dato un rettangolo in input, restituisce un rettangolo con dimensioni definite in funzione del rettangolo di input. Anche la posizione
        /// è espressa in funzione del rettangolo di input e viene deciso se posizionare l'output rispetto al suo centro o al suo angolo in alto a
        /// sinistra.
        /// </summary>
        /// <param name="input">Rettangolo in input che fa da riferimento.</param>
        /// <param name="offsetRatioX">Quanto è l'offset X del rettangolo rispetto all'input</param>
        /// <param name="offsetRatioY">Quanto è l'offset Y del rettangolo rispetto all'input</param>
        /// <param name="widthRatio">Quanto è largo rispetto all'input</param>
        /// <param name="heightRatio">Quanto è alto rispetto all'input</param>
        /// <param name="centralPositioning">True se e solo se si vuole che il posizionamento sia relativo al centro del rettangolo</param>
        /// <returns>Rettangolo dimensionato.</returns>
        public static Rectangle zoomedAndSizedRectangleOf(Rectangle input, float offsetRatioX, float offsetRatioY, float widthRatio, float heightRatio, bool centralPositioning)
        {
            //Controllo sugli input, non voglio rettangoli vuoti o inesistenti
            if (input != null && (!input.IsEmpty) && (widthRatio * heightRatio != 0))
            {
                //Sistemo i width e height ratio, devono essere positivi
                widthRatio = Math.Abs(widthRatio);
                heightRatio = Math.Abs(heightRatio);

                //Memorizzo in floating i 4 parametri del rettangolo di input
                float inW = (float)input.Width;
                float inH = (float)input.Height;
                float inX = (float)input.X;
                float inY = (float)input.Y;

                //Determino larghezza e altezza del rettangolo
                float w = inW * widthRatio;
                float h = inH * heightRatio;

                //Preparo il punto in cui posizionare il rettangolo, se è al centro, mi sposto in diagonale in alto a sinistra
                float x = inX, y = inY;
                if (centralPositioning)
                {
                    x += -w / 2.0f;
                    y += -h / 2.0f;
                }
                x += (inW * offsetRatioX);
                y += (inH * offsetRatioY);

                return new Rectangle((int)x, (int)y, (int)w, (int)h);
            }
            return new Rectangle();
        }

        /// <summary>
        /// Disegna l'immagine adattandola al rettangolo specificato e all'alpha indicato
        /// </summary>
        /// <param name="e">Contesto grafico che disegna</param>
        /// <param name="alpha">Canale alpha da applicare all'immagine</param>
        /// <param name="img">Immagine da ridisegnare</param>
        /// <param name="where">Riquadro in cui disegnare l'immagine</param>
        public static void DrawAlphaImage(Graphics e, int alpha, Image img, Rectangle where)
        {
            ImageAttributes im = new ImageAttributes();
            ColorMatrix cm = new ColorMatrix();
            cm.Matrix33 = (((float)alpha) / 255.0f);
            im.SetColorMatrix(cm);
            e.DrawImage(img, where, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, im);
        }

        /// <summary>
        /// Metodo statico che disegna, dato un contesto grafico, un testo centrato e ridimensionato rispetto ad un rettangolo di contenimento
        /// </summary>
        /// <param name="e">Contesto grafico dato</param>
        /// <param name="s">Stringa da scrivere</param>
        /// <param name="box">Rettangolo di contenimento</param>
        /// <param name="alignment">Allineamento del testo</param>
        /// <param name="format">Formato del testo</param>
        public static void DrawPromptTextInBox(Graphics e, string s, Rectangle box, StringAlignment alignment, TextFormatFlags format)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = alignment;
            sf.LineAlignment = alignment;
            
            Size txtBox = box.Size;
            float desiredFont = txtBox.Height / 1.8f;
            Font f = new Font("Verdana", desiredFont, FontStyle.Bold);
            Size cell = TextRenderer.MeasureText(s, f, txtBox, format);

            if (cell.Width > txtBox.Width || cell.Height > txtBox.Height)
            {
                float wRatio = ((float)(cell.Width)) / (txtBox.Width);
                float hRatio = ((float)(cell.Height)) / ((float)txtBox.Height);
                float cur_desiredRatio = Math.Max(wRatio, hRatio);
                desiredFont = desiredFont / (cur_desiredRatio + 0.2f);
            }

            GraphicsPath p = new GraphicsPath();
            p.AddString(s, new FontFamily("Verdana"), (int)FontStyle.Bold, desiredFont, box, sf);
            p.FillMode = FillMode.Winding;
            p.CloseAllFigures();
            e.FillPath(new SolidBrush(Color.FromArgb(180, 220, 220, 220)), p);
            e.DrawPath(Pens.Black, p);

            p.Dispose();
            f.Dispose();
            sf.Dispose();
        }
    }
    
}
