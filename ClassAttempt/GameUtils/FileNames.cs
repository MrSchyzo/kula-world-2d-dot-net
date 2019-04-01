using System.Collections.Generic;

namespace GameUtils
{
    #region MatrixUtils: classe statica con metodi di utilità per le trasformazioni matriciali sui punti
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
}
