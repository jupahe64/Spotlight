namespace Spotlight.GUI
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.GamePathLabel = new System.Windows.Forms.Label();
            this.GamePathButton = new System.Windows.Forms.Button();
            this.RebuildDatabaseButton = new System.Windows.Forms.Button();
            this.ObjectParameterGroupBox = new System.Windows.Forms.GroupBox();
            this.DescriptionInfoLabel = new System.Windows.Forms.Label();
            this.ClearDescriptionsButton = new System.Windows.Forms.Button();
            this.DatabaseInfoLabel = new System.Windows.Forms.Label();
            this.RenderingGroupBox = new System.Windows.Forms.GroupBox();
            this.RenderTransparentWallsCheckBox = new System.Windows.Forms.CheckBox();
            this.RenderSkyboxesCheckBox = new System.Windows.Forms.CheckBox();
            this.PlayerLabel = new System.Windows.Forms.Label();
            this.PlayerComboBox = new System.Windows.Forms.ComboBox();
            this.RenderAreaCheckBox = new System.Windows.Forms.CheckBox();
            this.LoadingAndSavingGroupBox = new System.Windows.Forms.GroupBox();
            this.DoNotLoadTexturesCheckBox = new System.Windows.Forms.CheckBox();
            this.UniqueIDsCheckBox = new System.Windows.Forms.CheckBox();
            this.ResetSpotlightButton = new System.Windows.Forms.Button();
            this.IDEditingCheckBox = new System.Windows.Forms.CheckBox();
            this.EditingGroupBox = new System.Windows.Forms.GroupBox();
            this.ProjectPathButton = new System.Windows.Forms.Button();
            this.ProjectPathLabel = new System.Windows.Forms.Label();
            this.MiscellaneousGroupBox = new System.Windows.Forms.GroupBox();
            this.SplashTestButton = new System.Windows.Forms.Button();
            this.SplashSizeComboBox = new System.Windows.Forms.ComboBox();
            this.SplashSizeLabel = new System.Windows.Forms.Label();
            this.LanguageComboBox = new System.Windows.Forms.ComboBox();
            this.LanguageLabel = new System.Windows.Forms.Label();
            this.ProjectPathTextBox = new GL_EditorFramework.SuggestingTextBox();
            this.GamePathTextBox = new GL_EditorFramework.SuggestingTextBox();
            this.ObjectParameterGroupBox.SuspendLayout();
            this.RenderingGroupBox.SuspendLayout();
            this.LoadingAndSavingGroupBox.SuspendLayout();
            this.EditingGroupBox.SuspendLayout();
            this.MiscellaneousGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // GamePathLabel
            // 
            this.GamePathLabel.AutoSize = true;
            this.GamePathLabel.Location = new System.Drawing.Point(12, 15);
            this.GamePathLabel.Name = "GamePathLabel";
            this.GamePathLabel.Size = new System.Drawing.Size(83, 13);
            this.GamePathLabel.TabIndex = 0;
            this.GamePathLabel.Text = "Game Directory:";
            // 
            // GamePathButton
            // 
            this.GamePathButton.Location = new System.Drawing.Point(575, 12);
            this.GamePathButton.Name = "GamePathButton";
            this.GamePathButton.Size = new System.Drawing.Size(35, 20);
            this.GamePathButton.TabIndex = 2;
            this.GamePathButton.Text = "· · ·";
            this.GamePathButton.UseVisualStyleBackColor = true;
            this.GamePathButton.Click += new System.EventHandler(this.GamePathButton_Click);
            // 
            // RebuildDatabaseButton
            // 
            this.RebuildDatabaseButton.Location = new System.Drawing.Point(6, 19);
            this.RebuildDatabaseButton.Name = "RebuildDatabaseButton";
            this.RebuildDatabaseButton.Size = new System.Drawing.Size(108, 23);
            this.RebuildDatabaseButton.TabIndex = 3;
            this.RebuildDatabaseButton.Text = "Rebuild";
            this.RebuildDatabaseButton.UseVisualStyleBackColor = true;
            this.RebuildDatabaseButton.Click += new System.EventHandler(this.RebuildDatabaseButton_Click);
            // 
            // ObjectParameterGroupBox
            // 
            this.ObjectParameterGroupBox.Controls.Add(this.DescriptionInfoLabel);
            this.ObjectParameterGroupBox.Controls.Add(this.ClearDescriptionsButton);
            this.ObjectParameterGroupBox.Controls.Add(this.DatabaseInfoLabel);
            this.ObjectParameterGroupBox.Controls.Add(this.RebuildDatabaseButton);
            this.ObjectParameterGroupBox.Location = new System.Drawing.Point(12, 64);
            this.ObjectParameterGroupBox.Name = "ObjectParameterGroupBox";
            this.ObjectParameterGroupBox.Size = new System.Drawing.Size(598, 78);
            this.ObjectParameterGroupBox.TabIndex = 4;
            this.ObjectParameterGroupBox.TabStop = false;
            this.ObjectParameterGroupBox.Text = "Databases";
            // 
            // DescriptionInfoLabel
            // 
            this.DescriptionInfoLabel.AutoSize = true;
            this.DescriptionInfoLabel.Location = new System.Drawing.Point(120, 53);
            this.DescriptionInfoLabel.Name = "DescriptionInfoLabel";
            this.DescriptionInfoLabel.Size = new System.Drawing.Size(341, 13);
            this.DescriptionInfoLabel.TabIndex = 6;
            this.DescriptionInfoLabel.Text = "Descriptions Last Edited on: [DATABASEGENDATE].    Version: [VER]";
            // 
            // ClearDescriptionsButton
            // 
            this.ClearDescriptionsButton.Location = new System.Drawing.Point(6, 48);
            this.ClearDescriptionsButton.Name = "ClearDescriptionsButton";
            this.ClearDescriptionsButton.Size = new System.Drawing.Size(108, 23);
            this.ClearDescriptionsButton.TabIndex = 5;
            this.ClearDescriptionsButton.Text = "Clear";
            this.ClearDescriptionsButton.UseVisualStyleBackColor = true;
            this.ClearDescriptionsButton.Click += new System.EventHandler(this.ClearDescriptionsButton_Click);
            // 
            // DatabaseInfoLabel
            // 
            this.DatabaseInfoLabel.AutoSize = true;
            this.DatabaseInfoLabel.Location = new System.Drawing.Point(120, 24);
            this.DatabaseInfoLabel.Name = "DatabaseInfoLabel";
            this.DatabaseInfoLabel.Size = new System.Drawing.Size(319, 13);
            this.DatabaseInfoLabel.TabIndex = 4;
            this.DatabaseInfoLabel.Text = "Database Last Built on: [DATABASEGENDATE].    Version: [VER]";
            // 
            // RenderingGroupBox
            // 
            this.RenderingGroupBox.Controls.Add(this.RenderTransparentWallsCheckBox);
            this.RenderingGroupBox.Controls.Add(this.RenderSkyboxesCheckBox);
            this.RenderingGroupBox.Controls.Add(this.PlayerLabel);
            this.RenderingGroupBox.Controls.Add(this.PlayerComboBox);
            this.RenderingGroupBox.Controls.Add(this.RenderAreaCheckBox);
            this.RenderingGroupBox.Location = new System.Drawing.Point(12, 148);
            this.RenderingGroupBox.Name = "RenderingGroupBox";
            this.RenderingGroupBox.Size = new System.Drawing.Size(598, 75);
            this.RenderingGroupBox.TabIndex = 5;
            this.RenderingGroupBox.TabStop = false;
            this.RenderingGroupBox.Text = "Rendering";
            // 
            // RenderTransparentWallsCheckBox
            // 
            this.RenderTransparentWallsCheckBox.AutoSize = true;
            this.RenderTransparentWallsCheckBox.Checked = true;
            this.RenderTransparentWallsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RenderTransparentWallsCheckBox.Location = new System.Drawing.Point(298, 19);
            this.RenderTransparentWallsCheckBox.Name = "RenderTransparentWallsCheckBox";
            this.RenderTransparentWallsCheckBox.Size = new System.Drawing.Size(147, 17);
            this.RenderTransparentWallsCheckBox.TabIndex = 4;
            this.RenderTransparentWallsCheckBox.Text = "Render TransparentWalls";
            this.RenderTransparentWallsCheckBox.UseVisualStyleBackColor = true;
            this.RenderTransparentWallsCheckBox.CheckedChanged += new System.EventHandler(this.RenderTransparentWallsCheckBox_CheckedChanged);
            // 
            // RenderSkyboxesCheckBox
            // 
            this.RenderSkyboxesCheckBox.AutoSize = true;
            this.RenderSkyboxesCheckBox.Checked = true;
            this.RenderSkyboxesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RenderSkyboxesCheckBox.Location = new System.Drawing.Point(149, 19);
            this.RenderSkyboxesCheckBox.Name = "RenderSkyboxesCheckBox";
            this.RenderSkyboxesCheckBox.Size = new System.Drawing.Size(110, 17);
            this.RenderSkyboxesCheckBox.TabIndex = 3;
            this.RenderSkyboxesCheckBox.Text = "Render Skyboxes";
            this.RenderSkyboxesCheckBox.UseVisualStyleBackColor = true;
            this.RenderSkyboxesCheckBox.CheckedChanged += new System.EventHandler(this.RenderSkyboxesCheckBox_CheckedChanged);
            // 
            // PlayerLabel
            // 
            this.PlayerLabel.AutoSize = true;
            this.PlayerLabel.Location = new System.Drawing.Point(6, 45);
            this.PlayerLabel.Name = "PlayerLabel";
            this.PlayerLabel.Size = new System.Drawing.Size(39, 13);
            this.PlayerLabel.TabIndex = 2;
            this.PlayerLabel.Text = "Player:";
            // 
            // PlayerComboBox
            // 
            this.PlayerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PlayerComboBox.FormattingEnabled = true;
            this.PlayerComboBox.Items.AddRange(new object[] {
            "None",
            "Mario",
            "Luigi",
            "Peach",
            "Toad",
            "Rosalina"});
            this.PlayerComboBox.Location = new System.Drawing.Point(86, 42);
            this.PlayerComboBox.Name = "PlayerComboBox";
            this.PlayerComboBox.Size = new System.Drawing.Size(106, 21);
            this.PlayerComboBox.TabIndex = 1;
            this.PlayerComboBox.SelectedIndexChanged += new System.EventHandler(this.PlayerComboBox_SelectedIndexChanged);
            // 
            // RenderAreaCheckBox
            // 
            this.RenderAreaCheckBox.AutoSize = true;
            this.RenderAreaCheckBox.Checked = true;
            this.RenderAreaCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RenderAreaCheckBox.Location = new System.Drawing.Point(6, 19);
            this.RenderAreaCheckBox.Name = "RenderAreaCheckBox";
            this.RenderAreaCheckBox.Size = new System.Drawing.Size(91, 17);
            this.RenderAreaCheckBox.TabIndex = 0;
            this.RenderAreaCheckBox.Text = "Render Areas";
            this.RenderAreaCheckBox.UseVisualStyleBackColor = true;
            this.RenderAreaCheckBox.CheckedChanged += new System.EventHandler(this.RenderAreaCheckBox_CheckedChanged);
            // 
            // LoadingAndSavingGroupBox
            // 
            this.LoadingAndSavingGroupBox.Controls.Add(this.DoNotLoadTexturesCheckBox);
            this.LoadingAndSavingGroupBox.Controls.Add(this.UniqueIDsCheckBox);
            this.LoadingAndSavingGroupBox.Location = new System.Drawing.Point(12, 229);
            this.LoadingAndSavingGroupBox.Name = "LoadingAndSavingGroupBox";
            this.LoadingAndSavingGroupBox.Size = new System.Drawing.Size(598, 63);
            this.LoadingAndSavingGroupBox.TabIndex = 6;
            this.LoadingAndSavingGroupBox.TabStop = false;
            this.LoadingAndSavingGroupBox.Text = "Loading and Saving";
            // 
            // DoNotLoadTexturesCheckBox
            // 
            this.DoNotLoadTexturesCheckBox.AutoSize = true;
            this.DoNotLoadTexturesCheckBox.Location = new System.Drawing.Point(9, 40);
            this.DoNotLoadTexturesCheckBox.Name = "DoNotLoadTexturesCheckBox";
            this.DoNotLoadTexturesCheckBox.Size = new System.Drawing.Size(296, 17);
            this.DoNotLoadTexturesCheckBox.TabIndex = 1;
            this.DoNotLoadTexturesCheckBox.Text = "Don\'t load textures (speeds up loading on so many levels)";
            this.DoNotLoadTexturesCheckBox.UseVisualStyleBackColor = true;
            this.DoNotLoadTexturesCheckBox.CheckedChanged += new System.EventHandler(this.DoNotLoadTexturesCheckBox_CheckedChanged);
            // 
            // UniqueIDsCheckBox
            // 
            this.UniqueIDsCheckBox.AutoSize = true;
            this.UniqueIDsCheckBox.Location = new System.Drawing.Point(9, 19);
            this.UniqueIDsCheckBox.Name = "UniqueIDsCheckBox";
            this.UniqueIDsCheckBox.Size = new System.Drawing.Size(428, 17);
            this.UniqueIDsCheckBox.TabIndex = 0;
            this.UniqueIDsCheckBox.Text = "Only load unique ObjectIDs (disable if objects disappear when loading a custom le" +
    "vel)";
            this.UniqueIDsCheckBox.UseVisualStyleBackColor = true;
            this.UniqueIDsCheckBox.CheckedChanged += new System.EventHandler(this.UniqueIDsCheckBox_CheckedChanged);
            // 
            // ResetSpotlightButton
            // 
            this.ResetSpotlightButton.BackColor = System.Drawing.Color.Maroon;
            this.ResetSpotlightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResetSpotlightButton.ForeColor = System.Drawing.Color.OrangeRed;
            this.ResetSpotlightButton.Location = new System.Drawing.Point(522, 25);
            this.ResetSpotlightButton.Name = "ResetSpotlightButton";
            this.ResetSpotlightButton.Size = new System.Drawing.Size(74, 23);
            this.ResetSpotlightButton.TabIndex = 7;
            this.ResetSpotlightButton.Text = "Reset";
            this.ResetSpotlightButton.UseVisualStyleBackColor = false;
            this.ResetSpotlightButton.Click += new System.EventHandler(this.ResetSpotlightButton_Click);
            // 
            // IDEditingCheckBox
            // 
            this.IDEditingCheckBox.AutoSize = true;
            this.IDEditingCheckBox.Location = new System.Drawing.Point(9, 19);
            this.IDEditingCheckBox.Name = "IDEditingCheckBox";
            this.IDEditingCheckBox.Size = new System.Drawing.Size(108, 17);
            this.IDEditingCheckBox.TabIndex = 8;
            this.IDEditingCheckBox.Text = "Enable ID Editing";
            this.IDEditingCheckBox.UseVisualStyleBackColor = true;
            this.IDEditingCheckBox.CheckedChanged += new System.EventHandler(this.IDEditingCheckBox_CheckedChanged);
            // 
            // EditingGroupBox
            // 
            this.EditingGroupBox.Controls.Add(this.IDEditingCheckBox);
            this.EditingGroupBox.Location = new System.Drawing.Point(12, 298);
            this.EditingGroupBox.Name = "EditingGroupBox";
            this.EditingGroupBox.Size = new System.Drawing.Size(598, 52);
            this.EditingGroupBox.TabIndex = 9;
            this.EditingGroupBox.TabStop = false;
            this.EditingGroupBox.Text = "Editing";
            // 
            // ProjectPathButton
            // 
            this.ProjectPathButton.Location = new System.Drawing.Point(575, 38);
            this.ProjectPathButton.Name = "ProjectPathButton";
            this.ProjectPathButton.Size = new System.Drawing.Size(35, 20);
            this.ProjectPathButton.TabIndex = 12;
            this.ProjectPathButton.Text = "· · ·";
            this.ProjectPathButton.UseVisualStyleBackColor = true;
            this.ProjectPathButton.Click += new System.EventHandler(this.ProjectPathButton_Click);
            // 
            // ProjectPathLabel
            // 
            this.ProjectPathLabel.AutoSize = true;
            this.ProjectPathLabel.Location = new System.Drawing.Point(12, 41);
            this.ProjectPathLabel.Name = "ProjectPathLabel";
            this.ProjectPathLabel.Size = new System.Drawing.Size(88, 13);
            this.ProjectPathLabel.TabIndex = 10;
            this.ProjectPathLabel.Text = "Project Directory:";
            // 
            // MiscellaneousGroupBox
            // 
            this.MiscellaneousGroupBox.Controls.Add(this.SplashTestButton);
            this.MiscellaneousGroupBox.Controls.Add(this.SplashSizeComboBox);
            this.MiscellaneousGroupBox.Controls.Add(this.SplashSizeLabel);
            this.MiscellaneousGroupBox.Controls.Add(this.LanguageComboBox);
            this.MiscellaneousGroupBox.Controls.Add(this.LanguageLabel);
            this.MiscellaneousGroupBox.Controls.Add(this.ResetSpotlightButton);
            this.MiscellaneousGroupBox.Location = new System.Drawing.Point(12, 356);
            this.MiscellaneousGroupBox.Name = "MiscellaneousGroupBox";
            this.MiscellaneousGroupBox.Size = new System.Drawing.Size(598, 51);
            this.MiscellaneousGroupBox.TabIndex = 13;
            this.MiscellaneousGroupBox.TabStop = false;
            this.MiscellaneousGroupBox.Text = "Miscellaneous";
            // 
            // SplashTestButton
            // 
            this.SplashTestButton.Location = new System.Drawing.Point(414, 19);
            this.SplashTestButton.Name = "SplashTestButton";
            this.SplashTestButton.Size = new System.Drawing.Size(62, 21);
            this.SplashTestButton.TabIndex = 12;
            this.SplashTestButton.Text = "Test";
            this.SplashTestButton.UseVisualStyleBackColor = true;
            this.SplashTestButton.Click += new System.EventHandler(this.SplashTestButton_Click);
            // 
            // SplashSizeComboBox
            // 
            this.SplashSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SplashSizeComboBox.FormattingEnabled = true;
            this.SplashSizeComboBox.Items.AddRange(new object[] {
            "846x462",
            "1280x720",
            "1920x1080"});
            this.SplashSizeComboBox.Location = new System.Drawing.Point(328, 19);
            this.SplashSizeComboBox.Name = "SplashSizeComboBox";
            this.SplashSizeComboBox.Size = new System.Drawing.Size(80, 21);
            this.SplashSizeComboBox.TabIndex = 11;
            this.SplashSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.SplashSizeComboBox_SelectedIndexChanged);
            // 
            // SplashSizeLabel
            // 
            this.SplashSizeLabel.AutoSize = true;
            this.SplashSizeLabel.Location = new System.Drawing.Point(239, 22);
            this.SplashSizeLabel.Name = "SplashSizeLabel";
            this.SplashSizeLabel.Size = new System.Drawing.Size(65, 13);
            this.SplashSizeLabel.TabIndex = 10;
            this.SplashSizeLabel.Text = "Splash Size:";
            // 
            // LanguageComboBox
            // 
            this.LanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LanguageComboBox.FormattingEnabled = true;
            this.LanguageComboBox.Items.AddRange(new object[] {
            "English"});
            this.LanguageComboBox.Location = new System.Drawing.Point(95, 19);
            this.LanguageComboBox.Name = "LanguageComboBox";
            this.LanguageComboBox.Size = new System.Drawing.Size(97, 21);
            this.LanguageComboBox.TabIndex = 9;
            this.LanguageComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageComboBox_SelectedIndexChanged);
            // 
            // LanguageLabel
            // 
            this.LanguageLabel.AutoSize = true;
            this.LanguageLabel.Location = new System.Drawing.Point(6, 22);
            this.LanguageLabel.Name = "LanguageLabel";
            this.LanguageLabel.Size = new System.Drawing.Size(58, 13);
            this.LanguageLabel.TabIndex = 8;
            this.LanguageLabel.Text = "Language:";
            // 
            // ProjectPathTextBox
            // 
            this.ProjectPathTextBox.FilterSuggestions = false;
            this.ProjectPathTextBox.Location = new System.Drawing.Point(135, 38);
            this.ProjectPathTextBox.Name = "ProjectPathTextBox";
            this.ProjectPathTextBox.PossibleSuggestions = new string[0];
            this.ProjectPathTextBox.Size = new System.Drawing.Size(434, 20);
            this.ProjectPathTextBox.SuggestClear = true;
            this.ProjectPathTextBox.TabIndex = 14;
            this.ProjectPathTextBox.ValueEntered += new System.ComponentModel.CancelEventHandler(this.ProjectPathTextBox_ValueEntered);
            // 
            // GamePathTextBox
            // 
            this.GamePathTextBox.FilterSuggestions = false;
            this.GamePathTextBox.Location = new System.Drawing.Point(135, 13);
            this.GamePathTextBox.Name = "GamePathTextBox";
            this.GamePathTextBox.PossibleSuggestions = new string[0];
            this.GamePathTextBox.Size = new System.Drawing.Size(434, 20);
            this.GamePathTextBox.SuggestClear = false;
            this.GamePathTextBox.TabIndex = 8;
            this.GamePathTextBox.ValueEntered += new System.ComponentModel.CancelEventHandler(this.GamePathTextBox_ValueEntered);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 421);
            this.Controls.Add(this.ProjectPathTextBox);
            this.Controls.Add(this.GamePathTextBox);
            this.Controls.Add(this.MiscellaneousGroupBox);
            this.Controls.Add(this.ProjectPathButton);
            this.Controls.Add(this.ProjectPathLabel);
            this.Controls.Add(this.EditingGroupBox);
            this.Controls.Add(this.LoadingAndSavingGroupBox);
            this.Controls.Add(this.RenderingGroupBox);
            this.Controls.Add(this.ObjectParameterGroupBox);
            this.Controls.Add(this.GamePathButton);
            this.Controls.Add(this.GamePathLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Spotlight - Settings";
            this.ObjectParameterGroupBox.ResumeLayout(false);
            this.ObjectParameterGroupBox.PerformLayout();
            this.RenderingGroupBox.ResumeLayout(false);
            this.RenderingGroupBox.PerformLayout();
            this.LoadingAndSavingGroupBox.ResumeLayout(false);
            this.LoadingAndSavingGroupBox.PerformLayout();
            this.EditingGroupBox.ResumeLayout(false);
            this.EditingGroupBox.PerformLayout();
            this.MiscellaneousGroupBox.ResumeLayout(false);
            this.MiscellaneousGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label GamePathLabel;
        private System.Windows.Forms.Button GamePathButton;
        private System.Windows.Forms.Button RebuildDatabaseButton;
        private System.Windows.Forms.GroupBox ObjectParameterGroupBox;
        private System.Windows.Forms.Label DatabaseInfoLabel;
        private System.Windows.Forms.Label DescriptionInfoLabel;
        private System.Windows.Forms.Button ClearDescriptionsButton;
        private System.Windows.Forms.GroupBox RenderingGroupBox;
        private System.Windows.Forms.CheckBox RenderAreaCheckBox;
        private System.Windows.Forms.Label PlayerLabel;
        private System.Windows.Forms.ComboBox PlayerComboBox;
        private System.Windows.Forms.CheckBox RenderSkyboxesCheckBox;
        private System.Windows.Forms.GroupBox LoadingAndSavingGroupBox;
        private System.Windows.Forms.CheckBox UniqueIDsCheckBox;
        private System.Windows.Forms.Button ResetSpotlightButton;
        private System.Windows.Forms.CheckBox IDEditingCheckBox;
        private System.Windows.Forms.GroupBox EditingGroupBox;
        private System.Windows.Forms.Button ProjectPathButton;
        private System.Windows.Forms.Label ProjectPathLabel;
        private System.Windows.Forms.GroupBox MiscellaneousGroupBox;
        private System.Windows.Forms.ComboBox LanguageComboBox;
        private System.Windows.Forms.Label LanguageLabel;
        private GL_EditorFramework.SuggestingTextBox GamePathTextBox;
        private GL_EditorFramework.SuggestingTextBox ProjectPathTextBox;
        private System.Windows.Forms.ComboBox SplashSizeComboBox;
        private System.Windows.Forms.Label SplashSizeLabel;
        private System.Windows.Forms.Button SplashTestButton;
        private System.Windows.Forms.CheckBox RenderTransparentWallsCheckBox;
        private System.Windows.Forms.CheckBox DoNotLoadTexturesCheckBox;
    }
}