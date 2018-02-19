using System;
using System.Net;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace SterCore
{
    public partial class UvodKlienta : MaterialForm
    {
        public static bool ZmenaUdaju;

        public UvodKlienta()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue400, Primary.LightBlue900,
                Primary.Cyan100, Accent.LightBlue400, TextShade.WHITE);
        }

        /// <summary>
        ///     Zkontroluje údaje a pokusí se připojit na server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPripojit_Click(object sender, EventArgs e)
        {
            ZmenaUdaju = false;

            if (TxtPrezdivka.Text.Length != 0 && TxtPrezdivka.Text.Length <= 30 &&
                !string.IsNullOrWhiteSpace(TxtPrezdivka.Text))
            {
                IPEndPoint AdresaServeru = null;

                try
                {
                    var IP = IPAddress.Parse(TxtIP.Text);
                    var Port = int.Parse(TxtPort.Text);
                    AdresaServeru = new IPEndPoint(IP, Port); //Zpracování adresy a portu
                    var Okno = new OknoKlienta(TxtPrezdivka.Text, AdresaServeru);
                    Hide();
                    Okno.ShowDialog();

                    if (!ZmenaUdaju)
                        Close();
                    else
                        Show();
                }
                catch
                {
                    MessageBox.Show("Adresa IP nebo portu byla špatně napsána!", "Chyba!");
                    TxtIP.Focus();
                    TxtIP.SelectAll();
                }
            }
            else
            {
                MessageBox.Show("Přezdívka se musí skládat ze znaků a nesmí být delší než 30 znaků!", "Chyba!");
                TxtPrezdivka.Focus();
                TxtPrezdivka.SelectAll();
            }
        }

        /// <summary>
        ///     Nastaví komponenty po načtení formuláře.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KlientUvod_Load(object sender, EventArgs e)
        {
            TxtPrezdivka.Focus();
            TxtPrezdivka.SelectAll();
            TxtPort.Enabled = false;
        }

        /// <summary>
        ///     Podle checkboxu dovolí změnu čísla portu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckPort_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckPort.Checked)
            {
                TxtPort.Enabled = true;
                TxtPort.TabStop = true;
            }
            else
            {
                TxtPort.Enabled = false;
                TxtPort.TabStop = false;
            }
        }

        /// <summary>
        ///     Zjednodušení zadávání údajů.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPrezdivka_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
            {
                TxtIP.Focus();
                TxtIP.SelectAll();
            }
        }

        /// <summary>
        ///     Zjednodušení připojení.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter) BtnPripojit_Click(null, null);
        }
    }
}