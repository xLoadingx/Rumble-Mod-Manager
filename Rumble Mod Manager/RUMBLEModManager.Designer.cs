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
            panel2 = new Panel();
            SettingsButton = new PictureBox();
            UpdateButton = new PictureBox();
            DependenciesLabel = new Label();
            ModAuthorLabel = new Label();
            ModNameLabel = new Label();
            ModDescriptionLabel = new Label();
            ModVersionLabel = new Label();
            ModPictureDisplay = new PictureBox();
            ThunderstoreButton = new Button();
            UninstallButton = new Button();
            DateUpdated = new Label();
            pictureBox1 = new PictureBox();
            ToggleModButton = new PictureBox();
            pictureBox2 = new PictureBox();
            ToggleModLabel = new Button();
            panel1 = new Panel();
            CustomMapsDownloadButton = new Button();
            ((System.ComponentModel.ISupportInitialize)SettingsButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)UpdateButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ToggleModButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(40, 40, 40);
            panel2.Location = new Point(12, 12);
            panel2.Name = "panel2";
            panel2.Size = new Size(592, 420);
            panel2.TabIndex = 2;
            // 
            // SettingsButton
            // 
            SettingsButton.BackColor = Color.Transparent;
            SettingsButton.BackgroundImage = Properties.Resources.darkgray;
            SettingsButton.BackgroundImageLayout = ImageLayout.Zoom;
            SettingsButton.Location = new Point(0, 377);
            SettingsButton.Name = "SettingsButton";
            SettingsButton.Size = new Size(67, 66);
            SettingsButton.TabIndex = 20;
            SettingsButton.TabStop = false;
            SettingsButton.Click += Settings_Button_Click;
            // 
            // UpdateButton
            // 
            UpdateButton.BackColor = Color.Transparent;
            UpdateButton.BackgroundImage = Properties.Resources.blue;
            UpdateButton.BackgroundImageLayout = ImageLayout.Zoom;
            UpdateButton.Location = new Point(161, 55);
            UpdateButton.Name = "UpdateButton";
            UpdateButton.Size = new Size(67, 66);
            UpdateButton.TabIndex = 19;
            UpdateButton.TabStop = false;
            UpdateButton.Click += button1_Click;
            // 
            // DependenciesLabel
            // 
            DependenciesLabel.BackColor = Color.FromArgb(20, 20, 20);
            DependenciesLabel.BorderStyle = BorderStyle.Fixed3D;
            DependenciesLabel.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DependenciesLabel.ForeColor = Color.White;
            DependenciesLabel.Location = new Point(161, 158);
            DependenciesLabel.Name = "DependenciesLabel";
            DependenciesLabel.Size = new Size(243, 162);
            DependenciesLabel.TabIndex = 12;
            DependenciesLabel.Text = "Dependencies:\r\nLavaGang-MelonLoader-0.6.3\r\nUlvakSkillz-Rumble-Additional_Sounds-2.2.0\r\n";
            DependenciesLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // ModAuthorLabel
            // 
            ModAuthorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModAuthorLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModAuthorLabel.Font = new Font("Arial Narrow", 15F);
            ModAuthorLabel.ForeColor = Color.White;
            ModAuthorLabel.Location = new Point(161, 39);
            ModAuthorLabel.Name = "ModAuthorLabel";
            ModAuthorLabel.Size = new Size(242, 24);
            ModAuthorLabel.TabIndex = 7;
            ModAuthorLabel.Text = "By UlvakSkillz";
            ModAuthorLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ModNameLabel
            // 
            ModNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModNameLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModNameLabel.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModNameLabel.ForeColor = Color.White;
            ModNameLabel.ImageAlign = ContentAlignment.MiddleLeft;
            ModNameLabel.Location = new Point(162, 10);
            ModNameLabel.Name = "ModNameLabel";
            ModNameLabel.Size = new Size(242, 29);
            ModNameLabel.TabIndex = 8;
            ModNameLabel.Text = "Rumble Modding API";
            ModNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ModDescriptionLabel
            // 
            ModDescriptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ModDescriptionLabel.BackColor = Color.FromArgb(20, 20, 20);
            ModDescriptionLabel.BorderStyle = BorderStyle.Fixed3D;
            ModDescriptionLabel.Font = new Font("Arial Narrow", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModDescriptionLabel.ForeColor = Color.White;
            ModDescriptionLabel.Location = new Point(0, 158);
            ModDescriptionLabel.Name = "ModDescriptionLabel";
            ModDescriptionLabel.Size = new Size(155, 162);
            ModDescriptionLabel.TabIndex = 4;
            ModDescriptionLabel.Text = "API to Help Modders Get Started and to remove the necessity of GameObject.Find";
            ModDescriptionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ModVersionLabel
            // 
            ModVersionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModVersionLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModVersionLabel.Font = new Font("Arial Narrow", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModVersionLabel.ForeColor = Color.White;
            ModVersionLabel.Location = new Point(229, 65);
            ModVersionLabel.Name = "ModVersionLabel";
            ModVersionLabel.Size = new Size(174, 42);
            ModVersionLabel.TabIndex = 9;
            ModVersionLabel.Text = "Version 6.9.4";
            ModVersionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ModPictureDisplay
            // 
            ModPictureDisplay.Anchor = AnchorStyles.None;
            ModPictureDisplay.BackgroundImage = Properties.Resources.UnknownMod;
            ModPictureDisplay.BackgroundImageLayout = ImageLayout.Zoom;
            ModPictureDisplay.Location = new Point(0, 3);
            ModPictureDisplay.Name = "ModPictureDisplay";
            ModPictureDisplay.Size = new Size(155, 155);
            ModPictureDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
            ModPictureDisplay.TabIndex = 13;
            ModPictureDisplay.TabStop = false;
            // 
            // ThunderstoreButton
            // 
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
            // DateUpdated
            // 
            DateUpdated.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DateUpdated.BackColor = Color.FromArgb(40, 40, 40);
            DateUpdated.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DateUpdated.ForeColor = Color.White;
            DateUpdated.Location = new Point(162, 119);
            DateUpdated.Name = "DateUpdated";
            DateUpdated.Size = new Size(242, 39);
            DateUpdated.TabIndex = 18;
            DateUpdated.Text = "Version 6.9.4";
            DateUpdated.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = Properties.Resources.Gear;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(17, 395);
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
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.BackgroundImage = Properties.Resources.UpdateIcon;
            pictureBox2.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox2.Location = new Point(178, 80);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(27, 27);
            pictureBox2.TabIndex = 24;
            pictureBox2.TabStop = false;
            pictureBox2.Click += button1_Click;
            // 
            // ToggleModLabel
            // 
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
            panel1.Controls.Add(CustomMapsDownloadButton);
            panel1.Controls.Add(ToggleModLabel);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(ToggleModButton);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(DateUpdated);
            panel1.Controls.Add(UninstallButton);
            panel1.Controls.Add(ThunderstoreButton);
            panel1.Controls.Add(ModPictureDisplay);
            panel1.Controls.Add(ModVersionLabel);
            panel1.Controls.Add(ModDescriptionLabel);
            panel1.Controls.Add(ModNameLabel);
            panel1.Controls.Add(ModAuthorLabel);
            panel1.Controls.Add(DependenciesLabel);
            panel1.Controls.Add(UpdateButton);
            panel1.Controls.Add(SettingsButton);
            panel1.Location = new Point(628, -1);
            panel1.Name = "panel1";
            panel1.Size = new Size(406, 446);
            panel1.TabIndex = 2;
            // 
            // CustomMapsDownloadButton
            // 
            CustomMapsDownloadButton.BackColor = Color.Purple;
            CustomMapsDownloadButton.ForeColor = Color.FromArgb(64, 0, 64);
            CustomMapsDownloadButton.Location = new Point(232, 385);
            CustomMapsDownloadButton.Name = "CustomMapsDownloadButton";
            CustomMapsDownloadButton.Size = new Size(161, 48);
            CustomMapsDownloadButton.TabIndex = 26;
            CustomMapsDownloadButton.Text = "Custom Maps";
            CustomMapsDownloadButton.UseVisualStyleBackColor = false;
            CustomMapsDownloadButton.Click += CustomMapsDownloadButton_Click;
            // 
            // RUMBLEModManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(20, 20, 20);
            ClientSize = new Size(1033, 444);
            Controls.Add(panel1);
            Controls.Add(panel2);
            MaximizeBox = false;
            MaximumSize = new Size(1049, 483);
            MinimumSize = new Size(1049, 483);
            Name = "RUMBLEModManager";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)SettingsButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)UpdateButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ToggleModButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel2;
        private PictureBox SettingsButton;
        private PictureBox UpdateButton;
        private Label DependenciesLabel;
        private Label ModAuthorLabel;
        private Label ModNameLabel;
        private Label ModDescriptionLabel;
        private Label ModVersionLabel;
        private PictureBox ModPictureDisplay;
        private Button ThunderstoreButton;
        private Button UninstallButton;
        private Label DateUpdated;
        private PictureBox pictureBox1;
        private PictureBox ToggleModButton;
        private PictureBox pictureBox2;
        private Button ToggleModLabel;
        private Panel panel1;
        private Button CustomMapsDownloadButton;
    }
}
