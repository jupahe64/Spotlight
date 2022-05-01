namespace Spotlight
{
    partial class LevelParameterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelParameterForm));
            this.ParamMenuStrip = new System.Windows.Forms.MenuStrip();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StageTypeComboBox = new System.Windows.Forms.ComboBox();
            this.StageTypeLabel = new System.Windows.Forms.Label();
            this.StageNameTextBox = new System.Windows.Forms.TextBox();
            this.StageNameLabel = new System.Windows.Forms.Label();
            this.WorldIDNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.LevelIDNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CourseLabel = new System.Windows.Forms.Label();
            this.GlobalIDLabel = new System.Windows.Forms.Label();
            this.CourseIDNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TimerNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.GreenStarsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.GreenStarLabel = new System.Windows.Forms.Label();
            this.GreenStarUnlockCountLabel = new System.Windows.Forms.Label();
            this.GreenStarLockNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.GhostIDLabel = new System.Windows.Forms.Label();
            this.GhostIDNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.GhostTimeLabel = new System.Windows.Forms.Label();
            this.GhostBaseTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.DoubleCherryLabel = new System.Windows.Forms.Label();
            this.DoubleMarioNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.StampCheckBox = new System.Windows.Forms.CheckBox();
            this.HelpToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.DividerBLabel = new System.Windows.Forms.Label();
            this.DividerALabel = new System.Windows.Forms.Label();
            this.LevelsListView = new System.Windows.Forms.ListView();
            this.CourseIDColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LevelNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WorldComboBox = new System.Windows.Forms.ComboBox();
            this.AddLevelButton = new System.Windows.Forms.Button();
            this.DeleteLevelButton = new System.Windows.Forms.Button();
            this.ParamMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WorldIDNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LevelIDNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CourseIDNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimerNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GreenStarsNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GreenStarLockNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GhostIDNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GhostBaseTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DoubleMarioNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // ParamMenuStrip
            // 
            this.ParamMenuStrip.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ParamMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ParamMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveToolStripMenuItem});
            this.ParamMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.ParamMenuStrip.Name = "ParamMenuStrip";
            this.ParamMenuStrip.Size = new System.Drawing.Size(907, 30);
            this.ParamMenuStrip.TabIndex = 0;
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(49, 26);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.ToolTipText = "Save Changes";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // StageTypeComboBox
            // 
            this.StageTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StageTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StageTypeComboBox.Enabled = false;
            this.StageTypeComboBox.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StageTypeComboBox.FormattingEnabled = true;
            this.StageTypeComboBox.Location = new System.Drawing.Point(559, 98);
            this.StageTypeComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.StageTypeComboBox.Name = "StageTypeComboBox";
            this.StageTypeComboBox.Size = new System.Drawing.Size(331, 25);
            this.StageTypeComboBox.TabIndex = 1;
            this.StageTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.StageTypeComboBox_SelectedIndexChanged);
            // 
            // StageTypeLabel
            // 
            this.StageTypeLabel.AutoSize = true;
            this.StageTypeLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StageTypeLabel.Location = new System.Drawing.Point(464, 102);
            this.StageTypeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StageTypeLabel.Name = "StageTypeLabel";
            this.StageTypeLabel.Size = new System.Drawing.Size(75, 17);
            this.StageTypeLabel.TabIndex = 2;
            this.StageTypeLabel.Text = "Stage Type:";
            // 
            // StageNameTextBox
            // 
            this.StageNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StageNameTextBox.Enabled = false;
            this.StageNameTextBox.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StageNameTextBox.Location = new System.Drawing.Point(559, 66);
            this.StageNameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.StageNameTextBox.Name = "StageNameTextBox";
            this.StageNameTextBox.Size = new System.Drawing.Size(331, 25);
            this.StageNameTextBox.TabIndex = 3;
            this.StageNameTextBox.Text = "No Level Selected";
            this.StageNameTextBox.TextChanged += new System.EventHandler(this.StageNameTextBox_TextChanged);
            // 
            // StageNameLabel
            // 
            this.StageNameLabel.AutoSize = true;
            this.StageNameLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StageNameLabel.Location = new System.Drawing.Point(459, 66);
            this.StageNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StageNameLabel.Name = "StageNameLabel";
            this.StageNameLabel.Size = new System.Drawing.Size(83, 17);
            this.StageNameLabel.TabIndex = 4;
            this.StageNameLabel.Text = "Stage Name:";
            // 
            // WorldIDNumericUpDown
            // 
            this.WorldIDNumericUpDown.Enabled = false;
            this.WorldIDNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WorldIDNumericUpDown.Location = new System.Drawing.Point(465, 153);
            this.WorldIDNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.WorldIDNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WorldIDNumericUpDown.Name = "WorldIDNumericUpDown";
            this.WorldIDNumericUpDown.Size = new System.Drawing.Size(61, 25);
            this.WorldIDNumericUpDown.TabIndex = 5;
            this.HelpToolTip.SetToolTip(this.WorldIDNumericUpDown, "The world that this stage belongs to");
            this.WorldIDNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.WorldIDNumericUpDown.ValueChanged += new System.EventHandler(this.WorldIDNumericUpDown_ValueChanged);
            // 
            // LevelIDNumericUpDown
            // 
            this.LevelIDNumericUpDown.Enabled = false;
            this.LevelIDNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LevelIDNumericUpDown.Location = new System.Drawing.Point(535, 153);
            this.LevelIDNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LevelIDNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.LevelIDNumericUpDown.Name = "LevelIDNumericUpDown";
            this.LevelIDNumericUpDown.Size = new System.Drawing.Size(63, 25);
            this.LevelIDNumericUpDown.TabIndex = 6;
            this.HelpToolTip.SetToolTip(this.LevelIDNumericUpDown, "The number of the Stage in this world");
            this.LevelIDNumericUpDown.ValueChanged += new System.EventHandler(this.LevelIDNumericUpDown_ValueChanged);
            // 
            // CourseLabel
            // 
            this.CourseLabel.AutoSize = true;
            this.CourseLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CourseLabel.Location = new System.Drawing.Point(460, 133);
            this.CourseLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CourseLabel.Name = "CourseLabel";
            this.CourseLabel.Size = new System.Drawing.Size(67, 17);
            this.CourseLabel.TabIndex = 7;
            this.CourseLabel.Text = "World X-Y";
            this.HelpToolTip.SetToolTip(this.CourseLabel, "Use the two numbers on the right to change this");
            // 
            // GlobalIDLabel
            // 
            this.GlobalIDLabel.AutoSize = true;
            this.GlobalIDLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GlobalIDLabel.Location = new System.Drawing.Point(811, 133);
            this.GlobalIDLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GlobalIDLabel.Name = "GlobalIDLabel";
            this.GlobalIDLabel.Size = new System.Drawing.Size(65, 17);
            this.GlobalIDLabel.TabIndex = 8;
            this.GlobalIDLabel.Text = "Global ID:";
            // 
            // CourseIDNumericUpDown
            // 
            this.CourseIDNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CourseIDNumericUpDown.Enabled = false;
            this.CourseIDNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CourseIDNumericUpDown.Location = new System.Drawing.Point(764, 169);
            this.CourseIDNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CourseIDNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.CourseIDNumericUpDown.Name = "CourseIDNumericUpDown";
            this.CourseIDNumericUpDown.Size = new System.Drawing.Size(127, 25);
            this.CourseIDNumericUpDown.TabIndex = 9;
            this.HelpToolTip.SetToolTip(this.CourseIDNumericUpDown, "The global ID used by this level");
            this.CourseIDNumericUpDown.ValueChanged += new System.EventHandler(this.CourseIDNumericUpDown_ValueChanged);
            // 
            // TimerNumericUpDown
            // 
            this.TimerNumericUpDown.Enabled = false;
            this.TimerNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimerNumericUpDown.Location = new System.Drawing.Point(764, 279);
            this.TimerNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TimerNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.TimerNumericUpDown.Name = "TimerNumericUpDown";
            this.TimerNumericUpDown.Size = new System.Drawing.Size(127, 25);
            this.TimerNumericUpDown.TabIndex = 10;
            this.HelpToolTip.SetToolTip(this.TimerNumericUpDown, "The time you have to clear the level");
            this.TimerNumericUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.TimerNumericUpDown.ValueChanged += new System.EventHandler(this.TimerNumericUpDown_ValueChanged);
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeLabel.Location = new System.Drawing.Point(712, 282);
            this.TimeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(39, 17);
            this.TimeLabel.TabIndex = 11;
            this.TimeLabel.Text = "Time:";
            // 
            // GreenStarsNumericUpDown
            // 
            this.GreenStarsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GreenStarsNumericUpDown.Enabled = false;
            this.GreenStarsNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GreenStarsNumericUpDown.Location = new System.Drawing.Point(764, 343);
            this.GreenStarsNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GreenStarsNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.GreenStarsNumericUpDown.Name = "GreenStarsNumericUpDown";
            this.GreenStarsNumericUpDown.Size = new System.Drawing.Size(127, 25);
            this.GreenStarsNumericUpDown.TabIndex = 12;
            this.HelpToolTip.SetToolTip(this.GreenStarsNumericUpDown, "The number of Green Stars in this level");
            this.GreenStarsNumericUpDown.ValueChanged += new System.EventHandler(this.GreenStarsNumericUpDown_ValueChanged);
            // 
            // GreenStarLabel
            // 
            this.GreenStarLabel.AutoSize = true;
            this.GreenStarLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GreenStarLabel.Location = new System.Drawing.Point(664, 346);
            this.GreenStarLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GreenStarLabel.Name = "GreenStarLabel";
            this.GreenStarLabel.Size = new System.Drawing.Size(79, 17);
            this.GreenStarLabel.TabIndex = 13;
            this.GreenStarLabel.Text = "Green Stars:";
            // 
            // GreenStarUnlockCountLabel
            // 
            this.GreenStarUnlockCountLabel.AutoSize = true;
            this.GreenStarUnlockCountLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GreenStarUnlockCountLabel.Location = new System.Drawing.Point(541, 314);
            this.GreenStarUnlockCountLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GreenStarUnlockCountLabel.Name = "GreenStarUnlockCountLabel";
            this.GreenStarUnlockCountLabel.Size = new System.Drawing.Size(191, 17);
            this.GreenStarUnlockCountLabel.TabIndex = 15;
            this.GreenStarUnlockCountLabel.Text = "Green Star Unlock Requirement";
            // 
            // GreenStarLockNumericUpDown
            // 
            this.GreenStarLockNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GreenStarLockNumericUpDown.Enabled = false;
            this.GreenStarLockNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GreenStarLockNumericUpDown.Location = new System.Drawing.Point(764, 311);
            this.GreenStarLockNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GreenStarLockNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.GreenStarLockNumericUpDown.Name = "GreenStarLockNumericUpDown";
            this.GreenStarLockNumericUpDown.Size = new System.Drawing.Size(127, 25);
            this.GreenStarLockNumericUpDown.TabIndex = 14;
            this.HelpToolTip.SetToolTip(this.GreenStarLockNumericUpDown, "Number of Green Stars needed to unlock this level");
            this.GreenStarLockNumericUpDown.ValueChanged += new System.EventHandler(this.GreenStarLockNumericUpDown_ValueChanged);
            // 
            // GhostIDLabel
            // 
            this.GhostIDLabel.AutoSize = true;
            this.GhostIDLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GhostIDLabel.Location = new System.Drawing.Point(665, 417);
            this.GhostIDLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GhostIDLabel.Name = "GhostIDLabel";
            this.GhostIDLabel.Size = new System.Drawing.Size(83, 17);
            this.GhostIDLabel.TabIndex = 19;
            this.GhostIDLabel.Text = "Mii Ghost ID:";
            // 
            // GhostIDNumericUpDown
            // 
            this.GhostIDNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GhostIDNumericUpDown.Enabled = false;
            this.GhostIDNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GhostIDNumericUpDown.Location = new System.Drawing.Point(764, 415);
            this.GhostIDNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GhostIDNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.GhostIDNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147418112});
            this.GhostIDNumericUpDown.Name = "GhostIDNumericUpDown";
            this.GhostIDNumericUpDown.Size = new System.Drawing.Size(127, 25);
            this.GhostIDNumericUpDown.TabIndex = 18;
            this.HelpToolTip.SetToolTip(this.GhostIDNumericUpDown, "The ID of the Ghost that belongs to this stage");
            this.GhostIDNumericUpDown.ValueChanged += new System.EventHandler(this.GhostIDNumericUpDown_ValueChanged);
            // 
            // GhostTimeLabel
            // 
            this.GhostTimeLabel.AutoSize = true;
            this.GhostTimeLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GhostTimeLabel.Location = new System.Drawing.Point(649, 455);
            this.GhostTimeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GhostTimeLabel.Name = "GhostTimeLabel";
            this.GhostTimeLabel.Size = new System.Drawing.Size(96, 17);
            this.GhostTimeLabel.TabIndex = 17;
            this.GhostTimeLabel.Text = "Mii Ghost Time";
            // 
            // GhostBaseTimeNumericUpDown
            // 
            this.GhostBaseTimeNumericUpDown.Enabled = false;
            this.GhostBaseTimeNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GhostBaseTimeNumericUpDown.Location = new System.Drawing.Point(764, 447);
            this.GhostBaseTimeNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GhostBaseTimeNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.GhostBaseTimeNumericUpDown.Name = "GhostBaseTimeNumericUpDown";
            this.GhostBaseTimeNumericUpDown.Size = new System.Drawing.Size(127, 25);
            this.GhostBaseTimeNumericUpDown.TabIndex = 16;
            this.HelpToolTip.SetToolTip(this.GhostBaseTimeNumericUpDown, "Unknown - Default 100");
            this.GhostBaseTimeNumericUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.GhostBaseTimeNumericUpDown.ValueChanged += new System.EventHandler(this.GhostBaseTimeNumericUpDown_ValueChanged);
            // 
            // DoubleCherryLabel
            // 
            this.DoubleCherryLabel.AutoSize = true;
            this.DoubleCherryLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoubleCherryLabel.Location = new System.Drawing.Point(619, 375);
            this.DoubleCherryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DoubleCherryLabel.Name = "DoubleCherryLabel";
            this.DoubleCherryLabel.Size = new System.Drawing.Size(124, 17);
            this.DoubleCherryLabel.TabIndex = 21;
            this.DoubleCherryLabel.Text = "Double Cherry Max:";
            // 
            // DoubleMarioNumericUpDown
            // 
            this.DoubleMarioNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DoubleMarioNumericUpDown.Enabled = false;
            this.DoubleMarioNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoubleMarioNumericUpDown.Location = new System.Drawing.Point(764, 375);
            this.DoubleMarioNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DoubleMarioNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.DoubleMarioNumericUpDown.Name = "DoubleMarioNumericUpDown";
            this.DoubleMarioNumericUpDown.Size = new System.Drawing.Size(127, 25);
            this.DoubleMarioNumericUpDown.TabIndex = 20;
            this.HelpToolTip.SetToolTip(this.DoubleMarioNumericUpDown, "Amount of Extra Players you can get with a Double Cherry");
            this.DoubleMarioNumericUpDown.ValueChanged += new System.EventHandler(this.DoubleMarioNumericUpDown_ValueChanged);
            // 
            // StampCheckBox
            // 
            this.StampCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StampCheckBox.AutoSize = true;
            this.StampCheckBox.Enabled = false;
            this.StampCheckBox.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StampCheckBox.Location = new System.Drawing.Point(792, 240);
            this.StampCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.StampCheckBox.Name = "StampCheckBox";
            this.StampCheckBox.Size = new System.Drawing.Size(99, 21);
            this.StampCheckBox.TabIndex = 22;
            this.StampCheckBox.Text = "Has Stamp?";
            this.HelpToolTip.SetToolTip(this.StampCheckBox, "Does the level have a Stamp in it?");
            this.StampCheckBox.UseVisualStyleBackColor = true;
            this.StampCheckBox.CheckedChanged += new System.EventHandler(this.StampCheckBox_CheckedChanged);
            // 
            // HelpToolTip
            // 
            this.HelpToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.HelpToolTip.ToolTipTitle = "Help";
            // 
            // DividerBLabel
            // 
            this.DividerBLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DividerBLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DividerBLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DividerBLabel.Location = new System.Drawing.Point(463, 404);
            this.DividerBLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DividerBLabel.Name = "DividerBLabel";
            this.DividerBLabel.Size = new System.Drawing.Size(428, 2);
            this.DividerBLabel.TabIndex = 23;
            // 
            // DividerALabel
            // 
            this.DividerALabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DividerALabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DividerALabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DividerALabel.Location = new System.Drawing.Point(464, 224);
            this.DividerALabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DividerALabel.Name = "DividerALabel";
            this.DividerALabel.Size = new System.Drawing.Size(440, 2);
            this.DividerALabel.TabIndex = 24;
            // 
            // LevelsListView
            // 
            this.LevelsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CourseIDColumnHeader,
            this.LevelNameColumnHeader});
            this.LevelsListView.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LevelsListView.FullRowSelect = true;
            this.LevelsListView.GridLines = true;
            this.LevelsListView.HideSelection = false;
            this.LevelsListView.Location = new System.Drawing.Point(16, 66);
            this.LevelsListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LevelsListView.MultiSelect = false;
            this.LevelsListView.Name = "LevelsListView";
            this.LevelsListView.Size = new System.Drawing.Size(433, 393);
            this.LevelsListView.TabIndex = 25;
            this.LevelsListView.UseCompatibleStateImageBehavior = false;
            this.LevelsListView.View = System.Windows.Forms.View.Details;
            this.LevelsListView.SelectedIndexChanged += new System.EventHandler(this.LevelsListView_SelectedIndexChanged);
            // 
            // CourseIDColumnHeader
            // 
            this.CourseIDColumnHeader.Text = "CourseID";
            this.CourseIDColumnHeader.Width = 65;
            // 
            // LevelNameColumnHeader
            // 
            this.LevelNameColumnHeader.Text = "Level Name";
            this.LevelNameColumnHeader.Width = 220;
            // 
            // WorldComboBox
            // 
            this.WorldComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WorldComboBox.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WorldComboBox.FormattingEnabled = true;
            this.WorldComboBox.Items.AddRange(new object[] {
            "World 1",
            "World 2",
            "World 3",
            "World 4",
            "World 5",
            "World 6",
            "World Castle",
            "World Bowser",
            "World Star",
            "World Mushroom",
            "World Flower",
            "World Crown"});
            this.WorldComboBox.Location = new System.Drawing.Point(16, 33);
            this.WorldComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.WorldComboBox.Name = "WorldComboBox";
            this.WorldComboBox.Size = new System.Drawing.Size(433, 25);
            this.WorldComboBox.TabIndex = 26;
            this.WorldComboBox.SelectedIndexChanged += new System.EventHandler(this.WorldComboBox_SelectedIndexChanged);
            // 
            // AddLevelButton
            // 
            this.AddLevelButton.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddLevelButton.Location = new System.Drawing.Point(17, 469);
            this.AddLevelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AddLevelButton.Name = "AddLevelButton";
            this.AddLevelButton.Size = new System.Drawing.Size(205, 28);
            this.AddLevelButton.TabIndex = 27;
            this.AddLevelButton.Text = "Add Level";
            this.AddLevelButton.UseVisualStyleBackColor = true;
            this.AddLevelButton.Click += new System.EventHandler(this.AddLevelButton_Click);
            // 
            // DeleteLevelButton
            // 
            this.DeleteLevelButton.Enabled = false;
            this.DeleteLevelButton.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteLevelButton.Location = new System.Drawing.Point(231, 469);
            this.DeleteLevelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DeleteLevelButton.Name = "DeleteLevelButton";
            this.DeleteLevelButton.Size = new System.Drawing.Size(220, 28);
            this.DeleteLevelButton.TabIndex = 28;
            this.DeleteLevelButton.Text = "Delete Level";
            this.DeleteLevelButton.UseVisualStyleBackColor = true;
            this.DeleteLevelButton.Click += new System.EventHandler(this.DeleteLevelButton_Click);
            // 
            // LevelParameterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 512);
            this.Controls.Add(this.DeleteLevelButton);
            this.Controls.Add(this.AddLevelButton);
            this.Controls.Add(this.WorldComboBox);
            this.Controls.Add(this.LevelsListView);
            this.Controls.Add(this.DividerALabel);
            this.Controls.Add(this.DividerBLabel);
            this.Controls.Add(this.StampCheckBox);
            this.Controls.Add(this.DoubleCherryLabel);
            this.Controls.Add(this.DoubleMarioNumericUpDown);
            this.Controls.Add(this.GhostIDLabel);
            this.Controls.Add(this.GhostIDNumericUpDown);
            this.Controls.Add(this.GhostTimeLabel);
            this.Controls.Add(this.GhostBaseTimeNumericUpDown);
            this.Controls.Add(this.GreenStarUnlockCountLabel);
            this.Controls.Add(this.GreenStarLockNumericUpDown);
            this.Controls.Add(this.GreenStarLabel);
            this.Controls.Add(this.GreenStarsNumericUpDown);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.TimerNumericUpDown);
            this.Controls.Add(this.CourseIDNumericUpDown);
            this.Controls.Add(this.GlobalIDLabel);
            this.Controls.Add(this.CourseLabel);
            this.Controls.Add(this.LevelIDNumericUpDown);
            this.Controls.Add(this.WorldIDNumericUpDown);
            this.Controls.Add(this.StageNameLabel);
            this.Controls.Add(this.StageNameTextBox);
            this.Controls.Add(this.StageTypeLabel);
            this.Controls.Add(this.StageTypeComboBox);
            this.Controls.Add(this.ParamMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.ParamMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LevelParameterForm";
            this.Text = "Spotlight - Level Parameters";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LevelParameterForm_FormClosing);
            this.ParamMenuStrip.ResumeLayout(false);
            this.ParamMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WorldIDNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LevelIDNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CourseIDNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimerNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GreenStarsNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GreenStarLockNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GhostIDNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GhostBaseTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DoubleMarioNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip ParamMenuStrip;
        private System.Windows.Forms.ComboBox StageTypeComboBox;
        private System.Windows.Forms.Label StageTypeLabel;
        private System.Windows.Forms.TextBox StageNameTextBox;
        private System.Windows.Forms.Label StageNameLabel;
        private System.Windows.Forms.NumericUpDown WorldIDNumericUpDown;
        private System.Windows.Forms.NumericUpDown LevelIDNumericUpDown;
        private System.Windows.Forms.Label CourseLabel;
        private System.Windows.Forms.Label GlobalIDLabel;
        private System.Windows.Forms.NumericUpDown CourseIDNumericUpDown;
        private System.Windows.Forms.NumericUpDown TimerNumericUpDown;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.NumericUpDown GreenStarsNumericUpDown;
        private System.Windows.Forms.Label GreenStarLabel;
        private System.Windows.Forms.Label GreenStarUnlockCountLabel;
        private System.Windows.Forms.NumericUpDown GreenStarLockNumericUpDown;
        private System.Windows.Forms.Label GhostIDLabel;
        private System.Windows.Forms.NumericUpDown GhostIDNumericUpDown;
        private System.Windows.Forms.Label GhostTimeLabel;
        private System.Windows.Forms.NumericUpDown GhostBaseTimeNumericUpDown;
        private System.Windows.Forms.Label DoubleCherryLabel;
        private System.Windows.Forms.NumericUpDown DoubleMarioNumericUpDown;
        private System.Windows.Forms.CheckBox StampCheckBox;
        private System.Windows.Forms.ToolTip HelpToolTip;
        private System.Windows.Forms.Label DividerBLabel;
        private System.Windows.Forms.Label DividerALabel;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ListView LevelsListView;
        private System.Windows.Forms.ColumnHeader CourseIDColumnHeader;
        private System.Windows.Forms.ColumnHeader LevelNameColumnHeader;
        private System.Windows.Forms.ComboBox WorldComboBox;
        private System.Windows.Forms.Button AddLevelButton;
        private System.Windows.Forms.Button DeleteLevelButton;
    }
}