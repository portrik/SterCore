using System;
using System.Drawing;
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
        public static int Port, PocetPripojeni, Kontrola;
        public static bool UlozeniHistorie;

        public static string SlozkaSouboru = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Stercore soubory", "Server");

        public static ColorScheme Vzhled = new ColorScheme(Primary.Red700, Primary.Red900, Primary.Red100,
            Accent.Red400, TextShade.WHITE);

        public static MaterialSkinManager.Themes Tema = MaterialSkinManager.Themes.LIGHT;

        private IPAddress _adresaServeru;

        /// <summary>
        ///     Po spuštění načte nastavení.
        /// </summary>
        public UvodServeru()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            if (File.Exists(SlozkaSouboru + "\\Nastaveni.txt"))
            {
                NacistNastaveni();
                materialSkinManager.Theme = Tema;
            }
            else
            {
                materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            }

            materialSkinManager.ColorScheme = Vzhled;

            _adresaServeru = LokalniAdresa();
        }

        /// <summary>
        ///     Zkontroluje zadané údaje a spustí server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnServerStart_Click(object sender, EventArgs e)
        {
            try
            {
                _adresaServeru = IPAddress.Parse(txtServerIP.Text);
                PocetPripojeni = int.Parse(txtPocetKlientu.Text); //Načte maximální počet klientů

                if (ChckUlozitNast.Checked) UlozeniNastaveni();

                var okno = new OknoServeru();
                Hide();
                okno.ShowDialog();
                Show();
            }
            catch
            {
                MessageBox.Show("Adresa IP nebo portu byla špatně napsána!", "Chyba!");
                txtServerIP.Focus();
                txtServerIP.SelectAll();
            }
        }

        /// <summary>
        ///     Načte hodnoty po spuštění formuláře.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UvodServeru_Load(object sender, EventArgs e)
        {
            if (File.Exists(SlozkaSouboru + "\\Nastaveni.txt"))
            {
                txtPocetKlientu.Text = PocetPripojeni.ToString();
            }
            else
            {
                ZakladniNastaveni();
            }

            txtServerIP.Enabled = false;
            txtServerIP.Text = _adresaServeru.ToString();
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

        /// <summary>
        ///     Otevře okno s rozšířeným nastavením.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRozsNastaveni_click(object sender, EventArgs e)
        {
            var okno = new RozsNastaveni();

            okno.ShowDialog();

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
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;

            return IPAddress.Parse("127.0.0.1"); //Pokud není počítač připojen k síti, vrátí loopback adresu
        }

        /// <summary>
        ///     Uloží nastavení do souboru.
        /// </summary>
        private void UlozeniNastaveni()
        {
            if (!SlozkaExistuje(SlozkaSouboru)) Directory.CreateDirectory(SlozkaSouboru);
            using (var zapis = new StreamWriter(SlozkaSouboru + "\\Nastaveni.txt"))
            {
                zapis.WriteLine("Port: " + Port);
                zapis.WriteLine("Téma: " + Tema);
                zapis.WriteLine("Počet klientů: " + PocetPripojeni);
                zapis.WriteLine("Uložit historii: " + UlozeniHistorie);
                zapis.WriteLine("Rychlost kontroly: " + Kontrola);
            }
        }

        /// <summary>
        ///     Načte nastavení z uloženého souboru. V případě poškození načte základní nastavení.
        /// </summary>
        private void NacistNastaveni()
        {
            using (var cteni = new StreamReader(SlozkaSouboru + "\\Nastaveni.txt"))
            {
                try
                {
                    var radek = cteni.ReadLine().Split(':');
                    Port = int.Parse(radek[1].Trim());

                    radek = cteni.ReadLine().Split(':');

                    if (radek[1].Trim() == "LIGHT")
                        Tema = MaterialSkinManager.Themes.LIGHT;
                    else
                        Tema = MaterialSkinManager.Themes.DARK;

                    radek = cteni.ReadLine().Split(':');
                    PocetPripojeni = int.Parse(radek[1].Trim());

                    radek = cteni.ReadLine().Split(':');
                    UlozeniHistorie = bool.Parse(radek[1].Trim());

                    radek = cteni.ReadLine().Split(':');
                    Kontrola = int.Parse(radek[1].Trim());
                }
                catch
                {
                    ZakladniNastaveni();
                }
            }
        }

        /// <summary>
        ///     Načte základní nastavení.
        /// </summary>
        private void ZakladniNastaveni()
        {
            Port = 8888;
            PocetPripojeni = 0;
            txtPocetKlientu.Text = PocetPripojeni.ToString();
            Tema = MaterialSkinManager.Themes.LIGHT;
            UlozeniHistorie = true;
            Kontrola = 100;
        }

        /// <summary>
        ///     Zjistí, zda zadaný adresář existuje.
        /// </summary>
        /// <param name="cesta">Cesta k adresáři</param>
        /// <returns>True - složka existuje, False - složka neexistuje</returns>
        public static bool SlozkaExistuje(string cesta)
        {
            return Directory.Exists(cesta);
        }
    }
}