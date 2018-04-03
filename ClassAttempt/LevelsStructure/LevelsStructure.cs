using GameUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelsStructure
{
    #region KulaLevel: Classe per mantenere le informazioni statiche del livello
    /// <summary>
    /// Questa classe rappresenta un calco per creare i livelli di gioco, basandosi su due file che contengono informazioni.
    /// La classe è pure serializzabile, così è possibile salvarne lo stato.
    /// </summary>
    [Serializable()]
    public class KulaLevel
    {
        //Definizione degli orientamenti
        #region
        /// <summary>
        /// Enumera i tipi di orientamento nel gioco.
        /// </summary>
        [Serializable()]
        public enum Orientation
        {
            /// <summary>
            /// Orientamento generico verso l'alto.
            /// </summary>
            Up,
            /// <summary>
            /// Orientamento generico verso il basso.
            /// </summary>
            Down,
            /// <summary>
            /// Orientamento generico verso sinistra.
            /// </summary>
            Left,
            /// <summary>
            /// Orientamento generico verso destra.
            /// </summary>
            Right
        }
        #endregion

        //Definizione dei tipi di tile.
        #region
        /// <summary>
        /// Enumera i tipi di tile inseribili nella griglia del KulaLevel.
        /// </summary>
        public enum TileType
        {
            /// <summary>
            /// Blocco del livello.
            /// </summary>
            Block,
            /// <summary>
            /// Oggetto piazzabile nel livello (possibilmente raccoglibile dal giocatore)
            /// </summary>
            Placeable,
            /// <summary>
            /// Oggetto che interagisce negativamente con il giocatore
            /// </summary>
            Enemy
        }
        #endregion

        //Definizione dei tipi di superficie.
        #region
        /// <summary>
        /// Enumera tutti i tipi di superficie attaccabili ai blocchi.
        /// </summary>
        public enum SurfaceType
        {
            /// <summary>
            /// Nessuna superficie
            /// </summary>
            Nothing,
            /// <summary>
            /// Superficie ghiacciata
            /// </summary>
            Ice,
            /// <summary>
            /// Superficie rovente
            /// </summary>
            Fire,
            /// <summary>
            /// Superficie con spine
            /// </summary>
            Spikes,
            /// <summary>
            /// Superficie con spine temporizzate
            /// </summary>
            TimedSpikes,
            /// <summary>
            /// Superficie con teletrasporto
            /// </summary>
            Teleport,
            /// <summary>
            /// Superficie con rampa
            /// </summary>
            Ramp,
            /// <summary>
            /// Superficie con uscita
            /// </summary>
            Exit,
            /// <summary>
            /// Superficie con forzatura di salto
            /// </summary>
            NoJump
        }
        #endregion

        //Definizione delle superfici di gioco.
        #region
        /// <summary>
        /// Questa classe rappresenta una superficie del blocco.
        /// La parte del teleport può essere solo impostata via proprietà.
        /// </summary>
        [Serializable()]
        public class Surface
        {
            //Variabili private.
            #region
            private Orientation direction;
            private Surface nextTeleport;
            private Block myBlock;
            private List<Surface> sources;
            private SurfaceType type;
            private uint sPer;
            private uint sBeg;
            #endregion

            //Proprietà pubbliche (get & set)
            #region
            /// <summary>
            /// Restituisce o imposta il tipo della superficie.
            /// </summary>
            public SurfaceType Type
            {
                get { return type; }
                set
                {
                    if (type == SurfaceType.Teleport && value != type)
                    {
                        foreach (Surface s in IncomingTeleports)
                            s.NextTeleport = null;
                        this.NextTeleport.RemoveIncomingTeleport(this);
                        this.NextTeleport = null;
                    }
                    else
                        type = value;
                }
            }
            /// <summary>
            /// Restituisce o imposta il periodo delle spine ad intermittenza.
            /// </summary>
            public uint SpikesPeriod
            {
                get { return sPer; }
                set
                {
                    if (value >= 2000 && value <= 6000)
                        sPer = value;
                }
            }
            /// <summary>
            /// Imposta o restituisce il millisecondo in cui si attivano le spine ad intermittenza.
            /// </summary>
            public uint SpikesBegin
            {
                get { return sBeg; }
                set
                {
                    if (value >= 0 && value <= 2000)
                        sBeg = value;
                }
            }
            //Inizio dell'intermittenza
            /// <summary>
            /// Restituisce e imposta il prossimo teleporter. Aggiorna il riferimento source alle altre superfici.
            /// </summary>
            public Surface NextTeleport
            {
                get { return nextTeleport; }
                set
                {
                    if (nextTeleport != null)
                        nextTeleport.RemoveIncomingTeleport(this);

                    if ((value == null) || (value.Type == SurfaceType.Teleport))
                        nextTeleport = value;

                    if (nextTeleport != null)
                        value.AddIncomingTeleport(this);
                }
            }
            #endregion

            //Metodi pubblici: BindToBlock, AddSource, RemoveSource
            #region
            /// <summary>
            /// Assegna la superficie ad un blocco.
            /// </summary>
            /// <param name="b">Blocco a cui viene collegata la superficie.</param>
            public void BindToBlock(Block b)
            {
                if (myBlock == null)
                    this.myBlock = b;
            }
            /// <summary>
            /// Aggiunge una superficie come teletrasporto che punta alla superficie corrente.
            /// </summary>
            /// <param name="s">Superficie da collegare</param>
            public void AddIncomingTeleport(Surface s)
            {
                if (s != null && s.Type == SurfaceType.Teleport)
                    sources.Add(s);
            }
            /// <summary>
            /// Scollega una superficie dalla lista dei teletrasporti collegati alla superficie corrente.
            /// </summary>
            /// <param name="s">Superficie da scollegare</param>
            public void RemoveIncomingTeleport(Surface s)
            {
                if (s != null && s.Type == SurfaceType.Teleport)
                    sources.Remove(s);
            }
            /// <summary>
            /// Svuota la lista di teletrasporti che si collegano alla superficie corrente.
            /// </summary>
            public void FlushIncomingTeleport()
            {
                sources.Clear();
            }
            #endregion

            //Costruttori
            #region
            /// <summary>
            /// Crea una superficie da allegare ad un blocco. La parte del teleport può essere solo impostata via proprietà.
            /// </summary>
            /// <param name="type">Specifica il tipo della superficie.</param>
            /// <param name="direction">Specifica l'orientamento della superficie (In che direzione si trova il blocco su cui poggia.)</param>
            /// <param name="period">Periodo di ripetizione (non significativo se non è una superficie con spine ad intermittenza)</param>
            /// <param name="begin">Millisecondo in cui partono le spine (non significativo se non è una superficie con spine ad intermittenza)</param>
            public Surface(SurfaceType type, Orientation direction, uint period, uint begin)
            {
                this.type = type;
                this.SpikesPeriod = period;
                this.SpikesBegin = begin;
                this.direction = direction;
                this.sources = new List<Surface>();
            }
            /// <summary>
            /// Crea una superficie da allegare ad un blocco in maniera standard.
            /// </summary>
            /// <param name="direction">Specifica l'orientamento della superficie (In che direzione si trova il blocco su cui poggia.)</param>
            public Surface(Orientation direction)
            {
                this.type = SurfaceType.Nothing;
                this.SpikesPeriod = 2000;
                this.SpikesBegin = 0;
                this.direction = direction;
                this.sources = new List<Surface>();
            }
            #endregion

            //Proprietà solo get
            #region
            /// <summary>
            /// Indica la direzione in cui si trova il blocco su cui questa superficie poggia (dove è per la superficie il "basso"?).
            /// </summary>
            public Orientation Orientation
            {
                get { return direction; }
            }

            /// <summary>
            /// Restituisce il blocco a cui la superficie corrente è collegata.
            /// </summary>
            public Block BindingBlock
            {
                get { return myBlock; }
            }

            /// <summary>
            /// Restituisce l'insieme delle superfici che si collegano alla superficie corrente.
            /// </summary>
            public List<Surface> IncomingTeleports
            {
                get { return sources; }
            }

            /// <summary>
            /// Restituisce le coordinate della tile occupata dalla superficie, non quella del blocco a cui appartiene.
            /// Viene restituito null se non c'è un'orientamento della superficie o se non è relativa a nessun blocco.
            /// </summary>
            public Pair<byte> SurfaceTileCoordinates
            {
                get
                {
                    if (myBlock != null)
                    {
                        if (direction == KulaLevel.Orientation.Down)
                            return new Pair<byte>(myBlock.X, (byte)(myBlock.Y - 1));
                        else if (direction == KulaLevel.Orientation.Left)
                            return new Pair<byte>((byte)(myBlock.X - 1), myBlock.Y);
                        else if (direction == KulaLevel.Orientation.Right)
                            return new Pair<byte>((byte)(myBlock.X + 1), myBlock.Y);
                        else if (direction == KulaLevel.Orientation.Up)
                            return new Pair<byte>(myBlock.X, (byte)(myBlock.Y + 1));
                        else return null;
                    }
                    else return null;
                }
            }
            #endregion
        }
        #endregion

        //Definizione di maptile
        #region
        /// <summary>
        /// Questa classe rappresenta tutti i pezzi della mappa che occupano un'intera cella della griglia.
        /// </summary>
        [Serializable()]
        public abstract class MapTile
        {
            //Variabili protected
            #region
            /// <summary>
            /// Coordinate della tile
            /// </summary>
            protected Pair<byte> coords;
            /// <summary>
            /// Orientamento della tile (in che direzione è il suo sotto?)
            /// </summary>
            protected Orientation orientation; //Orientamento della tile: in che direzione è il sotto?
            /// <summary>
            /// Tipo della tile
            /// </summary>
            protected TileType tileType; //Tipo della tile: 0 = Blocco, 1 = Posizionabile, 2 = Nemico
            #endregion

            //Proprietà get-set: Type
            #region
            /// <summary>
            /// Restituisce il tipo specifico di una tile.
            /// </summary>
            public byte Type { get; set; } //Tipo specifico della tile.
            #endregion

            //Proprietà get: X, Y, TileType, Orientation
            #region
            /// <summary>
            /// Restituisce in quale colonna della mappa si trova la tile.
            /// </summary>
            public byte X
            {
                get { return coords.FirstValue; }
            }

            /// <summary>
            /// Restituisce in quale riga della mappa si trova la tile.
            /// </summary>
            public byte Y
            {
                get { return coords.SecondValue; }
            }

            /// <summary>
            /// Restituisce il tipo della tile: 0 = Blocco, 1 = Oggetto posizionabile, 2 = Nemico
            /// </summary>
            public TileType TileType
            {
                get { return tileType; }
            }

            /// <summary>
            /// Restituisce l'orientamento dell'elemento di gioco (quale è per esso la direzione "in basso")
            /// </summary>
            public Orientation Orientation
            {
                get { return orientation; }
            }
            #endregion

            /// <summary>
            /// Metodo pubblico: modifica le coordinate del blocco.
            /// </summary>
            /// <param name="X">Numero della colonna</param>
            /// <param name="Y">Numero della riga</param>
            public void ChangeCoords(byte X, byte Y)
            {
                coords = new Pair<byte>(X, Y);
            }
        }
        #endregion

        //Definizione di oggetto piazzabile
        #region
        /// <summary>
        /// Questa classe rappresenta tutti gli oggetti piazzabili (e raccoglibili) del gioco.
        /// </summary>
        [Serializable()]
        public class Placeable : MapTile
        {
            /// <summary>
            /// Direzione del modificatore di gravità (se il piazzabile corrente è un modificatore di gravità)
            /// </summary>
            public Orientation GChangerDirection { get; set; }

            /// <summary>
            /// Inizializza un oggetto piazzabile in modo standard
            /// </summary>
            public Placeable()
            {
                this.tileType = TileType.Placeable;
                coords = new Pair<byte>(0, 0);
                orientation = Orientation.Down;
                GChangerDirection = Orientation.Down;
                Type = TileConverter.FromStringSpecificType(TileType.Placeable, "Bronze Coin");
            }

            /// <summary>
            /// Inizializza un oggetto piazzabile specificando posizione, orientamento, orientamento del modificatore di gravità ed il tipo
            /// </summary>
            /// <param name="axisX">Numero della colonna</param>
            /// <param name="axisY">Numero della riga</param>
            /// <param name="orientation">Orientamento del piazzabile</param>
            /// <param name="gcdir">Orientamento del modificatore di gravità</param>
            /// <param name="type">Tipo specifico del piazzabile</param>
            public Placeable(byte axisX, byte axisY, Orientation orientation, Orientation gcdir, byte type)
            {
                coords = new Pair<byte>(axisX, axisY);
                this.orientation = orientation;
                this.Type = type;
                GChangerDirection = gcdir;
                this.tileType = TileType.Placeable;
            }

            /// <summary>
            /// Cambia l'orientamento dell'oggetto piazzabile.
            /// </summary>
            /// <param name="to">Orientamento desiderato dell'oggetto</param>
            public void ChangeOrientation(Orientation to)
            {
                orientation = to;
            }
        }
        #endregion

        //Definizione di blocco
        #region
        /// <summary>
        /// Questa classe rappresenta tutti i tipi di blocchi del livello in cui è possibile attaccare delle superfici.
        /// </summary>
        [Serializable()]
        public class Block : MapTile
        {
            private uint dBeg = 0;
            private uint dPer = 2000;
            private SortedDictionary<Orientation, Surface> surfs = new SortedDictionary<Orientation, Surface>();

            //Proprietà get-set: DisappearBegin, DisappearPeriod
            #region
            /// <summary>
            /// Restituisce i millisecondi prima che parta la fase di intermittenza (dedicata ai blocchi intermittenti)
            /// </summary>
            public uint DisappearBegin
            {
                get { return dBeg; }
                set
                {
                    if (value >= 0 && value <= 2000)
                        dBeg = value;
                }
            }
            /// <summary>
            /// Restituisce i millisecondi di durata della fase di intermittenza (dedicata ai blocchi intermittenti)
            /// </summary>
            public uint DisappearPeriod
            {
                get { return dPer; }
                set
                {
                    if (value >= 2000 && value <= 6000)
                        dPer = value;
                }
            }
            #endregion

            //Costruttori
            #region
            /// <summary>
            /// Crea un blocco specificando dove si trova, quali superfici contiene e il tipo del blocco.
            /// </summary>
            /// <param name="axisX">Colonna in cui si troverà il blocco</param>
            /// <param name="axisY">Riga in cui si troverà il blocco</param>
            /// <param name="type">Tipo del blocco</param>
            /// <param name="disappearBegin">Inizio del ciclo di intermittenza del blocco</param>
            /// <param name="disappearPeriod">Lunghezza del ciclo di intermittenza del blocco</param>
            /// <param name="up">Superficie superiore del blocco</param>
            /// <param name="left">Superficie sinistra del blocco</param>
            /// <param name="down">Superficie inferiore del blocco</param>
            /// <param name="right">Superficie destra del blocco</param>
            public Block(byte axisX, byte axisY, byte type, byte disappearBegin, byte disappearPeriod, Surface up, Surface left, Surface down, Surface right)
            {
                if (up != null && left != null && down != null && right != null)
                {
                    coords = new Pair<byte>(axisX, axisY);
                    this.Type = type;
                    this.DisappearBegin = disappearBegin;
                    this.DisappearPeriod = disappearPeriod;
                    this.tileType = TileType.Block;
                    surfs.Add(Orientation.Down, up);
                    surfs.Add(Orientation.Up, down);
                    surfs.Add(Orientation.Left, right);
                    surfs.Add(Orientation.Right, left);
                    foreach (Surface s in surfs.Values.ToList<Surface>())
                        s.BindToBlock(this);
                    orientation = KulaLevel.Orientation.Down;
                }

            }
            /// <summary>
            /// Crea un blocco standard, da modificare.
            /// </summary>
            public Block()
            {
                coords = new Pair<byte>(0, 0);
                this.Type = TileConverter.FromStringSpecificType(TileType.Block, "Normal");
                this.DisappearBegin = 0;
                this.DisappearPeriod = 1000;
                this.tileType = TileType.Block;
                surfs.Add(Orientation.Down, new Surface(KulaLevel.Orientation.Up));
                surfs.Add(Orientation.Up, new Surface(KulaLevel.Orientation.Down));
                surfs.Add(Orientation.Right, new Surface(KulaLevel.Orientation.Left));
                surfs.Add(Orientation.Left, new Surface(KulaLevel.Orientation.Right));
                foreach (Surface s in surfs.Values.ToArray<Surface>())
                    s.BindToBlock(this);
                orientation = KulaLevel.Orientation.Down;
            }
            #endregion

            //Metodi pubblici: GetSurfaceAt
            #region
            /// <summary>
            /// Restituisce la superficie della faccia specificata.
            /// </summary>
            /// <param name="face">Faccia specificata in direzione (UP = faccia superiore, DOWN = faccia inferiore, ecc...)</param>
            /// <returns></returns>
            public Surface GetSurfaceAtFace(Orientation face)
            {
                Surface s;
                if (surfs.TryGetValue(face, out s))
                    return s;
                else
                    return null;
            }
            #endregion

            //Proprietà get: Surfaces
            #region
            /// <summary>
            /// Restituisce i riferimenti alle superfici che costituiscono il blocco.
            /// </summary>
            public Surface[] Surfaces
            {
                get { return surfs.Values.ToArray<Surface>(); }
            }
            #endregion
        }
        #endregion

        //Definizione di nemici
        #region
        /// <summary>
        /// Rappresenta tutti i nemici del gioco.
        /// </summary>
        [Serializable()]
        public class Enemy : MapTile
        {
            private uint ePer = 2000;
            private byte eRan = 0;

            //Proprietà get-set: Period, Range
            #region
            /// <summary>
            /// Restituisce il periodo di ciclo del nemico.
            /// </summary>
            public uint Period
            {
                get { return ePer; }
                set
                {
                    if (value >= 2000 && value <= 6000)
                        ePer = value;
                }
            }
            /// <summary>
            /// Restituisce la distanza, in blocchi, percorribile dal nemico.
            /// </summary>
            public byte Range
            {
                get { return eRan; }
                set
                {
                    if (value >= 0 && value <= 100)
                        eRan = value;
                }
            }
            #endregion

            //Costruttori
            #region
            /// <summary>
            /// Costruisce un nemico indicando l'orientamento, il tipo, la distanza percorribile e il periodo di ripetizione.
            /// </summary>
            /// <param name="axisX">Colonna in cui si troverà il blocco</param>
            /// <param name="axisY">Riga in cui si troverà il blocco</param>
            /// <param name="type">Tipo del nemico</param>
            /// <param name="orientation">Orientamento del nemico (quale è per esso il "di sotto")</param>
            /// <param name="range">Distanza, in blocchi, percorribile dal nemico</param>
            /// <param name="period">Periodo di ciclo del nemico</param>
            public Enemy(byte axisX, byte axisY, byte type, Orientation orientation, byte range, uint period)
            {
                coords = new Pair<byte>(axisX, axisY);
                this.orientation = orientation;
                this.Type = type;
                this.Range = range;
                this.Period = period;
                this.tileType = TileType.Enemy;
            }

            /// <summary>
            /// Costruisce un nemico standard, da modificare.
            /// </summary>
            public Enemy()
            {
                coords = new Pair<byte>(0, 0);
                this.orientation = Orientation.Down;
                this.Type = TileConverter.FromStringSpecificType(KulaLevel.TileType.Enemy, "Jumper");
                this.Range = 1;
                this.Period = 2000;
                this.tileType = TileType.Enemy;
            }
            #endregion

            /// <summary>
            /// Cambia l'orientamento del nemico (dove è per lui il basso?)
            /// </summary>
            /// <param name="to">Direzione desiderata</param>
            public void ChangeOrientation(Orientation to)
            {
                orientation = to;
            }
        }
        #endregion


        /*Definizione finale di livello*/
        #region

        //Proprietà get-set: StartingSeconds, LossPenalty, Theme, NextLevel, Width, Height, IsBonus
        #region
        /// <summary>
        /// Secondi di partenza del livello
        /// </summary>
        public uint StartingSeconds { get; set; }
        /// <summary>
        /// Punti detratti in caso di perdita
        /// </summary>
        public uint LossPenalty { get; set; }
        /// <summary>
        /// Tema del livello
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// Livello successivo
        /// </summary>
        public string NextLevel { get; set; }
        /// <summary>
        /// Larghezza del livello (in blocchi)
        /// </summary>
        public byte Width { get; set; }
        /// <summary>
        /// Altezza del livello (in blocchi)
        /// </summary>
        public byte Height { get; set; }
        /// <summary>
        /// Indica se il livello è un bonus
        /// </summary>
        public bool IsBonus { get; set; }
        #endregion

        //Variabili private: blocks, enemies, placeables
        #region
        private List<Block> blocks;
        private List<Enemy> enemies;
        private List<Placeable> placeables;
        #endregion

        //Metodi privati: tryLoadLevel
        #region
        /// <summary>
        /// Carica le informazioni del file binario dal file .bin
        /// </summary>
        /// <param name="filename">Path completa del file da elaborare</param>
        /// <returns></returns>
        private void tryLoadLevel(string filename)
        {
            try
            {
                Stream lvlReader = File.OpenRead(filename + ".bin");
                BinaryFormatter deserial = new BinaryFormatter();
                KulaLevel cur = (KulaLevel)deserial.Deserialize(lvlReader);

                if (cur != null)
                {
                    this.blocks = cur.Blocks;
                    this.enemies = cur.Enemies;
                    this.placeables = cur.Placeables;
                    this.IsBonus = cur.IsBonus;
                    this.Height = cur.Height;
                    this.Width = cur.Width;
                    this.Theme = cur.Theme;
                    this.NextLevel = cur.NextLevel;
                    this.LossPenalty = cur.LossPenalty;
                    this.StartingSeconds = cur.StartingSeconds;
                }

                cur = null;
                lvlReader.Dispose();
                lvlReader.Close();
                GC.Collect();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "C'è stato qualche problema nel trovare " +
                    filename +
                    "\nTraccia dello stack nell'eccezione" +
                    e.StackTrace,
                    "Loading error!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1
                    );
                Application.Exit();
            }
        }
        #endregion

        //Costruttori
        #region
        /// <summary>
        /// Resitituisce un livello utilizzando i due file utili per definirlo. Inoltre va specificato se il livello è un bonus o no.
        /// (Se è bonus, i files vengono cercati in «CurrentDir»\BONUSLEVELS, altrimenti in «CurrentDir»\LEVELS)
        /// </summary>
        /// <param name="filesname">Nome dei due file da cercare, estensione esclusa. (I due file devono quindi avere lo stesso nome)</param>
        /// <param name="isBonus">True se e solo se il livello è un bonus</param>
        public KulaLevel(string filesname, bool isBonus)
        {
            string gameDir = GameApp.CurDir();
            string folder;
            if (isBonus)
                folder = GameConstraints.OtherPaths.Bonuses + "\\";
            else
                folder = GameConstraints.OtherPaths.Levels + "\\";
            string filename = gameDir + folder + filesname;

            tryLoadLevel(filename);
        }

        /// <summary>
        /// Inizializza un livello in maniera standard.
        /// </summary>
        public KulaLevel()
        {
            blocks = new List<Block>();
            enemies = new List<Enemy>();
            placeables = new List<Placeable>();
            IsBonus = false;
        }
        #endregion

        //Proprietà get: HasNext, Blocks, Placeables, Enemies
        #region
        /// <summary>
        /// Restituisce se il seguente livello ha livelli successivi.
        /// </summary>
        public bool HasNext
        {
            get { return (NextLevel != ""); }
        }
        /// <summary>
        /// Restituisce la lista di blocchi del livello
        /// </summary>
        public List<Block> Blocks
        {
            get { return blocks; }
        }
        /// <summary>
        /// Restituisce la lista di oggetti piazzabili del livello
        /// </summary>
        public List<Placeable> Placeables
        {
            get { return placeables; }
        }
        /// <summary>
        /// Restituisce la lista di nemici
        /// </summary>
        public List<Enemy> Enemies
        {
            get { return enemies; }
        }
        #endregion

        #endregion
    }
    #endregion

    #region TileConverter: classe statica per la conversione di tipi di oggetti del gioco
    /// <summary>
    /// Classe statica utilizzata per convertire tipi specifici di tile da stringhe a byte e viceversa.
    /// </summary>
    public static class TileConverter
    {
        /// <summary>
        /// Array contenente i SurfaceType possibili.
        /// </summary>
        #region
        public static readonly KulaLevel.SurfaceType[] surfaces =
            new KulaLevel.SurfaceType[9]
            {
                KulaLevel.SurfaceType.Nothing,
                KulaLevel.SurfaceType.Ice,
                KulaLevel.SurfaceType.Fire,
                KulaLevel.SurfaceType.Spikes,
                KulaLevel.SurfaceType.TimedSpikes,
                KulaLevel.SurfaceType.Ramp,
                KulaLevel.SurfaceType.Teleport,
                KulaLevel.SurfaceType.NoJump,
                KulaLevel.SurfaceType.Exit
            };
        #endregion

        /// <summary>
        /// Array contenente i tipi di blocchi possibili.
        /// </summary>
        #region
        public static readonly string[] blocks =
            new string[6]
            {
                "Normal",
                "Transparent",
                "Intermittent",
                "Ice",
                "Fire",
                "Destructible"
            };
        #endregion

        /// <summary>
        /// Array contenente i tipi di oggetti posizionabili possibili.
        /// </summary>
        #region
        public static readonly string[] placeables =
            new string[14]
            {
                "Bronze Coin",
                "Silver Coin",
                "Golden Coin",
                "Sapphire",
                "Ruby",
                "Emerald",
                "Diamond",
                "Key",
                "Fruit",
                "Glasshour",
                "Slow Pill",
                "Glasses",
                "Gravity Changer",
                "Spawn Point"
            };
        #endregion

        /// <summary>
        /// Array contenente i tipi di nemici possibili.
        /// </summary>
        #region
        public static readonly string[] enemies =
            new string[2]
            {
                "SinusoidalEnemy",
                "Jumper"
            };
        #endregion

        /// <summary>
        /// Restituisce l'array contenente i tipi specifici di un dato tipo di tile, sottoforma di stringa
        /// </summary>
        /// <param name="tt">Tipo di tile di cui si vuole ottenere le stringhe relative ai tipi specifici.</param>
        /// <returns></returns>
        public static string[] GetSpecificTypesOf(KulaLevel.TileType tt)
        {
            if (tt == KulaLevel.TileType.Block)
                return blocks.DeepClone<string[]>();
            else if (tt == KulaLevel.TileType.Enemy)
                return enemies.DeepClone<string[]>();
            else if (tt == KulaLevel.TileType.Placeable)
                return placeables.DeepClone<string[]>();
            else
                return null;
        }

        //Metodi statici di conversione dei tipi: FromStringSpecificType, FromByteSpecificByte, StringToSurfaceType, SurfaceTypeToString
        #region
        /// <summary>
        /// Restituisce la stringa relativa al tipo specifico di tile indicato in input. Viene restituita la stringa vuota
        /// in caso di mancata conversione.
        /// </summary>
        /// <param name="tt">Tipo di tile di cui si vuole sapere il tipo specifico.</param>
        /// <param name="t">Tipo specifico della tile.</param>
        /// <returns>Tipo specifico della tile convertito in stringa.</returns>
        public static string FromByteSpecificType(KulaLevel.TileType tt, byte t)
        {
            string res = "";
            if (tt == KulaLevel.TileType.Block)
            {
                if (t >= 0 && t < blocks.Length)
                    res = blocks[t];
            }
            else if (tt == KulaLevel.TileType.Placeable)
            {
                if (t >= 0 && t < placeables.Length)
                    res = placeables[t];
            }
            else if (tt == KulaLevel.TileType.Enemy)
            {
                if (t >= 0 && t < enemies.Length)
                    res = enemies[t];
            }
            else
                throw new Exception("The input type is unknown: " + tt.ToString());
            return res;
        }

        /// <summary>
        /// Restituisce il tipo specifico di un tile sottoforma di byte. In caso di stringa non valida, viene restituito 255
        /// </summary>
        /// <param name="tt">Tipo di tile in input</param>
        /// <param name="s">Stringa che dovrebbe rappresentare un tipo specifico di tile.</param>
        /// <returns></returns>
        public static byte FromStringSpecificType(KulaLevel.TileType tt, string s)
        {
            byte res = 255;
            if (tt == KulaLevel.TileType.Block)
                res = (byte)(blocks.ToList<string>().FindIndex(0, (x => x == s)));
            else if (tt == KulaLevel.TileType.Placeable)
                res = (byte)(placeables.ToList<string>().FindIndex(0, (x => x == s)));
            else if (tt == KulaLevel.TileType.Enemy)
                res = (byte)(enemies.ToList<string>().FindIndex(0, (x => x == s)));
            return res;
        }

        /// <summary>
        /// Restituisce il tipo di superficie a partire dalla stringa data.
        /// </summary>
        /// <param name="s">Stringa data.</param>
        /// <returns></returns>
        public static KulaLevel.SurfaceType StringToSurfaceType(string s)
        {
            if (surfaces.ToList<KulaLevel.SurfaceType>().Exists(x => x.ToString().Equals(s)))
                return surfaces.ToList<KulaLevel.SurfaceType>().First<KulaLevel.SurfaceType>(x => x.ToString().Equals(s));
            else
                return KulaLevel.SurfaceType.Nothing;
        }

        /// <summary>
        /// Restituisce la stringa associata il tipo di superficie specificata
        /// </summary>
        /// <param name="st">Tipo di superficie in input</param>
        /// <returns></returns>
        public static string SurfaceTypeToString(KulaLevel.SurfaceType st)
        {
            return st.ToString();
        }

        /// <summary>
        /// Restituisce l'orientamento a partire da una stringa data, restituisce Down in caso di mismatch
        /// </summary>
        /// <param name="s">Stringa che indica l'orientamento desiderato</param>
        /// <returns></returns>
        public static KulaLevel.Orientation StringToOrientation(string s)
        {
            switch (s)
            {
                case ("Up"):
                    return KulaLevel.Orientation.Up;
                case ("Down"):
                    return KulaLevel.Orientation.Down;
                case ("Left"):
                    return KulaLevel.Orientation.Left;
                case ("Right"):
                    return KulaLevel.Orientation.Right;
                default:
                    return KulaLevel.Orientation.Down;
            }
        }

        /// <summary>
        /// Restituisce l'orientamento inversa a quella data.
        /// </summary>
        /// <param name="o">Orientamento di cui si vuol sapere l'inverso.</param>
        /// <returns></returns>
        public static KulaLevel.Orientation Reverse(KulaLevel.Orientation o)
        {
            if (o == KulaLevel.Orientation.Up)
                return KulaLevel.Orientation.Down;
            else if (o == KulaLevel.Orientation.Down)
                return KulaLevel.Orientation.Up;
            else if (o == KulaLevel.Orientation.Left)
                return KulaLevel.Orientation.Right;
            else if (o == KulaLevel.Orientation.Right)
                return KulaLevel.Orientation.Left;
            else
                return KulaLevel.Orientation.Up;
        }
        #endregion
    }
    #endregion
}
