using GameUtils;
using LevelsStructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace EditorViewPort
{
    #region TODO: Classe utile per giudicare la correttezza di eventuali modifiche.
    public static class R
    {
        public static void r(string s)
        {
            MessageBox.Show(s);
        }
    }
    
    public static class Judge
    {
        public static bool CanISave(Editor ed)
        {
            return true;
        }
    }
    #endregion

    #region Utilità varie per l'editor.
    /// <summary>
    /// Classe statica utile per l'editor.
    /// </summary>
    public static class EditorUtils
    {
        #region Parte privata: variabili private e metodi privati
        private static SortedDictionary<Pair<string>, Color> colors = new SortedDictionary<Pair<string>, Color>();
        private static Dictionary<KulaLevel.SurfaceType, Color> surColors = new Dictionary<KulaLevel.SurfaceType, Color>();
        
        private static void AddColor(KulaLevel.TileType tt, byte st, Color c)
        {
            Pair<string> a;
            if (!colors.ContainsKey(a = new Pair<string>(tt.ToString(), TileConverter.FromByteSpecificType(tt, st))))
                colors.Add(a, c);
        }

        /// <summary>
        /// Inizializza la tabella dei colori delle superfici
        /// </summary>
        private static void InitializeSurfaceColors()
        {
            surColors.Add(KulaLevel.SurfaceType.Nothing, Color.Gray);
            surColors.Add(KulaLevel.SurfaceType.Ice, Color.DeepSkyBlue);
            surColors.Add(KulaLevel.SurfaceType.Fire, Color.DarkRed);
            surColors.Add(KulaLevel.SurfaceType.Spikes, Color.Maroon);
            surColors.Add(KulaLevel.SurfaceType.TimedSpikes, Color.SaddleBrown);
            surColors.Add(KulaLevel.SurfaceType.Ramp, Color.Yellow);
            surColors.Add(KulaLevel.SurfaceType.Exit, Color.LimeGreen);
            surColors.Add(KulaLevel.SurfaceType.Teleport, Color.Blue);
            surColors.Add(KulaLevel.SurfaceType.NoJump, Color.Tan);
        }

        /// <summary>
        /// Inizializza la tabella dei colori delle tile
        /// </summary>
        private static void InitializeColors()
        {
            AddColor(KulaLevel.TileType.Block, 0, Color.PeachPuff);
            AddColor(KulaLevel.TileType.Block, 1, Color.Bisque);
            AddColor(KulaLevel.TileType.Block, 2, Color.DarkGray);
            AddColor(KulaLevel.TileType.Block, 3, Color.PaleTurquoise);
            AddColor(KulaLevel.TileType.Block, 4, Color.Crimson);
            AddColor(KulaLevel.TileType.Block, 5, Color.Peru);

            AddColor(KulaLevel.TileType.Enemy, 0, Color.FromArgb(150, Color.Navy));
            AddColor(KulaLevel.TileType.Enemy, 1, Color.FromArgb(150, Color.Coral));

            AddColor(KulaLevel.TileType.Placeable, 0, Color.FromArgb(160, Color.DarkOrange));
            AddColor(KulaLevel.TileType.Placeable, 1, Color.FromArgb(160, Color.Silver));
            AddColor(KulaLevel.TileType.Placeable, 2, Color.FromArgb(160, Color.Gold));
            AddColor(KulaLevel.TileType.Placeable, 3, Color.FromArgb(160, Color.Blue));
            AddColor(KulaLevel.TileType.Placeable, 4, Color.FromArgb(160, Color.Red));
            AddColor(KulaLevel.TileType.Placeable, 5, Color.FromArgb(160, Color.Green));
            AddColor(KulaLevel.TileType.Placeable, 6, Color.FromArgb(160, Color.PowderBlue));
            AddColor(KulaLevel.TileType.Placeable, 7, Color.FromArgb(160, Color.Goldenrod));
            AddColor(KulaLevel.TileType.Placeable, 8, Color.FromArgb(160, Color.GreenYellow));
            AddColor(KulaLevel.TileType.Placeable, 9, Color.FromArgb(160, Color.Sienna));
            AddColor(KulaLevel.TileType.Placeable, 10, Color.FromArgb(160, Color.Orange));
            AddColor(KulaLevel.TileType.Placeable, 11, Color.FromArgb(160, Color.Black));
            AddColor(KulaLevel.TileType.Placeable, 12, Color.FromArgb(160, Color.Purple));
            AddColor(KulaLevel.TileType.Placeable, 13, Color.FromArgb(160, Color.Fuchsia));
        }
        #endregion

        #region Parte dei colori: AssociateToSurface, AssociateToTile
        /// <summary>
        /// Questo metodo associa un colore ad un tipo noto di superficie del gioco.
        /// </summary>
        /// <param name="s">Tipo di superficie da "convertire"</param>
        /// <returns></returns>
        public static Color AssociateToSurface(KulaLevel.SurfaceType s)
        {
            Color res;
            if (!surColors.TryGetValue(s, out res))
                InitializeSurfaceColors();
            else
                return res;

            if (!surColors.TryGetValue(s, out res))
                return Color.FromArgb(0, 0, 0, 0);
            else
                return res;
        }

        /// <summary>
        /// Metodo che associa un colore ad un tipo specifico di tile che sia noto.
        /// </summary>
        /// <param name="TileType">Tipo di tile</param>
        /// <param name="SpecType">Tipo specifico della tile</param>
        /// <returns></returns>
        public static Color AssociateToTile(string TileType, string SpecType)
        {
            Color res;
            if (!colors.TryGetValue(new Pair<string>(TileType, SpecType), out res))
                InitializeColors();
            else
                return res;

            if (!colors.TryGetValue(new Pair<string>(TileType, SpecType), out res))
                return Color.FromArgb(0, 0, 0, 0);
            else
                return res;
        }
        #endregion
    }
    public enum EditorMode
    {
        SelectMode,
        InsertMode,
        DeleteMode,
        LvlPropMode,
    }
    #endregion

    #region Definizione degli EventArgs specializzati agli eventi dell'editor e dei relativi delegates.
    public class ChangedPointerEventArgs : EventArgs
    {
        private KulaLevel.MapTile mt;
        public KulaLevel.MapTile CurrentMapTile
        {
            get { return mt; }
        }
        public ChangedPointerEventArgs(Editor ed)
        {
            if (ed == null)
                return;
            mt = ed.PointedTile;
        }
    }
    public delegate void ChangedPointerEventHandler(object sender, ChangedPointerEventArgs e);
    public class ChangedTilePropertiesEventArgs : EventArgs
    {
        private KulaLevel.TileType curTileType;
        private string curSpecificTileType;
        private EditorMode curMode;
        private KulaLevel.Orientation curSurface;
        public KulaLevel.TileType CurrentTileType { get { return curTileType; } }
        public string CurrentSpecificTileType { get { return curSpecificTileType; } }
        public EditorMode CurrentMode { get { return curMode; } }
        public KulaLevel.Orientation CurrentSurface { get { return curSurface; } }
        public ChangedTilePropertiesEventArgs(Editor ed)
        {
            if (ed == null)
                return;
            if (ed.Mode == EditorMode.SelectMode)
            {
                KulaLevel.MapTile t = ed.PointedTile;
                if (t != null)
                {
                    curTileType = t.TileType;
                    curSpecificTileType = TileConverter.FromByteSpecificType(curTileType, t.Type);
                    curMode = ed.Mode;
                    curSurface = ed.ChosenFaceDirection;
                }
            }
            else if (ed.Mode == EditorMode.InsertMode)
            {
                KulaLevel.MapTile t = ed.TileToBeAdded;
                if (t != null)
                {
                    curTileType = t.TileType;
                    curSpecificTileType = TileConverter.FromByteSpecificType(curTileType, t.Type);
                    curMode = ed.Mode;
                    curSurface = ed.ChosenFaceDirection;
                }
            }
        }
    }
    public delegate void ChangedTilePropertiesEventHandler(object sender, ChangedTilePropertiesEventArgs e);
    public class ChangedModeEventArgs : EventArgs
    {
        private EditorMode edmode;
        public EditorMode EditorMode { get { return edmode; } }

        public ChangedModeEventArgs(Editor ed)
        {
            if (ed == null)
                return;
            edmode = ed.Mode;
        }
    }
    public delegate void ChangedModeEventHandler(object sender, ChangedModeEventArgs e);

    #endregion

    public class Editor : Control
    {
        #region Variabili private: isMouseDown, isReadyToInsert, isLoaded, lvlGrid, lvlW, lvlH, cellSize, lineWidth, pointer, pointedSurf, lossP, startS
        private bool isMouseDown;
        private bool isLoaded;
        private bool isReadyToInsert;
        private SortedDictionary<Pair<byte>, KulaLevel.MapTile> lvlGrid;
        private byte lvlW;
        private byte lvlH;
        private byte cellSize;
        private byte lineWidth;
        private uint lossP;
        private uint startS;
        private EditorMode mode;
        private Pair<byte> pointer;
        private KulaLevel.Orientation pointedSurf;
        #endregion

        #region Eventi da lanciare: ChangedPointer, ChangedTileProperties, ChangedMode
        /// <summary>
        /// Generato quando il puntatore dell'editor viene spostato.
        /// </summary>
        public event ChangedPointerEventHandler ChangedPointer;
        protected virtual void OnChangedPointer(ChangedPointerEventArgs e)
        {
            if (ChangedPointer != null && this != null)
                ChangedPointer(this, e);
        }

        /// <summary>
        /// Generato quando le proprietà di una tile (da inserire, o selezionata) vengono modificate.
        /// </summary>
        public event ChangedTilePropertiesEventHandler ChangedTileProperties;
        protected virtual void OnChangedTileProperties(ChangedTilePropertiesEventArgs e)
        {
            if (ChangedTileProperties != null && this != null)
                ChangedTileProperties(this, e);
        }

        /// <summary>
        /// Generato quando la modalità viene cambiata.
        /// </summary>
        public event ChangedModeEventHandler ChangedMode;
        protected virtual void OnChangedMode(ChangedModeEventArgs e)
        {
            if (ChangedMode != null && this != null)
                ChangedMode(this, e);
        }
        #endregion

        #region Proprietà get: IsReadyToInsert, IsLoaded, PointedTile, CurrentPointer
        public bool IsReadyToInsert { get { return (TileToBeAdded != null && isReadyToInsert); } }
        public bool IsLoaded { get { return isLoaded; } }
        public KulaLevel.MapTile PointedTile
        {
            get 
            { 
                KulaLevel.MapTile t;
                if (lvlGrid.TryGetValue(CurrentPointer, out t))
                    return t;
                else
                    return null;
            }
        }
        public Pair<byte> CurrentPointer { get { return pointer; } }
        #endregion
            
        #region Proprietà get-set: IsABonusLevel, TileToBeAdded, LvlWidth, LvlHeight, LvlStartingSeconds, LvlNextLevel, LvlLossPenalty, LvlTheme, SecureMode, Mode, ChosenFaceDirection
        public bool IsABonusLevel { get; set; }
        public KulaLevel.MapTile TileToBeAdded { get; set; }
        /// <summary>
        /// Imposta o restituisce se l'editor impedirà di fare determinate azioni al fine di avere maggior integrità nel livello.
        /// </summary>
        public bool SecureMode { get; set; }
        /// <summary>
        /// Imposta o restituisce l'ampiezza del livello: non è permesso rimpicciolire un livello se non caricando un altro livello.
        /// Il range di ampiezza permesso è 2 - 100
        /// </summary>
        public byte LvlWidth 
        {
            get { return lvlW; }
            set
            {
                if (value >= 2 && value <= 100 && lvlW > value && lvlGrid != null)
                {
                    if (SecureMode)
                    {
                        MessageBox.Show("Si è tentato di rimpicciolire il livello: ciò non viene permesso per evitare problemi di integrità del livello.");
                        return;
                    }
                    else
                    {
                        List<Pair<byte>> toBeEliminated = lvlGrid.Keys.ToList<Pair<byte>>().FindAll(x => x.First >= value);
                        KulaLevel.MapTile mt;
                        foreach (Pair<byte> p in toBeEliminated)
                        {
                            if (lvlGrid.TryGetValue(p, out mt))
                                RemoveTile(mt);
                        }
                    }
                }
                else if (value < 2 || value > 100)
                    return;
                lvlW = value;
                Width = lvlW * (cellSize + lineWidth) + lineWidth;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Imposta o restituisce l'altezza del livello: non è permesso rimpicciolire un livello se non caricando un altro livello.
        /// Il range di altezza permesso è 2 - 100
        /// </summary>
        public byte LvlHeight 
        {
            get { return lvlH; }
            set
            {
                if (value >= 2 && value <= 100 && lvlH > value && lvlGrid != null)
                {
                    if (SecureMode)
                    {
                        MessageBox.Show("Si è tentato di rimpicciolire il livello: ciò non viene permesso per evitare problemi di integrità del livello.");
                        return;
                    }
                    else
                    {
                        List<Pair<byte>> toBeEliminated = lvlGrid.Keys.ToList<Pair<byte>>().FindAll(x => x.Second >= value);
                        KulaLevel.MapTile mt;
                        foreach (Pair<byte> p in toBeEliminated)
                        {
                            if (lvlGrid.TryGetValue(p, out mt))
                                RemoveTile(mt);
                        }
                    }
                }
                else if (value < 2 || value > 100)
                    return;
                lvlH = value;
                Height = lvlH * (cellSize + lineWidth) + lineWidth;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Modifica la modalità con cui l'editor interagisce.
        /// </summary>
        public EditorMode Mode 
        { 
            get
            {
                return mode;
            }
            set
            {
                ChangePointer(new Pair<byte>(0, 0));
                TileToBeAdded = null;
                isReadyToInsert = false;
                mode = value;
                GC.Collect();
                if (this != null)
                    OnChangedMode(new ChangedModeEventArgs(this));
            }
        }
        /// <summary>
        /// Imposta o restituisce la faccia da modificare nell'editor.
        /// </summary>
        public KulaLevel.Orientation ChosenFaceDirection
        {
            get { return pointedSurf; }
            set
            {
                pointedSurf = value;
                if (this != null)
                    OnChangedTileProperties(new ChangedTilePropertiesEventArgs(this));
            }
        }
        public string LvlTheme { get; set; }
        public string LvlNextLevel { get; set; }
        public uint LvlLossPenalty 
        {
            get { return lossP; } 
            set
            {
                if (value >= 50 && value <= 10000)
                    lossP = value;
            }
        }
        public uint LvlStartingSeconds
        {
            get { return startS; }
            set
            {
                if (value >= 5 && value <= 120)
                    startS = value;
            }
        }
        #endregion

        #region Metodi privati: GetPairFromMouseEvent, ChangePointer, InitGrid, LoadKulaLevel, ExportAll, FillGrid, TreatBlock, TreatPlaceable, TreatEnemy, RemoveTile, RequestNewLevel, LoadLevel, SaveLevel
        private Pair<byte> GetPairFromMouseEvent(MouseEventArgs e)
        {
            int tX, ty;
            byte X, Y;
            tX = e.X / (cellSize + lineWidth);
            ty = e.Y / (cellSize + lineWidth);
            if (tX >= 0 && tX < 256 && ty >= 0 && ty < 256)
            {
                X = (byte)(e.X / (cellSize + lineWidth));
                Y = (byte)(e.Y / (cellSize + lineWidth));
                return new Pair<byte>(X, Y);
            }
            throw new ArgumentOutOfRangeException("L'evento del mouse ricevuto sta indicando un blocco fuori dai limiti");
        }
        private void ChangePointer(Pair<byte> x)
        {
            pointer = x;
            OnChangedPointer(new ChangedPointerEventArgs(this));
            this.Invalidate();
        }
        private void InitGrid(KulaLevel lvl)
        {
            this.lvlGrid = new SortedDictionary<Pair<byte>, KulaLevel.MapTile>();
            lvlGrid = new SortedDictionary<Pair<byte>, KulaLevel.MapTile>();
            isMouseDown = false;
            TileToBeAdded = null;
            isReadyToInsert = false;
            ChangePointer(new Pair<byte>(0, 0));
            isLoaded = true;
            lvlH = lvl.Height;
            lvlW = lvl.Width;
            IsABonusLevel = lvl.IsBonus;
            LvlStartingSeconds = lvl.StartingSeconds;
            LvlTheme = lvl.Theme;
            LvlLossPenalty = lvl.LossPenalty;
            LvlNextLevel = lvl.NextLevel;
            Mode = EditorMode.LvlPropMode;
        }
        private void FillGrid(KulaLevel lvl)
        {
            Pair<byte> placer;
            foreach (KulaLevel.Block b in lvl.Blocks)
                if (!lvlGrid.ContainsKey(placer = new Pair<byte>(b.X, b.Y)))
                    lvlGrid.Add(placer, b);

            foreach (KulaLevel.Placeable p in lvl.Placeables)
                if (!lvlGrid.ContainsKey(placer = new Pair<byte>(p.X, p.Y)))
                    lvlGrid.Add(placer, p);

            foreach (KulaLevel.Enemy e in lvl.Enemies)
                if (!lvlGrid.ContainsKey(placer = new Pair<byte>(e.X, e.Y)))
                    lvlGrid.Add(placer, e);
        }
        private bool LoadKulaLevel(KulaLevel lvl)
        {
            if (lvl == null)
                return false;
            else
            {
                InitGrid(lvl);
                FillGrid(lvl);
                this.Invalidate();
                return true;
            }
        }
        private KulaLevel ExportAll()
        {
            KulaLevel res = new KulaLevel();
            res.IsBonus = IsABonusLevel;
            res.Height = LvlHeight;
            res.Width = LvlWidth;
            res.StartingSeconds = LvlStartingSeconds;
            res.Theme = LvlTheme;
            if (IsABonusLevel)
                res.Theme = "Bonus";
            res.LossPenalty = LvlLossPenalty;
            if (!IsABonusLevel)
                res.NextLevel = LvlNextLevel;
            foreach (Pair<byte> p in lvlGrid.Keys.ToList<Pair<byte>>())
            {
                KulaLevel.MapTile got;
                if (lvlGrid.TryGetValue(p, out got))
                {
                    if (got.TileType == KulaLevel.TileType.Block)
                        res.Blocks.Add((KulaLevel.Block)got);
                    else if (got.TileType == KulaLevel.TileType.Enemy)
                        res.Enemies.Add((KulaLevel.Enemy)got);
                    else
                        res.Placeables.Add((KulaLevel.Placeable)got);
                }
            }
            return res;
        }
        private bool InsertTile(Pair<byte> where)
        {
            TileToBeAdded.ChangeCoords(where.First, where.Second);
            if (lvlGrid.ContainsKey(where))
                return false;
            else
                lvlGrid.Add(where, ExtensionMethods.DeepClone<KulaLevel.MapTile>(TileToBeAdded));
            this.Invalidate();
            return true;
        }
        /// <summary>
        /// Questo metodo rimuove una tile dal livello, cancellando a cascata tutte le tile collegate ad esso.
        /// </summary>
        /// <param name="mt">Tile da eliminare.</param>
        private void RemoveTile(KulaLevel.MapTile mt)
        {
            if (mt != null)
            {
                if (mt.TileType == KulaLevel.TileType.Block)
                {
                    KulaLevel.MapTile nearTile;
                    Pair<byte> neigh;
                    if (lvlGrid.TryGetValue(neigh = new Pair<byte>(mt.X, (byte)(mt.Y - 1)), out nearTile))
                        if ((nearTile.TileType != KulaLevel.TileType.Block) && (nearTile.Orientation == KulaLevel.Orientation.Down))
                            RemoveTile(nearTile);
                    if (lvlGrid.TryGetValue(neigh = new Pair<byte>(mt.X, (byte)(mt.Y + 1)), out nearTile))
                        if ((nearTile.TileType != KulaLevel.TileType.Block) && (nearTile.Orientation == KulaLevel.Orientation.Up))
                            RemoveTile(nearTile);
                    if (lvlGrid.TryGetValue(neigh = new Pair<byte>((byte)(mt.X - 1), mt.Y), out nearTile))
                        if ((nearTile.TileType != KulaLevel.TileType.Block) && (nearTile.Orientation == KulaLevel.Orientation.Right))
                            RemoveTile(nearTile);
                    if (lvlGrid.TryGetValue(neigh = new Pair<byte>((byte)(mt.X + 1), mt.Y), out nearTile))
                        if ((nearTile.TileType != KulaLevel.TileType.Block) && (nearTile.Orientation == KulaLevel.Orientation.Left))
                            RemoveTile(nearTile);

                    foreach(KulaLevel.Surface s in ((KulaLevel.Block)mt).Surfaces)
                    {
                        if (s.Type == KulaLevel.SurfaceType.Teleport)
                        {
                            s.FlushIncomingTeleport();
                            s.NextTeleport = null;
                        }
                    }
                }

                lvlGrid.Remove(new Pair<byte>(mt.X, mt.Y));
                this.Invalidate();
                GC.Collect();
            }
        }
        private bool TreatSwitch(KulaLevel.MapTile t, string SpecType, string Prop1, string Prop2, string Prop3, string Prop4, uint Prop5, uint Prop6, uint SProp1, uint SProp2)
        {
            if (t == null)
                return false;
            t.Type = TileConverter.FromStringSpecificType(t.TileType, SpecType);
            switch (t.TileType)
            {
                case (KulaLevel.TileType.Block):
                {
                    return TreatBlock((KulaLevel.Block)t, Prop1, Prop2, Prop3, Prop4, Prop5, Prop6, SProp1, SProp2);
                }
                case (KulaLevel.TileType.Placeable):
                {
                    return TreatPlaceable((KulaLevel.Placeable)t, Prop1, Prop2);
                }
                case (KulaLevel.TileType.Enemy):
                {
                    return TreatEnemy((KulaLevel.Enemy)t, Prop1, SProp1, SProp2);
                }
                default:
                    return false;
            }
        }
        private bool TreatBlock(KulaLevel.Block b, string UpperSurf, string LowerSurf, string LeftSurf, string RightSurf, uint IntPeriod, uint IntBegin, uint SpikePeriod, uint SpikeBegin)
        {
            //NB UpLowLeftRight
            b.GetSurfaceAtFace(KulaLevel.Orientation.Up).Type = TileConverter.StringToSurfaceType(UpperSurf);
            b.GetSurfaceAtFace(KulaLevel.Orientation.Down).Type = TileConverter.StringToSurfaceType(LowerSurf);
            b.GetSurfaceAtFace(KulaLevel.Orientation.Left).Type = TileConverter.StringToSurfaceType(LeftSurf);
            b.GetSurfaceAtFace(KulaLevel.Orientation.Right).Type = TileConverter.StringToSurfaceType(RightSurf);
            b.DisappearPeriod = IntPeriod;
            b.DisappearBegin = IntBegin;
            b.GetSurfaceAtFace(ChosenFaceDirection).SpikesBegin = SpikeBegin;
            b.GetSurfaceAtFace(ChosenFaceDirection).SpikesPeriod = SpikePeriod;
            return true;
        }
        /// <summary>
        /// Modifica il blocco placeable indicato. (Mancano i controlli di integrità).
        /// </summary>
        /// <param name="p">Blocco placeable indicato</param>
        /// <param name="Orient">Stringa che riguarda l'orientamento sulla superficie (dov'è il "basso" per tale blocco)</param>
        /// <returns></returns>
        private bool TreatPlaceable(KulaLevel.Placeable p, string Orient, string GCDirection)
        {
            p.ChangeOrientation(TileConverter.StringToOrientation(Orient));
            p.GChangerDirection = TileConverter.StringToOrientation(GCDirection);
            return true;
        }
        /// <summary>
        /// Modifica il blocco nemico indicato. (Mancano i controlli di integrità).
        /// </summary>
        /// <param name="p">Blocco nemico indicato</param>
        /// <param name="Orient">Stringa che riguarda l'orientamento sulla superficie (dov'è il "basso" per tale blocco)</param>
        /// <param name="enPer">Indica quanti millisecondi ci mette a percorrere il suo ciclo</param>
        /// <param name="enRan">Indica quanti blocchi è in grado di coprire.</param>
        /// <returns></returns>
        private bool TreatEnemy(KulaLevel.Enemy e, string Orient, uint enPer, uint enRan)
        {
            e.ChangeOrientation(TileConverter.StringToOrientation(Orient));
            e.Period = enPer;
            e.Range = (byte)(enRan % 256);
            return true;
        }
        private bool RequestNewLevel()
        {
            string v = "10";
            string v1 = "10";
            if (Interaction.DoubleTextInputBox("Insert the level size", "Width (2 - 100): ", "Height (2 - 100): ", ref v, ref v1) == DialogResult.OK)
            {
                byte w;
                byte h;
                try
                {
                    w = Byte.Parse(v);
                    h = Byte.Parse(v1);
                }
                catch (Exception e)
                {
                    e.ToString();
                    w = 0;
                    h = 0;
                }

                if ((w >= 2) && (h >= 2) && (w <= 100) && (h <= 100))
                {
                    KulaLevel l = new KulaLevel();
                    l.Width = w;
                    l.Height = h;
                    l.NextLevel = "";
                    l.IsBonus = false;
                    l.StartingSeconds = 60;
                    l.Theme = "Egypt";
                    l.LossPenalty = 100;

                    InitGrid(l);
                    FillGrid(l);
                    isLoaded = true;
                    this.Invalidate();
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        private bool SaveLevel(string path)
        {
            SaveFileDialog a = new SaveFileDialog();
            a.AddExtension = false;
            a.DefaultExt = ".bin";
            a.FileName = "NewLevel";
            a.InitialDirectory = path;
            a.RestoreDirectory = true;
            if (a.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    KulaLevel lvl = ExportAll();
                    Stream saver = File.Create(a.FileName);
                    BinaryFormatter serz = new BinaryFormatter();
                    serz.Serialize(saver, lvl);
                    saver.Close();
                    saver.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Sembrano esserci problemi nel salvataggio, infatti l'eccezione scaturita dice: \n" + e.ToString());
                    return false;
                }
            }
            else
                return false;
            return true;
        }
        private bool LoadLevel(string path)
        {
            OpenFileDialog a = new OpenFileDialog();
            a.AddExtension = false;
            a.DefaultExt = ".bin";
            a.Filter = "binary files (*.bin) | *.bin";
            a.FilterIndex = 1;
            a.InitialDirectory = path;
            a.RestoreDirectory = true;
            if (a.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream loader = File.OpenRead(a.FileName);
                    BinaryFormatter deserz = new BinaryFormatter();
                    KulaLevel lvl = (KulaLevel)deserz.Deserialize(loader);
                    loader.Close();
                    loader.Dispose();
                    return LoadKulaLevel(lvl);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Sembrano esserci problemi nel caricamento, infatti l'eccezione scaturita dice: \n" + e.ToString());
                    return false;
                }
            }
            else
                return false;
        }
        private void PaintGrid(PaintEventArgs e)
        {
            base.OnPaint(e);
            Width = lvlW * (lineWidth + cellSize) + lineWidth;
            Height = lvlH * (lineWidth + cellSize) + lineWidth;
            Graphics g = e.Graphics;

            Pen line = new Pen(this.ForeColor);
            line.Width = lineWidth;

            g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, Width - 1, Height - 1);

            for (int i = lineWidth / 2; i <= Width - lineWidth / 2; i += (cellSize + lineWidth))
                g.DrawLine(line, new Point(i, 0), new Point(i, Height));

            for (int i = lineWidth / 2; i <= Height - lineWidth / 2; i += (cellSize + lineWidth))
                g.DrawLine(line, new Point(0, i), new Point(Width, i));
            //g.Dispose();
            line.Dispose();
        }
        private void PaintPieces(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush m = new SolidBrush(Color.FromArgb(255, 180, 0, 0));
            Pen p = new Pen(Color.FromArgb(255, 0, 0, 0), lineWidth);

            foreach (KulaLevel.MapTile t in lvlGrid.Values.ToList<KulaLevel.MapTile>())
            {
                m.Color = EditorUtils.AssociateToTile(t.TileType.ToString(), TileConverter.FromByteSpecificType(t.TileType, t.Type));
                int w = cellSize;
                int h = cellSize;
                int x = lineWidth + t.X * (lineWidth + cellSize);
                int y = lineWidth + t.Y * (lineWidth + cellSize);
                g.FillRectangle(m, new Rectangle(x, y, w, h));

                #region Nel caso in cui il tile sia un blocco: disegno le superfici.
                if (t.TileType == KulaLevel.TileType.Block)
                {
                    int lw = w + lineWidth;
                    int lh = h + lineWidth;
                    int lx = x - lineWidth/2;
                    int ly = y - lineWidth/2;
                    KulaLevel.Block b = (KulaLevel.Block)t;
                    KulaLevel.Surface s;
                    s = b.GetSurfaceAtFace(KulaLevel.Orientation.Down);
                    p.Color = EditorUtils.AssociateToSurface(s.Type);
                    g.DrawLine(p, new Point(lx, ly + lh), new Point(lx + lw, ly + lh));

                    s = b.GetSurfaceAtFace(KulaLevel.Orientation.Up);
                    p.Color = EditorUtils.AssociateToSurface(s.Type);
                    g.DrawLine(p, new Point(lx, ly), new Point(lx + lw, ly));

                    s = b.GetSurfaceAtFace(KulaLevel.Orientation.Left);
                    p.Color = EditorUtils.AssociateToSurface(s.Type);
                    g.DrawLine(p, new Point(lx, ly), new Point(lx, ly + lh));

                    s = b.GetSurfaceAtFace(KulaLevel.Orientation.Right);
                    p.Color = EditorUtils.AssociateToSurface(s.Type);
                    g.DrawLine(p, new Point(lx + lw, ly), new Point(lx + lw, ly + lh));
                }
                #endregion
                #region Caso in cui gli oggetti non siano blocchi, ne evidenzio la direzione.
                else
                {
                    #region Sistemo la freccia che mi indica la direzione.
                    float unit =((float)cellSize) / 4.0f;
                    float lw = (float)lineWidth;

                    float startX = t.X * (lw + 4.0f*unit);
                    float startY = t.Y * (lw + 4.0f*unit);

                    PointF[] pts = 
                    {
                        new PointF(startX + lw + unit, startY + lw + unit*2.0f),
                        new PointF(startX + lw + 3.0f*unit, startY + lw + unit*2.0f),
                        new PointF(startX + lw + 2.0f*unit, startY + lw + unit*4.0f),
                        new PointF(startX + lw + 2.0f*unit, startY + lw + unit*2.0f),
                    };

                    Matrix trans = new Matrix();
                    if (t.Orientation == KulaLevel.Orientation.Up)
                        trans.RotateAt(180.0f, pts[3], MatrixOrder.Append);
                    else if (t.Orientation == KulaLevel.Orientation.Left)
                        trans.RotateAt(90.0f, pts[3], MatrixOrder.Append);
                    else if (t.Orientation == KulaLevel.Orientation.Right)
                        trans.RotateAt(-90.0f, pts[3], MatrixOrder.Append);
                    trans.TransformPoints(pts);

                    GraphicsPath gp = new GraphicsPath();
                    gp.AddLine(pts[0], pts[1]);
                    gp.AddLine(pts[1], pts[2]);
                    gp.AddLine(pts[2], pts[0]);
                    gp.CloseAllFigures();

                    g.FillPath(Brushes.White, gp);
                    g.DrawPath(Pens.DarkRed, gp);
                    gp.Dispose();
                    trans.Dispose();
                    #endregion
                }
                #endregion

            }
            m.Dispose();
            p.Dispose();
        }
        private void PaintDetails(PaintEventArgs e)
        {
            if (Mode == EditorMode.SelectMode)
            {
                Graphics g = e.Graphics;
                SolidBrush m = new SolidBrush(Color.FromArgb(130, 200, 245, 0));
                int w = cellSize;
                int h = cellSize;
                int x = lineWidth + pointer.First * (lineWidth + cellSize);
                int y = lineWidth + pointer.Second * (lineWidth + cellSize);
                g.FillRectangle(m, new Rectangle(x, y, w, h));
            }
        }
        #endregion

        #region Costruttori:
        /// <summary>
        /// Crea un editor vuoto con righe e colonne uguali.
        /// Come standard, riempie tutto il controllo che lo contiene.
        /// </summary>
        public Editor()
        {
            lvlGrid = new SortedDictionary<Pair<byte>, KulaLevel.MapTile>();
            isMouseDown = false;
            isLoaded = false;
            Mode = EditorMode.InsertMode;
            TileToBeAdded = null;
            lvlW = 0;
            lvlH = 0;
            cellSize = 20;
            lineWidth = 2;
            pointedSurf = KulaLevel.Orientation.Up;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.FromArgb(255, 255, 255, 255);
            ChangePointer(new Pair<byte>(0, 0));
        }
        #endregion

        #region Metodi con override: OnPaint, OnPaintBackground, OnMouseDown, OnMouseMove, OnMouseUp
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, Width - 1, Height - 1);
            //g.Dispose();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (isLoaded)
            {
                PaintGrid(e);
                PaintPieces(e);
                PaintDetails(e);
            }
            else
            {

            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            isMouseDown = true;
            KulaLevel.MapTile mt;
            if (Mode == EditorMode.InsertMode && IsReadyToInsert)
                InsertTile(GetPairFromMouseEvent(e));
            else if (Mode == EditorMode.SelectMode)
                ChangePointer(GetPairFromMouseEvent(e));
            else if (Mode == EditorMode.DeleteMode && lvlGrid.TryGetValue(GetPairFromMouseEvent(e), out mt))
                RemoveTile(mt);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            KulaLevel.MapTile mt;
            Pair<byte> p = GetPairFromMouseEvent(e);
            if (isMouseDown && Mode == EditorMode.InsertMode && IsReadyToInsert && !lvlGrid.ContainsKey(GetPairFromMouseEvent(e)))
                InsertTile(GetPairFromMouseEvent(e));
            else if (isMouseDown && Mode == EditorMode.DeleteMode && lvlGrid.TryGetValue(GetPairFromMouseEvent(e), out mt))
                RemoveTile(mt);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            isMouseDown = false;
            //Altro
        }
        #endregion

        #region Metodi pubblici: NewKulaLevel, LoadNewLevel, ChangeSelectedTile, ChangeTeleportProp, ChangePointerToTile, SaveCurLevel, GetATeleportSurface
        public bool ChangeSelectedTile(string SpecType, string Prop1, string Prop2, string Prop3, string Prop4, uint Prop5, uint Prop6, uint SProp1, uint SProp2)
        {
            if (isLoaded && Mode == EditorMode.SelectMode && lvlGrid.ContainsKey(CurrentPointer))
            {
                KulaLevel.MapTile tile;
                lvlGrid.TryGetValue(CurrentPointer, out tile);
                if (tile == null)
                    return false;

                TreatSwitch(tile, SpecType, Prop1, Prop2, Prop3, Prop4, Prop5, Prop6, SProp1, SProp2);
                if (this != null)
                    OnChangedTileProperties(new ChangedTilePropertiesEventArgs(this));
                return true;
            }
            else
                return false;
        }

        public bool ChangeInsertingTile(string TileType, string SpecType, string Prop1, string Prop2, string Prop3, string Prop4, uint Prop5, uint Prop6, uint SProp1, uint SProp2)
        {
            if (isLoaded && Mode == EditorMode.InsertMode)
            {
                #region Caso in cui la tile da inserire sia null o sia di tipo diverso da quello ora selezionato
                if (TileToBeAdded == null || TileToBeAdded.TileType.ToString() != TileType)
                {
                    isReadyToInsert = false;
                    switch(TileType)
                    {
                        case ("Block"):
                        {
                            TileToBeAdded = new KulaLevel.Block();
                            break;
                        }
                        case ("Placeable"):
                        {
                            TileToBeAdded = new KulaLevel.Placeable();
                            break;
                        }
                        case ("Enemy"):
                        {
                            TileToBeAdded = new KulaLevel.Enemy();
                            break;
                        }
                        default:
                        {
                            TileToBeAdded = null;
                            break;
                        }
                    }
                    return true;
                }
                #endregion
                #region Caso in cui la tile da inserire sia già del tipo selezionato.
                else
                {
                    KulaLevel.MapTile t = TileToBeAdded;
                    TreatSwitch(t, SpecType, Prop1, Prop2, Prop3, Prop4, Prop5, Prop6, SProp1, SProp2);
                    isReadyToInsert = true;
                }
                #endregion
                OnChangedTileProperties(new ChangedTilePropertiesEventArgs(this));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Modifica la destinazione di una superficie teletrasporto con un'altra superficie di teletrasporto.
        /// </summary>
        /// <param name="surfSource">Superficie che punterà all'altro teletrasporto</param>
        /// <param name="surfDest">Superficie puntata dal teletrasporto surfSource</param>
        /// <returns></returns>
        public bool ChangeTeleportProp(KulaLevel.Surface surfSource, KulaLevel.Surface surfDest)
        {
            if ((surfSource != null) &&
                (surfSource.Type == KulaLevel.SurfaceType.Teleport) &&
                (surfDest != null) &&
                (surfDest.Type == KulaLevel.SurfaceType.Teleport))
            {
                if (!surfDest.SurfaceTileCoordinates.Equals(surfSource.SurfaceTileCoordinates))
                    surfSource.NextTeleport = surfDest;
                OnChangedTileProperties(new ChangedTilePropertiesEventArgs(this));
                return true;
            }
            else 
                return false;
        }

        /// <summary>
        /// Modifica il puntatore della griglia. Restituisce true se il cambiamento è avvenuto con successo.
        /// </summary>
        /// <param name="X">Colonna in cui puntare.</param>
        /// <param name="Y">Riga in cui puntare.</param>
        /// <returns>True se e solo se il cambiamento è avvenuto con successo.</returns>
        public bool ChangePointerTo(byte X, byte Y)
        {
            if (X < lvlW && Y < lvlH)
            {
                ChangePointer(new Pair<byte>(X, Y));
                return true;
            }
            else 
                return false;
        }

        public bool NewKulaLevel(string appPath)
        {
            if (!isLoaded)
                return RequestNewLevel();
            else
            {
                DialogResult dr = MessageBox.Show(
                                    "Creating a new level makes you lose the unsaved progress of your previous level.\n" +
                                    "\"Yes\" will save your level, \"No\" will make you lose the unsaved progress, \"Cancel\" will not create a new level.",
                                    "Saving current level?",
                                    MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    if (IsABonusLevel)
                        SaveLevel(appPath + @"\BONUSLEVELS");
                    else
                        SaveLevel(appPath + @"\LEVELS");
                    return RequestNewLevel();
                }
                else if (dr == DialogResult.No)
                    return RequestNewLevel();
                else
                    return false;
            }
        }

        /// <summary>
        /// Salva il livello memorizzato nell'editor a partire dal path specificato.
        /// </summary>
        /// <param name="path">Path di partenza specificato.</param>
        public bool SaveCurLevel(string path)
        {
            if (isLoaded)
                return SaveLevel(path);
            else
                return false;
        }

        /// <summary>
        /// Carica un file con un dialog che parte dal path specificato.
        /// </summary>
        /// <param name="path">Path da cui partirà il dialog.</param>
        /// <returns></returns>
        public bool LoadNewLevel(string loadPath)
        {
            if (!isLoaded)
                return LoadLevel(loadPath);
            else
            {
                DialogResult dr = MessageBox.Show(
                                    "Loading another level makes you lose the unsaved progress of your previous level.\n" +
                                    "\"Yes\" will save your level, \"No\" will make you lose the unsaved progress, \"Cancel\" will not load another level.",
                                    "Saving current level?",
                                    MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    if (this.IsABonusLevel)
                        SaveLevel(GameApp.CurDir() + @"\BONUSLEVELS");
                    else
                        SaveLevel(GameApp.CurDir() + @"\LEVELS");
                    return LoadLevel(loadPath);
                }
                else if (dr == DialogResult.No)
                    return LoadLevel(loadPath);
                else
                    return false;
            }
        }

        #endregion
    }
}
