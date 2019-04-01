using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ResourcesBasics
{
    #region ResourceItem: Classe basilare per creare file "logici"
    #endregion

    #region ResourceDirectory: Classe che implementa un meccanismo non gerarchico di directory logiche che conterranno ResourceItem
    /// <summary>
    /// Questa classe crea una Directory "Logica" delle risorse condivisibili dalle varie scene.
    /// È possibile creare nuove Folder all'interno di questa classe, e, su ogni folder, è possibile aggiungere o rimuovere risorse.
    /// </summary>
    public class ResourceDirectory
    {
        /* TODO:
         */
        /* Tipi di risorse:
         * - Wav e SoundPlayer associato [V]
         * - Bmp e Bitmap associata [V]
         * - kwl e LevelInfo(parte mappa) associato [?]
         * - xml e LevelInfo(parte metadati) associato [?]
         * - xml e PlayerInfo associato [?]
         */
        /* Organizzazione:
         * - Accessibilità da ogni IIndipendentlyAnimable [V]
         * - Possibilità di aggiungere e rimuovere roba [V]
         * - Cartelle logiche [V]
         */
        SortedDictionary<string, SortedDictionary<string, ResourceItem>> root;

        /// <summary>
        /// Svuota una directory.
        /// </summary>
        /// <param name="Dir">Identifica la directory.</param>
        public void FlushDirectory(string Dir)
        {
            SortedDictionary<string, ResourceItem> directory;
            if (root.TryGetValue(Dir, out directory))
            {
                List<ResourceItem> l = directory.Values.ToList<ResourceItem>();
                foreach (ResourceItem r in l)
                    r.Unload();
                directory.Clear();
            }
            else
                throw new Exception("The directory named " + Dir + " doesn't exist.");
        }

        /// <summary>
        /// Rimuove una directory.
        /// </summary>
        /// <param name="Dir">Identifica la directory.</param>
        private void removeDirectory(string Dir)
        {
            root.Remove(Dir);
        }

        /// <summary>
        /// Inizializza una ResourceDirectory vuota.
        /// </summary>
        public ResourceDirectory()
        {
            root = new SortedDictionary<string, SortedDictionary<string, ResourceItem>>();
        }

        /// <summary>
        /// Crea una directory vuota con l'identificativo dato (se già non è stato usato).
        /// </summary>
        /// <param name="Dir">Stringa che identifica la directory.</param>
        /// <returns>true se e solo se la creazione ha avuto successo.</returns>
        public bool NewDirectory(string Dir)
        {
            if (!root.ContainsKey(Dir) && (Dir != null))
            {
                root.Add(Dir, new SortedDictionary<string, ResourceItem>());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Elimina la directory indicata con tutto il contenuto.
        /// </summary>
        /// <param name="Dir">Stringa che identifica la directory.</param>
        /// <returns>true se e solo se l'eliminazione ha avuto successo.</returns>
        public bool RemoveDirectory(string Dir)
        {
            if (root.ContainsKey(Dir) && (Dir != null))
            {
                FlushDirectory(Dir);
                removeDirectory(Dir);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Inserisce e nomina una risorsa dentro una directory.
        /// </summary>
        /// <param name="directory">Stringa che identifica la directory in cui inserire la risorsa.</param>
        /// <param name="resourceName">Stringa che verrà associata alla risorsa.</param>
        /// <param name="resource">Risorsa che verrà inseria nella directory.</param>
        /// <returns>true se e solo se l'inserimento della risorsa ha avuto successo.</returns>
        public bool InsertFile(string directory, string resourceName, ResourceItem resource)
        {
            SortedDictionary<string, ResourceItem> d;
            if (root.TryGetValue(directory, out d) && (resource != null))
            {
                if (!d.ContainsKey(resourceName))
                {
                    d.Add(resourceName, resource);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Rimuove la risorsa contenuta in una directory specificata.
        /// </summary>
        /// <param name="directory">Directory specificata.</param>
        /// <param name="resourceName">Identificatore della risorsa.</param>
        /// <returns>true se e solo se la rimozione della risorsa ha avuto successo.</returns>
        public bool RemoveFile(string directory, string resourceName)
        {
            SortedDictionary<string, ResourceItem> d;
            ResourceItem r;
            if (root.TryGetValue(directory, out d))
            {
                if (d.TryGetValue(resourceName, out r))
                {
                    r.Unload();
                    d.Remove(resourceName);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Controlla se la ResourceDirectory contiene già una directory logica col nome indicato in input.
        /// </summary>
        /// <param name="directory">Nome della directory logica</param>
        /// <returns>True se e solo se la directory è già presente</returns>
        public bool ContainsDirectory(string directory)
        {
            return root.ContainsKey(directory);
        }

        /// <summary>
        /// Restituisce la risorsa all'interno della directory logica con il nome specificato in input
        /// </summary>
        /// <param name="directory">Nome della directory logica</param>
        /// <param name="resourceName">Nome della risorsa contenuta nella directory logica</param>
        /// <returns>Riferimento al ResourceItem, null soltanto se non è stata trovata alcuna risorsa</returns>
        public ResourceItem GetFile(string directory, string resourceName)
        {
            SortedDictionary<string, ResourceItem> d;
            ResourceItem r;
            if (root.TryGetValue(directory, out d))
            {
                if (d.TryGetValue(resourceName, out r))
                {
                    return r;
                }
            }
            return null;
        }

        /// <summary>
        /// Versione potenziata del metodo RemoveFile: supporta la ricerca con l'asterisco.
        /// (WARNING: Non si sa se funziona...)
        /// </summary>
        /// <param name="directory">Stringa che rappresenta il nome "parziale" della directory</param>
        /// <param name="resourceName">Stringa che rappresenta il nome "parziale" della risorsa</param>
        public void RemoveFileStar(string directory, string resourceName)
        {
            List<string> dirList = root.Keys.ToList<string>();
            List<string> resList;
            SortedDictionary<string, ResourceItem> currentDir;
            Regex starSeek = new Regex(@"\**");
            string directorySeek = starSeek.Replace(directory, @"(\w|\W)*");
            string resourceSeek = starSeek.Replace(resourceName, @"(\w|\W)*");
            Regex dirSeeker = new Regex(directorySeek);
            Regex resSeeker = new Regex(resourceSeek);
            foreach (string d in dirList)
            {
                if (dirSeeker.IsMatch(d))
                {
                    if (root.TryGetValue(d, out currentDir))
                    {
                        resList = currentDir.Keys.ToList<string>();
                        foreach (string r in resList)
                        {
                            if (resSeeker.IsMatch(r) && currentDir.ContainsKey(r))
                                RemoveFile(d, r);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metodo che restituisce true se e solo se esiste la risorsa specificata nella directory logica specificata.
        /// </summary>
        /// <param name="directory">Nome della directory logica in cui cercare il file</param>
        /// <param name="resourceName">Nome della risorsa da cercare</param>
        /// <returns></returns>
        public bool ContainsFile(string directory, string resourceName)
        {
            SortedDictionary<string, ResourceItem> d;
            bool directoryOk = root.TryGetValue(directory, out d);
            bool fileOk = false;
            if (directoryOk)
                fileOk = d.ContainsKey(resourceName);
            if (fileOk)
                Console.WriteLine(directory + " contains " + resourceName);
            else
                Console.WriteLine(directory + " doesn't contain " + resourceName);
            return fileOk;
        }

        /// <summary>
        /// Restituisce tutte le risorse all'interno della directory logica con il nome specificato in input
        /// </summary>
        /// <param name="directory">Nome della directory logica</param>
        /// <returns>Riferimento ai ResourceItem, null se la directory non esiste</returns>
        public List<ResourceItem> GetDirectoryContents(string directory)
        {
            if (root.ContainsKey(directory))
            {
                List<ResourceItem> res = new List<ResourceItem>();
                SortedDictionary<string, ResourceItem> dir;
                if (root.TryGetValue(directory, out dir))
                    res = dir.Values.ToList<ResourceItem>();
                return res;
            }
            else
                return null;
        }

        /// <summary>
        /// Restituisce i nomi di tutte le risorse all'interno della directory logica con il nome specificato in input
        /// </summary>
        /// <param name="directory">Nome della directory logica</param>
        /// <returns>Nome dei ResourceItem, null se la directory non esiste</returns>
        public List<string> GetDirectoryItemNames(string directory)
        {
            if (root.ContainsKey(directory))
            {
                List<string> res = new List<string>();
                SortedDictionary<string, ResourceItem> dir;
                if (root.TryGetValue(directory, out dir))
                    res = dir.Keys.ToList<string>();
                return res;
            }
            else
                return null;
        }
    }
    #endregion
}
