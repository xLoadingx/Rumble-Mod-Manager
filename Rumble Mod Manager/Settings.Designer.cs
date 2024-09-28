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
            SwitchProfile = new Button();
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
            textBox1.BackColor = Color.FromArgb(30, 30, 30);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Arial", 18F);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(12, 52);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(400, 28);
            textBox1.TabIndex = 3;
            // 
            // button1
            // 
            button1.AutoSize = true;
            button1.BackColor = Color.SteelBlue;
            button1.Font = new Font("Arial Narrow", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.GhostWhite;
            button1.Location = new Point(163, 99);
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
            button2.Location = new Point(163, 157);
            button2.Name = "button2";
            button2.Size = new Size(104, 39);
            button2.TabIndex = 5;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // SwitchProfile
            // 
            SwitchProfile.AutoSize = true;
            SwitchProfile.BackColor = Color.SteelBlue;
            SwitchProfile.Font = new Font("Arial Narrow", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SwitchProfile.ForeColor = Color.GhostWhite;
            SwitchProfile.Location = new Point(137, 216);
            SwitchProfile.Name = "SwitchProfile";
            SwitchProfile.Size = new Size(157, 41);
            SwitchProfile.TabIndex = 6;
            SwitchProfile.Text = "Switch Profile";
            SwitchProfile.UseVisualStyleBackColor = false;
            SwitchProfile.Click += SwitchProfile_Click;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(20, 20, 20);
            ClientSize = new Size(424, 269);
            Controls.Add(SwitchProfile);
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
        private Button SwitchProfile;
    }
}