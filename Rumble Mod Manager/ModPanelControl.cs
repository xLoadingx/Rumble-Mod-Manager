using System.Diagnostics.CodeAnalysis;

namespace Rumble_Mod_Manager
{
    public partial class ModPanelControl : UserControl
    {
        private bool _modEnabled = true;
        private bool _outdated = false;
        private string _modDLLPath = string.Empty;
        private string _versionString = string.Empty;
        private string _onlineModLink = string.Empty;
        private long _fileSize = 0;
        int fixedRight;
        public ThunderstoreMods.Mod Mod { get; set; }


        public ModPanelControl()
        {
            InitializeComponent();
            fixedRight = this.Right;
        }

        public string ModNameLabel
        {
            get { return ModName.Text; }
            set { ModName.Text = value; }
        }

        public string VersionString
        {
            get { return _versionString; }
            set { _versionString = value; }
        }

        public string DetailsLabel
        {
            get { return label2.Text; }
            set { label2.Text = value; }
        }

        public string FileSizeLabel
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        public Font FileSizeFont
        {
            get { return label1.Font; }
            set { label1.Font = value; }
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

        public bool Outdated
        {
            get { return _outdated; }
            set { _outdated = value; }
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
                this.label1.ForeColor = _modEnabled ? Color.DimGray : Color.White;
            }
        }

        public string ModDllPath
        {
            get { return _modDLLPath; }
            set { _modDLLPath = value; }
        }

        public string OnlineModLink
        {
            get { return _onlineModLink; }
            set { _onlineModLink = value; }
        }

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }
    }
}
