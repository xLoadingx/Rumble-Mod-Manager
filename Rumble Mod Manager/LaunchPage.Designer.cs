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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LaunchPage));
            RUMBLELabel = new Label();
            ModManagerLabel = new Label();
            Credits = new Label();
            PathNotFound = new Label();
            AutoFindButton = new Button();
            ManualFindButton = new Button();
            label1 = new Label();
            LaunchButton = new Guna.UI2.WinForms.Guna2Button();
            SettingsButton = new Guna.UI2.WinForms.Guna2Button();
            CreditsButton = new Guna.UI2.WinForms.Guna2Button();
            progressBar1 = new Guna.UI2.WinForms.Guna2CircleProgressBar();
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
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 302);
            label1.Name = "label1";
            label1.Size = new Size(408, 34);
            label1.TabIndex = 7;
            label1.Text = "Fetching Mods...";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LaunchButton
            // 
            LaunchButton.BorderColor = Color.White;
            LaunchButton.BorderRadius = 10;
            LaunchButton.BorderThickness = 3;
            LaunchButton.CustomizableEdges = customizableEdges1;
            LaunchButton.DisabledState.BorderColor = Color.White;
            LaunchButton.DisabledState.CustomBorderColor = Color.White;
            LaunchButton.DisabledState.FillColor = Color.FromArgb(0, 192, 0);
            LaunchButton.DisabledState.ForeColor = Color.FromArgb(0, 64, 0);
            LaunchButton.FillColor = Color.FromArgb(128, 255, 128);
            LaunchButton.Font = new Font("Arial", 14.25F);
            LaunchButton.ForeColor = Color.FromArgb(22, 149, 12);
            LaunchButton.Location = new Point(124, 192);
            LaunchButton.Name = "LaunchButton";
            LaunchButton.ShadowDecoration.CustomizableEdges = customizableEdges2;
            LaunchButton.Size = new Size(165, 61);
            LaunchButton.TabIndex = 12;
            LaunchButton.Text = "Launch";
            LaunchButton.Click += Continue;
            // 
            // SettingsButton
            // 
            SettingsButton.BorderColor = Color.White;
            SettingsButton.BorderRadius = 10;
            SettingsButton.BorderThickness = 3;
            SettingsButton.CustomizableEdges = customizableEdges3;
            SettingsButton.DisabledState.BorderColor = Color.White;
            SettingsButton.DisabledState.CustomBorderColor = Color.White;
            SettingsButton.DisabledState.FillColor = Color.FromArgb(60, 60, 60);
            SettingsButton.DisabledState.ForeColor = Color.FromArgb(36, 37, 38);
            SettingsButton.FillColor = Color.FromArgb(60, 60, 60);
            SettingsButton.Font = new Font("Arial", 14.25F);
            SettingsButton.ForeColor = Color.FromArgb(36, 37, 38);
            SettingsButton.Location = new Point(124, 259);
            SettingsButton.Name = "SettingsButton";
            SettingsButton.ShadowDecoration.CustomizableEdges = customizableEdges4;
            SettingsButton.Size = new Size(165, 61);
            SettingsButton.TabIndex = 13;
            SettingsButton.Text = "Settings";
            SettingsButton.Click += SettingsButton_Click;
            // 
            // CreditsButton
            // 
            CreditsButton.BorderColor = Color.White;
            CreditsButton.BorderRadius = 10;
            CreditsButton.BorderThickness = 3;
            CreditsButton.CustomizableEdges = customizableEdges5;
            CreditsButton.DisabledState.BorderColor = Color.White;
            CreditsButton.DisabledState.CustomBorderColor = Color.White;
            CreditsButton.DisabledState.FillColor = Color.SlateBlue;
            CreditsButton.DisabledState.ForeColor = Color.FromArgb(63, 54, 150);
            CreditsButton.FillColor = Color.SlateBlue;
            CreditsButton.Font = new Font("Arial", 14.25F);
            CreditsButton.ForeColor = Color.FromArgb(63, 54, 150);
            CreditsButton.Location = new Point(124, 326);
            CreditsButton.Name = "CreditsButton";
            CreditsButton.ShadowDecoration.CustomizableEdges = customizableEdges6;
            CreditsButton.Size = new Size(165, 61);
            CreditsButton.TabIndex = 14;
            CreditsButton.Text = "Credits";
            CreditsButton.Click += CreditsButton_Click;
            // 
            // progressBar1
            // 
            progressBar1.Animated = true;
            progressBar1.AnimationSpeed = 0.5F;
            progressBar1.FillColor = Color.FromArgb(64, 64, 64);
            progressBar1.Font = new Font("Segoe UI", 12F);
            progressBar1.ForeColor = Color.White;
            progressBar1.Location = new Point(139, 166);
            progressBar1.Minimum = 0;
            progressBar1.Name = "progressBar1";
            progressBar1.ProgressColor = Color.FromArgb(0, 192, 0);
            progressBar1.ProgressColor2 = Color.Green;
            progressBar1.ProgressEndCap = System.Drawing.Drawing2D.LineCap.Round;
            progressBar1.ProgressStartCap = System.Drawing.Drawing2D.LineCap.Round;
            progressBar1.ShadowDecoration.BorderRadius = 10;
            progressBar1.ShadowDecoration.CustomizableEdges = customizableEdges7;
            progressBar1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            progressBar1.ShowText = true;
            progressBar1.Size = new Size(130, 130);
            progressBar1.TabIndex = 15;
            progressBar1.Text = "guna2CircleProgressBar1";
            // 
            // LaunchPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(407, 450);
            Controls.Add(SettingsButton);
            Controls.Add(LaunchButton);
            Controls.Add(progressBar1);
            Controls.Add(CreditsButton);
            Controls.Add(label1);
            Controls.Add(ManualFindButton);
            Controls.Add(AutoFindButton);
            Controls.Add(Credits);
            Controls.Add(ModManagerLabel);
            Controls.Add(RUMBLELabel);
            Controls.Add(PathNotFound);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(423, 489);
            MinimumSize = new Size(423, 489);
            Name = "LaunchPage";
            Text = "LaunchPage";
            ResumeLayout(false);
        }

        #endregion
#pragma warning disable CS0169
        private Label RUMBLELabel;
        private Label ModManagerLabel;
        private Label Credits;
        private Label PathNotFound;
        private Button AutoFindButton;
        private Button ManualFindButton;
        private Label label1;
        private System.Windows.Forms.Timer timer1;
        private Guna.UI2.WinForms.Guna2Button LaunchButton;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2Button guna2Button2;
        private Guna.UI2.WinForms.Guna2Button SettingsButton;
        private Guna.UI2.WinForms.Guna2Button CreditsButton;
        private Guna.UI2.WinForms.Guna2CircleProgressBar guna2CircleProgressBar1;
        private Guna.UI2.WinForms.Guna2CircleProgressBar progressBar1;
    }
}