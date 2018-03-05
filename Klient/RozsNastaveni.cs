using System;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Klient
{
    public partial class RozsNastaveni : MaterialForm
    {
        public RozsNastaveni()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = UvodKlienta.Tema;
            materialSkinManager.ColorScheme = UvodKlienta.Vzhled;

            if (UvodKlienta.Tema == MaterialSkinManager.Themes.LIGHT)
                RadioSv.Checked = true;
            else
                RadioTm.Checked = true;
        }

        private void BtnPotvrdit_Click(object sender, EventArgs e)
        {
            try
            {
                UvodKlienta.Port = int.Parse(TxtPort.Text);

                if (RadioSv.Checked)
                {
                    UvodKlienta.Tema = MaterialSkinManager.Themes.LIGHT;
                    UvodKlienta.Vzhled = new ColorScheme(Primary.LightBlue400, Primary.LightBlue900,
                        Primary.Cyan100, Accent.LightBlue400, TextShade.WHITE);
                }
                else if (RadioTm.Checked)
                {
                    UvodKlienta.Tema = MaterialSkinManager.Themes.DARK;
                    UvodKlienta.Vzhled = new ColorScheme(Primary.LightBlue400, Primary.LightBlue900,
                        Primary.Cyan100, Accent.LightBlue400, TextShade.WHITE);
                }

                Close();
            }
            catch
            {
                MessageBox.Show("Byla zadána neplatná hodnota portu!", "Chyba!");
                TxtPort.Focus();
                TxtPort.SelectAll();
            }
        }

        private void RozsNastaveni_Load(object sender, EventArgs e)
        {
            TxtPort.Text = UvodKlienta.Port.ToString();
        }

        private void BtnZrusit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}