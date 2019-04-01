using ResourcesBasics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UIEssentials
{

    #region SceneContainer: rappresenta una collezione di scene che dispatcha i metodi Draw e Update alla scena ritenuta attiva in quel momento.
    /// <summary>
    /// Questa classe rappresenta una collezione di scene che dispatcha i metodi Draw e Update alla scena ritenuta attiva in quel momento.
    /// Il cambio di scena attiva blocca automaticamente la scena uscente e fa partire la scena selezionata. Le scene devono essere associate ad un ID.
    /// La collezione di scene dev'essere avviata per avviare le scene.
    /// Inoltre è contenuto un ResourceDirectory in cui ogni IIndipendentlyAnimable può accedere per caricare e scaricare risorse.
    /// </summary>
    public class SceneContainer
    {
        /*Variabili di istanza:
            * scenes: collezioni di coppie <ID, scena>
            * selectedID: ID della scena selezionata
            * hasBegun: booleano per dire se 
            */
        private SortedDictionary<byte, IIndipendentlyAnimable> scenes;
        private byte selectedID;
        private bool hasBegun;
        private int soundFX;
        private int musicVOL;
        private ResourceDirectory dir;
        private GameViewport gameVContainer;

        /// <summary>
        /// Inizializza un contenitore di scene vuoto a partire dalla GameViewport che lo conterrà.
        /// </summary>
        public SceneContainer(GameViewport vp)
        {
            soundFX = 50;
            musicVOL = 50;
            this.gameVContainer = vp;
            this.scenes = new SortedDictionary<byte, IIndipendentlyAnimable>();
            this.dir = new ResourceDirectory();
            this.hasBegun = false;
        }

        /// <summary>
        /// Inizializza un contenitore di scene basandosi su una lista di coppie (ID, Scena) a partire dalla GameViewport che lo conterrà.
        /// </summary>
        /// <param name="list">Lista di coppie (ID, Scena) (List(KeyValuePair(byte, IIndipendentlyAnimable)))</param>
        /// <param name="vp">Viewport a cui collegare il contenitore della scena</param>
        public SceneContainer(GameViewport vp, List<KeyValuePair<byte, IIndipendentlyAnimable>> list)
        {
            soundFX = 50;
            musicVOL = 50;
            this.gameVContainer = vp;
            this.scenes = new SortedDictionary<byte, IIndipendentlyAnimable>();
            this.dir = new ResourceDirectory();
            if (list != null)
            {
                foreach (KeyValuePair<byte, IIndipendentlyAnimable> pair in list)
                    scenes.Add(pair.Key, pair.Value);
            }
            this.hasBegun = false;
        }

        /// <summary>
        /// Aggiunge una coppia (ID, Scena) alla collezione, se la chiave ID è già presente, la modifica non viene eseguita.
        /// </summary>
        /// <param name="ID">Chiave associata alla scena</param>
        /// <param name="scene">Scena da inserire</param>
        public void addScene(byte ID, IIndipendentlyAnimable scene)
        {
            if ((!scenes.ContainsKey(ID)) && (scene != null))
            {
                if (scenes.Count == 0)
                    selectedID = ID;
                scenes.Add(ID, scene);
                scene.Container = this;
            }
        }

        /// <summary>
        /// Metodo di comodo per restituire la scena associata all'ID
        /// </summary>
        /// <param name="ID">ID associato alla coppia</param>
        /// <returns>La scena, se questa esiste, null altrimenti</returns>
        private IIndipendentlyAnimable getScene(byte ID)
        {
            IIndipendentlyAnimable value;
            if (scenes.TryGetValue(ID, out value))
                return value;
            else
                return null;
        }

        /// <summary>
        /// Restituisce la directory di risorse associata al contenitore di scene.
        /// </summary>
        public ResourceDirectory getResourceDirectory
        {
            get { return dir; }
        }

        /// <summary>
        /// Rimuove la coppia identificata da ID, se tale coppia non esiste, la modifica non ha effetto.
        /// Se si rimuove la scena attiva, questa viene bloccata ed il contenitore viene fermato
        /// </summary>
        /// <param name="ID">Identificatore per la coppia</param>
        /// <returns>true, se è necessario far ripartire il contenitore di scene, false se non è necessario</returns>
        public bool removeScene(byte ID)
        {
            if (scenes.ContainsKey(ID))
            {
                if (ID == selectedID)
                {
                    hasBegun = false;
                    this.getScene(ID).Pause();
                    if (scenes.Count >= 2)
                    {
                        selectedID = scenes.Keys.Min<byte>();
                        this.getScene(selectedID).Play();
                    }
                    this.getScene(ID).Dispose();
                    scenes.Remove(ID);

                    return true;
                }
                else
                {
                    scenes.Remove(ID);
                }
            }
            return false;
        }

        /// <summary>
        /// Cambia la scena attiva; se il cambio è effettivo, la scena uscente viene fermata e quella entrante viene avviata.
        /// </summary>
        /// <param name="ID">ID associato alla scena</param>
        public void changeScene(byte ID)
        {
            IIndipendentlyAnimable value;
            if (scenes.TryGetValue(ID, out value) && (selectedID != ID))
            {
                this.getScene(selectedID).Pause();
                selectedID = ID;
                this.getScene(selectedID).Play();
            }
        }

        /// <summary>
        /// Restituisce l'ID della scena attiva.
        /// </summary>
        public byte CurrentSceneID
        {
            get { return selectedID; }
        }

        /// <summary>
        /// Avvia il contenitore di scene, se questo non è vuoto.
        /// </summary>
        public void Begin()
        {
            if (scenes.Count != 0 && !hasBegun)
                this.getScene(selectedID).Play();
            hasBegun = true;
        }

        /// <summary>
        /// Ferma l'animazione della scena corrente.
        /// </summary>
        public void Pause()
        {
            if (hasBegun)
            {
                hasBegun = false;
                this.getScene(selectedID).Pause();
            }

        }

        /// <summary>
        /// Dispatcha il metodo Update alla scena avviata.
        /// </summary>
        public void Update()
        {
            IIndipendentlyAnimable scene = this.getScene(selectedID);
            if (scene != null)
                scene.Update();
        }

        /// <summary>
        /// Dispatcha il metodo Draw alla scena avviata.
        /// </summary>
        /// <param name="e">Argomento per l'evento Paint</param>
        public void Draw(Graphics e)
        {
            IIndipendentlyAnimable scene = this.getScene(selectedID);
            if (scene != null)
                scene.Draw(e);
        }

        /// <summary>
        /// Dispatcha la gestione della coda degli eventi alla scena attiva, se c'è.
        /// </summary>
        /// <param name="bundle">Coda degli eventi passata al gestore.</param>
        public void HandleEvent(KeyboardMouseEventBundle bundle)
        {
            IIndipendentlyAnimable scene = this.getScene(selectedID);
            if (scene != null)
                scene.HandleEvent(bundle);
            else
                Console.WriteLine("Il bundle sembra vuoto...");
        }
        /// <summary>
        /// Modifica il massimo numero di fotogrammi al secondo del gioco.
        /// </summary>
        /// <param name="fps"></param>
        public void ChangeFPSCap(byte fps)
        {
            gameVContainer.FramesPerSecondCap = fps;
        }
        /// <summary>
        /// Imposta o restituisce il volume per gli effetti sonori.
        /// </summary>
        public int VolumeFX
        {
            get { return soundFX; }
            set
            {
                if (value >= 0 && value <= 100)
                    soundFX = value;
            }
        }

        /// <summary>
        /// Imposta o restituisce il volume per la musica.
        /// </summary>
        public int VolumeMusic
        {
            get { return musicVOL; }
            set
            {
                if (value >= 0 && value <= 100)
                    musicVOL = value;
            }
        }
    }
    #endregion

}
