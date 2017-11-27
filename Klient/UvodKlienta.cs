using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Animations;
using MaterialSkin.Controls;
using System.Net;

namespace SterCore
{
    public partial class UvodKlienta : MaterialForm
    {
        public UvodKlienta()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        public static bool ZmenaUdaju;       

        /// <summary>
        /// Zkontroluje údaje a pokusí se připojit na server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPripojit_Click(object sender, EventArgs e)
        {
            ZmenaUdaju = false;

            if (TxtPrezdivka.Text.Length != 0 && TxtPrezdivka.Text.Length <= 30 && !string.IsNullOrWhiteSpace(TxtPrezdivka.Text))
            {
                IPEndPoint AdresaServeru = null;

                try
                {
                    IPAddress IP = IPAddress.Parse(TxtIP.Text);
                    int Port = int.Parse(TxtPort.Text);
                    AdresaServeru = new IPEndPoint(IP, Port);//Zpracování adresy a portu
                    OknoKlienta Okno = new OknoKlienta(TxtPrezdivka.Text, AdresaServeru);
                    Hide();
                    Okno.ShowDialog();

                    if (!ZmenaUdaju)
                    {
                        Close();
                    }
                    else
                    {
                        Show();
                    }                    
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
        /// Nastaví komponenty po načtení formuláře.
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
        /// Podle checkboxu dovolí změnu čísla portu.
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
        /// Zjednodušení zadávání údajů.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPrezdivka_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                TxtIP.Focus();
                TxtIP.SelectAll();
            }
        }

        /// <summary>
        /// Zjednodušení připojení.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnPripojit_Click(null, null);
            }
        }
    }
}
