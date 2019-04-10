using GameUtils;
using EditorViewPort;
using LevelsStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LvlEditor
{
    public partial class MainForm : Form
    {
        private bool isManipulatingOptions = false;
        private bool isReadyToListen = true;
        #region Riferimenti ai controlli da manipolare.
        private RadioButton[] options;
        private Control[] topLabels;
        private Control[] midLabels;
        private Control[] lowLabels;
        private Control[] topControls;
        private Control[] midControls;
        private Control[] lowControls;
        private KulaLevel.Orientation[] a;
        #endregion
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            #region Avvertire l'utenza che questa versione è misera.
            MessageBox.Show(
                "This is the first version of the Level Editor. This version doesn't have the following features: \n" +
                "- Level integrity control, \n" +
                "- Level animation preview, \n" +
                "- More realistic level view, \n" +
                "- Teleport binding.",
                "Level editor v.1.a", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information, 
                MessageBoxDefaultButton.Button1);
            #endregion
            isReadyToListen = false;
            #region definizione di options:
            options = new RadioButton[] 
            { 
                rdoProperty1,
                rdoProperty2,
                rdoProperty3,
                rdoProperty4
            };
            #endregion
            #region definizione di topLabels:
            topLabels = new Control[] 
            { 
                lblTileTyping,
                lblTileType,
                lblSpecificType 
            };
            #endregion
            #region definizione di midLabels:
            midLabels = new Control[]
            { 
                lblTileProperties,
                lblProperty1,
                lblProperty2, 
                lblProperty3, 
                lblProperty4, 
                lblProperty5, 
                lblProperty6 
            };
            #endregion
            #region definizione di lowLabels:
            lowLabels = new Control[] 
            { 
                lblSpecificProperties, 
                lblSpecificProp1, 
                lblSpecificProp2, 
                lblSpecificProp3, 
                lblSpecificProp4, 
                lblSpecificProp5, 
                lblSpecificProp6 
            };
            #endregion
            #region definizione di topControls:
            topControls = new Control[]
            {
                cboTileType,
                cboSpecificType
            };
            #endregion
            #region definizione di midControls:
            midControls = new Control[]
            {
                cboProperty1,
                cboProperty2,
                cboProperty3,
                cboProperty4,
                numProperty5,
                numProperty6
            };
            #endregion
            #region definizione di lowControls:
            lowControls = new Control[]
            {
                numSpecificProp1,
                numSpecificProp2,
                numSpecificProp3,
                numSpecificProp4,
                cboSpecificProp5,
                chkBonusLvl,
                txtSpecificProp7,
                btnSpecificProp7,
                btnApplyChanges
            };
            #endregion
            #region Preparo l'array di direzioni.
            a = new KulaLevel.Orientation[]
                    { 
                        KulaLevel.Orientation.Up, 
                        KulaLevel.Orientation.Down, 
                        KulaLevel.Orientation.Left, 
                        KulaLevel.Orientation.Right 
                    };
            #endregion
            #region Imposto l'indice di tutte le combobox
            cboProperty1.SelectedIndex = 0;
            cboProperty2.SelectedIndex = 0;
            cboProperty3.SelectedIndex = 0;
            cboProperty4.SelectedIndex = 0;
            cboSpecificProp5.SelectedIndex = 0;
            cboSpecificType.SelectedIndex = 0;
            cboTileType.SelectedIndex = 0;
            #endregion
            isReadyToListen = true;
        }

        #region Metodi privati per la preparazione dell'interfaccia dell'editor.
        private string extractFromComboBox(ComboBox c)
        {
            if (c != null)
                return (string)c.Items[c.SelectedIndex];
            else
                return null;
        }
        private void SetupSurfaceInterface(KulaLevel.Block b)
        {
            isReadyToListen = false;
            KulaLevel.Surface surf = b.GetSurfaceAtFace(editLvlEditor.ChosenFaceDirection);
            KulaLevel.Surface nxtTele = surf.NextTeleport;
            changeNumericValues(numSpecificProp1, 2000, 6000, 100, (int)surf.SpikesPeriod);
            changeNumericValues(numSpecificProp2, 2000, 6000, 100, (int)surf.SpikesBegin);
            if (surf.Type.ToString() != "TimedSpikes")
            {
                numSpecificProp1.Enabled = false;
                numSpecificProp2.Enabled = false;
                lblSpecificProp1.Enabled = false;
                lblSpecificProp2.Enabled = false;
            }
            if (nxtTele != null)
                txtSpecificProp7.Text =
                    (
                        "Block @(X: " +
                        nxtTele.BindingBlock.X +
                        ", Y: " +
                        nxtTele.BindingBlock.Y +
                        ") Face: " +
                        TileConverter.Reverse(nxtTele.Orientation)
                    );
            else
                txtSpecificProp7.Text = "";
            if (surf.Type.ToString() != "Teleport")
            {
                btnSpecificProp7.Enabled = false;
                txtSpecificProp7.Enabled = false;
            }
            isReadyToListen = true;
        }
        private void ActivateGoodControls(KulaLevel.MapTile mt, bool isSelectionMode)
        {
            TopSplitter.Panel1.Enabled = true;
            lblTileTyping.Enabled = true;
            BottomSplitter.Panel2.Enabled = true;
            btnApplyChanges.Enabled = true;
            if (!isSelectionMode)
            {
                cboTileType.Enabled = true;
                lblTileType.Enabled = true;
            }
            if (mt != null)
            {
                string s;
                #region Imposto i controlli del pannello superiore.
                lblSpecificType.Enabled = true;
                if (!isSelectionMode)
                {
                    cboTileType.Enabled = true;
                    lblTileType.Enabled = true;
                }

                if (cboTileType.Items.Contains(s = mt.TileType.ToString()))
                    cboTileType.SelectedIndex = cboTileType.Items.IndexOf(s);
                else
                    cboTileType.SelectedIndex = 0;

                cboSpecificType.Enabled = true;
                cboSpecificType.Items.Clear();
                cboSpecificType.Items.AddRange(TileConverter.GetSpecificTypesOf(mt.TileType));
                if (cboSpecificType.Items.Contains(s = TileConverter.FromByteSpecificType(mt.TileType, mt.Type)))
                    cboSpecificType.SelectedIndex = cboSpecificType.Items.IndexOf(s);
                else
                    cboSpecificType.SelectedIndex = 0;
                #endregion

                #region Imposto i controlli del pannello in mezzo e inferiore.
                BottomSplitter.Panel1.Enabled = true;
                lblTileProperties.Enabled = true;
                BottomSplitter.Panel2.Enabled = true;
                btnApplyChanges.Enabled = true;
                #region Caso di blocco
                if (mt.TileType == KulaLevel.TileType.Block)
                {
                    KulaLevel.Block b = (KulaLevel.Block) mt;
                    #region Abilito i controlli del pannello intermedio
                    EnablingTheseControls(true, BottomSplitter.Panel1.Controls);
                    #endregion
                    #region Sistemo i radiobuttons.
                    isManipulatingOptions = true;
                    foreach (RadioButton o in options)
                        o.Checked = false;
                    if (editLvlEditor.ChosenFaceDirection == KulaLevel.Orientation.Up)
                        options[0].Checked = true;
                    else if (editLvlEditor.ChosenFaceDirection == KulaLevel.Orientation.Down)
                        options[1].Checked = true;
                    else if (editLvlEditor.ChosenFaceDirection == KulaLevel.Orientation.Left)
                        options[2].Checked = true;
                    else if (editLvlEditor.ChosenFaceDirection == KulaLevel.Orientation.Right)
                        options[3].Checked = true;
                    isManipulatingOptions = false;
                    #endregion
                    #region Sistemo i combobox e la label principale.
                    lblSpecificProperties.Text = "Block Properties";
                    lblProperty1.Text = "Upper Surface:";
                    lblProperty2.Text = "Lower Surface:";
                    lblProperty3.Text = "Left Surface:";
                    lblProperty4.Text = "Right Surface:";
                    for(int i = 0; i < 4; i++)
                    {
                        ComboBox c = (ComboBox)midControls[i];
                        c.Items.Clear();
                        foreach (KulaLevel.SurfaceType t in TileConverter.surfaces)
                            c.Items.Add(t.ToString());
                        if (c.Items.Contains(s = b.GetSurfaceAtFace(a[i]).Type.ToString()))
                            c.SelectedIndex = c.Items.IndexOf(s);
                        else
                            c.SelectedIndex = 0;
                    }
                    #endregion
                    #region Sistemo i numericupdown
                    changeNumericValues(numProperty5, 2000, 6000, 100, (int)b.DisappearPeriod);
                    changeNumericValues(numProperty6, 0, 2000, 100, (int)b.DisappearBegin);
                    #endregion
                    #region Abilito i controlli del pannello inferiore
                    BottomSplitter.Panel2.Enabled = true;
                    btnSpecificProp7.Enabled = true;
                    btnSpecificProp7.Text = "Bind Teleport";
                    txtSpecificProp7.Enabled = true;
                    lblSpecificProp1.Enabled = true;
                    lblSpecificProp1.Text = "Spikes Period:";
                    lblSpecificProp2.Enabled = true;
                    lblSpecificProp2.Text = "Spikes Begin:";
                    numSpecificProp1.Enabled = true;
                    numSpecificProp2.Enabled = true;
                    #endregion
                    #region Sistemo le proprietà per la superficie scelta
                    SetupSurfaceInterface(b);
                    #endregion
                }
                #endregion
                #region Caso di oggetto posizionabile
                else if (mt.TileType == KulaLevel.TileType.Placeable)
                {
                    KulaLevel.Placeable p = (KulaLevel.Placeable) mt;
                    #region Abilito i controlli del pannello intermedio
                    lblProperty1.Enabled = true;
                    cboProperty1.Enabled = true;
                    #endregion
                    #region Sistemo i testi delle label e della combobox
                    lblTileProperties.Text = "Placeable Properties";
                    lblProperty1.Text = "Orientation";
                    #endregion
                    #region Sistemo la/le combobox
                    if (TileConverter.FromByteSpecificType(p.TileType, p.Type) == "Gravity Changer")
                    {
                        cboProperty2.Items.Clear();
                        foreach (KulaLevel.Orientation o in a)
                            cboProperty2.Items.Add(o.ToString());
                        cboProperty2.SelectedIndex = cboProperty2.Items.IndexOf(p.GChangerDirection.ToString());
                        cboProperty2.Enabled = true;
                        lblProperty2.Enabled = true;
                        lblProperty2.Text = "G.Changer Direction: ";
                    }
                    cboProperty1.Enabled = true;
                    cboProperty1.Items.Clear();
                    foreach (KulaLevel.Orientation o in a)
                        cboProperty1.Items.Add(o.ToString());
                    cboProperty1.SelectedIndex = cboProperty1.Items.IndexOf(p.Orientation.ToString());
                    #endregion
                }
                #endregion
                #region Caso di nemico
                else if (mt.TileType == KulaLevel.TileType.Enemy)
                {
                    KulaLevel.Enemy e = (KulaLevel.Enemy) mt;
                    #region Abilito i controlli del pannello intermedio
                    lblProperty1.Enabled = true;
                    cboProperty1.Enabled = true;
                    #endregion
                    #region Sistemo i testi delle label e della combobox
                    lblTileProperties.Text = "Enemy Properties";
                    lblProperty1.Text = "Orientation";
                    #endregion
                    #region Mi occupo della combobox intermedia.
                    cboProperty1.Enabled = true;
                    cboProperty1.Items.Clear();
                    foreach (KulaLevel.Orientation o in a)
                        cboProperty1.Items.Add(o.ToString());
                    cboProperty1.SelectedIndex = cboProperty1.Items.IndexOf(e.Orientation.ToString());
                    #endregion
                    #region Mi occupo dell'interfaccia inferiore, se il nemico è un sinusoidale
                    if (e.Type == 0)
                    {
                        numSpecificProp2.Enabled = true;
                        numSpecificProp1.Enabled = true;
                        lblSpecificProp1.Enabled = true;
                        lblSpecificProp2.Enabled = true;
                        lblSpecificProp1.Text = "Enemy period:";
                        lblSpecificProp2.Text = "Enemy range:";
                        changeNumericValues(numSpecificProp1, 2000, 6000, 100, (int)e.Period);
                        changeNumericValues(numSpecificProp2, 0, 100, 1, (int)e.Range);
                    }
                    #endregion
                }
                #endregion
                #endregion
            }
        }

        private void changeNumericValues(NumericUpDown nud, int min, int max, int step, int val)
        {
            if (min <= max && val >= min && val <= max && step <= max - min && step >= 1)
            {
                isReadyToListen = false;
                nud.Minimum = min;
                nud.Maximum = max;
                nud.Value = val;
                nud.Increment = step;
                isReadyToListen = true;
            }
        }
        
        private void EnablingTheseControls(bool En_Disen, Control.ControlCollection c)
        {
            foreach (Control cc in c)
                cc.Enabled = En_Disen;
        }
        
        private void DisableAllControls()
        {
            EnablingTheseControls(false, TopSplitter.Panel1.Controls);
            EnablingTheseControls(false, BottomSplitter.Panel1.Controls);
            EnablingTheseControls(false, BottomSplitter.Panel2.Controls);
        }
        
        private void PrepareToDelete()
        {
            isReadyToListen = false;
            DisableAllControls();
            isReadyToListen = true;
        }

        private void PrepareToLvlProp()
        {
            isReadyToListen = false;
            DisableAllControls();

            #region Riabilito tutti i controlli del pannello in fondo
            BottomSplitter.Panel2.Enabled = true;
            foreach (Control c in BottomSplitter.Panel2.Controls)
                c.Enabled = true;
            #endregion

            #region Modifico il testo dei controlli e il loro contenuto
            lblSpecificProperties.Enabled = true;
            lblSpecificProperties.Text = "Level Properties";

            lblSpecificProp1.Text = "Level Width:";
            changeNumericValues(numSpecificProp1, 2, 100, 1, editLvlEditor.LvlWidth);

            lblSpecificProp2.Text = "Level Height:";
            changeNumericValues(numSpecificProp2, 2, 100, 1, editLvlEditor.LvlHeight);

            lblSpecificProp3.Text = "Loss Penalty:";
            changeNumericValues(numSpecificProp3, 50, 10000, 50, (int)editLvlEditor.LvlLossPenalty);
            
            lblSpecificProp4.Text = "Starting Seconds:";
            changeNumericValues(numSpecificProp4, 5, 120, 1, (int)editLvlEditor.LvlStartingSeconds);

            lblSpecificProp5.Text = "Level Theme:";
            if (cboSpecificProp5.Items.Contains(editLvlEditor.LvlTheme))
                cboSpecificProp5.SelectedIndex = cboSpecificProp5.Items.IndexOf(editLvlEditor.LvlTheme);
            else
                cboSpecificProp5.SelectedIndex = 0;
            
            lblSpecificProp6.Text = "Level Type:";
            chkBonusLvl.Checked = editLvlEditor.IsABonusLevel;

            txtSpecificProp7.Text = editLvlEditor.LvlNextLevel;
            btnSpecificProp7.Text = "Find Level";
            btnApplyChanges.Enabled = true;
            #endregion
            isReadyToListen = true;
        }

        private void PrepareToSelect()
        {
            isReadyToListen = false;
            DisableAllControls();
            ActivateGoodControls(editLvlEditor.PointedTile, true);
            isReadyToListen = true;
        }

        private void PrepareToInsert()
        {
            isReadyToListen = false;
            DisableAllControls();
            ActivateGoodControls(editLvlEditor.TileToBeAdded, false);
            isReadyToListen = true;
        }
        #endregion

        private void HandleClosing()
        {
            if (editLvlEditor.IsLoaded)
            {
                DialogResult dr = MessageBox.Show(
                                    "Closing the editor makes you lose the unsaved progress of your previous level.\n" +
                                    "\"Yes\" will save your level, \"No\" will make you lose the unsaved progress, \"Cancel\" will not quit the editor.",
                                    "Saving current level?",
                                    MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    if (editLvlEditor.IsABonusLevel)
                        editLvlEditor.SaveCurLevel(GameApp.CurDir() + @"\BONUSLEVELS");
                    else
                        editLvlEditor.SaveCurLevel(GameApp.CurDir() + @"\LEVELS");
                }
                else if (dr == DialogResult.Cancel)
                    return;
            }
            Application.Exit();
        }

        #region Eventi: exit, onclosing, onclose
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            HandleClosing();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            HandleClosing();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            HandleClosing();
        }
        #endregion

        #region Eventi: NewLevel, SaveLevel, OpenLevel, OpenBonus
        private void tsmiNewLevel_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.NewKulaLevel(GameApp.CurDir()))
                lblMessageViewer.Text = "New level created: editor ready.";
            else
                lblMessageViewer.Text = "New level creation aborted.";
        }

        private void tsmiSaveLevel_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.IsLoaded)
            {
                if (editLvlEditor.IsABonusLevel)
                {
                    if (editLvlEditor.SaveCurLevel(GameApp.CurDir() + @"\BONUSLEVELS"))
                        lblMessageViewer.Text = "Bonus level saved.";
                    else
                        lblMessageViewer.Text = "Bonus level saving aborted.";
                }
                else 
                {
                    if (editLvlEditor.SaveCurLevel(GameApp.CurDir() + @"\LEVELS"))
                        lblMessageViewer.Text = "Regular level saved.";
                    else
                        lblMessageViewer.Text = "Regular level saving aborted.";
                }
            }
            else
                lblMessageViewer.Text = "Level Save aborted: no levels to save.";
            
        }

        private void tsmiOpenLevel_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.LoadNewLevel(GameApp.CurDir() + @"\LEVELS"))
                lblMessageViewer.Text = "Regular level loaded: editor ready.";
            else
                lblMessageViewer.Text = "Regular level loading aborted.";
        }

        private void tsmiOpenBonus_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.LoadNewLevel(GameApp.CurDir() + @"\BONUSLEVELS"))
                lblMessageViewer.Text = "Bonus level loaded: editor ready.";
            else
                lblMessageViewer.Text = "Bonus level loading aborted.";
        }
        #endregion

        #region Eventi: ChangedMode, InsertMode, DeleteMode, SelectMode, LvlPropMode
        private void editLvlEditor_ChangedMode(object sender, EditorViewPort.ChangedModeEventArgs e)
        {
            lblModeDisplay.Text = "(Current mode: " + editLvlEditor.Mode.ToString() + ")";
            EditorViewPort.Editor ed = editLvlEditor;
            switch (ed.Mode)
            {
                case (EditorMode.LvlPropMode):
                {
                    PrepareToLvlProp();
                    this.Invalidate();
                    break;
                }
                case (EditorMode.DeleteMode):
                {
                    PrepareToDelete();
                    this.Invalidate();
                    break;
                }
                case (EditorMode.SelectMode):
                {
                    PrepareToSelect();
                    this.Invalidate();
                    break;
                }
                case (EditorMode.InsertMode):
                {
                    PrepareToInsert();
                    this.Invalidate();
                    break;
                }
            }
        }

        private void tsmiInsertMode_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.IsLoaded)
                editLvlEditor.Mode = EditorViewPort.EditorMode.InsertMode;
        }

        private void tsmiDeleteMode_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.IsLoaded)
                editLvlEditor.Mode = EditorViewPort.EditorMode.DeleteMode;
        }

        private void tsmiSelectMode_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.IsLoaded)
                editLvlEditor.Mode = EditorViewPort.EditorMode.SelectMode;
        }

        private void tsmiLevelProperties_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.IsLoaded)
                editLvlEditor.Mode = EditorViewPort.EditorMode.LvlPropMode;
        }
        #endregion

        #region Eventi: CheckedChanged, btnSpecificProp7Click, btnApplyChanges
        #region CheckedChanged1, CheckedChanged2, CheckedChanged3, CheckedChanged4
        private void rdoProperty1_CheckedChanged(object sender, EventArgs e)
        {
            if (!isManipulatingOptions && rdoProperty1.Checked)
            {
                editLvlEditor.ChosenFaceDirection = a[0];
                KulaLevel.MapTile mt = null;
                if (editLvlEditor.Mode == EditorMode.InsertMode)
                    mt = editLvlEditor.TileToBeAdded;
                else if (editLvlEditor.Mode == EditorMode.SelectMode)
                    mt = editLvlEditor.PointedTile;
                if (mt != null && (mt.TileType == KulaLevel.TileType.Block))
                    SetupSurfaceInterface((KulaLevel.Block)mt);
            }
        }
        private void rdoProperty2_CheckedChanged(object sender, EventArgs e)
        {
            if (!isManipulatingOptions && rdoProperty2.Checked)
            {
                editLvlEditor.ChosenFaceDirection = a[1];
                KulaLevel.MapTile mt = null;
                if (editLvlEditor.Mode == EditorMode.InsertMode)
                    mt = editLvlEditor.TileToBeAdded;
                else if (editLvlEditor.Mode == EditorMode.SelectMode)
                    mt = editLvlEditor.PointedTile;
                if (mt != null && (mt.TileType == KulaLevel.TileType.Block))
                    SetupSurfaceInterface((KulaLevel.Block)mt);
            }
        }
        private void rdoProperty3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isManipulatingOptions && rdoProperty3.Checked)
            {
                editLvlEditor.ChosenFaceDirection = a[2];
                KulaLevel.MapTile mt = null;
                if (editLvlEditor.Mode == EditorMode.InsertMode)
                    mt = editLvlEditor.TileToBeAdded;
                else if (editLvlEditor.Mode == EditorMode.SelectMode)
                    mt = editLvlEditor.PointedTile;
                if (mt != null && (mt.TileType == KulaLevel.TileType.Block))
                    SetupSurfaceInterface((KulaLevel.Block)mt);
            }
        }
        private void rdoProperty4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isManipulatingOptions && rdoProperty4.Checked)
            {
                editLvlEditor.ChosenFaceDirection = a[3];
                KulaLevel.MapTile mt = null;
                if (editLvlEditor.Mode == EditorMode.InsertMode)
                    mt = editLvlEditor.TileToBeAdded;
                else if (editLvlEditor.Mode == EditorMode.SelectMode)
                    mt = editLvlEditor.PointedTile;
                if (mt != null && (mt.TileType == KulaLevel.TileType.Block))
                    SetupSurfaceInterface((KulaLevel.Block)mt);
            }
        }
        #endregion

        private void btnSpecificProp7_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.Mode == EditorMode.LvlPropMode)
            {
                OpenFileDialog d = new OpenFileDialog();
                d.AddExtension = false;
                d.DefaultExt = ".bin";
                d.Filter = "binary files (*.bin) | *.bin";
                d.FilterIndex = 1;
                d.InitialDirectory = GameApp.CurDir() + @"\LEVELS";
                d.RestoreDirectory = true;
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    txtSpecificProp7.Text = d.SafeFileName;
                else
                {
                    txtSpecificProp7.Text = "";
                    R.r("The level has been unbound. It hasn't now any next level.\n");
                }
                PrepareToLvlProp();
            }
            else if (editLvlEditor.Mode == EditorMode.InsertMode || editLvlEditor.Mode == EditorMode.SelectMode)
            {

            }
        }

        private void btnApplyChanges_Click(object sender, EventArgs e)
        {
            if (editLvlEditor.Mode == EditorMode.LvlPropMode)
            {                
                editLvlEditor.LvlLossPenalty = (uint)numSpecificProp3.Value;
                editLvlEditor.LvlStartingSeconds = (uint)numSpecificProp4.Value;
                editLvlEditor.LvlTheme = (string)cboSpecificProp5.Items[cboSpecificProp5.SelectedIndex];
                editLvlEditor.IsABonusLevel = chkBonusLvl.Checked;
                editLvlEditor.LvlNextLevel = txtSpecificProp7.Text;
                editLvlEditor.LvlHeight = (byte)(numSpecificProp2.Value % 256);
                editLvlEditor.LvlWidth = (byte)(numSpecificProp1.Value % 256);
                PrepareToLvlProp();
            }
            else if (editLvlEditor.Mode == EditorMode.InsertMode)
            {
                editLvlEditor.ChangeInsertingTile
                    (
                    extractFromComboBox(cboTileType),
                    extractFromComboBox(cboSpecificType),
                    extractFromComboBox(cboProperty1),
                    extractFromComboBox(cboProperty2),
                    extractFromComboBox(cboProperty3),
                    extractFromComboBox(cboProperty4),
                    (uint)numProperty5.Value,
                    (uint)numProperty6.Value,
                    (uint)numSpecificProp1.Value,
                    (uint)numSpecificProp2.Value
                    );
                editLvlEditor.ChangeInsertingTile
                    (
                    extractFromComboBox(cboTileType),
                    extractFromComboBox(cboSpecificType),
                    extractFromComboBox(cboProperty1),
                    extractFromComboBox(cboProperty2),
                    extractFromComboBox(cboProperty3),
                    extractFromComboBox(cboProperty4),
                    (uint)numProperty5.Value,
                    (uint)numProperty6.Value,
                    (uint)numSpecificProp1.Value,
                    (uint)numSpecificProp2.Value
                    );
                PrepareToInsert();
            }
            else if (editLvlEditor.Mode == EditorMode.SelectMode)
            {
                editLvlEditor.ChangeSelectedTile
                    (
                    extractFromComboBox(cboSpecificType),
                    extractFromComboBox(cboProperty1),
                    extractFromComboBox(cboProperty2),
                    extractFromComboBox(cboProperty3),
                    extractFromComboBox(cboProperty4),
                    (uint)numProperty5.Value,
                    (uint)numProperty6.Value,
                    (uint)numSpecificProp1.Value,
                    (uint)numSpecificProp2.Value
                    );
                editLvlEditor.ChangeSelectedTile
                    (
                    extractFromComboBox(cboSpecificType),
                    extractFromComboBox(cboProperty1),
                    extractFromComboBox(cboProperty2),
                    extractFromComboBox(cboProperty3),
                    extractFromComboBox(cboProperty4),
                    (uint)numProperty5.Value,
                    (uint)numProperty6.Value,
                    (uint)numSpecificProp1.Value,
                    (uint)numSpecificProp2.Value
                    );
                PrepareToSelect();
            }
        }
        #endregion

        #region Eventi: ChangedPointer
        private void editLvlEditor_ChangedPointer(object sender, ChangedPointerEventArgs e)
        {
            if (editLvlEditor.Mode == EditorMode.SelectMode)
                PrepareToSelect();
        }
        private void cboTileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isReadyToListen)
                btnApplyChanges_Click(sender, e);
        }
        #endregion

        

        
    }
}
