using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rumble_Mod_Manager
{
    public partial class Credits : Form
    {
        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        public Credits()
        {
            InitializeComponent();
            LoadCustomFont();
        }

        private void LoadCustomFont()
        {
            using (FileStream fs = new FileStream("CRUMBLE.otf", FileMode.Open, FileAccess.Read))
            {
                privateFonts.AddFontFile(fs.Name);
            }

            using (FileStream fs = new FileStream("GoodDogPlain.ttf", FileMode.Open, FileAccess.Read))
            {
                privateFonts.AddFontFile(fs.Name);
            }

            foreach (Control control in this.Controls)
            {
                if (control is Label)
                {
                    control.Font = new Font(privateFonts.Families[1], control.Font.Size, FontStyle.Regular);
                }

                if (control.HasChildren)
                {
                    foreach (Control childControl in control.Controls)
                    {
                        if (childControl is Label)
                        {
                            childControl.Font = new Font(privateFonts.Families[1], control.Font.Size, FontStyle.Regular);
                        }
                    }
                }
            }

            CreditsLabel.Font = new Font(privateFonts.Families[0], 54.0F, FontStyle.Regular);
        }
    }
}
