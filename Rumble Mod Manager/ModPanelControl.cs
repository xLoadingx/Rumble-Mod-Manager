﻿using System;
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

        public string ModNameLabel
        {
            get { return ModName.Text; }
            set { ModName.Text = value; }
        }

        public string DetailsLabel
        {
            get { return label2.Text; }
            set { label2.Text = value; }
        }

        public Font ModLabelFont
        {
            get { return ModName.Font; }
            set { ModName.Font = value; }
        }

        public Font DetailsLabelFont
        {
            get { return label2.Font; }
            set { label2.Font = value; }
        }

        public Image UpdateNeededImage
        {
            get { return ImageButton.Image; }
            set { ImageButton.Image = value; }
        }

        public Color UpdateColor
        {
            get { return ImageButton.ForeColor; }
            set { ImageButton.BackColor = value; }
        }

        //public string toolTip1Text
        //{
        //    get { return toolTip1.GetToolTip(Details); }
        //    set { toolTip1.SetToolTip(Details, value); toolTip1.SetToolTip(pictureBox1, value); }
        //}

        public Image ModImage
        {
            get { return guna2PictureBox1.Image; }
            set { guna2PictureBox1.Image = value; }
        }

        public bool ModEnabled
        {
            get { return _modEnabled; }
            set
            {
                _modEnabled = value;
                this.ForeColor = _modEnabled ? Color.White : Color.Maroon; this.BackColor = value == true ? Color.FromArgb(255, 30, 30, 30) : Color.FromArgb(255, 192, 0, 0);
            }
        }

        public string ModDllPath
        {
            get { return _modDLLPath; }
            set { _modDLLPath = value; }
        }
    }
}
