using System;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Server
{
    public partial class RozsNastaveni : MaterialForm
    {
        public RozsNastaveni()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = UvodServeru.Tema;
            materialSkinManager.ColorScheme = UvodServeru.Vzhled;

            if (UvodServeru.Tema == MaterialSkinManager.Themes.LIGHT)
                RadioSv.Checked = true;
            else
                RadioTm.Checked = true;
        }

        private void BtnPotvrdit_Click(object sender, EventArgs e)
        {
            try
            {
                UvodServeru.Port = int.Parse(txtServerPort.Text);
                UvodServeru.UlozeniHistorie = ChckUlozitHistorii.Checked;
                UvodServeru.Kontrola = int.Parse(TxtKontrola.Text);

                if (RadioSv.Checked)
                {
                    UvodServeru.Tema = MaterialSkinManager.Themes.LIGHT;
                    UvodServeru.Vzhled = new ColorScheme(Primary.Red700, Primary.Red900, Primary.Red100, Accent.Red400,
                        TextShade.WHITE);
                }
                else if (RadioTm.Checked)
                {
                    UvodServeru.Tema = MaterialSkinManager.Themes.DARK;
                    UvodServeru.Vzhled = new ColorScheme(Primary.Red700, Primary.Red900, Primary.Red100, Accent.Red400,
                        TextShade.WHITE);
                }

                Close();
            }
            catch
            {
                MessageBox.Show("Byla zadána neplatná hodnota portu!", "Chyba!");
                txtServerPort.Focus();
                txtServerPort.SelectAll();
            }
        }

        private void RozsNastaveni_Load(object sender, EventArgs e)
        {
            txtServerPort.Text = UvodServeru.Port.ToString();
            ChckUlozitHistorii.Checked = UvodServeru.UlozeniHistorie;
            TxtKontrola.Text = UvodServeru.Kontrola.ToString();
        }

        private void BtnZrusit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}