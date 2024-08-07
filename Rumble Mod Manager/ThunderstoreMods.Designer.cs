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
            panel1 = new Panel();
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
            ModDisplayGrid = new TableLayoutPanel();
            textBox1 = new TextBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(40, 40, 40);
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
            ModVersionLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModVersionLabel.Font = new Font("Arial Narrow", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModVersionLabel.ForeColor = Color.White;
            ModVersionLabel.Location = new Point(161, 115);
            ModVersionLabel.Name = "ModVersionLabel";
            ModVersionLabel.Size = new Size(242, 40);
            ModVersionLabel.TabIndex = 6;
            ModVersionLabel.Text = "Version 6.9.4";
            ModVersionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ModNameLabel
            // 
            ModNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModNameLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModNameLabel.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModNameLabel.ForeColor = Color.White;
            ModNameLabel.ImageAlign = ContentAlignment.MiddleLeft;
            ModNameLabel.Location = new Point(161, 6);
            ModNameLabel.Name = "ModNameLabel";
            ModNameLabel.Size = new Size(242, 29);
            ModNameLabel.TabIndex = 5;
            ModNameLabel.Text = "Rumble Modding API";
            ModNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ModAuthorLabel
            // 
            ModAuthorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModAuthorLabel.BackColor = Color.FromArgb(40, 40, 40);
            ModAuthorLabel.Font = new Font("Arial Narrow", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModAuthorLabel.ForeColor = Color.White;
            ModAuthorLabel.Location = new Point(161, 61);
            ModAuthorLabel.Name = "ModAuthorLabel";
            ModAuthorLabel.Size = new Size(242, 38);
            ModAuthorLabel.TabIndex = 3;
            ModAuthorLabel.Text = "By UlvakSkillz";
            ModAuthorLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // ModPictureDisplay
            // 
            ModPictureDisplay.Anchor = AnchorStyles.None;
            ModPictureDisplay.Location = new Point(0, 0);
            ModPictureDisplay.Name = "ModPictureDisplay";
            ModPictureDisplay.Size = new Size(155, 155);
            ModPictureDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
            ModPictureDisplay.TabIndex = 2;
            ModPictureDisplay.TabStop = false;
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
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(40, 40, 40);
            panel2.Location = new Point(12, 33);
            panel2.Name = "panel2";
            panel2.Size = new Size(592, 405);
            panel2.TabIndex = 3;
            // 
            // ModDisplayGrid
            // 
            ModDisplayGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ModDisplayGrid.AutoScroll = true;
            ModDisplayGrid.AutoSize = true;
            ModDisplayGrid.ColumnCount = 2;
            ModDisplayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ModDisplayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ModDisplayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ModDisplayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            ModDisplayGrid.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            ModDisplayGrid.Location = new Point(12, 33);
            ModDisplayGrid.Name = "ModDisplayGrid";
            ModDisplayGrid.RowCount = 2;
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            ModDisplayGrid.Size = new Size(592, 405);
            ModDisplayGrid.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(64, 64, 64);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(12, 6);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Search for mods";
            textBox1.Size = new Size(253, 21);
            textBox1.TabIndex = 4;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // ThunderstoreMods
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(20, 20, 20);
            ClientSize = new Size(1033, 444);
            Controls.Add(textBox1);
            Controls.Add(ModDisplayGrid);
            Controls.Add(panel2);
            Controls.Add(panel1);
            MaximumSize = new Size(1049, 483);
            MinimumSize = new Size(1049, 483);
            Name = "ThunderstoreMods";
            Text = "a";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panel1;
        private Panel panel2;
        private TableLayoutPanel ModDisplayGrid;
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
    }
}