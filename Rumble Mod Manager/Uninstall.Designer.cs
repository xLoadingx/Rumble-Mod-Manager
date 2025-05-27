namespace Rumble_Mod_Manager
{
    partial class Uninstall
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Uninstall));
            label1 = new Label();
            button2 = new Button();
            AllProfiles = new Button();
            panel1 = new Panel();
            label2 = new Label();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            button4 = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(448, 54);
            label1.TabIndex = 0;
            label1.Text = "Do you want to delete this mod from just this \r\nprofile or from all profiles?";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(128, 255, 128);
            button2.Font = new Font("Arial Narrow", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.Green;
            button2.Location = new Point(12, 161);
            button2.Name = "button2";
            button2.Size = new Size(128, 44);
            button2.TabIndex = 6;
            button2.Text = "This Profile";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // AllProfiles
            // 
            AllProfiles.BackColor = Color.Red;
            AllProfiles.Font = new Font("Arial Narrow", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AllProfiles.ForeColor = Color.Maroon;
            AllProfiles.Location = new Point(308, 160);
            AllProfiles.Name = "AllProfiles";
            AllProfiles.Size = new Size(128, 44);
            AllProfiles.TabIndex = 7;
            AllProfiles.Text = "All Profiles";
            AllProfiles.UseVisualStyleBackColor = false;
            AllProfiles.Click += button1_Click_1;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(40, 40, 40);
            panel1.Controls.Add(label2);
            panel1.Location = new Point(57, 66);
            panel1.Name = "panel1";
            panel1.Size = new Size(331, 77);
            panel1.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoEllipsis = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Arial Narrow", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(331, 77);
            label2.TabIndex = 0;
            label2.Text = "ModName";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 12;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // button4
            // 
            button4.BackColor = Color.Red;
            button4.Font = new Font("Arial Narrow", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.Red;
            button4.Location = new Point(429, 0);
            button4.Name = "button4";
            button4.RightToLeft = RightToLeft.No;
            button4.Size = new Size(19, 19);
            button4.TabIndex = 10;
            button4.UseVisualStyleBackColor = false;
            button4.Click += button1_Click;
            // 
            // Uninstall
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(20, 20, 20);
            ClientSize = new Size(448, 216);
            Controls.Add(button4);
            Controls.Add(panel1);
            Controls.Add(AllProfiles);
            Controls.Add(button2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(448, 216);
            MinimumSize = new Size(448, 216);
            Name = "Uninstall";
            Text = "UninstallDialog";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Button button2;
        private Button AllProfiles;
        private Panel panel1;
        private Label label2;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Button button4;
    }
}