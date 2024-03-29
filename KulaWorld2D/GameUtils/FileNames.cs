﻿using System.Collections.Generic;

namespace GameUtils
{
    
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

        public static bool EndsWithIgnoreCase(this string s, string other) => s.EndsWith(other, System.StringComparison.InvariantCultureIgnoreCase);
        public static bool StartsWithIgnoreCase(this string s, string other) => s.StartsWith(other, System.StringComparison.InvariantCultureIgnoreCase);
        public static bool EqualsIgnoreCase(this string s, string other) => s.Equals(other, System.StringComparison.InvariantCultureIgnoreCase);
    }
    
}
