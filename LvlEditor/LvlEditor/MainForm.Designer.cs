using LevelsStructure;
namespace LvlEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MessageStrip = new System.Windows.Forms.StatusStrip();
            this.prbMainProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lblMessageViewer = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblModeDisplay = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.tsmiFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNewLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOpenLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenBonus = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInsertMode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteMode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSelectMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiLevelProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.NonEditorPanel = new System.Windows.Forms.Panel();
            this.TopSplitter = new System.Windows.Forms.SplitContainer();
            this.cboSpecificType = new System.Windows.Forms.ComboBox();
            this.lblSpecificType = new System.Windows.Forms.Label();
            this.lblTileType = new System.Windows.Forms.Label();
            this.cboTileType = new System.Windows.Forms.ComboBox();
            this.lblTileTyping = new System.Windows.Forms.Label();
            this.BottomSplitter = new System.Windows.Forms.SplitContainer();
            this.rdoProperty4 = new System.Windows.Forms.RadioButton();
            this.rdoProperty3 = new System.Windows.Forms.RadioButton();
            this.rdoProperty2 = new System.Windows.Forms.RadioButton();
            this.rdoProperty1 = new System.Windows.Forms.RadioButton();
            this.numProperty6 = new System.Windows.Forms.NumericUpDown();
            this.numProperty5 = new System.Windows.Forms.NumericUpDown();
            this.lblProperty6 = new System.Windows.Forms.Label();
            this.lblProperty5 = new System.Windows.Forms.Label();
            this.cboProperty4 = new System.Windows.Forms.ComboBox();
            this.lblProperty4 = new System.Windows.Forms.Label();
            this.cboProperty3 = new System.Windows.Forms.ComboBox();
            this.lblProperty3 = new System.Windows.Forms.Label();
            this.cboProperty2 = new System.Windows.Forms.ComboBox();
            this.lblProperty2 = new System.Windows.Forms.Label();
            this.cboProperty1 = new System.Windows.Forms.ComboBox();
            this.lblProperty1 = new System.Windows.Forms.Label();
            this.btnApplyChanges = new System.Windows.Forms.Button();
            this.txtSpecificProp7 = new System.Windows.Forms.TextBox();
            this.lblSpecificProp6 = new System.Windows.Forms.Label();
            this.chkBonusLvl = new System.Windows.Forms.CheckBox();
            this.cboSpecificProp5 = new System.Windows.Forms.ComboBox();
            this.numSpecificProp4 = new System.Windows.Forms.NumericUpDown();
            this.lblSpecificProp5 = new System.Windows.Forms.Label();
            this.numSpecificProp3 = new System.Windows.Forms.NumericUpDown();
            this.lblSpecificProp3 = new System.Windows.Forms.Label();
            this.lblSpecificProp4 = new System.Windows.Forms.Label();
            this.btnSpecificProp7 = new System.Windows.Forms.Button();
            this.numSpecificProp2 = new System.Windows.Forms.NumericUpDown();
            this.lblSpecificProperties = new System.Windows.Forms.Label();
            this.numSpecificProp1 = new System.Windows.Forms.NumericUpDown();
            this.lblSpecificProp1 = new System.Windows.Forms.Label();
            this.lblSpecificProp2 = new System.Windows.Forms.Label();
            this.lblTileProperties = new System.Windows.Forms.Label();
            this.LvlEditorContainer = new System.Windows.Forms.Panel();
            this.editLvlEditor = new EditorViewPort.Editor();
            this.MessageStrip.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.NonEditorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TopSplitter)).BeginInit();
            this.TopSplitter.Panel1.SuspendLayout();
            this.TopSplitter.Panel2.SuspendLayout();
            this.TopSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BottomSplitter)).BeginInit();
            this.BottomSplitter.Panel1.SuspendLayout();
            this.BottomSplitter.Panel2.SuspendLayout();
            this.BottomSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProperty6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProperty5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpecificProp4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpecificProp3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpecificProp2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpecificProp1)).BeginInit();
            this.LvlEditorContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // MessageStrip
            // 
            this.MessageStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prbMainProgress,
            this.lblMessageViewer,
            this.lblModeDisplay});
            this.MessageStrip.Location = new System.Drawing.Point(0, 540);
            this.MessageStrip.Name = "MessageStrip";
            this.MessageStrip.Size = new System.Drawing.Size(784, 22);
            this.MessageStrip.TabIndex = 0;
            this.MessageStrip.Text = "statusStrip1";
            // 
            // prbMainProgress
            // 
            this.prbMainProgress.Name = "prbMainProgress";
            this.prbMainProgress.Size = new System.Drawing.Size(150, 16);
            // 
            // lblMessageViewer
            // 
            this.lblMessageViewer.BackColor = System.Drawing.Color.Transparent;
            this.lblMessageViewer.Name = "lblMessageViewer";
            this.lblMessageViewer.Size = new System.Drawing.Size(211, 17);
            this.lblMessageViewer.Text = "Ready to load or create an empty level.";
            // 
            // lblModeDisplay
            // 
            this.lblModeDisplay.BackColor = System.Drawing.Color.Transparent;
            this.lblModeDisplay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblModeDisplay.ForeColor = System.Drawing.Color.Blue;
            this.lblModeDisplay.Name = "lblModeDisplay";
            this.lblModeDisplay.Size = new System.Drawing.Size(118, 17);
            this.lblModeDisplay.Text = "(Empty Level Mode)";
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFileMenu,
            this.tsmiEdit});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(784, 24);
            this.MainMenu.TabIndex = 1;
            this.MainMenu.Text = "menuStrip1";
            // 
            // tsmiFileMenu
            // 
            this.tsmiFileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNewLevel,
            this.tsmiSaveLevel,
            this.toolStripSeparator3,
            this.tsmiOpenLevel,
            this.tsmiOpenBonus,
            this.toolStripSeparator2,
            this.tsmiExit});
            this.tsmiFileMenu.Name = "tsmiFileMenu";
            this.tsmiFileMenu.Size = new System.Drawing.Size(37, 20);
            this.tsmiFileMenu.Text = "File";
            // 
            // tsmiNewLevel
            // 
            this.tsmiNewLevel.Name = "tsmiNewLevel";
            this.tsmiNewLevel.Size = new System.Drawing.Size(166, 22);
            this.tsmiNewLevel.Text = "New level";
            this.tsmiNewLevel.Click += new System.EventHandler(this.tsmiNewLevel_Click);
            // 
            // tsmiSaveLevel
            // 
            this.tsmiSaveLevel.Name = "tsmiSaveLevel";
            this.tsmiSaveLevel.Size = new System.Drawing.Size(166, 22);
            this.tsmiSaveLevel.Text = "Save level as...";
            this.tsmiSaveLevel.Click += new System.EventHandler(this.tsmiSaveLevel_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(163, 6);
            // 
            // tsmiOpenLevel
            // 
            this.tsmiOpenLevel.Name = "tsmiOpenLevel";
            this.tsmiOpenLevel.Size = new System.Drawing.Size(166, 22);
            this.tsmiOpenLevel.Text = "Open level";
            this.tsmiOpenLevel.Click += new System.EventHandler(this.tsmiOpenLevel_Click);
            // 
            // tsmiOpenBonus
            // 
            this.tsmiOpenBonus.Name = "tsmiOpenBonus";
            this.tsmiOpenBonus.Size = new System.Drawing.Size(166, 22);
            this.tsmiOpenBonus.Text = "Open bonus level";
            this.tsmiOpenBonus.Click += new System.EventHandler(this.tsmiOpenBonus_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(163, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(166, 22);
            this.tsmiExit.Text = "Exit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiInsertMode,
            this.tsmiDeleteMode,
            this.tsmiSelectMode,
            this.toolStripSeparator1,
            this.tsmiLevelProperties});
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(39, 20);
            this.tsmiEdit.Text = "Edit";
            // 
            // tsmiInsertMode
            // 
            this.tsmiInsertMode.Name = "tsmiInsertMode";
            this.tsmiInsertMode.Size = new System.Drawing.Size(157, 22);
            this.tsmiInsertMode.Text = "Insert Mode";
            this.tsmiInsertMode.Click += new System.EventHandler(this.tsmiInsertMode_Click);
            // 
            // tsmiDeleteMode
            // 
            this.tsmiDeleteMode.Name = "tsmiDeleteMode";
            this.tsmiDeleteMode.Size = new System.Drawing.Size(157, 22);
            this.tsmiDeleteMode.Text = "Delete Mode";
            this.tsmiDeleteMode.Click += new System.EventHandler(this.tsmiDeleteMode_Click);
            // 
            // tsmiSelectMode
            // 
            this.tsmiSelectMode.Name = "tsmiSelectMode";
            this.tsmiSelectMode.Size = new System.Drawing.Size(157, 22);
            this.tsmiSelectMode.Text = "Select Mode";
            this.tsmiSelectMode.Click += new System.EventHandler(this.tsmiSelectMode_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(154, 6);
            // 
            // tsmiLevelProperties
            // 
            this.tsmiLevelProperties.Name = "tsmiLevelProperties";
            this.tsmiLevelProperties.Size = new System.Drawing.Size(157, 22);
            this.tsmiLevelProperties.Text = "Level Properties";
            this.tsmiLevelProperties.Click += new System.EventHandler(this.tsmiLevelProperties_Click);
            // 
            // NonEditorPanel
            // 
            this.NonEditorPanel.BackColor = System.Drawing.Color.Transparent;
            this.NonEditorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NonEditorPanel.Controls.Add(this.TopSplitter);
            this.NonEditorPanel.Location = new System.Drawing.Point(517, 24);
            this.NonEditorPanel.Name = "NonEditorPanel";
            this.NonEditorPanel.Size = new System.Drawing.Size(267, 516);
            this.NonEditorPanel.TabIndex = 3;
            // 
            // TopSplitter
            // 
            this.TopSplitter.BackColor = System.Drawing.Color.Transparent;
            this.TopSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopSplitter.IsSplitterFixed = true;
            this.TopSplitter.Location = new System.Drawing.Point(0, 0);
            this.TopSplitter.Name = "TopSplitter";
            this.TopSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // TopSplitter.Panel1
            // 
            this.TopSplitter.Panel1.BackColor = System.Drawing.Color.LightGray;
            this.TopSplitter.Panel1.Controls.Add(this.cboSpecificType);
            this.TopSplitter.Panel1.Controls.Add(this.lblSpecificType);
            this.TopSplitter.Panel1.Controls.Add(this.lblTileType);
            this.TopSplitter.Panel1.Controls.Add(this.cboTileType);
            this.TopSplitter.Panel1.Controls.Add(this.lblTileTyping);
            this.TopSplitter.Panel1MinSize = 80;
            // 
            // TopSplitter.Panel2
            // 
            this.TopSplitter.Panel2.BackColor = System.Drawing.Color.LightGray;
            this.TopSplitter.Panel2.Controls.Add(this.BottomSplitter);
            this.TopSplitter.Panel2.Controls.Add(this.lblTileProperties);
            this.TopSplitter.Panel2MinSize = 100;
            this.TopSplitter.Size = new System.Drawing.Size(265, 514);
            this.TopSplitter.SplitterDistance = 80;
            this.TopSplitter.SplitterWidth = 8;
            this.TopSplitter.TabIndex = 0;
            // 
            // cboSpecificType
            // 
            this.cboSpecificType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSpecificType.Enabled = false;
            this.cboSpecificType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboSpecificType.FormattingEnabled = true;
            this.cboSpecificType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cboSpecificType.Items.AddRange(new object[] {
            "Normal",
            "Transparent",
            "Intermittent",
            "Fire",
            "Ice",
            "Destructible"});
            this.cboSpecificType.Location = new System.Drawing.Point(133, 52);
            this.cboSpecificType.MaxDropDownItems = 16;
            this.cboSpecificType.Name = "cboSpecificType";
            this.cboSpecificType.Size = new System.Drawing.Size(121, 21);
            this.cboSpecificType.TabIndex = 5;
            this.cboSpecificType.SelectedIndexChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblSpecificType
            // 
            this.lblSpecificType.BackColor = System.Drawing.Color.Transparent;
            this.lblSpecificType.Enabled = false;
            this.lblSpecificType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificType.Location = new System.Drawing.Point(3, 51);
            this.lblSpecificType.Name = "lblSpecificType";
            this.lblSpecificType.Size = new System.Drawing.Size(124, 21);
            this.lblSpecificType.TabIndex = 3;
            this.lblSpecificType.Text = "Specific type:";
            this.lblSpecificType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTileType
            // 
            this.lblTileType.BackColor = System.Drawing.Color.Transparent;
            this.lblTileType.Enabled = false;
            this.lblTileType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTileType.Location = new System.Drawing.Point(3, 24);
            this.lblTileType.Name = "lblTileType";
            this.lblTileType.Size = new System.Drawing.Size(124, 21);
            this.lblTileType.TabIndex = 2;
            this.lblTileType.Text = "Tile type:";
            this.lblTileType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboTileType
            // 
            this.cboTileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTileType.Enabled = false;
            this.cboTileType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTileType.FormattingEnabled = true;
            this.cboTileType.Items.AddRange(new object[] {
            "Block",
            "Placeable",
            "Enemy"});
            this.cboTileType.Location = new System.Drawing.Point(133, 25);
            this.cboTileType.Name = "cboTileType";
            this.cboTileType.Size = new System.Drawing.Size(121, 21);
            this.cboTileType.TabIndex = 1;
            this.cboTileType.SelectedIndexChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblTileTyping
            // 
            this.lblTileTyping.BackColor = System.Drawing.Color.Silver;
            this.lblTileTyping.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTileTyping.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTileTyping.Enabled = false;
            this.lblTileTyping.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTileTyping.ForeColor = System.Drawing.Color.Gray;
            this.lblTileTyping.Location = new System.Drawing.Point(0, 0);
            this.lblTileTyping.Name = "lblTileTyping";
            this.lblTileTyping.Size = new System.Drawing.Size(265, 22);
            this.lblTileTyping.TabIndex = 0;
            this.lblTileTyping.Text = "Select a tile to be placed";
            this.lblTileTyping.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BottomSplitter
            // 
            this.BottomSplitter.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.BottomSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomSplitter.IsSplitterFixed = true;
            this.BottomSplitter.Location = new System.Drawing.Point(0, 22);
            this.BottomSplitter.Name = "BottomSplitter";
            this.BottomSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // BottomSplitter.Panel1
            // 
            this.BottomSplitter.Panel1.BackColor = System.Drawing.Color.LightGray;
            this.BottomSplitter.Panel1.Controls.Add(this.rdoProperty4);
            this.BottomSplitter.Panel1.Controls.Add(this.rdoProperty3);
            this.BottomSplitter.Panel1.Controls.Add(this.rdoProperty2);
            this.BottomSplitter.Panel1.Controls.Add(this.rdoProperty1);
            this.BottomSplitter.Panel1.Controls.Add(this.numProperty6);
            this.BottomSplitter.Panel1.Controls.Add(this.numProperty5);
            this.BottomSplitter.Panel1.Controls.Add(this.lblProperty6);
            this.BottomSplitter.Panel1.Controls.Add(this.lblProperty5);
            this.BottomSplitter.Panel1.Controls.Add(this.cboProperty4);
            this.BottomSplitter.Panel1.Controls.Add(this.lblProperty4);
            this.BottomSplitter.Panel1.Controls.Add(this.cboProperty3);
            this.BottomSplitter.Panel1.Controls.Add(this.lblProperty3);
            this.BottomSplitter.Panel1.Controls.Add(this.cboProperty2);
            this.BottomSplitter.Panel1.Controls.Add(this.lblProperty2);
            this.BottomSplitter.Panel1.Controls.Add(this.cboProperty1);
            this.BottomSplitter.Panel1.Controls.Add(this.lblProperty1);
            this.BottomSplitter.Panel1MinSize = 50;
            // 
            // BottomSplitter.Panel2
            // 
            this.BottomSplitter.Panel2.BackColor = System.Drawing.Color.LightGray;
            this.BottomSplitter.Panel2.Controls.Add(this.btnApplyChanges);
            this.BottomSplitter.Panel2.Controls.Add(this.txtSpecificProp7);
            this.BottomSplitter.Panel2.Controls.Add(this.lblSpecificProp6);
            this.BottomSplitter.Panel2.Controls.Add(this.chkBonusLvl);
            this.BottomSplitter.Panel2.Controls.Add(this.cboSpecificProp5);
            this.BottomSplitter.Panel2.Controls.Add(this.numSpecificProp4);
            this.BottomSplitter.Panel2.Controls.Add(this.lblSpecificProp5);
            this.BottomSplitter.Panel2.Controls.Add(this.numSpecificProp3);
            this.BottomSplitter.Panel2.Controls.Add(this.lblSpecificProp3);
            this.BottomSplitter.Panel2.Controls.Add(this.lblSpecificProp4);
            this.BottomSplitter.Panel2.Controls.Add(this.btnSpecificProp7);
            this.BottomSplitter.Panel2.Controls.Add(this.numSpecificProp2);
            this.BottomSplitter.Panel2.Controls.Add(this.lblSpecificProperties);
            this.BottomSplitter.Panel2.Controls.Add(this.numSpecificProp1);
            this.BottomSplitter.Panel2.Controls.Add(this.lblSpecificProp1);
            this.BottomSplitter.Panel2.Controls.Add(this.lblSpecificProp2);
            this.BottomSplitter.Panel2MinSize = 100;
            this.BottomSplitter.Size = new System.Drawing.Size(265, 404);
            this.BottomSplitter.SplitterDistance = 165;
            this.BottomSplitter.SplitterWidth = 8;
            this.BottomSplitter.TabIndex = 2;
            // 
            // rdoProperty4
            // 
            this.rdoProperty4.AutoSize = true;
            this.rdoProperty4.Enabled = false;
            this.rdoProperty4.Location = new System.Drawing.Point(6, 87);
            this.rdoProperty4.Name = "rdoProperty4";
            this.rdoProperty4.Size = new System.Drawing.Size(14, 13);
            this.rdoProperty4.TabIndex = 22;
            this.rdoProperty4.UseVisualStyleBackColor = true;
            this.rdoProperty4.CheckedChanged += new System.EventHandler(this.rdoProperty4_CheckedChanged);
            // 
            // rdoProperty3
            // 
            this.rdoProperty3.AutoSize = true;
            this.rdoProperty3.Enabled = false;
            this.rdoProperty3.Location = new System.Drawing.Point(6, 60);
            this.rdoProperty3.Name = "rdoProperty3";
            this.rdoProperty3.Size = new System.Drawing.Size(14, 13);
            this.rdoProperty3.TabIndex = 21;
            this.rdoProperty3.UseVisualStyleBackColor = true;
            this.rdoProperty3.CheckedChanged += new System.EventHandler(this.rdoProperty3_CheckedChanged);
            // 
            // rdoProperty2
            // 
            this.rdoProperty2.AutoSize = true;
            this.rdoProperty2.Enabled = false;
            this.rdoProperty2.Location = new System.Drawing.Point(6, 33);
            this.rdoProperty2.Name = "rdoProperty2";
            this.rdoProperty2.Size = new System.Drawing.Size(14, 13);
            this.rdoProperty2.TabIndex = 20;
            this.rdoProperty2.UseVisualStyleBackColor = true;
            this.rdoProperty2.CheckedChanged += new System.EventHandler(this.rdoProperty2_CheckedChanged);
            // 
            // rdoProperty1
            // 
            this.rdoProperty1.AutoSize = true;
            this.rdoProperty1.Checked = true;
            this.rdoProperty1.Enabled = false;
            this.rdoProperty1.Location = new System.Drawing.Point(6, 6);
            this.rdoProperty1.Name = "rdoProperty1";
            this.rdoProperty1.Size = new System.Drawing.Size(14, 13);
            this.rdoProperty1.TabIndex = 19;
            this.rdoProperty1.TabStop = true;
            this.rdoProperty1.UseVisualStyleBackColor = true;
            this.rdoProperty1.CheckedChanged += new System.EventHandler(this.rdoProperty1_CheckedChanged);
            // 
            // numProperty6
            // 
            this.numProperty6.Enabled = false;
            this.numProperty6.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numProperty6.Location = new System.Drawing.Point(134, 138);
            this.numProperty6.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numProperty6.Name = "numProperty6";
            this.numProperty6.ReadOnly = true;
            this.numProperty6.Size = new System.Drawing.Size(120, 20);
            this.numProperty6.TabIndex = 18;
            this.numProperty6.ThousandsSeparator = true;
            this.numProperty6.ValueChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // numProperty5
            // 
            this.numProperty5.Enabled = false;
            this.numProperty5.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numProperty5.Location = new System.Drawing.Point(134, 112);
            this.numProperty5.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.numProperty5.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numProperty5.Name = "numProperty5";
            this.numProperty5.ReadOnly = true;
            this.numProperty5.Size = new System.Drawing.Size(120, 20);
            this.numProperty5.TabIndex = 17;
            this.numProperty5.ThousandsSeparator = true;
            this.numProperty5.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numProperty5.ValueChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblProperty6
            // 
            this.lblProperty6.BackColor = System.Drawing.Color.Transparent;
            this.lblProperty6.Enabled = false;
            this.lblProperty6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperty6.Location = new System.Drawing.Point(3, 137);
            this.lblProperty6.Name = "lblProperty6";
            this.lblProperty6.Size = new System.Drawing.Size(124, 21);
            this.lblProperty6.TabIndex = 16;
            this.lblProperty6.Text = "Intermittent Begin:";
            this.lblProperty6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblProperty5
            // 
            this.lblProperty5.BackColor = System.Drawing.Color.Transparent;
            this.lblProperty5.Enabled = false;
            this.lblProperty5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperty5.Location = new System.Drawing.Point(3, 110);
            this.lblProperty5.Name = "lblProperty5";
            this.lblProperty5.Size = new System.Drawing.Size(124, 21);
            this.lblProperty5.TabIndex = 14;
            this.lblProperty5.Text = "Intermittent Period:";
            this.lblProperty5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboProperty4
            // 
            this.cboProperty4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProperty4.Enabled = false;
            this.cboProperty4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboProperty4.FormattingEnabled = true;
            this.cboProperty4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cboProperty4.Items.AddRange(new object[] {
            "Nothing",
            "Ice",
            "Fire",
            "Spikes",
            "Timed Spikes",
            "Ramp",
            "Teleport",
            "Exit",
            "No Jump"});
            this.cboProperty4.Location = new System.Drawing.Point(133, 84);
            this.cboProperty4.MaxDropDownItems = 16;
            this.cboProperty4.Name = "cboProperty4";
            this.cboProperty4.Size = new System.Drawing.Size(121, 21);
            this.cboProperty4.TabIndex = 13;
            this.cboProperty4.SelectedIndexChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblProperty4
            // 
            this.lblProperty4.BackColor = System.Drawing.Color.Transparent;
            this.lblProperty4.Enabled = false;
            this.lblProperty4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperty4.Location = new System.Drawing.Point(3, 83);
            this.lblProperty4.Name = "lblProperty4";
            this.lblProperty4.Size = new System.Drawing.Size(124, 21);
            this.lblProperty4.TabIndex = 12;
            this.lblProperty4.Text = "Right Surface:";
            this.lblProperty4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboProperty3
            // 
            this.cboProperty3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProperty3.Enabled = false;
            this.cboProperty3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboProperty3.FormattingEnabled = true;
            this.cboProperty3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cboProperty3.Items.AddRange(new object[] {
            "Nothing",
            "Ice",
            "Fire",
            "Spikes",
            "Timed Spikes",
            "Ramp",
            "Teleport",
            "Exit",
            "No Jump"});
            this.cboProperty3.Location = new System.Drawing.Point(133, 57);
            this.cboProperty3.MaxDropDownItems = 16;
            this.cboProperty3.Name = "cboProperty3";
            this.cboProperty3.Size = new System.Drawing.Size(121, 21);
            this.cboProperty3.TabIndex = 11;
            this.cboProperty3.SelectedIndexChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblProperty3
            // 
            this.lblProperty3.BackColor = System.Drawing.Color.Transparent;
            this.lblProperty3.Enabled = false;
            this.lblProperty3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperty3.Location = new System.Drawing.Point(3, 56);
            this.lblProperty3.Name = "lblProperty3";
            this.lblProperty3.Size = new System.Drawing.Size(124, 21);
            this.lblProperty3.TabIndex = 10;
            this.lblProperty3.Text = "Left Surface:";
            this.lblProperty3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboProperty2
            // 
            this.cboProperty2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProperty2.Enabled = false;
            this.cboProperty2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboProperty2.FormattingEnabled = true;
            this.cboProperty2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cboProperty2.Items.AddRange(new object[] {
            "Nothing",
            "Ice",
            "Fire",
            "Spikes",
            "Timed Spikes",
            "Ramp",
            "Teleport",
            "Exit",
            "No Jump"});
            this.cboProperty2.Location = new System.Drawing.Point(133, 30);
            this.cboProperty2.MaxDropDownItems = 16;
            this.cboProperty2.Name = "cboProperty2";
            this.cboProperty2.Size = new System.Drawing.Size(121, 21);
            this.cboProperty2.TabIndex = 9;
            this.cboProperty2.SelectedIndexChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblProperty2
            // 
            this.lblProperty2.BackColor = System.Drawing.Color.Transparent;
            this.lblProperty2.Enabled = false;
            this.lblProperty2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperty2.Location = new System.Drawing.Point(3, 29);
            this.lblProperty2.Name = "lblProperty2";
            this.lblProperty2.Size = new System.Drawing.Size(124, 21);
            this.lblProperty2.TabIndex = 8;
            this.lblProperty2.Text = "Lower Surface:";
            this.lblProperty2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboProperty1
            // 
            this.cboProperty1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProperty1.Enabled = false;
            this.cboProperty1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboProperty1.FormattingEnabled = true;
            this.cboProperty1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cboProperty1.Items.AddRange(new object[] {
            "Nothing",
            "Ice",
            "Fire",
            "Spikes",
            "Timed Spikes",
            "Ramp",
            "Teleport",
            "Exit",
            "No Jump"});
            this.cboProperty1.Location = new System.Drawing.Point(133, 3);
            this.cboProperty1.MaxDropDownItems = 16;
            this.cboProperty1.Name = "cboProperty1";
            this.cboProperty1.Size = new System.Drawing.Size(121, 21);
            this.cboProperty1.TabIndex = 7;
            this.cboProperty1.SelectedIndexChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblProperty1
            // 
            this.lblProperty1.BackColor = System.Drawing.Color.Transparent;
            this.lblProperty1.Enabled = false;
            this.lblProperty1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperty1.Location = new System.Drawing.Point(3, 2);
            this.lblProperty1.Name = "lblProperty1";
            this.lblProperty1.Size = new System.Drawing.Size(124, 21);
            this.lblProperty1.TabIndex = 6;
            this.lblProperty1.Text = "Upper Surface:";
            this.lblProperty1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnApplyChanges
            // 
            this.btnApplyChanges.Enabled = false;
            this.btnApplyChanges.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyChanges.Location = new System.Drawing.Point(77, 204);
            this.btnApplyChanges.Name = "btnApplyChanges";
            this.btnApplyChanges.Size = new System.Drawing.Size(99, 24);
            this.btnApplyChanges.TabIndex = 35;
            this.btnApplyChanges.Text = "Apply Changes";
            this.btnApplyChanges.UseVisualStyleBackColor = true;
            this.btnApplyChanges.Click += new System.EventHandler(this.btnApplyChanges_Click);
            // 
            // txtSpecificProp7
            // 
            this.txtSpecificProp7.Enabled = false;
            this.txtSpecificProp7.Location = new System.Drawing.Point(112, 178);
            this.txtSpecificProp7.Name = "txtSpecificProp7";
            this.txtSpecificProp7.ReadOnly = true;
            this.txtSpecificProp7.Size = new System.Drawing.Size(142, 20);
            this.txtSpecificProp7.TabIndex = 34;
            this.txtSpecificProp7.TextChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblSpecificProp6
            // 
            this.lblSpecificProp6.BackColor = System.Drawing.Color.Transparent;
            this.lblSpecificProp6.Enabled = false;
            this.lblSpecificProp6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificProp6.Location = new System.Drawing.Point(4, 152);
            this.lblSpecificProp6.Name = "lblSpecificProp6";
            this.lblSpecificProp6.Size = new System.Drawing.Size(124, 21);
            this.lblSpecificProp6.TabIndex = 33;
            this.lblSpecificProp6.Text = "Level Type:";
            this.lblSpecificProp6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkBonusLvl
            // 
            this.chkBonusLvl.AutoSize = true;
            this.chkBonusLvl.Enabled = false;
            this.chkBonusLvl.Location = new System.Drawing.Point(133, 155);
            this.chkBonusLvl.Name = "chkBonusLvl";
            this.chkBonusLvl.Size = new System.Drawing.Size(101, 17);
            this.chkBonusLvl.TabIndex = 32;
            this.chkBonusLvl.Text = "Is this a Bonus?";
            this.chkBonusLvl.UseVisualStyleBackColor = true;
            this.chkBonusLvl.CheckedChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // cboSpecificProp5
            // 
            this.cboSpecificProp5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSpecificProp5.Enabled = false;
            this.cboSpecificProp5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboSpecificProp5.FormattingEnabled = true;
            this.cboSpecificProp5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cboSpecificProp5.Items.AddRange(new object[] {
            "Egypt",
            "Hills",
            "Inca",
            "Arctic",
            "Rust",
            "Field",
            "Haze",
            "Atlantis",
            "Mars",
            "Hell",
            "Bonus"});
            this.cboSpecificProp5.Location = new System.Drawing.Point(134, 128);
            this.cboSpecificProp5.MaxDropDownItems = 16;
            this.cboSpecificProp5.Name = "cboSpecificProp5";
            this.cboSpecificProp5.Size = new System.Drawing.Size(121, 21);
            this.cboSpecificProp5.TabIndex = 24;
            this.cboSpecificProp5.SelectedIndexChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // numSpecificProp4
            // 
            this.numSpecificProp4.Enabled = false;
            this.numSpecificProp4.Location = new System.Drawing.Point(134, 102);
            this.numSpecificProp4.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numSpecificProp4.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numSpecificProp4.Name = "numSpecificProp4";
            this.numSpecificProp4.ReadOnly = true;
            this.numSpecificProp4.Size = new System.Drawing.Size(120, 20);
            this.numSpecificProp4.TabIndex = 31;
            this.numSpecificProp4.ThousandsSeparator = true;
            this.numSpecificProp4.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numSpecificProp4.ValueChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblSpecificProp5
            // 
            this.lblSpecificProp5.BackColor = System.Drawing.Color.Transparent;
            this.lblSpecificProp5.Enabled = false;
            this.lblSpecificProp5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificProp5.Location = new System.Drawing.Point(4, 127);
            this.lblSpecificProp5.Name = "lblSpecificProp5";
            this.lblSpecificProp5.Size = new System.Drawing.Size(124, 21);
            this.lblSpecificProp5.TabIndex = 23;
            this.lblSpecificProp5.Text = "Level Theme:";
            this.lblSpecificProp5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numSpecificProp3
            // 
            this.numSpecificProp3.Enabled = false;
            this.numSpecificProp3.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numSpecificProp3.Location = new System.Drawing.Point(134, 76);
            this.numSpecificProp3.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numSpecificProp3.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numSpecificProp3.Name = "numSpecificProp3";
            this.numSpecificProp3.ReadOnly = true;
            this.numSpecificProp3.Size = new System.Drawing.Size(120, 20);
            this.numSpecificProp3.TabIndex = 30;
            this.numSpecificProp3.ThousandsSeparator = true;
            this.numSpecificProp3.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numSpecificProp3.ValueChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblSpecificProp3
            // 
            this.lblSpecificProp3.BackColor = System.Drawing.Color.Transparent;
            this.lblSpecificProp3.Enabled = false;
            this.lblSpecificProp3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificProp3.Location = new System.Drawing.Point(3, 74);
            this.lblSpecificProp3.Name = "lblSpecificProp3";
            this.lblSpecificProp3.Size = new System.Drawing.Size(124, 21);
            this.lblSpecificProp3.TabIndex = 28;
            this.lblSpecificProp3.Text = "Loss Penalty:";
            this.lblSpecificProp3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSpecificProp4
            // 
            this.lblSpecificProp4.BackColor = System.Drawing.Color.Transparent;
            this.lblSpecificProp4.Enabled = false;
            this.lblSpecificProp4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificProp4.Location = new System.Drawing.Point(3, 101);
            this.lblSpecificProp4.Name = "lblSpecificProp4";
            this.lblSpecificProp4.Size = new System.Drawing.Size(124, 21);
            this.lblSpecificProp4.TabIndex = 29;
            this.lblSpecificProp4.Text = "Starting Seconds:";
            this.lblSpecificProp4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSpecificProp7
            // 
            this.btnSpecificProp7.Enabled = false;
            this.btnSpecificProp7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSpecificProp7.Location = new System.Drawing.Point(7, 178);
            this.btnSpecificProp7.Name = "btnSpecificProp7";
            this.btnSpecificProp7.Size = new System.Drawing.Size(99, 20);
            this.btnSpecificProp7.TabIndex = 27;
            this.btnSpecificProp7.Text = "Bind Teleport";
            this.btnSpecificProp7.UseVisualStyleBackColor = true;
            this.btnSpecificProp7.Click += new System.EventHandler(this.btnSpecificProp7_Click);
            // 
            // numSpecificProp2
            // 
            this.numSpecificProp2.Enabled = false;
            this.numSpecificProp2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSpecificProp2.Location = new System.Drawing.Point(134, 50);
            this.numSpecificProp2.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numSpecificProp2.Name = "numSpecificProp2";
            this.numSpecificProp2.ReadOnly = true;
            this.numSpecificProp2.Size = new System.Drawing.Size(120, 20);
            this.numSpecificProp2.TabIndex = 26;
            this.numSpecificProp2.ThousandsSeparator = true;
            this.numSpecificProp2.ValueChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblSpecificProperties
            // 
            this.lblSpecificProperties.BackColor = System.Drawing.Color.Silver;
            this.lblSpecificProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSpecificProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSpecificProperties.Enabled = false;
            this.lblSpecificProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificProperties.ForeColor = System.Drawing.Color.Gray;
            this.lblSpecificProperties.Location = new System.Drawing.Point(0, 0);
            this.lblSpecificProperties.Name = "lblSpecificProperties";
            this.lblSpecificProperties.Size = new System.Drawing.Size(265, 22);
            this.lblSpecificProperties.TabIndex = 2;
            this.lblSpecificProperties.Text = "Specific Properties";
            this.lblSpecificProperties.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numSpecificProp1
            // 
            this.numSpecificProp1.Enabled = false;
            this.numSpecificProp1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSpecificProp1.Location = new System.Drawing.Point(134, 24);
            this.numSpecificProp1.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.numSpecificProp1.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numSpecificProp1.Name = "numSpecificProp1";
            this.numSpecificProp1.ReadOnly = true;
            this.numSpecificProp1.Size = new System.Drawing.Size(120, 20);
            this.numSpecificProp1.TabIndex = 25;
            this.numSpecificProp1.ThousandsSeparator = true;
            this.numSpecificProp1.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numSpecificProp1.ValueChanged += new System.EventHandler(this.cboTileType_SelectedIndexChanged);
            // 
            // lblSpecificProp1
            // 
            this.lblSpecificProp1.BackColor = System.Drawing.Color.Transparent;
            this.lblSpecificProp1.Enabled = false;
            this.lblSpecificProp1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificProp1.Location = new System.Drawing.Point(3, 22);
            this.lblSpecificProp1.Name = "lblSpecificProp1";
            this.lblSpecificProp1.Size = new System.Drawing.Size(124, 21);
            this.lblSpecificProp1.TabIndex = 23;
            this.lblSpecificProp1.Text = "Spike Period:";
            this.lblSpecificProp1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSpecificProp2
            // 
            this.lblSpecificProp2.BackColor = System.Drawing.Color.Transparent;
            this.lblSpecificProp2.Enabled = false;
            this.lblSpecificProp2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecificProp2.Location = new System.Drawing.Point(3, 49);
            this.lblSpecificProp2.Name = "lblSpecificProp2";
            this.lblSpecificProp2.Size = new System.Drawing.Size(124, 21);
            this.lblSpecificProp2.TabIndex = 24;
            this.lblSpecificProp2.Text = "Spike Begin:";
            this.lblSpecificProp2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTileProperties
            // 
            this.lblTileProperties.BackColor = System.Drawing.Color.Silver;
            this.lblTileProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTileProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTileProperties.Enabled = false;
            this.lblTileProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTileProperties.ForeColor = System.Drawing.Color.Gray;
            this.lblTileProperties.Location = new System.Drawing.Point(0, 0);
            this.lblTileProperties.Name = "lblTileProperties";
            this.lblTileProperties.Size = new System.Drawing.Size(265, 22);
            this.lblTileProperties.TabIndex = 1;
            this.lblTileProperties.Text = "Tile Properties";
            this.lblTileProperties.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LvlEditorContainer
            // 
            this.LvlEditorContainer.AutoScroll = true;
            this.LvlEditorContainer.AutoScrollMinSize = new System.Drawing.Size(500, 500);
            this.LvlEditorContainer.BackColor = System.Drawing.Color.Gray;
            this.LvlEditorContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LvlEditorContainer.Controls.Add(this.editLvlEditor);
            this.LvlEditorContainer.Location = new System.Drawing.Point(0, 24);
            this.LvlEditorContainer.Name = "LvlEditorContainer";
            this.LvlEditorContainer.Size = new System.Drawing.Size(511, 515);
            this.LvlEditorContainer.TabIndex = 4;
            // 
            // editLvlEditor
            // 
            this.editLvlEditor.BackColor = System.Drawing.Color.White;
            this.editLvlEditor.ChosenFaceDirection = LevelsStructure.KulaLevel.Orientation.Up;
            this.editLvlEditor.Cursor = System.Windows.Forms.Cursors.Cross;
            this.editLvlEditor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(90)))));
            this.editLvlEditor.IsABonusLevel = false;
            this.editLvlEditor.Location = new System.Drawing.Point(-1, -1);
            this.editLvlEditor.LvlHeight = ((byte)(0));
            this.editLvlEditor.LvlLossPenalty = ((uint)(0u));
            this.editLvlEditor.LvlNextLevel = null;
            this.editLvlEditor.LvlStartingSeconds = ((uint)(0u));
            this.editLvlEditor.LvlTheme = null;
            this.editLvlEditor.LvlWidth = ((byte)(0));
            this.editLvlEditor.Mode = EditorViewPort.EditorMode.InsertMode;
            this.editLvlEditor.Name = "editLvlEditor";
            this.editLvlEditor.SecureMode = false;
            this.editLvlEditor.Size = new System.Drawing.Size(507, 511);
            this.editLvlEditor.TabIndex = 0;
            this.editLvlEditor.Text = "editor1";
            this.editLvlEditor.TileToBeAdded = null;
            this.editLvlEditor.ChangedPointer += new EditorViewPort.ChangedPointerEventHandler(this.editLvlEditor_ChangedPointer);
            this.editLvlEditor.ChangedMode += new EditorViewPort.ChangedModeEventHandler(this.editLvlEditor_ChangedMode);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.NonEditorPanel);
            this.Controls.Add(this.LvlEditorContainer);
            this.Controls.Add(this.MessageStrip);
            this.Controls.Add(this.MainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.Text = "Level Editor (Release 1.a)";
            this.MessageStrip.ResumeLayout(false);
            this.MessageStrip.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.NonEditorPanel.ResumeLayout(false);
            this.TopSplitter.Panel1.ResumeLayout(false);
            this.TopSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TopSplitter)).EndInit();
            this.TopSplitter.ResumeLayout(false);
            this.BottomSplitter.Panel1.ResumeLayout(false);
            this.BottomSplitter.Panel1.PerformLayout();
            this.BottomSplitter.Panel2.ResumeLayout(false);
            this.BottomSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BottomSplitter)).EndInit();
            this.BottomSplitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numProperty6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProperty5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpecificProp4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpecificProp3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpecificProp2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpecificProp1)).EndInit();
            this.LvlEditorContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip MessageStrip;
        private System.Windows.Forms.ToolStripProgressBar prbMainProgress;
        private System.Windows.Forms.ToolStripStatusLabel lblMessageViewer;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiFileMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveLevel;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.Panel NonEditorPanel;
        private System.Windows.Forms.SplitContainer TopSplitter;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiInsertMode;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteMode;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiLevelProperties;
        private System.Windows.Forms.Label lblTileTyping;
        private System.Windows.Forms.Label lblTileProperties;
        private System.Windows.Forms.ComboBox cboTileType;
        private System.Windows.Forms.Label lblTileType;
        private System.Windows.Forms.Label lblSpecificType;
        private System.Windows.Forms.ComboBox cboSpecificType;
        private System.Windows.Forms.SplitContainer BottomSplitter;
        private System.Windows.Forms.Label lblSpecificProperties;
        private System.Windows.Forms.ComboBox cboProperty1;
        private System.Windows.Forms.Label lblProperty1;
        private System.Windows.Forms.ComboBox cboProperty3;
        private System.Windows.Forms.Label lblProperty3;
        private System.Windows.Forms.ComboBox cboProperty2;
        private System.Windows.Forms.Label lblProperty2;
        private System.Windows.Forms.Label lblProperty5;
        private System.Windows.Forms.ComboBox cboProperty4;
        private System.Windows.Forms.Label lblProperty4;
        private System.Windows.Forms.Label lblProperty6;
        private System.Windows.Forms.NumericUpDown numProperty6;
        private System.Windows.Forms.NumericUpDown numProperty5;
        private System.Windows.Forms.RadioButton rdoProperty4;
        private System.Windows.Forms.RadioButton rdoProperty3;
        private System.Windows.Forms.RadioButton rdoProperty2;
        private System.Windows.Forms.RadioButton rdoProperty1;
        private System.Windows.Forms.NumericUpDown numSpecificProp2;
        private System.Windows.Forms.NumericUpDown numSpecificProp1;
        private System.Windows.Forms.Label lblSpecificProp1;
        private System.Windows.Forms.Label lblSpecificProp2;
        private System.Windows.Forms.Button btnSpecificProp7;
        private System.Windows.Forms.ComboBox cboSpecificProp5;
        private System.Windows.Forms.NumericUpDown numSpecificProp4;
        private System.Windows.Forms.Label lblSpecificProp5;
        private System.Windows.Forms.NumericUpDown numSpecificProp3;
        private System.Windows.Forms.Label lblSpecificProp3;
        private System.Windows.Forms.Label lblSpecificProp4;
        private System.Windows.Forms.CheckBox chkBonusLvl;
        private System.Windows.Forms.TextBox txtSpecificProp7;
        private System.Windows.Forms.Label lblSpecificProp6;
        private System.Windows.Forms.Panel LvlEditorContainer;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenBonus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private EditorViewPort.Editor editLvlEditor;
        private System.Windows.Forms.ToolStripStatusLabel lblModeDisplay;
        private System.Windows.Forms.Button btnApplyChanges;


    }
}

