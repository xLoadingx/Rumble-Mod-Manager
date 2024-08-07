using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rumble_Mod_Manager
{
    public partial class ModPanelControl : UserControl
    {
        private bool _modEnabled = true;
        private string _modDLLPath = string.Empty;
        public ThunderstoreMods.Mod Mod { get; set; }


        public ModPanelControl()
        {
            InitializeComponent();
        }

        public string ModName
        {
            get { return ModAuthorLabel.Text; }
            set { ModAuthorLabel.Text = value; }
        }

        public string DetailsLabel
        {
            get { return Details.Text; }
            set { Details.Text = value; }
        }

        public Font ModLabelFont
        {
            get { return ModAuthorLabel.Font; }
            set { ModAuthorLabel.Font = value; }
        }

        public Font DetailsLabelFont
        {
            get { return Details.Font; }
            set { Details.Font = value; }
        }

        public Image UpdateNeededImage
        {
            get { return UpdateIcon.BackgroundImage; }
            set { UpdateIcon.BackgroundImage = value; }
        }

        public Color UpdateColor
        {
            get { return Updated.ForeColor; }
            set { Updated.BackColor = value; }
        }

        public string toolTip1Text
        {
            get { return toolTip1.GetToolTip(ModAuthorLabel); }
            set { toolTip1.SetToolTip(ModAuthorLabel, value); toolTip1.SetToolTip(pictureBox1, value); }
        }

        public Image ModImage
        {
            get { return pictureBox1.BackgroundImage; }
            set { pictureBox1.BackgroundImage = value; }
        }

        public bool ModEnabled
        {
            get { return _modEnabled; }
            set { 
                _modEnabled = value;
                this.ForeColor = _modEnabled ? Color.White : Color.Maroon; this.BackColor = value == true ? Color.FromArgb(255, 30, 30, 30) : Color.FromArgb(255, 192, 0, 0); 
            }
        }

        public string ModDllPath
        {
            get { return _modDLLPath; }
            set { _modDLLPath = value; }
        }

        public event EventHandler PanelClicked;
    }
}
