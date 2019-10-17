namespace SpotLight
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
            this.ChangeLevelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StageTypeComboBox = new System.Windows.Forms.ComboBox();
            this.InfoLabel1 = new System.Windows.Forms.Label();
            this.StageNameTextBox = new System.Windows.Forms.TextBox();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.WorldIDNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.LevelIDNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CourseLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CourseIDNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TimerNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.GreenStarsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.GreenStarLockNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.GhostIDNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.GhostBaseTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.DoubleMarioNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.StampCheckBox = new System.Windows.Forms.CheckBox();
            this.HelpToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.DividerBLabel = new System.Windows.Forms.Label();
            this.DividerALabel = new System.Windows.Forms.Label();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.ParamMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChangeLevelsToolStripMenuItem,
            this.SaveToolStripMenuItem});
            this.ParamMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.ParamMenuStrip.Name = "ParamMenuStrip";
            this.ParamMenuStrip.Size = new System.Drawing.Size(304, 24);
            this.ParamMenuStrip.TabIndex = 0;
            // 
            // ChangeLevelsToolStripMenuItem
            // 
            this.ChangeLevelsToolStripMenuItem.Name = "ChangeLevelsToolStripMenuItem";
            this.ChangeLevelsToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.ChangeLevelsToolStripMenuItem.Text = "Change Levels";
            this.ChangeLevelsToolStripMenuItem.ToolTipText = "Change the current Level";
            this.ChangeLevelsToolStripMenuItem.Click += new System.EventHandler(this.ChangeLevelsToolStripMenuItem_Click);
            // 
            // StageTypeComboBox
            // 
            this.StageTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StageTypeComboBox.Enabled = false;
            this.StageTypeComboBox.FormattingEnabled = true;
            this.StageTypeComboBox.Location = new System.Drawing.Point(87, 53);
            this.StageTypeComboBox.Name = "StageTypeComboBox";
            this.StageTypeComboBox.Size = new System.Drawing.Size(205, 21);
            this.StageTypeComboBox.TabIndex = 1;
            this.HelpToolTip.SetToolTip(this.StageTypeComboBox, "The Type of stage that this is.\r\nThe Options are Self-Explanitory");
            this.StageTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.StageTypeComboBox_SelectedIndexChanged);
            // 
            // InfoLabel1
            // 
            this.InfoLabel1.AutoSize = true;
            this.InfoLabel1.Location = new System.Drawing.Point(13, 56);
            this.InfoLabel1.Name = "InfoLabel1";
            this.InfoLabel1.Size = new System.Drawing.Size(65, 13);
            this.InfoLabel1.TabIndex = 2;
            this.InfoLabel1.Text = "Stage Type:";
            this.HelpToolTip.SetToolTip(this.InfoLabel1, "The Type of stage that this is.\r\nThe Options are Self-Explanitory");
            // 
            // StageNameTextBox
            // 
            this.StageNameTextBox.Enabled = false;
            this.StageNameTextBox.Location = new System.Drawing.Point(87, 27);
            this.StageNameTextBox.Name = "StageNameTextBox";
            this.StageNameTextBox.Size = new System.Drawing.Size(205, 20);
            this.StageNameTextBox.TabIndex = 3;
            this.StageNameTextBox.Text = "No Level Selected";
            this.HelpToolTip.SetToolTip(this.StageNameTextBox, "The name of the Stage.\r\nUsually the name of the archive without \"Map1.szs\"");
            this.StageNameTextBox.TextChanged += new System.EventHandler(this.StageNameTextBox_TextChanged);
            // 
            // InfoLabel
            // 
            this.InfoLabel.AutoSize = true;
            this.InfoLabel.Location = new System.Drawing.Point(12, 30);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(69, 13);
            this.InfoLabel.TabIndex = 4;
            this.InfoLabel.Text = "Stage Name:";
            this.HelpToolTip.SetToolTip(this.InfoLabel, "The name of the Stage.\r\nUsually the name of the archive without \"Map1.szs\"");
            // 
            // WorldIDNumericUpDown
            // 
            this.WorldIDNumericUpDown.Enabled = false;
            this.WorldIDNumericUpDown.Location = new System.Drawing.Point(73, 84);
            this.WorldIDNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WorldIDNumericUpDown.Name = "WorldIDNumericUpDown";
            this.WorldIDNumericUpDown.Size = new System.Drawing.Size(46, 20);
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
            this.LevelIDNumericUpDown.Location = new System.Drawing.Point(125, 84);
            this.LevelIDNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.LevelIDNumericUpDown.Name = "LevelIDNumericUpDown";
            this.LevelIDNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.LevelIDNumericUpDown.TabIndex = 6;
            this.HelpToolTip.SetToolTip(this.LevelIDNumericUpDown, "The number of the Stage in this world");
            this.LevelIDNumericUpDown.ValueChanged += new System.EventHandler(this.LevelIDNumericUpDown_ValueChanged);
            // 
            // CourseLabel
            // 
            this.CourseLabel.AutoSize = true;
            this.CourseLabel.Location = new System.Drawing.Point(12, 86);
            this.CourseLabel.Name = "CourseLabel";
            this.CourseLabel.Size = new System.Drawing.Size(55, 13);
            this.CourseLabel.TabIndex = 7;
            this.CourseLabel.Text = "World X-Y";
            this.HelpToolTip.SetToolTip(this.CourseLabel, "Use the two numbers on the right to change this");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Global ID:";
            // 
            // CourseIDNumericUpDown
            // 
            this.CourseIDNumericUpDown.Enabled = false;
            this.CourseIDNumericUpDown.Location = new System.Drawing.Point(238, 84);
            this.CourseIDNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.CourseIDNumericUpDown.Name = "CourseIDNumericUpDown";
            this.CourseIDNumericUpDown.Size = new System.Drawing.Size(54, 20);
            this.CourseIDNumericUpDown.TabIndex = 9;
            this.HelpToolTip.SetToolTip(this.CourseIDNumericUpDown, "The global ID used by this level");
            this.CourseIDNumericUpDown.ValueChanged += new System.EventHandler(this.CourseIDNumericUpDown_ValueChanged);
            // 
            // TimerNumericUpDown
            // 
            this.TimerNumericUpDown.Enabled = false;
            this.TimerNumericUpDown.Location = new System.Drawing.Point(52, 119);
            this.TimerNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.TimerNumericUpDown.Name = "TimerNumericUpDown";
            this.TimerNumericUpDown.Size = new System.Drawing.Size(67, 20);
            this.TimerNumericUpDown.TabIndex = 10;
            this.HelpToolTip.SetToolTip(this.TimerNumericUpDown, "The time you have to clear the level");
            this.TimerNumericUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.TimerNumericUpDown.ValueChanged += new System.EventHandler(this.TimerNumericUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Time:";
            // 
            // GreenStarsNumericUpDown
            // 
            this.GreenStarsNumericUpDown.Enabled = false;
            this.GreenStarsNumericUpDown.Location = new System.Drawing.Point(197, 119);
            this.GreenStarsNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.GreenStarsNumericUpDown.Name = "GreenStarsNumericUpDown";
            this.GreenStarsNumericUpDown.Size = new System.Drawing.Size(95, 20);
            this.GreenStarsNumericUpDown.TabIndex = 12;
            this.HelpToolTip.SetToolTip(this.GreenStarsNumericUpDown, "The number of Green Stars in this level");
            this.GreenStarsNumericUpDown.ValueChanged += new System.EventHandler(this.GreenStarsNumericUpDown_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(125, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Green Stars:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(158, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Green Star Unlock Requirement";
            // 
            // GreenStarLockNumericUpDown
            // 
            this.GreenStarLockNumericUpDown.Enabled = false;
            this.GreenStarLockNumericUpDown.Location = new System.Drawing.Point(176, 146);
            this.GreenStarLockNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.GreenStarLockNumericUpDown.Name = "GreenStarLockNumericUpDown";
            this.GreenStarLockNumericUpDown.Size = new System.Drawing.Size(116, 20);
            this.GreenStarLockNumericUpDown.TabIndex = 14;
            this.HelpToolTip.SetToolTip(this.GreenStarLockNumericUpDown, "Number of Green Stars needed to unlock this level");
            this.GreenStarLockNumericUpDown.ValueChanged += new System.EventHandler(this.GreenStarLockNumericUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(157, 211);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Mii Ghost ID:";
            // 
            // GhostIDNumericUpDown
            // 
            this.GhostIDNumericUpDown.Enabled = false;
            this.GhostIDNumericUpDown.Location = new System.Drawing.Point(231, 209);
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
            this.GhostIDNumericUpDown.Size = new System.Drawing.Size(61, 20);
            this.GhostIDNumericUpDown.TabIndex = 18;
            this.HelpToolTip.SetToolTip(this.GhostIDNumericUpDown, "The ID of the Ghost that belongs to this stage");
            this.GhostIDNumericUpDown.ValueChanged += new System.EventHandler(this.GhostIDNumericUpDown_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 211);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Mii Ghost Time";
            // 
            // GhostBaseTimeNumericUpDown
            // 
            this.GhostBaseTimeNumericUpDown.Enabled = false;
            this.GhostBaseTimeNumericUpDown.Location = new System.Drawing.Point(96, 208);
            this.GhostBaseTimeNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.GhostBaseTimeNumericUpDown.Name = "GhostBaseTimeNumericUpDown";
            this.GhostBaseTimeNumericUpDown.Size = new System.Drawing.Size(55, 20);
            this.GhostBaseTimeNumericUpDown.TabIndex = 16;
            this.HelpToolTip.SetToolTip(this.GhostBaseTimeNumericUpDown, "Unknown - Default 100");
            this.GhostBaseTimeNumericUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.GhostBaseTimeNumericUpDown.ValueChanged += new System.EventHandler(this.GhostBaseTimeNumericUpDown_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 175);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Double Cherry Max:";
            // 
            // DoubleMarioNumericUpDown
            // 
            this.DoubleMarioNumericUpDown.Enabled = false;
            this.DoubleMarioNumericUpDown.Location = new System.Drawing.Point(118, 173);
            this.DoubleMarioNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.DoubleMarioNumericUpDown.Name = "DoubleMarioNumericUpDown";
            this.DoubleMarioNumericUpDown.Size = new System.Drawing.Size(84, 20);
            this.DoubleMarioNumericUpDown.TabIndex = 20;
            this.HelpToolTip.SetToolTip(this.DoubleMarioNumericUpDown, "Amount of Extra Players you can get with a Double Cherry");
            this.DoubleMarioNumericUpDown.ValueChanged += new System.EventHandler(this.DoubleMarioNumericUpDown_ValueChanged);
            // 
            // StampCheckBox
            // 
            this.StampCheckBox.AutoSize = true;
            this.StampCheckBox.Enabled = false;
            this.StampCheckBox.Location = new System.Drawing.Point(208, 174);
            this.StampCheckBox.Name = "StampCheckBox";
            this.StampCheckBox.Size = new System.Drawing.Size(84, 17);
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
            this.DividerBLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DividerBLabel.Location = new System.Drawing.Point(12, 200);
            this.DividerBLabel.Name = "DividerBLabel";
            this.DividerBLabel.Size = new System.Drawing.Size(280, 2);
            this.DividerBLabel.TabIndex = 23;
            // 
            // DividerALabel
            // 
            this.DividerALabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DividerALabel.Location = new System.Drawing.Point(12, 111);
            this.DividerALabel.Name = "DividerALabel";
            this.DividerALabel.Size = new System.Drawing.Size(280, 2);
            this.DividerALabel.TabIndex = 24;
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Enabled = false;
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.ToolTipText = "Save Changes";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // LevelParameterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 241);
            this.Controls.Add(this.DividerALabel);
            this.Controls.Add(this.DividerBLabel);
            this.Controls.Add(this.StampCheckBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.DoubleMarioNumericUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.GhostIDNumericUpDown);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.GhostBaseTimeNumericUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.GreenStarLockNumericUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.GreenStarsNumericUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TimerNumericUpDown);
            this.Controls.Add(this.CourseIDNumericUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CourseLabel);
            this.Controls.Add(this.LevelIDNumericUpDown);
            this.Controls.Add(this.WorldIDNumericUpDown);
            this.Controls.Add(this.InfoLabel);
            this.Controls.Add(this.StageNameTextBox);
            this.Controls.Add(this.InfoLabel1);
            this.Controls.Add(this.StageTypeComboBox);
            this.Controls.Add(this.ParamMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.ParamMenuStrip;
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
        private System.Windows.Forms.ToolStripMenuItem ChangeLevelsToolStripMenuItem;
        private System.Windows.Forms.ComboBox StageTypeComboBox;
        private System.Windows.Forms.Label InfoLabel1;
        private System.Windows.Forms.TextBox StageNameTextBox;
        private System.Windows.Forms.Label InfoLabel;
        private System.Windows.Forms.NumericUpDown WorldIDNumericUpDown;
        private System.Windows.Forms.NumericUpDown LevelIDNumericUpDown;
        private System.Windows.Forms.Label CourseLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown CourseIDNumericUpDown;
        private System.Windows.Forms.NumericUpDown TimerNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown GreenStarsNumericUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown GreenStarLockNumericUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown GhostIDNumericUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown GhostBaseTimeNumericUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown DoubleMarioNumericUpDown;
        private System.Windows.Forms.CheckBox StampCheckBox;
        private System.Windows.Forms.ToolTip HelpToolTip;
        private System.Windows.Forms.Label DividerBLabel;
        private System.Windows.Forms.Label DividerALabel;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
    }
}