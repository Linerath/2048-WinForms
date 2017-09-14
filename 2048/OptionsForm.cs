using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace _2048
{
    public partial class OptionsForm : Form
    {
        private bool options = false;
        private MainForm mf;

        public OptionsForm()
        {
            InitializeComponent();
        }
        public OptionsForm(Int32 matrixRows, Int32 matrixCells, Size tileSize, Int32 Int32ervalBetweenTiles, Int32 borderInt32erval, Color backColor)
        {
            InitializeComponent();

            nudRows.Value = matrixRows;
            nudCells.Value = matrixCells;
            nudTileSize.Value = tileSize.Width;
            nudInterval1.Value = Int32ervalBetweenTiles;
            nudInterval2.Value = borderInt32erval;
            pColor.BackColor = backColor;
        }

        private void OnOptions()
        {
            options = true;
            Show();
            if (mf != null) mf.Enabled = false;
        }
        private void ReadSettings()
        {
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream("data.2048", FileMode.OpenOrCreate)))
                {
                    nudRows.Value = br.ReadInt32();
                    nudCells.Value = br.ReadInt32();
                    nudTileSize.Value = br.ReadInt32();
                    nudInterval1.Value = br.ReadInt32();
                    nudInterval2.Value = br.ReadInt32();
                    cbEllipse.Checked = br.ReadBoolean();
                    pColor.BackColor = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка чтения файла!", "Error");
            }
            catch
            {
                MessageBox.Show("Ошибка!", "Error");
            }
        }

        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (options)
            {
                if (mf!=null)
                    mf.Enabled = true;
                e.Cancel = true;
                options = false;
                Hide();
            }
        }
        private void StartForm_Load(object sender, EventArgs e)
        {
            ReadSettings();
        }
        private void bOK_Click(object sender, EventArgs e)
        {
            Size tileSize = new Size(Convert.ToInt32(nudTileSize.Value), Convert.ToInt32(nudTileSize.Value));
            Int32 rows = Convert.ToInt32(nudRows.Value);
            Int32 cells = Convert.ToInt32(nudCells.Value);
            Int32 borderInt32erval = Convert.ToInt32(nudInterval2.Value);
            Int32 Int32erval = Convert.ToInt32(nudInterval1.Value);

            if (mf != null) mf.Close();
            mf = new MainForm(rows, cells, tileSize, Int32erval, borderInt32erval, cbEllipse.Checked, pColor.BackColor);
            Hide();
            mf.Show();
            mf.OptionsEvent += OnOptions;
        }
        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void pColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog(this);
            if (cd.Color != null)
            {
                pColor.BackColor = cd.Color;
                if (mf!=null)
                    mf.BackColor = cd.Color;
            }
        }
        private void OptionsForm_MouseDown(object sender, MouseEventArgs e)
        {
            Capture = false;
            Message m = Message.Create(this.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref m);
        }
    }
}