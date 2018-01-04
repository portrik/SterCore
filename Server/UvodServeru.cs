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
using System.Net.Sockets;

namespace SterCore
{
    public partial class UvodServeru : MaterialForm
    {
        public UvodServeru()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red700, Primary.Red900, Primary.Red100, Accent.Red400, TextShade.WHITE);
        }

        IPAddress AdresaServeru;
        int Port, PocetPripojeni;
        public static bool ZmenaUdaju;

        /// <summary>
        /// Zkontroluje zadané údaje a spustí server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnServerStart_Click(object sender, EventArgs e)
        {
            ZmenaUdaju = false;

            try
            {
                AdresaServeru = IPAddress.Parse(txtServerIP.Text);
                Port = int.Parse(txtServerPort.Text);//Zadá hodnotu proměné z textboxu
                PocetPripojeni = int.Parse(txtPocetKlientu.Text);//Načte maximální počet klientů

                OknoServeru Okno = new OknoServeru(AdresaServeru, Port, PocetPripojeni);
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
                /*MessageBox.Show("Adresa IP nebo portu byla špatně napsána!", "Chyba!");
                txtServerIP.Focus();
                txtServerIP.SelectAll();*/
            }
        }

        /// <summary>
        /// Načte hodnoty po spuštění formuláře.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UvodServeru_Load(object sender, EventArgs e)
        {
            AdresaServeru = LokalniAdresa();
            txtServerIP.Text = AdresaServeru.ToString(); //Nastaví lokální adresu do textboxu
            txtServerIP.Focus();
            txtServerIP.SelectAll();
        }

        /// <summary>
        /// Zjednodušení startu serveru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartServer_Enter(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnServerStart_Click(null, null);
            }
        }

        /// <summary>
        /// Získá lokální adresu zařízení.
        /// </summary>
        /// <returns>Lokální adresa</returns>
        public static IPAddress LokalniAdresa()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var IP in host.AddressList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                {
                    return IP;
                }
            }

            return IPAddress.Parse("127.0.0.1"); //Pokud není počítač připojen k síti, vrátí loopback adresu
        }
    }
}
