using GameUtils;
using MultimediaClasses;
using ResourcesBasics;
using ResourceItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIEssentials
{
    #region KeyboardMouseEventID: Enumerazione per simbolizzare gli eventi ascoltabili dal gioco.
    /// <summary>
    /// Enumerazione che serve a simbolizzare gli eventi ascoltabili dal gioco
    /// </summary>
    public enum KeyboardMouseEventID : byte
    {
        /// <summary>
        /// Tasto del mouse premuto
        /// </summary>
        Mouse_Down,
        /// <summary>
        /// Tasto del mouse rilasciato
        /// </summary>
        Mouse_Up,
        /// <summary>
        /// Rotellina del mouse mossa
        /// </summary>
        Mouse_Wheel,
        /// <summary>
        /// Mouse mosso
        /// </summary>
        Mouse_Move,
        /// <summary>
        /// Tasto della tastiera premuto
        /// </summary>
        Key_Down,
        /// <summary>
        /// Tasto della tastiera rilasciato
        /// </summary>
        Key_Up
    }
    #endregion

    #region KeyboardMouseEventArgs: Wrapper che ingloba MouseEventArgs o KeyEventArgs
    /// <summary>
    /// Questa classe è un wrapper che ingloba MouseEventArgs o KeyEventArgs
    /// </summary>
    public class KeyboardMouseEventArgs
    {
        /*Variabili di istanza:
         * kEvt = KeyEventArgs contenuto nel wrapper.
         * mEvt = MouseEventArgs contenuto nel wrapper.
         * isKeyEvt = booleano per decidere se il wrapper è stato inizializzato con un KeyEventArgs.
         */
        private KeyEventArgs kEvt;
        private MouseEventArgs mEvt;
        private KeyboardMouseEventID type;
        private bool isKeyEvt;

        /// <summary>
        /// Inizializza il wrapper con un KeyEventArgs e un ID che ne identifica il tipo.
        /// </summary>
        /// <param name="k">KeyEventArgs da inserire nel wrapper.</param>
        /// <param name="ID">Identificatore del tipo dell'evento inserito.</param>
        public KeyboardMouseEventArgs(KeyEventArgs k, KeyboardMouseEventID ID)
        {
            this.kEvt = k;
            this.type = ID;
            this.mEvt = null;
            this.isKeyEvt = true;
        }

        /// <summary>
        /// Inizializza il wrapper con un MouseEventArgs e un ID che ne identifica il tipo.
        /// </summary>
        /// <param name="m">MouseEventArgs da inserire nel wrapper.</param>
        /// <param name="ID">Identificatore del tipo dell'evento inserito.</param>
        public KeyboardMouseEventArgs(MouseEventArgs m, KeyboardMouseEventID ID)
        {
            this.mEvt = m;
            this.type = ID;
            this.kEvt = null;
            this.isKeyEvt = false;
        }

        /// <summary>
        /// Restituisce il tipo di evento contenuto.
        /// </summary>
        public KeyboardMouseEventID getEventType
        {
            get { return type; }
        }

        /// <summary>
        /// Identifica il tipo dell'evento contenuto. Restituisce true se l'evento contenuto è un keyevent.
        /// </summary>
        public bool isKeyEvent
        {
            get { return isKeyEvt; }
        }

        /// <summary>
        /// Metodo che indica se il KeyEventArgs contenuto è null o meno.
        /// </summary>
        /// <returns>true se il KeyEventArgs è null, false altrimenti.</returns>
        public bool isNullKeyEvent()
        {
            return (kEvt == null);
        }

        /// <summary>
        /// Metodo che indica se il MouseEventArgs contenuto è null o meno.
        /// </summary>
        /// <returns>true se il MouseEventArgs è null, false altrimenti.</returns>
        public bool isNullMouseEvent()
        {
            return (mEvt == null);
        }

        /// <summary>
        /// Restituisce il KeyEventArgs contenuto (può essere null).
        /// </summary>
        public KeyEventArgs keyEvent
        {
            get { return kEvt; }
        }

        /// <summary>
        /// Restituisce il MouseEventArgs contenuto (può essere null).
        /// </summary>
        public MouseEventArgs mouseEvent
        {
            get { return mEvt; }
        }
    }
    #endregion

    #region KeyboardMouseEventBundle: una coda FIFO di eventi che possono essere relativi al mouse oppure alla tastiera.
    /// <summary>
    /// Questa classe è una coda FIFO di eventi che possono essere relativi al mouse oppure alla tastiera.
    /// Si può aggiungere eventi in coda e estrarli dalla testa.
    /// </summary>
    public class KeyboardMouseEventBundle
    {
        private List<KeyboardMouseEventArgs> evtList;

        /// <summary>
        /// Inizializza una nuova coda vuota di eventi.
        /// </summary>
        public KeyboardMouseEventBundle()
        {
            this.evtList = new List<KeyboardMouseEventArgs>();
        }

        /// <summary>
        /// Aggiunge un evento in fondo alla coda.
        /// </summary>
        /// <param name="e">Evento da inserire.</param>
        public void addEvent(KeyboardMouseEventArgs e)
        {
            if ((e != null) && (!e.isNullKeyEvent() || !e.isNullMouseEvent()))
                evtList.Add(e);
        }

        /// <summary>
        /// Estrae l'evento in testa alla coda. Può restituire pure null.
        /// </summary>
        /// <returns>Un KeyboardEventArgs, se la coda non è vuota, null altrimenti.</returns>
        public KeyboardMouseEventArgs extractEvent()
        {
            if (evtList.Count != 0)
            {
                KeyboardMouseEventArgs output = evtList.ElementAt<KeyboardMouseEventArgs>(0);
                evtList.RemoveAt(0);
                return output;
            }
            else
                return null;
        }
    }
    #endregion

    #region MenuItem: classe basilare per costruire Menù a gerarchia.
    /// <summary>
    /// MenuItem rappresenta una classe basilare per costruire Menù a gerarchia: MenuItem è una singola voce del Menù e
    /// può contenere riferimenti a eventuali elementi figli. Con tale MenuItem è possibile consultare i riferimenti ai
    /// figli, il riferimento all'eventuale padre, modificare valori associati alla singola voce e aggiungere altri figli.
    /// </summary>
    public class MenuItem
    {
        /*Variabili di istanza:
         children = Lista ai figli
         father = riferimento possibilmente nullo all'item padre
         descr = descrizione testuale associata all'item
         boolVal = valore booleano associato all'item
         intVal = valore intero associato all'item
         doubleVal = valore a doppia precisione associato all'item
         */
        private List<MenuItem> children;
        private MenuItem father;
        private string descr;
        private bool boolVal;
        private IntSlider intVal;
        private DoubleSlider doubleVal;
        private bool editable;
        private int typePtr = 2;

        /// <summary>
        /// Crea un MenuItem senza specificare nessun valore associato, i quali saranno impostati a valori di default, cioè:
        /// (description = ""; booleanValue = false; int32Value = 0; doubleValue = 0.0)
        /// </summary>
        /// <param name="f">Riferimento al MenuItem padre (può essere pure null)</param>
        public MenuItem(MenuItem f)
        {
            this.children = new List<MenuItem>();
            this.father = f;
            this.descr = "";
            this.boolVal = false;
            this.editable = false;
            this.intVal = new IntSlider(-1, 1, 2);
            this.doubleVal = new DoubleSlider(-1, 1, 2);
        }
        /// <summary>
        /// Crea un MenuItem senza specificare il riferimento al padre, che sarà null.
        /// </summary>
        /// <param name="s">Stringa associata al MenuItem</param>
        /// <param name="b">Booleano associato al MenuItem</param>
        /// <param name="i">Intero 32b associato al MenuItem</param>
        /// <param name="d">Numero a doppia precisione associato al MenuItem</param>
        public MenuItem(string s, bool b, IntSlider i, DoubleSlider d)
        {
            this.children = new List<MenuItem>();
            this.father = null;
            this.descr = s;
            this.boolVal = b;
            this.intVal = i;
            this.doubleVal = d;
            editable = (i != null) || (d != null);
        }
        /// <summary>
        /// Crea un MenuItem specificando tutto, valori e riferimento al padre.
        /// </summary>
        /// <param name="f">Riferimento al MenuItem padre (può essere pure null)</param>
        /// <param name="s">Stringa associata al MenuItem</param>
        /// <param name="b">Booleano associato al MenuItem</param>
        /// <param name="i">Intero 32b associato al MenuItem</param>
        /// <param name="d">Numero a doppia precisione associato al MenuItem</param>
        public MenuItem(MenuItem f, string s, bool b, IntSlider i, DoubleSlider d)
        {
            this.children = new List<MenuItem>();
            this.father = f;
            this.descr = s;
            this.boolVal = b;
            this.intVal = i;
            this.doubleVal = d;
            editable = (i != null) || (d != null);
        }

        /// <summary>
        /// Imposta o restituisce la descrizione testuale dell'elemento del menù.
        /// </summary>
        public string description
        {
            get { return descr; }
            set { descr = value; }
        }
        /// <summary>
        /// Imposta o restituisce lo slider del valore intero associato all'elemento del menù.
        /// </summary>
        public IntSlider int32Value
        {
            get { return intVal; }
        }
        /// <summary>
        /// Imposta o restituisce lo slider del valore a doppia precisione associato all'elemento del menù.
        /// </summary>
        public DoubleSlider doublePValue
        {
            get { return doubleVal; }
        }
        /// <summary>
        /// Imposta o restituisce il valore booleano associato all'elemento del menù.
        /// </summary>
        public bool booleanValue
        {
            get { return boolVal; }
            set { boolVal = value; }
        }
        /// <summary>
        /// Restituisce il riferimento all'elemento padre. Se tale riferimento non esiste, viene restituito null
        /// </summary>
        public MenuItem fatherItem
        {
            get { return father; }
        }
        /// <summary>
        /// Restituisce una lista di riferimenti ai figli del MenuItem.
        /// </summary>
        /// <returns>Lista di riferimenti ai figli del MenuItem (lista di MenuItem)</returns>
        public List<MenuItem> getChildren()
        {
            List<MenuItem> returnedCopy = new List<MenuItem>();
            foreach (MenuItem child in children)
                returnedCopy.Add(child);
            return returnedCopy;
        }
        /// <summary>
        /// Inserisce, come figlio, una DEEP copy del MenuItem specificato.
        /// </summary>
        /// <param name="child">MenuItem da inserire nella gerarchia come figlio.</param>
        public void appendChild(MenuItem child)
        {
            if (child != null)
            {
                IntSlider iS;
                DoubleSlider dS;
                if ((iS = child.int32Value) != null)
                    iS = iS.Clone();
                if ((dS = child.doublePValue) != null)
                    dS = dS.Clone();
                children.Add(new MenuItem(this, child.description, child.booleanValue, iS, dS));
            }
        }
        /// <summary>
        /// Aumenta di uno step il valore intero associato.
        /// </summary>
        /// <returns>Restituisce il valore intero correntemente associato all'item.</returns>
        public int? increaseIntValue()
        {
            if (intVal != null)
            {
                intVal.IncreaseValue();
                return intVal.Value;
            }
            else
                return null;
        }
        /// <summary>
        /// Decrementa di uno step il valore intero associato.
        /// </summary>
        /// <returns>Restituisce il valore intero correntemente associato all'item.</returns>
        public int? decreaseIntValue()
        {
            if (intVal != null)
            {
                intVal.DecreaseValue();
                return intVal.Value;
            }
            else return null;
        }
        /// <summary>
        /// Inverte il valore booleano associato.
        /// </summary>
        /// <returns>Restituisce il valore booleano correntemente associato all'item.</returns>
        public bool switchBoolValue()
        {
            boolVal = !boolVal;
            return boolVal;
        }
        /// <summary>
        /// Aumenta di uno step il valore a doppia precisione associato.
        /// </summary>
        /// <returns>Restituisce il valore a doppia precisione correntemente associato all'item.</returns>
        public double? increaseDoubleValue()
        {
            if (doubleVal != null)
            {
                doubleVal.IncreaseValue();
                return doubleVal.Value;
            }
            else return null;
        }
        /// <summary>
        /// Decrementa di uno step il valore a doppia precisione associato.
        /// </summary>
        /// <returns>Restituisce il valore a doppia precisione correntemente associato all'item.</returns>
        public double? decreaseDoubleValue()
        {
            if (doubleVal != null)
            {
                doubleVal.DecreaseValue();
                return doubleVal.Value;
            }
            else return null;
        }
        /// <summary>
        /// Imposta o restituisce se gli slider sono significativi o meno
        /// </summary>
        public bool isEditable
        {
            set { editable = value; }
            get { return editable; }
        }
        /// <summary>
        /// Imposta il tipo di valore significativo (scelto tra bool, double, integer)
        /// </summary>
        /// <param name="type">intero che indica: 0 per boolean, 1 per double, 2 per integer</param>
        public void setValueType(int type)
        {
            if (type >= 0 && type <= 2)
                this.typePtr = type;
        }

        /// <summary>
        /// Converte il menuitem in stringa.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            MenuItem i = this;
            string testo = "";
            if (i.isEditable)
            {
                if (this.typePtr == 0)
                    testo = i.description + ": " + i.booleanValue.ToString();
                else if (this.typePtr == 1)
                    testo = i.description + ": " + i.doublePValue.Value.ToString();
                else if (this.typePtr == 2)
                    testo = i.description + ": " + i.int32Value.Value.ToString();
            }
            else
                testo = i.description;
            return testo;
        }
    }
    #endregion

    #region IIndipendentlyAnimable: rappresenta degli oggetti animabili indipendentemente (hanno la loro timeline indipendente).
    /// <summary>
    /// Questa classe rappresenta degli oggetti animabili indipendentemente (hanno la loro timeline indipendente).
    /// </summary>
    public abstract class IIndipendentlyAnimable
    {
        /*COMPLETE THIS SHIT!!!*/
        /// <summary>
        /// Timer per temporizzare la scena
        /// </summary>
        protected Stopwatch timer;
        /// <summary>
        /// Strati di disegno della scena
        /// </summary>
        protected List<Bitmap> layers;
        /// <summary>
        /// Riferimento al contenitore della scena
        /// </summary>
        protected SceneContainer container;
        /// <summary>
        /// Rettangolo di clipping per il disegno della scena
        /// </summary>
        protected Rectangle clipRegion;

        /// <summary>
        /// Avvia la timeline dell'animazione dell'oggetto.
        /// </summary>
        abstract public void Play();

        /// <summary>
        /// Serve per fermare la timeline dell'oggetto.
        /// </summary>
        abstract public void Pause();

        /// <summary>
        /// Serve a resettare lo stato dell'oggetto.
        /// </summary>
        abstract public void Reset();

        /// <summary>
        /// Serve a eliminare le risorse dell'oggetto.
        /// </summary>
        virtual public void Dispose() { }

        /// <summary>
        /// Disegna l'oggetto in un dato momento. (Si consiglia, in caso di subclassing, di chiamare base.Draw(e), se si vuole
        /// ricordare la regione di clipping del contesto grafico in input).
        /// </summary>
        /// <param name="e">PaintEventArgs e: argomenti dell'evento Paint (da cui ricavare il contesto grafico)</param>
        virtual public void Draw(Graphics e)
        {
            RectangleF fpBounds = e.ClipBounds;
            int x = (int)fpBounds.Width;
            int y = (int)fpBounds.Height;
            clipRegion = new Rectangle(0, 0, x, y);
        }

        /// <summary>
        /// Aggiorna lo stato dell'oggetto.
        /// </summary>
        abstract public void Update();

        /// <summary>
        /// Gestisce la coda di eventi passata come parametro, modificandola.
        /// </summary>
        /// <param name="bundle">Coda di eventi passata al gestore</param>
        abstract public void HandleEvent(KeyboardMouseEventBundle bundle);

        /// <summary>
        /// Imposta o restituisce lo scenecontainer a cui si fa riferimento.
        /// </summary>
        public SceneContainer Container
        {
            get { return container; }
            set { container = value; }
        }

        /// <summary>
        /// Carica tutti i file bmp/png e wav/mp3 di una cartella (tale path deve già contenere il backslash finale, prima del nome del file).
        /// </summary>
        /// <param name="logicPathDest">Cartella "logica" di destinazione</param>
        /// <param name="realPathSource">Cartella di sorgente del sistema operativo (deve già contenere il "\" finale)</param>
        protected void loadMediaFiles(string logicPathDest, string realPathSource)
        {
            ResourceDirectory root = container.getResourceDirectory;
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
    }
    #endregion

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

    #region GameViewport: Controllo grafico che rappresenta la viewport del gioco.
    /// <summary>
    /// Questa classe rappresenta la viewport del gioco. Contiene uno SceneContainer che può essere manipolato direttamente.
    /// Inoltre, è possibile fermare la viewport e farla ripartire. Dispatcha gli eventi che riceve allo SceneContainer.
    /// </summary>
    public class GameViewport : Control
    {
        /* Variabili di istanza:
            * fpsRate = numero massimo di fotogrammi elaborati al secondo
            * scenes = SceneContainer utilizzato e manipolabile
            * isFrozen = variabile per far fermare il gameloop
            * timer = timer utilizzato per il "benchmark"
            * mspF = valore in funzione di fpsRate (= 1000 / fpsRate)
            */
        private byte fpsRate;
        private SceneContainer scenes;
        private bool isFrozen;
        private Stopwatch timer;
        private int mspF;
        private KeyboardMouseEventBundle queue;

        /// <summary>
        /// Handler sugli eventi Paint. Dispatchato allo SceneContainer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            this.scenes.Draw(g);
        }

        /*Dispatching degli eventi:
         * - mousemove
         * - mousedown
         * - mouseup
         * - mousewheel
         * - keydown
         * - keyup
         */
        /// <summary>
        /// Gestisce il movimento della rotella del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Mouse_Move);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce il rilascio di un tasto del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Mouse_Down);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce la pressione di un tasto del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Mouse_Up);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce il movimento della rotella del mouse
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Mouse_Wheel);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce il rilascio di un tasto della tastiera
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Key_Down);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }
        /// <summary>
        /// Gestisce la pressione di un tasto della tastiera
        /// </summary>
        /// <param name="e"> </param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            KeyboardMouseEventArgs a = new KeyboardMouseEventArgs(e, KeyboardMouseEventID.Key_Up);
            queue.addEvent(a);
            scenes.HandleEvent(this.queue);
        }

        /// <summary>
        /// Effettua il setup del controllo grafico.
        /// </summary>
        /// <param name="FPS">Massima quantità fissata di frame disegnati al secondo</param>
        private void setup(byte FPS)
        {
            /* Impostazioni come controllo grafico:
                * Metto il double buffering, così da ridurre il flickering
                * Rendo il controllo tale da riempire tutto il clientsize del padre (client size = spazio dove mettere i controlli figli)
                * Aggiungo un colore solido al background
                */
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.ControlDarkDark;

            /* Impostazioni come GameViewport:
                * Setting del Frame Per Second Rate
                * Setting del Milliseconds Per Frame (in funzione del FPSRate)
                */
            if (FPS <= 10)
                FPS = 10;
            this.fpsRate = FPS;
            this.mspF = 1000 / this.fpsRate;

            //Creo il cronometro utile per la temporizzazione.
            this.timer = new Stopwatch();

            //Creo la coda di eventi "mouse e keyboard"
            this.queue = new KeyboardMouseEventBundle();
        }

        /// <summary>
        /// Costruisce la viewport precisando i FPS e lo SceneContainer da utilizzare.
        /// </summary>
        /// <param name="FPS">Numero massimo di fotogrammi processati al secondo</param>
        /// <param name="sList">SceneContainer da utilizzare, se è null se ne istanzia uno vuoto</param>
        public GameViewport(byte FPS, SceneContainer sList)
        {
            this.setup(FPS);
            if (sList == null)
                this.scenes = new SceneContainer(this);
            else
                this.scenes = sList;
            this.isFrozen = false;
        }

        /// <summary>
        /// Restituisce lo SceneContainer che può essere manipolato.
        /// </summary>
        public SceneContainer sceneList
        {
            get { return this.scenes; }
        }

        /// <summary>
        /// Imposta o restituisce il numero massimo di fotogrammi processati in un secondo.
        /// </summary>
        public byte FramesPerSecondCap
        {
            get { return fpsRate; }
            set
            {
                byte temp = value;
                if (value <= 10)
                    value = 10;
                this.fpsRate = value;
                this.mspF = 1000 / this.fpsRate;
            }
        }

        /// <summary>
        /// Ferma temporaneamente l'intera elaborazione del gioco.
        /// </summary>
        public void Freeze()
        {
            this.isFrozen = true;
            this.scenes.Pause();
            this.timer.Stop();
        }

        /// <summary>
        /// Fa avviare o riavviare l'elaborazione del gioco: è qui che è contenuto il game loop.
        /// </summary>
        public void Go()
        {
            #region Variabili per il calcolo del delta
            long cur = 0;
            long delta = 0;
            long now = 0;
            #endregion
            #region Variabili per la diagnostica
            double cycles = 0;
            double seconds = 0;
            long maxTime = 0;
            long minTime = long.MaxValue;
            #endregion
            this.timer.Start();
            this.isFrozen = false;
            this.scenes.Begin();
            #region GameLoop: input, update, draw
            while (!this.isFrozen)
            {
                cur = this.timer.ElapsedMilliseconds;
                Application.DoEvents();
                scenes.Update();
                this.Invalidate();
                now = this.timer.ElapsedMilliseconds;
                delta = now - cur;
                cur = now;
                if (delta < this.mspF)
                    Thread.Sleep(this.mspF - (int)delta);
                #region Parte di diagnostica
                if (++cycles % 14000 == 0)
                    GC.Collect();
                if (delta > maxTime)
                    maxTime = delta;
                if (delta < minTime)
                    minTime = delta;
                #endregion
            }
            #endregion
            #region Messaggio di diagnostica dei cicli medi al secondo
            seconds = cur / 1000.0;
            cycles /= seconds;
            MessageBox.Show("Avg Cycles per second: " + cycles + "\nSlowest cycle: " + maxTime + "ms\nFastest cycle: " + minTime + "ms.", "Game Loop Diagnostics");
            #endregion
        }
    }
    #endregion 
}
