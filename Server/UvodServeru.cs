using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Server
{
    public partial class UvodServeru : MaterialForm
    {
        public static int Port = 8888, PocetPripojeni;
        public static bool ZmenaUdaju;
        private IPAddress AdresaServeru;

        public static ColorScheme Vzhled = new ColorScheme(Primary.Red700, Primary.Red900, Primary.Red100,
            Accent.Red400, TextShade.WHITE);
        public static MaterialSkinManager.Themes Tema = MaterialSkinManager.Themes.LIGHT;

        public UvodServeru()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stercore soubory", "Server") + "\\Nastaveni.txt"))
            {
                NacistNastaveni();
                materialSkinManager.Theme = Tema;
            }
            else
            {
                materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            }

            materialSkinManager.ColorScheme = Vzhled;
        }

        /// <summary>
        ///     Zkontroluje zadané údaje a spustí server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnServerStart_Click(object sender, EventArgs e)
        {
            ZmenaUdaju = false;

            try
            {
                AdresaServeru = IPAddress.Parse(txtServerIP.Text);
                PocetPripojeni = int.Parse(txtPocetKlientu.Text); //Načte maximální počet klientů

                if(ChckUlozitNast.Checked)
                {
                    UlozeniNastaveni();
                }

                var Okno = new OknoServeru();
                Hide();
                Okno.ShowDialog();

                if (!ZmenaUdaju)
                    Close();
                else
                    Show();
            }
            catch
            {
                /*MessageBox.Show("Adresa IP nebo portu byla špatně napsána!", "Chyba!");
                txtServerIP.Focus();
                txtServerIP.SelectAll();*/
            }
        }

        /// <summary>
        ///     Načte hodnoty po spuštění formuláře.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UvodServeru_Load(object sender, EventArgs e)
        { 
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stercore soubory", "Server") + "\\Nastaveni.txt"))
            {
                txtServerIP.Text = AdresaServeru.ToString(); //Nastaví lokální adresu do textboxu
                txtPocetKlientu.Text = PocetPripojeni.ToString();
            }
            else
            {
                AdresaServeru = LokalniAdresa();
            }

            txtServerIP.Focus();
            txtServerIP.SelectAll();
        }

        /// <summary>
        ///     Zjednodušení startu serveru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartServer_Enter(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter) BtnServerStart_Click(null, null);
        }

        private void BtnRozsNastaveni_click(object sender, EventArgs e)
        {
            var Okno = new RozsNastaveni();

            Okno.ShowDialog();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Tema;
            materialSkinManager.ColorScheme = Vzhled;
        }

        /// <summary>
        ///     Získá lokální adresu zařízení.
        /// </summary>
        /// <returns>Lokální adresa</returns>
        public static IPAddress LokalniAdresa()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var IP in host.AddressList)
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                    return IP;

            return IPAddress.Parse("127.0.0.1"); //Pokud není počítač připojen k síti, vrátí loopback adresu
        }

        private void UlozeniNastaveni()
        {
            using (StreamWriter Zapis = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stercore soubory", "Server") + "\\Nastaveni.txt"))
            {
                Zapis.WriteLine("IP Adresa: " + AdresaServeru);
                Zapis.WriteLine("Port: " + Port);
                Zapis.WriteLine("Téma: " + Tema);
                Zapis.WriteLine("Počet klientů: " + PocetPripojeni);
            }
        }

        private void NacistNastaveni()
        {
            using (StreamReader Cteni = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Stercore soubory", "Server") + "\\Nastaveni.txt"))
            {
                string[] Radek = Cteni.ReadLine().Split(':');
                AdresaServeru = IPAddress.Parse(Radek[1].Trim());
                Radek = Cteni.ReadLine().Split(':');
                Port = int.Parse(Radek[1].Trim());
                Radek = Cteni.ReadLine().Split(':');

                if (Radek[1].Trim() == "LIGHT")
                {
                    Tema = MaterialSkinManager.Themes.LIGHT;
                }
                else
                {
                    Tema = MaterialSkinManager.Themes.DARK;
                }

                Radek = Cteni.ReadLine().Split(':');
                PocetPripojeni = int.Parse(Radek[1].Trim());
            }
        }
    }
}