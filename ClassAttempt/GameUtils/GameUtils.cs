using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Text;

namespace GameUtils
{
    #region ExtensionMethods: classe statica per avere metodi di deep cloning tra oggetti serializzabili.
    /// <summary>
    /// Classe statica che contiene un metodo per fare la deep copy di oggetti serializzabili.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Fa una deepcopy dell'oggetto serializzabile.
        /// </summary>
        /// <typeparam name="T">Tipo di oggetto da serializzare</typeparam>
        /// <param name="a">Oggetto da serializzare</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
    #endregion

    #region MatrixUtils: classe statica con metodi di utilità per le trasformazioni matriciali sui punti
    public static class MatrixUtils
    {
        #region Metodi privati che creano un wrap per la TransformPoints
        private static PointF[] pointf(PointF p)
        {
            return new PointF[] { new PointF(p.X, p.Y) };
        }

        private static Point[] point(Point p)
        {
            return new Point[] { new Point(p.X, p.Y) };
        }
        private static PointF[] pointf(Point p)
        {
            return new PointF[] { new PointF(p.X, p.Y) };
        }

        private static Point[] point(PointF p)
        {
            return new Point[] { new Point((int)p.X, (int)p.Y) };
        }
        #endregion

        #region Metodi pubblici per la trasformazione di un punto
        public static PointF TransformPointF(Matrix m, PointF p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");
            PointF[] pt = pointf(p);
            m.TransformPoints(pt);
            return pt[0];
        }
        public static PointF TransformPointF(Matrix m, Point p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");
            PointF[] pt = pointf(p);
            m.TransformPoints(pt);
            return pt[0];
        }
        public static Point TransformPoint(Matrix m, PointF p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");
            Point[] pt = point(p);
            m.TransformPoints(pt);
            return pt[0];
        }
        public static Point TransformPoint(Matrix m, Point p)
        {
            if (m == null || p == null)
                throw new ArgumentNullException("In TransformPoint(...) point or matrix are NULL.");
            Point[] pt = point(p);
            m.TransformPoints(pt);
            return pt[0];
        }
        #endregion

        public static PointF RoundPoint(PointF a)
        {
            return new PointF((float)Math.Round(a.X), (float)Math.Round(a.Y));
        }

        public static float ScalarProduct(PointF a, PointF b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        
    }
    #endregion

    #region FileNames: classe statica che restituisce il nome di file "notevoli"
    /// <summary>
    /// Classe statica che restituisce il nome di file notevoli
    /// </summary>
    public static class FileNames
    {
        /// <summary>
        /// Restituisce il nome del primo livello da caricare.
        /// </summary>
        public static string FirstLevelFilename
        { 
            get 
            { 
                return "L0001.bin";
            } 
        }
        /// <summary>
        /// Restituisce la lista di nomi dei livelli bonus presenti nel gioco.
        /// </summary>
        public static List<string> AllBonusLevelsFileName
        {
            get
            {
                return GameApp.getFileNames(GameApp.CurDir() + GameConstraints.OtherPaths.Bonuses + "\\");
            }
        }
        /// <summary>
        /// Restituisce il nome di tutti i file .png utilizzati dalla schermata di gioco
        /// </summary>
        public static List<string> GamePngsFileName
        {
            get
            {
                List<string> ret = GameApp.getFileNames(GameApp.CurDir() + GameConstraints.GameScreen.Path + "\\");
                return ret.FindAll(x => x.EndsWith(".png"));
            }
        }
        /// <summary>
        /// Restituisce il nome del file dei punteggi massimi
        /// </summary>
        public static string HighscoresFileName
        {
            get { return "HighScores.bin"; }
        }
    }
    #endregion

    #region GameConstraints: classe statica che restituisce costanti utili per il gioco.
    /// <summary>
    /// Contiene costanti utili per la localizzazione (a partire dalla cartella dell'applicazione) delle risorse di gioco.
    /// </summary>
    public static class GameConstraints
    {
        /// <summary>
        /// Questa classe definisce liste di stringhe utili per gestire i temi.
        /// </summary>
        public static class GameThemes
        {
            /// <summary>
            /// Restituisce la lista dei Path (a partire dalla cartella dell'applicazione) di ogni tema.
            /// </summary>
            public static List<string> ThemePaths
            {
                get
                {
                    List<string> output = new List<string>();
                    output.Add(GameConstraints.ThemePaths.Bonus);
                    output.Add(GameConstraints.ThemePaths.Egypt);
                    output.Add(GameConstraints.ThemePaths.Hills);
                    output.Add(GameConstraints.ThemePaths.Incas);
                    output.Add(GameConstraints.ThemePaths.Arctic);
                    output.Add(GameConstraints.ThemePaths.Rust);
                    output.Add(GameConstraints.ThemePaths.Field);
                    output.Add(GameConstraints.ThemePaths.Haze);
                    output.Add(GameConstraints.ThemePaths.Atlantis);
                    output.Add(GameConstraints.ThemePaths.Mars);
                    output.Add(GameConstraints.ThemePaths.Hell);
                    return output;
                }
            }

