using System;
using MaterialSkin;
using MaterialSkin.Controls;
using SterCore;

namespace PataChat
{
    public partial class RozsNastaveni : MaterialForm
    {
        public RozsNastaveni()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red700, Primary.Red900, Primary.Red100,
                Accent.Red400, TextShade.WHITE);
        }

        private void BtnPotvrdit_Click(object sender, EventArgs e)
        {
            UvodServeru.Port = int.Parse(txtServerPort.Text);

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
    }
}