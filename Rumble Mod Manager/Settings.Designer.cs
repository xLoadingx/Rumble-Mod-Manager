namespace Rumble_Mod_Manager
{
    partial class Settings
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
            label1 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            BackUp = new Button();
            Restore = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial Narrow", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(143, 9);
            label1.Name = "label1";
            label1.Size = new Size(163, 31);
            label1.TabIndex = 2;
            label1.Text = "RUMBLE Path:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 52);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(400, 21);
            textBox1.TabIndex = 3;
            // 
            // button1
            // 
            button1.AutoSize = true;
            button1.BackColor = Color.SteelBlue;
            button1.Font = new Font("Arial Narrow", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.GhostWhite;
            button1.Location = new Point(163, 79);
            button1.Name = "button1";
            button1.Size = new Size(104, 41);
            button1.TabIndex = 4;
            button1.Text = "Browse";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.AutoSize = true;
            button2.BackColor = Color.FromArgb(128, 255, 128);
            button2.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.Green;
            button2.Location = new Point(163, 137);
            button2.Name = "button2";
            button2.Size = new Size(104, 39);
            button2.TabIndex = 5;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // BackUp
            // 
            BackUp.AutoSize = true;
            BackUp.BackColor = Color.FromArgb(128, 255, 128);
            BackUp.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BackUp.ForeColor = Color.Green;
            BackUp.Location = new Point(12, 218);
            BackUp.Name = "BackUp";
            BackUp.Size = new Size(168, 39);
            BackUp.TabIndex = 6;
            BackUp.Text = "Backup ML Files";
            BackUp.UseVisualStyleBackColor = false;
            BackUp.Visible = false;
            // 
            // Restore
            // 
            Restore.AutoSize = true;
            Restore.BackColor = Color.FromArgb(192, 0, 0);
            Restore.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Restore.ForeColor = Color.Maroon;
            Restore.Location = new Point(244, 218);
            Restore.Name = "Restore";
            Restore.Size = new Size(171, 39);
            Restore.TabIndex = 7;
            Restore.Text = "Restore ML Files";
            Restore.UseVisualStyleBackColor = false;
            Restore.Visible = false;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(20, 20, 20);
            ClientSize = new Size(424, 269);
            Controls.Add(Restore);
            Controls.Add(BackUp);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            MaximumSize = new Size(440, 308);
            MinimumSize = new Size(440, 308);
            Name = "Settings";
            Text = "SettingsForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private Button button2;
        private Button BackUp;
        private Button Restore;
    }
}