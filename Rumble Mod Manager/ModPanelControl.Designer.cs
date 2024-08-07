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
            Updated = new Panel();
            UpdateIcon = new PictureBox();
            toolTip1 = new ToolTip(components);
            pictureBox1 = new PictureBox();
            ModAuthorLabel = new Label();
            Details = new Label();
            Updated.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)UpdateIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // Updated
            // 
            Updated.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Updated.AutoSize = true;
            Updated.BackColor = Color.Lime;
            Updated.BorderStyle = BorderStyle.Fixed3D;
            Updated.Controls.Add(UpdateIcon);
            Updated.Location = new Point(560, 53);
            Updated.Name = "Updated";
            Updated.Size = new Size(34, 33);
            Updated.TabIndex = 1;
            // 
            // UpdateIcon
            // 
            UpdateIcon.BackgroundImage = Properties.Resources.UpdateIcon;
            UpdateIcon.BackgroundImageLayout = ImageLayout.Zoom;
            UpdateIcon.Location = new Point(0, 0);
            UpdateIcon.Name = "UpdateIcon";
            UpdateIcon.Size = new Size(27, 26);
            UpdateIcon.TabIndex = 6;
            UpdateIcon.TabStop = false;
            // 
            // toolTip1
            // 
            toolTip1.ShowAlways = true;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(13, 13);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(57, 57);
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // ModAuthorLabel
            // 
            ModAuthorLabel.AutoSize = true;
            ModAuthorLabel.ForeColor = Color.White;
            ModAuthorLabel.Location = new Point(76, 13);
            ModAuthorLabel.Name = "ModAuthorLabel";
            ModAuthorLabel.Size = new Size(41, 15);
            ModAuthorLabel.TabIndex = 4;
            ModAuthorLabel.Text = "label1";
            // 
            // Details
            // 
            Details.AutoSize = true;
            Details.ForeColor = Color.White;
            Details.Location = new Point(76, 41);
            Details.Name = "Details";
            Details.Size = new Size(41, 15);
            Details.TabIndex = 5;
            Details.Text = "label1";
            // 
            // ModPanelControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(Details);
            Controls.Add(ModAuthorLabel);
            Controls.Add(Updated);
            Controls.Add(pictureBox1);
            Name = "ModPanelControl";
            Size = new Size(592, 84);
            Updated.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)UpdateIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel Updated;
        private ToolTip toolTip1;
        private PictureBox pictureBox1;
        private Label ModAuthorLabel;
        private Label Details;
        private PictureBox UpdateIcon;
    }
}
