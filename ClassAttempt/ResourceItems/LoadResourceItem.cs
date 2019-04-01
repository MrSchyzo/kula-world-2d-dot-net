using GameUtils;
using LevelsStructure;
using ResourcesBasics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ResourceItems
{
    #region SoundResourceItem: classe che incorpora dei file sonori in file "logici" (N.B. Per ogni SMP c'è un thread a parte)
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
}
