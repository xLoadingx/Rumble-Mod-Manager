namespace Rumble_Mod_Manager
{
    partial class ModPanelControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            toolTip1 = new ToolTip(components);
            guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            ModName = new Label();
            label2 = new Label();
            guna2Elipse3 = new Guna.UI2.WinForms.Guna2Elipse(components);
            ImageButton = new Guna.UI2.WinForms.Guna2ImageButton();
            guna2Elipse4 = new Guna.UI2.WinForms.Guna2Elipse(components);
            FavoritedStar = new Guna.UI2.WinForms.Guna2ImageCheckBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)guna2PictureBox1).BeginInit();
            SuspendLayout();
            // 
            // toolTip1
            // 
            toolTip1.ShowAlways = true;
            // 
            // guna2PictureBox1
            // 
            guna2PictureBox1.BackColor = Color.Transparent;
            guna2PictureBox1.BorderRadius = 12;
            guna2PictureBox1.CustomizableEdges = customizableEdges1;
            guna2PictureBox1.ImageRotate = 0F;
            guna2PictureBox1.Location = new Point(7, 9);
            guna2PictureBox1.Name = "guna2PictureBox1";
            guna2PictureBox1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2PictureBox1.Size = new Size(65, 65);
            guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            guna2PictureBox1.TabIndex = 0;
            guna2PictureBox1.TabStop = false;
            // 
            // ModName
            // 
            ModName.AutoSize = true;
            ModName.BackColor = Color.Transparent;
            ModName.ForeColor = Color.White;
            ModName.Location = new Point(78, 19);
            ModName.Name = "ModName";
            ModName.Size = new Size(38, 15);
            ModName.TabIndex = 1;
            ModName.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(78, 45);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 2;
            label2.Text = "label2";
            // 
            // guna2Elipse3
            // 
            guna2Elipse3.BorderRadius = 24;
            guna2Elipse3.TargetControl = this;
            // 
            // ImageButton
            // 
            ImageButton.CheckedState.ImageSize = new Size(64, 64);
            ImageButton.HoverState.ImageSize = new Size(41, 30);
            ImageButton.Image = Properties.Resources.UpdateIcon;
            ImageButton.ImageOffset = new Point(0, 0);
            ImageButton.ImageRotate = 0F;
            ImageButton.ImageSize = new Size(35, 24);
            ImageButton.Location = new Point(545, 0);
            ImageButton.Name = "ImageButton";
            ImageButton.PressedState.ImageSize = new Size(35, 24);
            ImageButton.ShadowDecoration.CustomizableEdges = customizableEdges4;
            ImageButton.Size = new Size(35, 35);
            ImageButton.TabIndex = 3;
            // 
            // guna2Elipse4
            // 
            guna2Elipse4.BorderRadius = 24;
            guna2Elipse4.TargetControl = ImageButton;
            // 
            // FavoritedStar
            // 
            FavoritedStar.BackColor = Color.Transparent;
            FavoritedStar.CheckedState.Image = Properties.Resources.Favoritied;
            FavoritedStar.CheckedState.ImageSize = new Size(25, 25);
            FavoritedStar.HoverState.ImageSize = new Size(28, 28);
            FavoritedStar.Image = Properties.Resources.Not_Favorited;
            FavoritedStar.ImageOffset = new Point(0, 0);
            FavoritedStar.ImageRotate = 0F;
            FavoritedStar.ImageSize = new Size(25, 25);
            FavoritedStar.Location = new Point(545, 45);
            FavoritedStar.Name = "FavoritedStar";
            FavoritedStar.ShadowDecoration.CustomizableEdges = customizableEdges3;
            FavoritedStar.Size = new Size(35, 35);
            FavoritedStar.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.ForeColor = Color.DimGray;
            label1.Location = new Point(122, 45);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 5;
            label1.Text = "label1";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ModPanelControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            Controls.Add(label1);
            Controls.Add(FavoritedStar);
            Controls.Add(ImageButton);
            Controls.Add(label2);
            Controls.Add(ModName);
            Controls.Add(guna2PictureBox1);
            Name = "ModPanelControl";
            Size = new Size(580, 80);
            ((System.ComponentModel.ISupportInitialize)guna2PictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
#pragma warning disable CS0169
        private ToolTip toolTip1;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2ImageButton Updated;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse2;
        private Guna.UI2.WinForms.Guna2HtmlLabel ModAuthorLabel;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private Guna.UI2.WinForms.Guna2HtmlLabel Details;
        private Label ModName;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse3;
        public Guna.UI2.WinForms.Guna2ImageButton ImageButton;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse4;
        public Label label2;
        public Guna.UI2.WinForms.Guna2ImageCheckBox FavoritedStar;
        public Label label1;
    }
}
