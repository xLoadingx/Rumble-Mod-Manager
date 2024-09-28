using Guna.UI2.WinForms;
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
    public partial class ThunderstoreModDisplay : UserControl
    {
        private string _onlineModLink;

        public ThunderstoreModDisplay()
        {
            InitializeComponent();
        }

        public Image ModImage
        {
            get { return ModPictureDisplay.Image; }
            set { ModPictureDisplay.Image = value; }
        }

        public string ModNameLabel
        {
            get { return ModName.Text; }
            set { ModName.Text = value; }
        }

        public string OnlineModLink
        {
            get { return _onlineModLink; }
            set { _onlineModLink = value; }
        }

        public string DescriptionLabel
        {
            get { return Description.Text; }
            set { Description.Text = value; }
        }

        public string CreditsLabel
        {
            get { return Credits.Text; }
            set { Credits.Text = value; }
        }

        public Font DescriptionFont
        {
            get { return Description.Font; }
            set { Description.Font = value; }
        }

        public Font CreditsFont
        {
            get { return Credits.Font; }
            set { Credits.Font = value; }
        }

        public Font ModLabelFont
        {
            get { return ModName.Font; }
            set { ModName.Font = value; }
        }
    }
}
