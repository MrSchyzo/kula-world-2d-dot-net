using GameMetadata;
using GameUtils;
using LevelsStructure;
using MultimediaClasses;
using ResourcesBasics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResourceItems
{
    #region ImageResourceItem: classe che incorpora un'immagine nei file "logici".
    /// <summary>
    /// ResourceItem specializzato nel trattare Bitmaps: è possibile caricare e scaricare un'immagine, oltre che a consultare il contenuto.
    /// Il caricamento della risorsa è sincrono!
    /// </summary>
    public class ImageResourceItem : ResourceItem
    {
        private Bitmap img;
        private Rectangle bounds;

        private void load(string Path)
        {
            try
            {
                img = new Bitmap(Path);
                bounds = new Rectangle(0, 0, img.Size.Width, img.Size.Height);
            }
            catch (Exception e)
            {
                Console.WriteLine("Minchia! L'immagine " + Path + " non è stata caricata.\n" + e.ToString());
                Console.WriteLine();
                MessageBox.Show("Minchia! L'immagine " + Path + " non è stata caricata.\n" + e.ToString());
                img = null;
            }
        }

        private void unload()
        {
            if (img != null)
                img.Dispose();
            img = null;
        }

        private bool IsEmpty()
        {
            return (img == null);
        }

        /// <summary>
        /// Inizializza una risorsa di immagine a partire dal path del file da rappresentare.
        /// </summary>
        /// <param name="Path">Path del file</param>
        public ImageResourceItem(string Path)
        {
            if (Path != null)
                load(Path);
            else
                img = null;
        }

        /// <summary>
        /// Carica un'altra immagine nella risorsa a partire dal path, restituisce true se e solo se il caricamento è andato a buon fine
        /// </summary>
        /// <param name="Path">Path del file</param>
        /// <returns></returns>
        public override bool Load(string Path)
        {
            if (img != null)
                unload();
            if (Path != null)
                load(Path);
            return !IsEmpty();
        }

        /// <summary>
        /// Scarica l'immagine contenuta dalla risorsa
        /// </summary>
        public override void Unload()
        {
            unload();
        }

        /// <summary>
        /// Restituisce il contenuto dell'image resource item.
        /// </summary>
        public Bitmap Content
        {
            get { return img; }
        }

        /// <summary>
        /// Ridimensiona il contenuto ad altezza e larghezza scelta.
        /// </summary>
        /// <param name="w">Larghezza in pixel</param>
        /// <param name="h">Altezza in pixel</param>
        public void resizeContent(int w, int h)
        {
            if (w > 0 && h > 0 && img != null)
            {
                Bitmap nuovo = GameApp.ResizeImg(img, w, h);
                img.Dispose();
                img = nuovo;
            }
        }

        /// <summary>
        /// Ridimensiona il contenuto secondo uno scalare, che manterrà l'aspect ratio.
        /// </summary>
        /// <param name="scale">Scalare che modifica le dimensioni.</param>
        public void resizeContent(float scale)
        {
            if (scale > 0 && img != null)
            {
                int w = (int)(scale * ((float)img.Width));
                int h = (int)(scale * ((float)img.Height));
                Bitmap nuovo = GameApp.ResizeImg(img, w, h);
                img.Dispose();
                img = nuovo;
            }
        }
    }
    #endregion

    #region SoundResourceItem: classe che incorpora dei file sonori in file "logici" (N.B. Per ogni SMP c'è un thread a parte)
    /// <summary>
    /// ResourceItem specializzato nel trattare Bitmaps: è possibile caricare e scaricare un suono, oltre che a avviarlo sia ciclicamente che non.
    /// Il caricamento della risorsa è sincrono!
    /// </summary>
    public class SoundResourceItem : ResourceItem
    {
        private SoundMediaPlayer sound;

        /// <summary>
        /// SoundMediaPlayer contenuto nella risorsa.
        /// </summary>
        public SoundMediaPlayer Content
        {
            get { return sound; }
        }

        private bool isValid()
        {
            return (sound != null);
        }

        private void load(string Path)
        {
            sound = new SoundMediaPlayer();
            try
            {
                sound.SoundLocation = Path;
                sound.Load();
            }
            catch (Exception e)
            {
                Console.WriteLine("Minchia! Non è stato possibile caricare l'audio " + Path + " !\n" + e.ToString());
                Console.WriteLine();
                MessageBox.Show("Minchia! Non è stato possibile caricare l'audio " + Path + " !\n" + e.ToString());
                sound.Dispose();
                sound = null;
            }
        }

        private void unload()
        {
            if (sound != null)
            {
                sound.Stop();
                sound.Dispose();
            }
            sound = null;
            GC.Collect();
        }

        /// <summary>
        /// Inizializza una risorsa di suono a partire dal file path.
        /// </summary>
        /// <param name="Path">Path del file</param>
        public SoundResourceItem(string Path)
        {
            sound = null;
            Load(Path);
        }

        /// <summary>
        /// Carica una risorsa di suono a partire dal file path, restituisce true se e solo se il caricamento è andato a buon fine
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public override bool Load(string Path)
        {
            if (sound != null)
                unload();
            if (Path != null)
                load(Path);
            return isValid();
        }

        /// <summary>
        /// Scarica il contenuto della risorsa.
        /// </summary>
        public override void Unload()
        {
            unload();
        }

        /// <summary>
        /// Suona asincronamente la risorsa.
        /// </summary>
        public void Play()
        {
            if (sound != null)
                sound.Play();
        }

        /// <summary>
        /// Suona asincronamente la risorsa al volume desiderato.
        /// </summary>
        /// <param name="vol">Volume desiderato</param>
        public void Play(int vol)
        {
            if (sound != null)
                sound.Play(vol);
        }

        /// <summary>
        /// Ferma la riproduzione del suono e lo riavvolge
        /// </summary>
        public void Stop()
        {
            if (sound != null)
                while (!sound.Stop()) ;
        }

        /// <summary>
        /// Ferma la riproduzione del suono
        /// </summary>
        public void Pause()
        {
            if (sound != null)
                sound.Pause();
        }

        /// <summary>
        /// Suona asincronamente e ciclicamente il suono
        /// </summary>
        public void PlayLooping()
        {
            if (sound != null)
                sound.PlayLooping();
        }

        /// <summary>
        /// Suona asincronamente e ciclicamente il suono al volume desiderato
        /// </summary>
        /// <param name="vol">Volume desiderato</param>
        public void PlayLooping(int vol)
        {
            if (sound != null)
                sound.PlayLooping(vol);
        }

        /// <summary>
        /// Modifica il volume della risorsa sonora.
        /// </summary>
        /// <param name="v">Volume desiderato</param>
        /// <returns></returns>
        public bool ChangeVolume(int v)
        {
            return sound.SetVolume(v);
        }
    }
    #endregion

    #region LoadResourceItem: Metarisorsa utile a caricare e scaricare i livelli del gioco (è incluso il caricamento automatico dei temi)
    /// <summary>
    /// Classe che incorpora una metarisorsa utile a caricare e scaricare i livelli del gioco (è incluso il caricamento automatico dei temi)
    /// </summary>
    public class LoadResourceItem : ResourceItem
    {
        private ResourceInfo nextNormalToLoad;
        private ResourceInfo nextBonusToLoad;
        private ResourceDirectory root;
        private List<string> themes2Keep;

        /// <summary>
        /// Restituisce o imposta true se e solo se i livelli vengono caricati temporaneamente.
        /// </summary>
        public bool IsCaching { get; set; }

        /// <summary>
        /// Imposta o restituisce true se e solo se i livelli verranno caricati a blocchi di massimo 10.
        /// </summary>
        public bool IsBatchLoading { get; set; }

        #region Costruttori
        /// <summary>
        /// Inizializza un LoadResourceItem collegato alla ResourceDirectory data.
        /// </summary>
        /// <param name="rootBound">ResourceDirectory da collegare.</param>
        public LoadResourceItem(ResourceDirectory rootBound)
        {
            if (rootBound != null)
            {
                nextBonusToLoad = null;
                nextBonusToLoad = null;
                root = rootBound;
                themes2Keep = new List<string>();
            }
        }
        #endregion

        #region Metodo per caricare tutti i file multimediali da un path
        /// <summary>
        /// Carica tutti i file bmp/png e wav/mp3 di una cartella (tale path deve già contenere il backslash finale, prima del nome del file).
        /// </summary>
        /// <param name="logicPathDest">Cartella "logica" di destinazione</param>
        /// <param name="realPathSource">Cartella di sorgente del sistema operativo (deve già contenere il "\" finale)</param>
        protected void loadMediaFiles(string logicPathDest, string realPathSource)
        {
            char[] c = new char[] { '\\' };
            string[] s = new string[] { };
            foreach (string f in GameApp.getFileNames(realPathSource))
            {
                if (f.EndsWith(".bmp") || f.EndsWith(".png"))
                    root.InsertFile(logicPathDest, f, new ImageResourceItem(realPathSource + f));
                else if (f.EndsWith(".wav") || f.EndsWith(".mp3"))
                    root.InsertFile(logicPathDest, f, new SoundResourceItem(realPathSource + f));
            }
        }
        #endregion

        #region Metodi privati per il caricamento dei livelli e dei temi
        private KulaLevel InsertLevel(ResourceInfo rInfo)
        {
            string logdir = rInfo.LogicDirectory;
            string resname = rInfo.ResourceName;
            string filename = rInfo.RealFilePath;
            if (!root.ContainsDirectory(logdir))
                root.NewDirectory(logdir);
            if (!root.ContainsFile(logdir, resname))
                root.InsertFile(logdir, resname, new LevelResourceItem(filename));
            if (root.GetFile(logdir, resname) != null)
                return ((LevelResourceItem)root.GetFile(logdir, resname)).Content;
            else
            {
                Console.Clear();
                Console.WriteLine("WARNING! A level was loaded but it still is not found!");
            }
                
            return null;
        }
        private void LoadEntireTheme(string Theme)
        {
            if (Theme != null && Theme != "")
            {
                if (GameConstraints.GameThemes.ThemeLogicDirectories.Contains(Theme))
                {
                    int i = GameConstraints.GameThemes.ThemeLogicDirectories.IndexOf(Theme);
                    string realPath = GameApp.CurDir() + GameConstraints.GameThemes.ThemePaths.ElementAt<string>(i) + "\\";
                    string logicPath = Theme;
                    if (!root.ContainsDirectory(logicPath))
                        root.NewDirectory(logicPath);
                    loadMediaFiles(logicPath, realPath);
                    if (!IsCaching)
                        AddThemeToKeep(Theme);
                }
                else
                    throw new Exception(Theme + " theme is not known!");
            }
        }
        #endregion

        #region Metodi pubblici di impostazione del LoadResourceItem
        /// <summary>
        /// Imposta il filename del successivo livello da caricare.
        /// </summary>
        /// <param name="name">Stringa che rappresenta il filename del livello successivo da caricare.</param>
        public void SetNextNormalLevelToLoad(string name)
        {
            if (name == null)
                return;
            else if (IsCaching)
                nextNormalToLoad = new ResourceInfo(
                    GameConstraints.OtherPaths.CachedLevelsDir,
                    name,
                    GameApp.CurDir() + GameConstraints.OtherPaths.Levels + "\\" + name
                    );
            else if (!IsCaching)
                nextNormalToLoad = new ResourceInfo(
                    GameConstraints.OtherPaths.PermanentLevelsDir,
                    name,
                    GameApp.CurDir() + GameConstraints.OtherPaths.Levels + "\\" + name
                    );
        }
        /// <summary>
        /// Imposta il filename del prossimo livello bonus da caricare.
        /// </summary>
        /// <param name="name">Filename del prossimo livello bonus da caricare</param>
        public void SetNextBonusLevelToLoad(string name)
        {
            if (name == null)
                return;
            else
                nextBonusToLoad = new ResourceInfo(
                    GameConstraints.OtherPaths.BonusLevelsDir,
                    name,
                    GameApp.CurDir() + GameConstraints.OtherPaths.Bonuses + "\\" + name
                    );
        }
        /// <summary>
        /// Aggiunge un tema alla lista dei temi da non scaricare durante il gioco.
        /// </summary>
        /// <param name="T"></param>
        public void AddThemeToKeep(string T)
        {
            if (GameConstraints.GameThemes.ThemeLogicDirectories.Contains(T))
                themes2Keep.Add(T);
        }
        /// <summary>
        /// Rimuove un tema alla lista dei temi da non scaricare durante il gioco.
        /// </summary>
        /// <param name="T"></param>
        public void RemoveThemeToKeep(string T)
        {
            if (themes2Keep.Contains(T))
                themes2Keep.Remove(T);
        }
        /// <summary>
        /// Svuota la lista dei temi da conservare.
        /// </summary>
        public void FlushThemesToKeep()
        {
            themes2Keep.Clear();
        }
        #endregion

        #region Metodi pubblici per processare i livelli da caricare.
        /// <summary>
        /// Carica il livello indicato dal corrente LoadResourceItem, restituisce TRUE se e solo se il livello è stato caricato
        /// correttamente E il livello ha un successore. Il LoadResourceItem, se il caricamento ha avuto buon termine, si imposta automaticamente
        /// al livello successivo (se il livello successivo non esiste, sarà necessario reimpostare il LoadResourceItem).
        /// </summary>
        /// <returns></returns>
        public bool ProcessNormalLevel()
        {
            if (nextNormalToLoad != null)
            {
                ResourceInfo rInfo = nextNormalToLoad;
                KulaLevel lv = InsertLevel(rInfo);
                string neededTheme = lv.Theme;
                if (!root.ContainsDirectory(neededTheme))
                    LoadEntireTheme(neededTheme);
                if (lv.NextLevel == "")
                {
                    nextNormalToLoad = null;
                    return false;
                }
                SetNextNormalLevelToLoad(lv.NextLevel);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Carica il livello bonus indicato dal corrente LoadResourceItem, restituisce TRUE se e solo se il livello bonus è stato caricato
        /// correttamente. Il LoadResourceItem, se il caricamento ha avuto buon termine, elimina qualsiasi riferimento al livello bonus da caricare.
        /// Sarà quindi necessario reimpostare il LoadResourceItem prima di ricaricare un bonus.
        /// </summary>
        /// <returns></returns>
        public bool ProcessBonusLevel()
        {
            if (nextBonusToLoad != null)
            {
                ResourceInfo rInfo = nextBonusToLoad;
                KulaLevel lv = InsertLevel(rInfo);
                string neededTheme = lv.Theme;
                if (!root.ContainsDirectory(neededTheme))
                    LoadEntireTheme(neededTheme);
                nextBonusToLoad = null;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Svuota i livelli e i temi memorizzati temporaneamente.
        /// </summary>
        public void FlushAllCachedLevelsAndThemes()
        {
            #region Rimuovo i livelli in cache
            root.FlushDirectory(GameConstraints.OtherPaths.CachedLevelsDir);
            #endregion
            #region Rimuovo i temi che non sono da conservare
            foreach (string theme in GameConstraints.GameThemes.ThemeLogicDirectories)
                if (!themes2Keep.Contains(theme) && root.ContainsDirectory(theme))
                    root.RemoveDirectory(theme);
            #endregion
        }
        #endregion

        #region Metodi ereditati dalla superclasse
        /// <summary>
        /// Non implementato. Inutile.
        /// </summary>
        /// <param name="Path"> </param>
        /// <returns></returns>
        public override bool Load(string Path)
        {
            Console.WriteLine("Metodo LoadResourceItem.Load(string Path) NON implementato, perché non utile");
            return false;
        }
        /// <summary>
        /// Non implementato. Inutile.
        /// </summary>
        public override void Unload()
        {
            Console.WriteLine("Metodo LoadResourceItem.Unload() NON implementato, perché non utile");
        }
        #endregion
    }
    #endregion

    #region LevelResourceItem: classe che incorpora un KulaLevel nei file "logici"
    /// <summary>
    /// Classe che incorpora un oggetto KulaLevel come una risorsa: il caricamento della risorsa è sincrono!
    /// </summary>
    public class LevelResourceItem : ResourceItem
    {
        private KulaLevel content;

        private bool load(string path)
        {
            try
            {
                Stream loading = File.OpenRead(path);
                BinaryFormatter bf = new BinaryFormatter();
                content = (KulaLevel)bf.Deserialize(loading);
                loading.Close();
                loading.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Ci sono stati problemi nel caricare e processare il livello " + path + ":\n" + e.ToString());
                return false;
            }

        }

        /// <summary>
        /// Restituisce una risorsa che contiene un livello a partire da un path di un file.
        /// </summary>
        /// <param name="path">Path del file</param>
        public LevelResourceItem(string path)
        {
            content = null;
            Load(path);
        }

        /// <summary>
        /// Carica un livello a partire da un path di un file.
        /// </summary>
        /// <param name="Path">Path del file</param>
        /// <returns></returns>
        public override bool Load(string Path)
        {
            if (content == null)
                return load(Path);
            else
            {
                Unload();
                return load(Path);
            }
        }

        /// <summary>
        /// Scarica il contenuto della risorsa
        /// </summary>
        public override void Unload()
        {
            content = null;
            GC.Collect();
        }

        /// <summary>
        /// Restituisce il livello contenuto nella risorsa
        /// </summary>
        public KulaLevel Content
        {
            get
            {
                if (content != null)
                    return content.DeepClone<KulaLevel>();
                else
                    return null;
            }
        }
    }
    #endregion

    #region HighScoresResourceItem: classe che incorpora la tabella dei punteggi alti
    public class HighScoresResourceItem : ResourceItem
    {
        private Highscores scores = null;

        public Highscores Content
        {
            get { return scores; }
        }

        public HighScoresResourceItem()
        {
            scores = Highscores.LoadHighscores();
        }
        
        public override bool Load(string Path)
        {
            scores = Highscores.LoadHighscores();
            return true;
        }

        public override void Unload()
        {
            //Non fa nulla
        }
    }
    #endregion
}
