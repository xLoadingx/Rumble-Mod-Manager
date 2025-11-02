using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Rumble_Mod_Manager
{
    public class PersistentForm : Form
    {
        protected string PersistenceKey => this.Name;

        public PersistentForm()
        {
            this.Load += PersistentForm_Load;
            this.FormClosing += PersistentForm_FormClosing;
        }

        private void PersistentForm_Load(object sender, EventArgs e)
        {
            var key = Registry.CurrentUser.OpenSubKey($"Software\\{Application.ProductName}\\{PersistenceKey}");
            if (key == null) return;

            int width = (int)key.GetValue("Width", this.Width);
            int height = (int)key.GetValue("Height", this.Height);
            int x = (int)key.GetValue("X", this.Location.X);
            int y = (int)key.GetValue("Y", this.Location.Y);

            this.Size = new Size(width, height);

            Rectangle allScreenBounds = Screen.AllScreens
                .Select(s => s.WorkingArea)
                .Aggregate(Rectangle.Union);

            Rectangle windowRect = new Rectangle(x, y, width, height);

            if (!allScreenBounds.IntersectsWith(windowRect))
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Location = new Point(
                    (allScreenBounds.Width - width) / 2,
                    (allScreenBounds.Height - height) / 2
                );
            } else
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(x, y);
            }

            key.Close();
        }

        private void PersistentForm_FormClosing(object sender, EventArgs e)
        {
            var key = Registry.CurrentUser.CreateSubKey($"Software\\{Application.ProductName}\\{PersistenceKey}");
            key.SetValue("Width", this.Width);
            key.SetValue("Height", this.Height);
            key.SetValue("X", this.Location.X);
            key.SetValue("Y", this.Location.Y);
            key.Close();
        }
    }
}
