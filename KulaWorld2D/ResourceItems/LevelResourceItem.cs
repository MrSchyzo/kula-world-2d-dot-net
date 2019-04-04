using LevelsStructure;
using ResourcesBasics;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GameUtils;

namespace ResourceItems
{
    
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
                    return content.DeepClone();
                else
                    return null;
            }
        }
    }
    
}
