namespace Rumble_Mod_Manager
{
    partial class ThunderstoreModDisplay
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
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            ModName = new Label();
            ModPictureDisplay = new PictureBox();
            Description = new Label();
            Credits = new Label();
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).BeginInit();
            SuspendLayout();
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 12;
            guna2Elipse1.TargetControl = this;
            // 
            // ModName
            // 
            ModName.AutoEllipsis = true;
            ModName.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ModName.ForeColor = Color.White;
            ModName.Location = new Point(0, 177);
            ModName.Name = "ModName";
            ModName.Size = new Size(171, 19);
            ModName.TabIndex = 4;
            ModName.Text = "label1";
            // 
            // ModPictureDisplay
            // 
            ModPictureDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ModPictureDisplay.Image = Properties.Resources.UnknownMod;
            ModPictureDisplay.Location = new Point(0, 0);
            ModPictureDisplay.Name = "ModPictureDisplay";
            ModPictureDisplay.Size = new Size(171, 174);
            ModPictureDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
            ModPictureDisplay.TabIndex = 5;
            ModPictureDisplay.TabStop = false;
            // 
            // Description
            // 
            Description.AutoEllipsis = true;
            Description.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Description.ForeColor = Color.White;
            Description.Location = new Point(0, 211);
            Description.Name = "Description";
            Description.Size = new Size(168, 55);
            Description.TabIndex = 7;
            Description.Text = "label1";
            // 
            // Credits
            // 
            Credits.AutoEllipsis = true;
            Credits.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Credits.ForeColor = Color.White;
            Credits.Location = new Point(0, 192);
            Credits.Name = "Credits";
            Credits.Size = new Size(171, 19);
            Credits.TabIndex = 8;
            Credits.Text = "label1";
            // 
            // ThunderstoreModDisplay
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            Controls.Add(Credits);
            Controls.Add(Description);
            Controls.Add(ModPictureDisplay);
            Controls.Add(ModName);
            Name = "ThunderstoreModDisplay";
            Size = new Size(171, 265);
            ((System.ComponentModel.ISupportInitialize)ModPictureDisplay).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Label ModName;
        private PictureBox ModPictureDisplay;
        private Label Description;
        private Label Credits;
    }
}
