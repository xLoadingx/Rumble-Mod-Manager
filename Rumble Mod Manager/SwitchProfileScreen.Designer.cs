namespace Rumble_Mod_Manager
{
    partial class SwitchProfileScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwitchProfileScreen));
            Title = new Label();
            panel1 = new Panel();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            panel2 = new Panel();
            guna2Elipse2 = new Guna.UI2.WinForms.Guna2Elipse(components);
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            Import = new Button();
            Export = new Button();
            SuspendLayout();
            // 
            // Title
            // 
            Title.Dock = DockStyle.Top;
            Title.ForeColor = Color.White;
            Title.Location = new Point(0, 0);
            Title.Name = "Title";
            Title.Size = new Size(387, 77);
            Title.TabIndex = 0;
            Title.Text = "Switch\r\nProfile";
            Title.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.BackColor = Color.FromArgb(40, 40, 40);
            panel1.Location = new Point(12, 80);
            panel1.Name = "panel1";
            panel1.Size = new Size(364, 10);
            panel1.TabIndex = 1;
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.TargetControl = panel1;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(40, 40, 40);
            panel2.Location = new Point(12, 109);
            panel2.Name = "panel2";
            panel2.Size = new Size(363, 284);
            panel2.TabIndex = 2;
            // 
            // guna2Elipse2
            // 
            guna2Elipse2.BorderRadius = 12;
            guna2Elipse2.TargetControl = panel2;
            // 
            // button1
            // 
            button1.AutoSize = true;
            button1.BackColor = Color.FromArgb(128, 255, 128);
            button1.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.Green;
            button1.Location = new Point(12, 399);
            button1.Name = "button1";
            button1.Size = new Size(104, 39);
            button1.TabIndex = 6;
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.AutoSize = true;
            button2.BackColor = Color.Red;
            button2.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.FromArgb(192, 0, 0);
            button2.Location = new Point(158, 399);
            button2.Name = "button2";
            button2.Size = new Size(104, 39);
            button2.TabIndex = 7;
            button2.Text = "Delete";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.AutoSize = true;
            button3.BackColor = Color.SteelBlue;
            button3.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.White;
            button3.Location = new Point(268, 399);
            button3.Name = "button3";
            button3.Size = new Size(104, 39);
            button3.TabIndex = 8;
            button3.Text = "Rename";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // Import
            // 
            Import.AutoSize = true;
            Import.BackColor = Color.FromArgb(128, 128, 255);
            Import.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Import.ForeColor = Color.SlateBlue;
            Import.Location = new Point(272, 12);
            Import.Name = "Import";
            Import.Size = new Size(104, 39);
            Import.TabIndex = 9;
            Import.Text = "Import";
            Import.UseVisualStyleBackColor = false;
            Import.Click += Import_Click;
            // 
            // Export
            // 
            Export.AutoSize = true;
            Export.BackColor = Color.FromArgb(255, 128, 255);
            Export.Font = new Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Export.ForeColor = Color.Fuchsia;
            Export.Location = new Point(12, 12);
            Export.Name = "Export";
            Export.Size = new Size(104, 39);
            Export.TabIndex = 10;
            Export.Text = "Export";
            Export.UseVisualStyleBackColor = false;
            Export.Click += Export_Click;
            // 
            // SwitchProfileScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(387, 450);
            Controls.Add(Export);
            Controls.Add(Import);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(Title);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(403, 489);
            MinimumSize = new Size(403, 489);
            Name = "SwitchProfileScreen";
            Text = "SwitchProfileScreen";
            Load += SwitchProfileScreen_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title;
        private Panel panel1;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Panel panel2;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse2;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button Import;
        private Button Export;
    }
}