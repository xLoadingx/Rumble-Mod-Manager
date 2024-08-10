namespace Rumble_Mod_Manager
{
    partial class LaunchPage
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
            RUMBLELabel = new Label();
            ModManagerLabel = new Label();
            Credits = new Label();
            PathNotFound = new Label();
            AutoFindButton = new Button();
            ManualFindButton = new Button();
            progressBar1 = new ProgressBar();
            label1 = new Label();
            LaunchButton = new Button();
            SettingsButton = new Button();
            CreditsButton = new Button();
            SuspendLayout();
            // 
            // RUMBLELabel
            // 
            RUMBLELabel.Anchor = AnchorStyles.Top;
            RUMBLELabel.Font = new Font("Microsoft Sans Serif", 72F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RUMBLELabel.ForeColor = Color.White;
            RUMBLELabel.Location = new Point(0, 0);
            RUMBLELabel.Name = "RUMBLELabel";
            RUMBLELabel.Size = new Size(408, 115);
            RUMBLELabel.TabIndex = 0;
            RUMBLELabel.Text = "èrFirstUMBLeFinalé";
            RUMBLELabel.TextAlign = ContentAlignment.BottomCenter;
            // 
            // ModManagerLabel
            // 
            ModManagerLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModManagerLabel.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModManagerLabel.ForeColor = Color.White;
            ModManagerLabel.Location = new Point(74, 102);
            ModManagerLabel.Name = "ModManagerLabel";
            ModManagerLabel.Size = new Size(265, 52);
            ModManagerLabel.TabIndex = 1;
            ModManagerLabel.Text = "Mod Manager ";
            ModManagerLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Credits
            // 
            Credits.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Credits.BackColor = Color.Transparent;
            Credits.Font = new Font("Microsoft Sans Serif", 47.9999924F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Credits.ForeColor = Color.White;
            Credits.Location = new Point(323, 393);
            Credits.Name = "Credits";
            Credits.Size = new Size(85, 57);
            Credits.TabIndex = 2;
            Credits.Text = "|";
            Credits.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PathNotFound
            // 
            PathNotFound.Font = new Font("Arial Narrow", 34.75F);
            PathNotFound.ForeColor = Color.White;
            PathNotFound.Location = new Point(115, 138);
            PathNotFound.Name = "PathNotFound";
            PathNotFound.Size = new Size(184, 118);
            PathNotFound.TabIndex = 3;
            PathNotFound.Text = "Path not Found!";
            PathNotFound.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AutoFindButton
            // 
            AutoFindButton.BackColor = Color.FromArgb(128, 128, 255);
            AutoFindButton.Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AutoFindButton.ForeColor = Color.SlateBlue;
            AutoFindButton.Location = new Point(151, 259);
            AutoFindButton.Name = "AutoFindButton";
            AutoFindButton.Size = new Size(104, 37);
            AutoFindButton.TabIndex = 4;
            AutoFindButton.Text = "Auto";
            AutoFindButton.UseVisualStyleBackColor = false;
            AutoFindButton.Click += AutoFindButton_Click;
            // 
            // ManualFindButton
            // 
            ManualFindButton.BackColor = Color.RoyalBlue;
            ManualFindButton.Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ManualFindButton.ForeColor = Color.CornflowerBlue;
            ManualFindButton.Location = new Point(151, 302);
            ManualFindButton.Name = "ManualFindButton";
            ManualFindButton.Size = new Size(104, 37);
            ManualFindButton.TabIndex = 5;
            ManualFindButton.Text = "Manual";
            ManualFindButton.UseVisualStyleBackColor = false;
            ManualFindButton.Click += ManualFindButton_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(115, 220);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(184, 23);
            progressBar1.TabIndex = 6;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 183);
            label1.Name = "label1";
            label1.Size = new Size(408, 34);
            label1.TabIndex = 7;
            label1.Text = "Fetching Mods...";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LaunchButton
            // 
            LaunchButton.BackColor = Color.FromArgb(128, 255, 128);
            LaunchButton.Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LaunchButton.ForeColor = Color.Green;
            LaunchButton.Location = new Point(124, 192);
            LaunchButton.Name = "LaunchButton";
            LaunchButton.Size = new Size(165, 61);
            LaunchButton.TabIndex = 8;
            LaunchButton.Text = "Launch";
            LaunchButton.UseVisualStyleBackColor = false;
            LaunchButton.Click += Continue;
            // 
            // SettingsButton
            // 
            SettingsButton.BackColor = Color.FromArgb(60, 60, 60);
            SettingsButton.Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SettingsButton.ForeColor = Color.FromArgb(40, 40, 40);
            SettingsButton.Location = new Point(124, 259);
            SettingsButton.Name = "SettingsButton";
            SettingsButton.Size = new Size(165, 61);
            SettingsButton.TabIndex = 10;
            SettingsButton.Text = "Settings";
            SettingsButton.UseVisualStyleBackColor = false;
            SettingsButton.Click += SettingsButton_Click;
            // 
            // CreditsButton
            // 
            CreditsButton.BackColor = Color.SlateBlue;
            CreditsButton.Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CreditsButton.ForeColor = Color.DarkSlateBlue;
            CreditsButton.Location = new Point(124, 326);
            CreditsButton.Name = "CreditsButton";
            CreditsButton.Size = new Size(165, 61);
            CreditsButton.TabIndex = 11;
            CreditsButton.Text = "Credits";
            CreditsButton.UseVisualStyleBackColor = false;
            CreditsButton.Click += CreditsButton_Click;
            // 
            // LaunchPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(407, 450);
            Controls.Add(CreditsButton);
            Controls.Add(SettingsButton);
            Controls.Add(LaunchButton);
            Controls.Add(label1);
            Controls.Add(progressBar1);
            Controls.Add(ManualFindButton);
            Controls.Add(AutoFindButton);
            Controls.Add(Credits);
            Controls.Add(ModManagerLabel);
            Controls.Add(RUMBLELabel);
            Controls.Add(PathNotFound);
            MaximizeBox = false;
            MaximumSize = new Size(423, 489);
            MinimumSize = new Size(423, 489);
            Name = "LaunchPage";
            Text = "LaunchPage";
            ResumeLayout(false);
        }

        #endregion

        private Label RUMBLELabel;
        private Label ModManagerLabel;
        private Label Credits;
        private Label PathNotFound;
        private Button AutoFindButton;
        private Button ManualFindButton;
        private ProgressBar progressBar1;
        private Label label1;
        private Button LaunchButton;
        private Button SettingsButton;
        private Button CreditsButton;
        private System.Windows.Forms.Timer timer1;
    }
}