            /// <summary>
            /// Restituisce la lista dei nomi delle cartelle logiche di ogni tema.
            /// </summary>
            public static List<string> ThemeLogicDirectories
            {
                get
                {
                    List<string> output = new List<string>();
                    output.Add(GameConstraints.ThemeLogicDirs.Bonus);
                    output.Add(GameConstraints.ThemeLogicDirs.Egypt);
                    output.Add(GameConstraints.ThemeLogicDirs.Hills);
                    output.Add(GameConstraints.ThemeLogicDirs.Incas);
                    output.Add(GameConstraints.ThemeLogicDirs.Arctic);
                    output.Add(GameConstraints.ThemeLogicDirs.Rust);
                    output.Add(GameConstraints.ThemeLogicDirs.Field);
                    output.Add(GameConstraints.ThemeLogicDirs.Haze);
                    output.Add(GameConstraints.ThemeLogicDirs.Atlantis);
                    output.Add(GameConstraints.ThemeLogicDirs.Mars);
                    output.Add(GameConstraints.ThemeLogicDirs.Hell);
                    return output;
                }
            }
        }

        /// <summary>
        /// Proprietà del beginscreen standard del gioco
        /// </summary>
        public static class BeginScreen
        {
            /// <summary>
            /// Restituisce il byte standard identificativo del beginscreen
            /// </summary>
            public static byte ID
            {
                get { return 0; }
            }

            /// <summary>
            /// Restituisce il percorso, a partire dalla cartella di gioco, delle risorse della schermata di inizio
            /// </summary>
            public static string Path
            {
                get { return @"\MAIN_RES\BEGINSCREEN"; }
            }
        }

