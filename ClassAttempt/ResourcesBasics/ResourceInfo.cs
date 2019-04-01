namespace ResourcesBasics
{
    #region ResourceInfo: classe che incorpora tutte le informazioni per il caricamento di una singola risorsa
    /// <summary>
    /// ResourceInfo: classe che incorpora tutte le informazioni per il caricamento di una singola risorsa
    /// </summary>
    public class ResourceInfo
    {
        private string LogDir;
        private string ResName;
        private string FilePath;

        /// <summary>
        /// Restituisce un oggetto resourceinfo.
        /// </summary>
        /// <param name="logicdir">Nome della directory logica.</param>
        /// <param name="resourcename">Nome della risorsa da inserire.</param>
        /// <param name="filepath">Nome intero (path + filename) del file da inserire come risorsa.</param>
        public ResourceInfo(string logicdir, string resourcename, string filepath)
        {
            LogDir = logicdir;
            ResName = resourcename;
            FilePath = filepath;
        }

        /// <summary>
        /// Restituisce il nome della directory logica in cui si vuole inserire la risorsa.
        /// </summary>
        public string LogicDirectory
        {
            get { return LogDir; }
        }

        /// <summary>
        /// Restituisce il nome della risorsa che si vuole inserire.
        /// </summary>
        public string ResourceName
        {
            get { return ResName; }
        }

        /// <summary>
        /// Restituisce il nome completo (path + filename) del file da inserire nella risorsa.
        /// </summary>
        public string RealFilePath
        {
            get { return FilePath; }
        }

    }
    #endregion
}
