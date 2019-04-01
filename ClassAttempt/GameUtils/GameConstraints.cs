using System.Collections.Generic;

namespace GameUtils
{
    #region FileNames: classe statica che restituisce il nome di file "notevoli"
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
}