        /// <summary>
        /// Proprietà del mainmenu standard del gioco
        /// </summary>
        public static class GameMainMenu
        {
            /// <summary>
            /// Restituisce il path delle risorse del menù principale, a partire dalla directory del gioco.
            /// </summary>
            public static string Path
            {
                get { return @"\MAIN_RES\MAINMENU"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella "logica" delle risorse del menù principale
            /// </summary>
            public static string LogicDir
            {
                get { return @"MainMenu"; }
            }

            /// <summary>
            /// Restituisce il byte standard identificativo del mainmenu
            /// </summary>
            public static byte ID
            {
                get { return 1; }
            }
        }

        /// <summary>
        /// Proprietà della schermata di caricamento standard del gioco
        /// </summary>
        public static class LoadingScreen
        {
            /// <summary>
            /// Restituisce il path delle risorse della schermata di caricamento, a partire dalla directory del gioco.
            /// </summary>
            public static string Path
            {
                get { return @"\MAIN_RES\LOADINGSCREEN"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella "logica" delle risorse della schermata di caricamento
            /// </summary>
            public static string LogicDir
            {
                get { return @"LoadingScreen"; }
            }

            /// <summary>
            /// Restituisce il byte standard identificativo della schermata di caricamento
            /// </summary>
            public static byte ID
            {
                get { return 2; }
            }
        }

        /// <summary>
        /// Proprietà della schermata standard del gioco
        /// </summary>
        public static class GameScreen
        {
            /// <summary>
            /// Restituisce il path delle risorse della schermata di caricamento, a partire dalla directory del gioco.
            /// </summary>
            public static string Path
            {
                get { return @"\GAME_RES"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella "logica" delle risorse della schermata di caricamento
            /// </summary>
            public static string LogicDir
            {
                get { return @"GameScreen"; }
            }

            /// <summary>
            /// Restituisce il byte standard identificativo della schermata di caricamento
            /// </summary>
            public static byte ID
            {
                get { return 3; }
            }
        }

        /// <summary>
        /// Proprietà del menu di pausa standard di gioco
        /// </summary>
        public static class PauseMenu
        {
            /// <summary>
            /// Restituisce il path delle risorse del menu di pausa, a partire dalla directory del gioco.
            /// </summary>
            public static string Path
            {
                get { return @"\MAIN_RES\PAUSEMENU"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella "logica" delle risorse del menu di pausa
            /// </summary>
            public static string LogicDir
            {
                get { return @"PauseMenu"; }
            }

            /// <summary>
            /// Restituisce il byte standard identificativo del menu di pausa
            /// </summary>
            public static byte ID
            {
                get { return 4; }
            }
        }

        /// <summary>
        /// Contiene altri path utili
        /// </summary>
        public static class OtherPaths
        {
            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) dei livelli ordinari.
            /// </summary>
            public static string Levels
            {
                get { return @"\LEVELS"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) dei livelli bonus.
            /// </summary>
            public static string Bonuses
            {
                get { return @"\BONUSLEVELS"; }
            }

            /// <summary>
            /// Restituisce la directory logica per i livelli ritenuti da tenere sempre.
            /// </summary>
            public static string PermanentLevelsDir
            {
                get { return "PermaLevels"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica in cui verranno contenuti i livelli bonus.
            /// </summary>
            public static string BonusLevelsDir
            {
                get
                {
                    return "BonusLevels";
                }
            }

            /// <summary>
            /// Restituisce la directory logica per i livelli tenuti temporaneamente, ma scartabili.
            /// </summary>
            public static string CachedLevelsDir
            {
                get { return "CachedLevels"; }
            }

            /// <summary>
            /// Restituisce la directory logica in cui verranno messe le metarisorse.
            /// </summary>
            public static string MetadataLogicDir
            {
                get { return "Metadata"; }
            }

            /// <summary>
            /// Restituisce il nome della metarisorsa dedicata al caricamento.
            /// </summary>
            public static string LoaderResourceName
            {
                get { return "Loader"; }
            }

        }

        /// <summary>
        /// Contiene i path dei temi standard a partire dalla directory che contiene l'applicazione.
        /// </summary>
        public static class ThemePaths
        {
            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema egiziano.
            /// </summary>
            public static string Egypt
            {
                get { return @"\THEMES\EGYPT"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema collinare.
            /// </summary>
            public static string Hills
            {
                get { return @"\THEMES\HILLS"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema inca.
            /// </summary>
            public static string Incas
            {
                get { return @"\THEMES\INCA"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema artico.
            /// </summary>
            public static string Arctic
            {
                get { return @"\THEMES\ARCTIC"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema ruggine.
            /// </summary>
            public static string Rust
            {
                get { return @"\THEMES\RUST"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema campo.
            /// </summary>
            public static string Field
            {
                get { return @"\THEMES\FIELD"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema nebbia.
            /// </summary>
            public static string Haze
            {
                get { return @"\THEMES\HAZE"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema atlantide.
            /// </summary>
            public static string Atlantis
            {
                get { return @"\THEMES\ATLANTIS"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema marziano.
            /// </summary>
            public static string Mars
            {
                get { return @"\THEMES\MARS"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema inferno.
            /// </summary>
            public static string Hell
            {
                get { return @"\THEMES\HELL"; }
            }

            /// <summary>
            /// Restituisce il Path (a partire dalla cartella di gioco) del tema bonus.
            /// </summary>
            public static string Bonus
            {
                get { return @"\THEMES\BONUS"; }
            }


        }

        /// <summary>
        /// Contiene il nome delle cartelle logiche attribuite ai temi.
        /// </summary>
        public static class ThemeLogicDirs
        {
            /// <summary>
            /// Restituisce il nome della cartella logica del tema egiziano.
            /// </summary>
            public static string Egypt
            {
                get { return @"Egypt"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema collinare.
            /// </summary>
            public static string Hills
            {
                get { return @"Hills"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema inca.
            /// </summary>
            public static string Incas
            {
                get { return @"Inca"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema artico.
            /// </summary>
            public static string Arctic
            {
                get { return @"Arctic"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema ruggine.
            /// </summary>
            public static string Rust
            {
                get { return @"Rust"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema campo.
            /// </summary>
            public static string Field
            {
                get { return @"Field"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema nebbia.
            /// </summary>
            public static string Haze
            {
                get { return @"Haze"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema atlantide.
            /// </summary>
            public static string Atlantis
            {
                get { return @"Atlantis"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema marziano.
            /// </summary>
            public static string Mars
            {
                get { return @"Mars"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema inferno.
            /// </summary>
            public static string Hell
            {
                get { return @"Hell"; }
            }

            /// <summary>
            /// Restituisce il nome della cartella logica del tema bonus.
            /// </summary>
            public static string Bonus
            {
                get { return @"Bonus"; }
            }
        }
    }
    #endregion

    #region GameApp: Classe statica per avere qualche metodo di utilità generale
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
    #endregion

    #region IntSlider: Classe che rappresenta uno slider che resta tra un massimo ed un minimo. Si può incrementare e decrementare di una quantità fissa.
    /// <summary>
    /// Classe che rappresenta uno slider che resta tra un massimo ed un minimo. Si può incrementare e decrementare di una quantità fissa.
    /// La quantità di aumento e diminuzione è definita al momento della costruzione dell'oggetto.
    /// Al momento della costruzione, il valore dello slider è a metà tra il massimo ed il minimo specificato.
    /// </summary>
    public class IntSlider
    {
        private double val;
        private int max;
        private int min;
        private double delta;
        private int steps;

        private void ConstructorsMutualPart(int m, int M)
        {
            min = Math.Min(m, M);
            max = Math.Max(m, M);
            val = ((double)(max + min)) / (2.0);
        }

        /// <summary>
        /// Inizializza uno slider che ha i valori compresi tra m ed M, oppure tra M ed m. Il numero di gradazioni è 10.
        /// </summary>
        /// <param name="m">Massimo o minimo del valore dello slider</param>
        /// <param name="M">Massimo o minimo del valore dello slider</param>
        public IntSlider(int m, int M)
        {
            ConstructorsMutualPart(m, M);
            delta = ((double)(max - min)) / 10.0;
            steps = 10;
        }

        /// <summary>
        /// Inizializza uno slider che ha i valori compresi tra m ed M, oppure tra M ed m. Il numero di gradazioni è 10.
        /// </summary>
        /// <param name="m">Massimo o minimo del valore dello slider</param>
        /// <param name="M">Massimo o minimo del valore dello slider</param>
        /// <param name="steps">Numero di gradazioni dello slider</param>
        public IntSlider(int m, int M, int steps)
        {
            ConstructorsMutualPart(m, M);
            delta = (((double)max) - ((double)min)) / ((double)steps);
            this.steps = steps;
        }

        /// <summary>
        /// Restituisce il valore dello slider.
        /// </summary>
        public int Value
        {
            get { return (int)val; }
        }

        /// <summary>
        /// Aumenta il valore dello slider di uno step.
        /// </summary>
        public void IncreaseValue()
        {
            if ((val += delta) > max)
                val = max;
        }

        /// <summary>
        /// Decrementa il valore dello slider di uno step.
        /// </summary>
        public void DecreaseValue()
        {
            if ((val -= delta) < min)
                val = min;
        }

        /// <summary>
        /// Restituisce una deep copy dello slider corrente. (Stato del valore escluso)
        /// </summary>
        public IntSlider Clone()
        {
            return new IntSlider(min, max, steps);
        }
    }
    #endregion

    #region DoubleSlider: Classe che rappresenta uno slider che resta tra un massimo ed un minimo. Si può incrementare e decrementare di una quantità fissa.
    /// <summary>
    /// Classe che rappresenta uno slider che resta tra un massimo ed un minimo. Si può incrementare e decrementare di una quantità fissa.
    /// La quantità di aumento e diminuzione è definita al momento della costruzione dell'oggetto.
    /// Al momento della costruzione, il valore dello slider è a metà tra il massimo ed il minimo specificato.
    /// </summary>
    public class DoubleSlider
    {
        private double val;
        private double max;
        private double min;
        private double delta;
        private int steps;

        private void ConstructorsMutualPart(double m, double M)
        {
            min = Math.Min(m, M);
            max = Math.Max(m, M);
            val = (max + min) / (2.0);
        }

        /// <summary>
        /// Genera un oggetto con valori numerici regolabili da un minimo e un massimo.
        /// </summary>
        /// <param name="m">Minimo o massimo dello slider</param>
        /// <param name="M">Minimo o massimo dello slider</param>
        public DoubleSlider(double m, double M)
        {
            ConstructorsMutualPart(m, M);
            delta = (max - min) / 10.0;
            this.steps = 10;
        }

        /// <summary>
        /// Genera un oggetto con valori numerici regolabili da un minimo e un massimo.
        /// </summary>
        /// <param name="m">Minimo o massimo dello slider</param>
        /// <param name="M">Minimo o massimo dello slider</param>
        /// <param name="steps">Numero di gradazioni dello slider</param>
        public DoubleSlider(double m, double M, int steps)
        {
            ConstructorsMutualPart(m, M);
            delta = (max - min) / ((double)steps);
            this.steps = steps;
        }

        /// <summary>
        /// Restituisce il valore dello slider.
        /// </summary>
        public double Value
        {
            get { return val; }
        }

        /// <summary>
        /// Aumenta il valore dello slider di uno step.
        /// </summary>
        public void IncreaseValue()
        {
            if ((val += delta) > max)
                val = max;
        }

        /// <summary>
        /// Decrementa il valore dello slider di uno step.
        /// </summary>
        public void DecreaseValue()
        {
            if ((val -= delta) < min)
                val = min;
        }

        /// <summary>
        /// Restituisce una deep copy dello slider corrente. (Stato del valore escluso)
        /// </summary>
        public DoubleSlider Clone()
        {
            return new DoubleSlider(min, max, steps);
        }
    }
    #endregion

    #region Pair<T>: Coppia immutabile di valori generici (è pure comparabile)
    /// <summary>
    /// Questa classe rappresenta una coppia immutabile di valori generici.
    /// </summary>
    /// <typeparam name="T">Tipo istanziato dei due elementi della coppia</typeparam>
    [Serializable()]
    public class Pair<T> : IComparable where T : IComparable
    {
        private T first;
        private T second;

        /// <summary>
        /// Inizializza una coppia immutabile di valori confrontabili.
        /// </summary>
        /// <param name="firstVal">Primo valore della coppia</param>
        /// <param name="secondVal">Secondo valore della coppia</param>
        public Pair(T firstVal, T secondVal)
        {
            first = firstVal;
            second = secondVal;
        }

        /// <summary>
        /// Restituisce il primo valore della coppia
        /// </summary>
        public T FirstValue
        {
            get { return first; }
        }

        /// <summary>
        /// Restituisce il secondo valore della coppia
        /// </summary>
        public T SecondValue
        {
            get { return second; }
        }

        /// <summary>
        /// Restituisce un intero > 0 se l'oggetto in input è più piccolo, un intero minore 0 se l'oggetto è più grande e un intero = o se l'oggetto è uguale.
        /// </summary>
        /// <param name="obj">Oggetto comparabile in input</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Pair<T> other = (Pair<T>)obj;
            int compare;
            if ((compare = first.CompareTo(other.FirstValue)) == 0)
                return second.CompareTo(other.SecondValue);
            else
                return compare;
        }
    }
    #endregion

    #region Definizione della classe statica Interaction
    public static class Interaction
    {
        /// <summary>
        /// Questo metodo crea un dialog che riceve in input due stringhe.
        /// </summary>
        /// <param name="title">Titolo del dialog</param>
        /// <param name="prompt1">Testo da scrivere sopra la prima casella</param>
        /// <param name="prompt2">Testo da scrivere sopra la seconda casella</param>
        /// <param name="val1">Primo valore che verrà modificato dal dialog</param>
        /// <param name="val2">Secondo valore che verrà modificato dal dialog</param>
        /// <returns></returns>
        public static DialogResult DoubleTextInputBox(string title, string prompt1, string prompt2, ref string val1, ref string val2)
        {
            Form form = new Form();
            Label label1 = new Label();
            Label label2 = new Label();
            TextBox textBox1 = new TextBox();
            TextBox textBox2 = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label1.Text = prompt1;
            label2.Text = prompt2;
            textBox1.Text = val1;
            textBox2.Text = val2;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            label1.SetBounds(9, 20, 120, 13);
            textBox1.SetBounds(12, 36, 100, 20);
            label2.SetBounds(19 + 120, 20, 120, 13);
            textBox2.SetBounds(22 + 120, 36, 100, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            //label1.AutoSize = true;
            //label2.AutoSize = true;

            form.ClientSize = new Size(389, 107);
            form.Controls.AddRange(new Control[] { label1, textBox1, label2, textBox2, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label2.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            val1 = textBox1.Text;
            val2 = textBox2.Text;
            return dialogResult;
        }

        /// <summary>
        /// Questo metodo crea un dialog che riceve in input una stringa.
        /// </summary>
        /// <param name="title">Titolo del dialog</param>
        /// <param name="promptText">Testo da scrivere sopra la casella</param>
        /// <param name="value">Valore che verrà modificato dal dialog</param>
        /// <returns></returns>
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
    #endregion
}
