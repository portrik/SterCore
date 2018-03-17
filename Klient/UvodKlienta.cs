using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Klient
{
    public partial class UvodKlienta : MaterialForm
    {
        public static ColorScheme Vzhled = new ColorScheme(Primary.LightBlue400, Primary.LightBlue900,
            Primary.Cyan100, Accent.LightBlue400, TextShade.WHITE); //Barvy oken

        public static MaterialSkinManager.Themes Tema = MaterialSkinManager.Themes.LIGHT; //Vzhled oken

        public static string SlozkaSouboru = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Stercore soubory", "Klient"); //Cesta ke složce se soubory

        public static IPAddress AdresaServeru;
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

                    if (ChckUlozitNast.Checked) UlozeniNastaveni();

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

        /// <summary>
        ///     Uloží nastavení
        /// </summary>
        private void UlozeniNastaveni()
        {
            using (var zapis = new StreamWriter(SlozkaSouboru + "\\Nastaveni.txt"))
            {
                zapis.WriteLine("IP Adresa: " + AdresaServeru);
                zapis.WriteLine("Port: " + Port);
                zapis.WriteLine("Téma: " + Tema);
                zapis.WriteLine("Přezdívka: " + Prezdivka);
            }
        }

        /// <summary>
        ///     Nastaví program podle uložených nastavení.
        ///     Pokud nastavení nelze načíst, načte základní hodnoty.
        /// </summary>
        private void NacistNastaveni()
        {
            using (var cteni = new StreamReader(SlozkaSouboru + "\\Nastaveni.txt"))
            {
                var radek = cteni.ReadLine().Split(':');
                AdresaServeru = IPAddress.Parse(radek[1].Trim());
                radek = cteni.ReadLine().Split(':');
                Port = int.Parse(radek[1].Trim());
                radek = cteni.ReadLine().Split(':');

                if (radek[1].Trim() == "LIGHT")
                    Tema = MaterialSkinManager.Themes.LIGHT;
                else
                    Tema = MaterialSkinManager.Themes.DARK;

                radek = cteni.ReadLine().Split(':');
                Prezdivka = radek[1].Trim();
            }
        }

        /// <summary>
        ///     Otevře okno s rozšířeným nastavením
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRozsNastaveni_Click(object sender, EventArgs e)
        {
            var okno = new RozsNastaveni();

            okno.ShowDialog();

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