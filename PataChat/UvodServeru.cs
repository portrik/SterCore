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
        }

        IPAddress AdresaServeru;
        int Port, PocetPripojeni;

        private void BtnServerStart_Click(object sender, EventArgs e)
        {
            try
            {
                AdresaServeru = IPAddress.Parse(txtServerIP.Text);
                Port = int.Parse(txtServerPort.Text);//Zadá hodnotu proměné z textboxu
                PocetPripojeni = int.Parse(txtPocetKlientu.Text);//Načte maximální počet klientů

                OknoServeru Okno = new OknoServeru(AdresaServeru, Port, PocetPripojeni);
                Hide();
                Okno.ShowDialog();
                Close();
            }
            catch
            {
                MessageBox.Show("Adresa IP nebo portu byla špatně napsána!", "Chyba!");
                txtServerIP.Focus();
                txtServerIP.SelectAll();
            }
        }

        private void UvodServeru_Load(object sender, EventArgs e)
        {
            AdresaServeru = LokalniAdresa();
            txtServerIP.Text = AdresaServeru.ToString(); //Nastaví lokální adresu do textboxu
            txtServerIP.Focus();
            txtServerIP.SelectAll();
        }

        public static IPAddress LokalniAdresa()//Získá lokální adresu serveru
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
        } //Zjistí lokální adresu klienta
    }
}
