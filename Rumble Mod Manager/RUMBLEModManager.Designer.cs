namespace Rumble_Mod_Manager
{
    partial class RUMBLEModManager
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RUMBLEModManager));
            panel2 = new Panel();
            WelcomeLabel = new Label();
            LaunchGame = new Button();
            SettingsButton = new PictureBox();
            DependenciesLabel = new Label();
            ModAuthorLabel = new Label();
            ModNameLabel = new Label();
            ModVersionLabel = new Label();
            ModPictureDisplay = new PictureBox();
            ThunderstoreButton = new Button();
            UninstallButton = new Button();
            pictureBox1 = new PictureBox();
            ToggleModButton = new PictureBox();
            ToggleModLabel = new Button();
            panel1 = new Panel();
            linkLabel1 = new LinkLabel();
            CustomMapsDownloadButton = new Button();
            ModDescriptionLabel = new Label();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            guna2Elipse2 = new Guna.UI2.WinForms.Guna2Elipse(components);
            guna2Elipse3 = new Guna.UI2.WinForms.Guna2Elipse(components);
            guna2Elipse4 = new Guna.UI2.WinForms.Guna2Elipse(components);
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            guna2Elipse5 = new Guna.UI2.WinForms.Guna2Elipse(components);
            panel3 = new Panel();
            button2 = new Button();
            pictureBox3 = new PictureBox();
            button1 = new Button();
            pictureBox2 = new PictureBox();
            FormTitle = new Label();
            guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(components);
            textBox1 = new TextBox();
            debounceTimer = new System.Windows.Forms.Timer(components);
            guna2Elipse6 = new Guna.UI2.WinForms.Guna2Elipse(components);
            gameCheckTimer = new System.Windows.Forms.Timer(components);
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SettingsButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ToggleModButton).BeginInit();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(40, 40, 40);
            panel2.Controls.Add(WelcomeLabel);
            panel2.Location = new Point(12, 61);
            panel2.Name = "panel2";
            panel2.Size = new Size(592, 447);
            panel2.TabIndex = 2;
            // 
            // WelcomeLabel
            // 
            WelcomeLabel.Dock = DockStyle.Fill;
            WelcomeLabel.ForeColor = Color.FromArgb(64, 64, 64);
            WelcomeLabel.Location = new Point(0, 0);
            WelcomeLabel.Name = "WelcomeLabel";
            WelcomeLabel.Size = new Size(592, 447);
            WelcomeLabel.TabIndex = 0;
            WelcomeLabel.Text = resources.GetString("WelcomeLabel.Text");
            WelcomeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LaunchGame
            // 
            LaunchGame.AutoSize = true;
            LaunchGame.BackColor = Color.Lime;
            LaunchGame.ForeColor = Color.Green;
            LaunchGame.Location = new Point(192, 5);
            LaunchGame.Name = "LaunchGame";
            LaunchGame.Size = new Size(104, 25);
            LaunchGame.TabIndex = 27;
            LaunchGame.Text = "Launch Modded";
            LaunchGame.UseVisualStyleBackColor = false;
            LaunchGame.Click += LaunchGameModded_Click;
            // 
            // SettingsButton
            // 
            SettingsButton.BackColor = Color.Transparent;
            SettingsButton.BackgroundImage = Properties.Resources.darkgray;
            SettingsButton.BackgroundImageLayout = ImageLayout.Zoom;
            SettingsButton.Location = new Point(0, 378);
            SettingsButton.Name = "SettingsButton";
            SettingsButton.Size = new Size(67, 66);
            SettingsButton.TabIndex = 20;
            SettingsButton.TabStop = false;
            SettingsButton.Click += Settings_Button_Click;
            // 
            // DependenciesLabel
            // 
            DependenciesLabel.BackColor = Color.FromArgb(20, 20, 20);
            DependenciesLabel.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DependenciesLabel.ForeColor = Color.White;
            DependenciesLabel.Location = new Point(176, 158);
            DependenciesLabel.Name = "DependenciesLabel";
            DependenciesLabel.Size = new Size(217, 153);
            DependenciesLabel.TabIndex = 12;
            DependenciesLabel.Text = "Dependencies:\r\nLavaGang-MelonLoader-0.6.3\r\nUlvakSkillz-Rumble-Additional_Sounds-2.2.0\r\n";
            DependenciesLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // ModAuthorLabel
            // 
            ModAuthorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModAuthorLabel.AutoEllipsis = true;
            ModAuthorLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModAuthorLabel.Font = new Font("Arial Narrow", 15F);
            ModAuthorLabel.ForeColor = Color.White;
            ModAuthorLabel.Location = new Point(154, 43);
            ModAuthorLabel.Name = "ModAuthorLabel";
            ModAuthorLabel.Size = new Size(252, 25);
            ModAuthorLabel.TabIndex = 7;
            ModAuthorLabel.Text = "By UlvakSkillz";
            ModAuthorLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ModNameLabel
            // 
            ModNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModNameLabel.AutoEllipsis = true;
            ModNameLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModNameLabel.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModNameLabel.ForeColor = Color.White;
            ModNameLabel.ImageAlign = ContentAlignment.MiddleLeft;
            ModNameLabel.Location = new Point(151, 13);
            ModNameLabel.Name = "ModNameLabel";
            ModNameLabel.Size = new Size(255, 30);
            ModNameLabel.TabIndex = 8;
            ModNameLabel.Text = "Rumble Modding API";
            ModNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ModVersionLabel
            // 
            ModVersionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModVersionLabel.AutoEllipsis = true;
            ModVersionLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModVersionLabel.Font = new Font("Arial Narrow", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModVersionLabel.ForeColor = Color.White;
            ModVersionLabel.Location = new Point(150, 109);
            ModVersionLabel.Name = "ModVersionLabel";
            ModVersionLabel.Size = new Size(253, 42);
            ModVersionLabel.TabIndex = 9;
            ModVersionLabel.Text = "Version 6.9.4";
            ModVersionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ModPictureDisplay
            // 
            ModPictureDisplay.Anchor = AnchorStyles.None;
            ModPictureDisplay.BackgroundImage = Properties.Resources.UnknownMod;
            ModPictureDisplay.BackgroundImageLayout = ImageLayout.Zoom;
            ModPictureDisplay.Location = new Point(17, 13);
            ModPictureDisplay.Name = "ModPictureDisplay";
            ModPictureDisplay.Size = new Size(134, 138);
            ModPictureDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
            ModPictureDisplay.TabIndex = 13;
            ModPictureDisplay.TabStop = false;
            // 
            // ThunderstoreButton
            // 
            ThunderstoreButton.AutoSize = true;
            ThunderstoreButton.BackColor = Color.SlateBlue;
            ThunderstoreButton.ForeColor = Color.DarkSlateBlue;
            ThunderstoreButton.Location = new Point(67, 385);
            ThunderstoreButton.Name = "ThunderstoreButton";
            ThunderstoreButton.Size = new Size(161, 48);
            ThunderstoreButton.TabIndex = 14;
            ThunderstoreButton.Text = "Thunderstore";
            ThunderstoreButton.UseVisualStyleBackColor = false;
            ThunderstoreButton.Click += ThunderstoreButton_Click;
            // 
            // UninstallButton
            // 
            UninstallButton.AutoSize = true;
            UninstallButton.BackColor = Color.FromArgb(192, 0, 0);
            UninstallButton.Font = new Font("Arial", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            UninstallButton.ForeColor = Color.Maroon;
            UninstallButton.Location = new Point(232, 331);
            UninstallButton.Name = "UninstallButton";
            UninstallButton.Size = new Size(161, 48);
            UninstallButton.TabIndex = 15;
            UninstallButton.Text = "Uninstall";
            UninstallButton.UseVisualStyleBackColor = false;
            UninstallButton.Click += Uninstall_Button_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = Properties.Resources.Gear;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(17, 396);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(27, 27);
            pictureBox1.TabIndex = 21;
            pictureBox1.TabStop = false;
            pictureBox1.Click += Settings_Button_Click;
            // 
            // ToggleModButton
            // 
            ToggleModButton.BackColor = Color.Transparent;
            ToggleModButton.BackgroundImage = Properties.Resources.blue;
            ToggleModButton.BackgroundImageLayout = ImageLayout.Zoom;
            ToggleModButton.Location = new Point(0, 323);
            ToggleModButton.Name = "ToggleModButton";
            ToggleModButton.Size = new Size(67, 66);
            ToggleModButton.TabIndex = 23;
            ToggleModButton.TabStop = false;
            ToggleModButton.Click += ToggleModButton_Click;
            // 
            // ToggleModLabel
            // 
            ToggleModLabel.AutoSize = true;
            ToggleModLabel.BackColor = Color.FromArgb(192, 0, 0);
            ToggleModLabel.Font = new Font("Arial", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ToggleModLabel.ForeColor = Color.Maroon;
            ToggleModLabel.Location = new Point(67, 331);
            ToggleModLabel.Name = "ToggleModLabel";
            ToggleModLabel.Size = new Size(161, 48);
            ToggleModLabel.TabIndex = 25;
            ToggleModLabel.Text = "Toggle Mod";
            ToggleModLabel.UseVisualStyleBackColor = false;
            ToggleModLabel.Click += ToggleModButton_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(40, 40, 40);
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(CustomMapsDownloadButton);
            panel1.Controls.Add(ToggleModLabel);
            panel1.Controls.Add(ToggleModButton);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(UninstallButton);
            panel1.Controls.Add(SettingsButton);
            panel1.Controls.Add(ThunderstoreButton);
            panel1.Controls.Add(ModPictureDisplay);
            panel1.Controls.Add(ModVersionLabel);
            panel1.Controls.Add(ModDescriptionLabel);
            panel1.Controls.Add(ModNameLabel);
            panel1.Controls.Add(ModAuthorLabel);
            panel1.Controls.Add(DependenciesLabel);
            panel1.Location = new Point(631, 61);
            panel1.Name = "panel1";
            panel1.Size = new Size(406, 447);
            panel1.TabIndex = 2;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = Color.ForestGreen;
            linkLabel1.Location = new Point(157, 82);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(146, 15);
            linkLabel1.TabIndex = 27;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "See Mod On Thunderstore";
            linkLabel1.VisitedLinkColor = Color.ForestGreen;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // CustomMapsDownloadButton
            // 
            CustomMapsDownloadButton.AutoSize = true;
            CustomMapsDownloadButton.BackColor = Color.Purple;
            CustomMapsDownloadButton.Font = new Font("Segoe UI", 9F);
            CustomMapsDownloadButton.ForeColor = Color.FromArgb(64, 0, 64);
            CustomMapsDownloadButton.Location = new Point(232, 385);
            CustomMapsDownloadButton.Name = "CustomMapsDownloadButton";
            CustomMapsDownloadButton.Size = new Size(161, 48);
            CustomMapsDownloadButton.TabIndex = 26;
            CustomMapsDownloadButton.Text = "Custom Maps";
            CustomMapsDownloadButton.UseVisualStyleBackColor = false;
            CustomMapsDownloadButton.Click += CustomMapsDownloadButton_Click;
            // 
            // ModDescriptionLabel
            // 
            ModDescriptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ModDescriptionLabel.AutoEllipsis = true;
            ModDescriptionLabel.BackColor = Color.FromArgb(20, 20, 20);
            ModDescriptionLabel.Font = new Font("Arial Narrow", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModDescriptionLabel.ForeColor = Color.White;
            ModDescriptionLabel.Location = new Point(17, 158);
            ModDescriptionLabel.Name = "ModDescriptionLabel";
            ModDescriptionLabel.Size = new Size(153, 153);
            ModDescriptionLabel.TabIndex = 4;
            ModDescriptionLabel.Text = "API to Help Modders Get Started and to remove the necessity of GameObject.Find";
            ModDescriptionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 12;
            guna2Elipse1.TargetControl = panel2;
            // 
            // guna2Elipse2
            // 
            guna2Elipse2.BorderRadius = 12;
            guna2Elipse2.TargetControl = ModDescriptionLabel;
            // 
            // guna2Elipse3
            // 
            guna2Elipse3.BorderRadius = 12;
            guna2Elipse3.TargetControl = DependenciesLabel;
            // 
            // guna2Elipse4
            // 
            guna2Elipse4.BorderRadius = 12;
            guna2Elipse4.TargetControl = ModPictureDisplay;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 12;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2Elipse5
            // 
            guna2Elipse5.BorderRadius = 12;
            guna2Elipse5.TargetControl = panel1;
            // 
            // panel3
            // 
            panel3.BackColor = Color.FromArgb(15, 15, 15);
            panel3.Controls.Add(button2);
            panel3.Controls.Add(pictureBox3);
            panel3.Controls.Add(button1);
            panel3.Controls.Add(LaunchGame);
            panel3.Controls.Add(pictureBox2);
            panel3.Controls.Add(FormTitle);
            panel3.Location = new Point(0, -1);
            panel3.Name = "panel3";
            panel3.Size = new Size(1049, 33);
            panel3.TabIndex = 3;
            // 
            // button2
            // 
            button2.AutoSize = true;
            button2.BackColor = Color.Lime;
            button2.ForeColor = Color.Green;
            button2.Location = new Point(311, 5);
            button2.Name = "button2";
            button2.Size = new Size(104, 25);
            button2.TabIndex = 30;
            button2.Text = "Launch Vanilla";
            button2.UseVisualStyleBackColor = false;
            button2.Click += LaunchGameVanilla_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.blue;
            pictureBox3.Location = new Point(965, -1);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(37, 37);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 29;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // button1
            // 
            button1.AutoSize = true;
            button1.BackColor = Color.SlateBlue;
            button1.ForeColor = Color.White;
            button1.Location = new Point(797, 4);
            button1.Name = "button1";
            button1.Size = new Size(164, 25);
            button1.TabIndex = 28;
            button1.Text = "Check for Manager Updates";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click_1;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.red;
            pictureBox2.Location = new Point(1009, -1);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(37, 37);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // FormTitle
            // 
            FormTitle.Dock = DockStyle.Left;
            FormTitle.ForeColor = Color.White;
            FormTitle.ImageAlign = ContentAlignment.MiddleLeft;
            FormTitle.Location = new Point(0, 0);
            FormTitle.Name = "FormTitle";
            FormTitle.Size = new Size(186, 33);
            FormTitle.TabIndex = 0;
            FormTitle.Text = "Rumble Mod Manager";
            FormTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // guna2DragControl1
            // 
            guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            guna2DragControl1.TargetControl = panel3;
            guna2DragControl1.UseTransparentDrag = true;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(64, 64, 64);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Segoe UI", 10F);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(12, 35);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Search for mods via name, author, or version";
            textBox1.Size = new Size(293, 23);
            textBox1.TabIndex = 5;
            textBox1.TextAlign = HorizontalAlignment.Center;
            textBox1.WordWrap = false;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // debounceTimer
            // 
            debounceTimer.Interval = 300;
            debounceTimer.Tick += debounceTimer_Tick;
            // 
            // guna2Elipse6
            // 
            guna2Elipse6.BorderRadius = 12;
            guna2Elipse6.TargetControl = textBox1;
            // 
            // gameCheckTimer
            // 
            gameCheckTimer.Interval = 1000;
            gameCheckTimer.Tick += gameCheckTimer_Tick;
            // 
            // RUMBLEModManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.FromArgb(20, 20, 20);
            ClientSize = new Size(1049, 520);
            Controls.Add(textBox1);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Controls.Add(panel2);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(1049, 497);
            Name = "RUMBLEModManager";
            Text = "Form1";
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SettingsButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ToggleModButton).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Panel panel2;
        private PictureBox SettingsButton;
        private Label DependenciesLabel;
        private Label ModAuthorLabel;
        private Label ModNameLabel;
        private Label ModVersionLabel;
        private PictureBox ModPictureDisplay;
        private Button ThunderstoreButton;
        private Button UninstallButton;
        private PictureBox pictureBox1;
        private PictureBox ToggleModButton;
        private Button ToggleModLabel;
        private Panel panel1;
        private Button CustomMapsDownloadButton;
        private Label WelcomeLabel;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse2;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse3;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse4;
        private Label ModDescriptionLabel;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Panel panel3;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse5;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Label FormTitle;
        private PictureBox pictureBox2;
        private Button LaunchGame;
        private TextBox textBox1;
        private System.Windows.Forms.Timer debounceTimer;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse6;
        private Button button1;
        private PictureBox pictureBox3;
        private System.Windows.Forms.Timer gameCheckTimer;
        private Button button2;
        private LinkLabel linkLabel1;
    }
}
