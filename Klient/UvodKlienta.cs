using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Klient;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Klient
{
    public partial class UvodKlienta : MaterialForm
    {
        public static ColorScheme Vzhled = new ColorScheme(Primary.LightBlue400, Primary.LightBlue900,
            Primary.Cyan100, Accent.LightBlue400, TextShade.WHITE);
        public static MaterialSkinManager.Themes Tema = MaterialSkinManager.Themes.LIGHT;
        public static string SlozkaSouboru = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Stercore soubory", "Klient");

        public static IPAddress AdresaServeru = null;
        public static int Port = 8888;
        public static string Prezdivka;

        public UvodKlienta()
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
        }

        /// <summary>
        ///     Zkontroluje údaje a pokusí se připojit na server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPripojit_Click(object sender, EventArgs e)
        {
            if (TxtPrezdivka.Text.Length != 0 && TxtPrezdivka.Text.Length <= 30 &&
                !string.IsNullOrWhiteSpace(TxtPrezdivka.Text))
            {
                try
                {
                    AdresaServeru = IPAddress.Parse(TxtIP.Text);
                    Prezdivka = TxtPrezdivka.Text;

                    if (ChckUlozitNast.Checked)
                    {
                        UlozeniNastaveni();
                    }

                    var okno = new OknoKlienta();
                    Hide();
                    okno.ShowDialog();

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
            if (File.Exists(SlozkaSouboru + "\\Nastaveni.txt"))
            {
                TxtPrezdivka.Text = Prezdivka;
                TxtIP.Text = AdresaServeru.ToString();
            }

            TxtPrezdivka.Focus();
            TxtPrezdivka.SelectAll();
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

        private void UlozeniNastaveni()
        {
            using (StreamWriter Zapis = new StreamWriter(SlozkaSouboru + "\\Nastaveni.txt"))
            {
                Zapis.WriteLine("IP Adresa: " + AdresaServeru);
                Zapis.WriteLine("Port: " + Port);
                Zapis.WriteLine("Téma: " + Tema);
                Zapis.WriteLine("Přezdívka: " + Prezdivka);
            }
        }

        private void NacistNastaveni()
        {
            using (StreamReader Cteni = new StreamReader(SlozkaSouboru + "\\Nastaveni.txt"))
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
                Prezdivka = Radek[1].Trim();
            }
        }

        private void BtnRozsNastaveni_Click(object sender, EventArgs e)
        {
            var Okno = new RozsNastaveni();

            Okno.ShowDialog();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Tema;
            materialSkinManager.ColorScheme = Vzhled;
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