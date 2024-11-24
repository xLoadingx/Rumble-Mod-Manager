namespace Rumble_Mod_Manager
{
    partial class ThunderstoreMods
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThunderstoreMods));
            panel1 = new Panel();
            linkLabel1 = new LinkLabel();
            ForwardButton = new Button();
            PageNumberLabel = new Button();
            BackButton = new Button();
            InstallButton = new Button();
            ModVersionLabel = new Label();
            ModNameLabel = new Label();
            ModAuthorLabel = new Label();
            ModPictureDisplay = new PictureBox();
            ModDescriptionLabel = new Label();
            DependenciesLabel = new Label();
            panel2 = new Panel();
            textBox1 = new TextBox();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            guna2Elipse2 = new Guna.UI2.WinForms.Guna2Elipse(components);
            guna2Elipse3 = new Guna.UI2.WinForms.Guna2Elipse(components);
            debounceTimer = new System.Windows.Forms.Timer(components);
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(40, 40, 40);
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(ForwardButton);
            panel1.Controls.Add(PageNumberLabel);
            panel1.Controls.Add(BackButton);
            panel1.Controls.Add(InstallButton);
            panel1.Controls.Add(ModVersionLabel);
            panel1.Controls.Add(ModNameLabel);
            panel1.Controls.Add(ModAuthorLabel);
            panel1.Controls.Add(ModPictureDisplay);
            panel1.Controls.Add(ModDescriptionLabel);
            panel1.Controls.Add(DependenciesLabel);
            panel1.Location = new Point(627, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(406, 442);
            panel1.TabIndex = 1;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = Color.ForestGreen;
            linkLabel1.Location = new Point(164, 80);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(146, 15);
            linkLabel1.TabIndex = 13;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "See Mod On Thunderstore";
            linkLabel1.VisitedLinkColor = Color.ForestGreen;
            // 
            // ForwardButton
            // 
            ForwardButton.BackColor = Color.FromArgb(128, 128, 255);
            ForwardButton.Font = new Font("Arial Narrow", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForwardButton.ForeColor = Color.SlateBlue;
            ForwardButton.Location = new Point(344, 379);
            ForwardButton.Name = "ForwardButton";
            ForwardButton.Size = new Size(50, 50);
            ForwardButton.TabIndex = 10;
            ForwardButton.Text = ">";
            ForwardButton.UseVisualStyleBackColor = false;
            ForwardButton.Click += ForwardButton_Click;
            // 
            // PageNumberLabel
            // 
            PageNumberLabel.BackColor = Color.FromArgb(128, 128, 255);
            PageNumberLabel.Font = new Font("Arial Narrow", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PageNumberLabel.ForeColor = Color.SlateBlue;
            PageNumberLabel.Location = new Point(122, 379);
            PageNumberLabel.Name = "PageNumberLabel";
            PageNumberLabel.Size = new Size(151, 50);
            PageNumberLabel.TabIndex = 9;
            PageNumberLabel.Text = "Page 1";
            PageNumberLabel.UseVisualStyleBackColor = false;
            // 
            // BackButton
            // 
            BackButton.BackColor = Color.FromArgb(128, 128, 255);
            BackButton.Font = new Font("Arial Narrow", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BackButton.ForeColor = Color.SlateBlue;
            BackButton.Location = new Point(12, 379);
            BackButton.Name = "BackButton";
            BackButton.Size = new Size(50, 50);
            BackButton.TabIndex = 8;
            BackButton.Text = "<";
            BackButton.UseVisualStyleBackColor = false;
            BackButton.Click += BackButton_Click;
            // 
            // InstallButton
            // 
            InstallButton.BackColor = Color.FromArgb(128, 255, 128);
            InstallButton.Font = new Font("Arial Narrow", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            InstallButton.ForeColor = Color.Green;
            InstallButton.Location = new Point(122, 323);
            InstallButton.Name = "InstallButton";
            InstallButton.Size = new Size(151, 50);
            InstallButton.TabIndex = 7;
            InstallButton.Text = "Install";
            InstallButton.UseVisualStyleBackColor = false;
            InstallButton.Click += InstallButton_Click;
            // 
            // ModVersionLabel
            // 
            ModVersionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModVersionLabel.AutoEllipsis = true;
            ModVersionLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModVersionLabel.Font = new Font("Arial Narrow", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModVersionLabel.ForeColor = Color.White;
            ModVersionLabel.Location = new Point(161, 115);
            ModVersionLabel.Name = "ModVersionLabel";
            ModVersionLabel.Size = new Size(242, 40);
            ModVersionLabel.TabIndex = 6;
            ModVersionLabel.Text = "Version 6.9.4";
            ModVersionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ModNameLabel
            // 
            ModNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModNameLabel.AutoEllipsis = true;
            ModNameLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModNameLabel.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModNameLabel.ForeColor = Color.White;
            ModNameLabel.ImageAlign = ContentAlignment.MiddleLeft;
            ModNameLabel.Location = new Point(161, 6);
            ModNameLabel.Name = "ModNameLabel";
            ModNameLabel.Size = new Size(242, 29);
            ModNameLabel.TabIndex = 5;
            ModNameLabel.Text = "Rumble Modding API";
            ModNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ModAuthorLabel
            // 
            ModAuthorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModAuthorLabel.AutoEllipsis = true;
            ModAuthorLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModAuthorLabel.Font = new Font("Arial Narrow", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModAuthorLabel.ForeColor = Color.White;
            ModAuthorLabel.Location = new Point(158, 37);
            ModAuthorLabel.Name = "ModAuthorLabel";
            ModAuthorLabel.Size = new Size(242, 38);
            ModAuthorLabel.TabIndex = 3;
            ModAuthorLabel.Text = "By UlvakSkillz";
            ModAuthorLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ModPictureDisplay
            // 
            ModPictureDisplay.Anchor = AnchorStyles.None;
            ModPictureDisplay.Location = new Point(2, 0);
            ModPictureDisplay.Name = "ModPictureDisplay";
            ModPictureDisplay.Size = new Size(155, 155);
            ModPictureDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
            ModPictureDisplay.TabIndex = 2;
            ModPictureDisplay.TabStop = false;
            // 
            // ModDescriptionLabel
            // 
            ModDescriptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ModDescriptionLabel.AutoEllipsis = true;
            ModDescriptionLabel.BackColor = Color.FromArgb(20, 20, 20);
            ModDescriptionLabel.Font = new Font("Arial Narrow", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModDescriptionLabel.ForeColor = Color.White;
            ModDescriptionLabel.Location = new Point(2, 158);
            ModDescriptionLabel.Name = "ModDescriptionLabel";
            ModDescriptionLabel.Size = new Size(155, 162);
            ModDescriptionLabel.TabIndex = 4;
            ModDescriptionLabel.Text = "API to Help Modders Get Started and to remove the necessity of GameObject.Find";
            ModDescriptionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DependenciesLabel
            // 
            DependenciesLabel.AutoEllipsis = true;
            DependenciesLabel.BackColor = Color.FromArgb(20, 20, 20);
            DependenciesLabel.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DependenciesLabel.ForeColor = Color.White;
            DependenciesLabel.Location = new Point(161, 158);
            DependenciesLabel.Name = "DependenciesLabel";
            DependenciesLabel.Size = new Size(243, 162);
            DependenciesLabel.TabIndex = 12;
            DependenciesLabel.Text = "Dependencies:\r\nLavaGang-MelonLoader-0.6.3\r\nUlvakSkillz-Rumble-Additional_Sounds-2.2.0\r\n";
            DependenciesLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(40, 40, 40);
            panel2.Location = new Point(12, 33);
            panel2.Name = "panel2";
            panel2.Size = new Size(592, 405);
            panel2.TabIndex = 3;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(64, 64, 64);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(12, 6);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Search for mods via name, author, or description";
            textBox1.Size = new Size(278, 23);
            textBox1.TabIndex = 4;
            textBox1.TextAlign = HorizontalAlignment.Center;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 12;
            guna2Elipse1.TargetControl = ModPictureDisplay;
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
            // ThunderstoreMods
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(20, 20, 20);
            ClientSize = new Size(1033, 444);
            Controls.Add(textBox1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(1049, 483);
            MinimumSize = new Size(1049, 483);
            Name = "ThunderstoreMods";
            Text = "a";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panel1;
        private Panel panel2;
        private PictureBox ModPictureDisplay;
        private Label ModAuthorLabel;
        private Label ModDescriptionLabel;
        private Label ModNameLabel;
        private Label ModVersionLabel;
        private Button InstallButton;
        private Button ForwardButton;
        private Button PageNumberLabel;
        private Button BackButton;
        private Label DependenciesLabel;
        private TextBox textBox1;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse2;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse3;
        private LinkLabel linkLabel1;
        private System.Windows.Forms.Timer debounceTimer;
    }
}