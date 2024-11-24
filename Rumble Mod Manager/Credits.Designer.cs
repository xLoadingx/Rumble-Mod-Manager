namespace Rumble_Mod_Manager
{
    partial class Credits
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Credits));
            CreditsLabel = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // CreditsLabel
            // 
            CreditsLabel.Dock = DockStyle.Top;
            CreditsLabel.ForeColor = Color.White;
            CreditsLabel.Location = new Point(0, 0);
            CreditsLabel.Name = "CreditsLabel";
            CreditsLabel.Size = new Size(434, 83);
            CreditsLabel.TabIndex = 1;
            CreditsLabel.Text = "CREDITS";
            CreditsLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial", 15F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(12, 83);
            label1.Name = "label1";
            label1.Size = new Size(550, 322);
            label1.TabIndex = 2;
            label1.Text = resources.GetString("label1.Text");
            // 
            // Credits
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(434, 450);
            Controls.Add(label1);
            Controls.Add(CreditsLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(450, 489);
            MinimumSize = new Size(450, 489);
            Name = "Credits";
            Text = "Credits";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label CreditsLabel;
        private Label label1;
    }
}