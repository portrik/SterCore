using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace SterCore
{
    public partial class OknoKlienta : MaterialForm
    {
        private readonly IPEndPoint Adresa;
        private readonly string Prezdivka;

        private readonly string Slozka = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Stercore soubory");

        private TcpClient Komunikace;
        private NetworkStream Prijem, Odesilani = default(NetworkStream);
        private Thread Prijmani;

        public OknoKlienta()
        {
            InitializeComponent();
        }

        public OknoKlienta(string Jmeno, IPEndPoint AdresaServeru)
        {
            InitializeComponent();

            Prezdivka = Jmeno;
            Adresa = AdresaServeru;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue400, Primary.LightBlue900,
                Primary.Cyan100, Accent.LightBlue400, TextShade.WHITE);

            Pripojeni();
        }

        /// <summary>
        ///     Připojí se na server podle zadané adresy.
        /// </summary>
        public void Pripojeni()
        {
            Komunikace = new TcpClient();

            try
            {
                Komunikace.Connect(Adresa); //Pokus o připojení na zadanou adresu a port

                if (Komunikace.Connected)
                {
                    Odesilani = Komunikace.GetStream(); //Nastavení proudu na adresu
                    VypisChatu.Text += DateTime.Now.ToShortTimeString() + " " + "Připojení bylo úspěšné!";
                    var Jmeno = Encoding.Unicode.GetBytes(Prezdivka); //Převedení přezdívky na byty
                    Odesilani.Write(Jmeno, 0, Jmeno.Length); //Odeslání přezdívky
                    Odesilani.Flush(); //Vyprázdnění proudu

                    Prijmani = new Thread(PrijmaniZprav)
                    {
                        IsBackground = true
                    }; //Nastaví thread pro přijímání zpráv a nastaví jej do pozadí

                    Prijmani.Start(); //Zapnutí poslouchání zpráv
                }
            }
            catch (Exception x)
            {
                VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + "Objevila se chyba: ";
                VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message;
            }
        }

        /// <summary>
        ///     Přijímá zprávy od serveru.
        /// </summary>
        private void PrijmaniZprav()
        {
            try
            {
                using (Prijem = Komunikace.GetStream())
                {
                    while (Komunikace.Connected)
                    {
                        var Data = new byte[1024 * 1024 * 4]; //Pole pro příjem sériových dat
                        var Znak = new byte[3];

                        Prijem.Read(Data, 0, Komunikace.ReceiveBufferSize); //Načtení sériových dat

                        Array.Copy(Data, Znak, 3);

                        var Uprava = Encoding.Unicode.GetString(Znak);

                        switch (Uprava[0])
                        {
                            case '0': //Běžná zpráva
                            {
                                var Dekodovani = Encoding.Unicode.GetString(Data).TrimEnd('\0');
                                var Zprava = Dekodovani.Split('φ');
                                Vypsani(Zprava[1] + Zprava[2]);
                                break;
                            }
                            case '1': //TODO: Zpracování obrázku
                            {
                                ZpracovaniSouboru(Data, "Obrazek");

                                break;
                            }
                            case '2': //TODO: Zpracování souboru
                            {
                                ZpracovaniSouboru(Data, "Soubor");

                                break;
                            }
                            case '3': //TODO: Seznam klientů
                            {
                                var Dekodovani = Encoding.Unicode.GetString(Data).TrimEnd('\0');
                                var Seznam = Dekodovani.Split('φ');
                                var Jmena = Seznam[1].Split(',');

                                foreach (var Jmeno in Jmena)
                                    if (InvokeRequired)
                                        Invoke((MethodInvoker) (() => LstPripojeni.Items.Add(Jmeno)));

                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
                MessageBox.Show(x.StackTrace);
                Vypsani("Spojení bylo ukončeno");
                UvodKlienta.ZmenaUdaju = true;

                if (InvokeRequired) Invoke((MethodInvoker) (() => Close()));

                Prijmani.Join();
            }
        }

        /// <summary>
        ///     Vypíše přijatou zprávu do okna chatu.
        /// </summary>
        /// <param name="Text">Přijatá zpráva</param>
        private void Vypsani(string Text)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker) (() => Vypsani(Text)));
            else
                VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + Text;
        }

        /// <summary>
        ///     Zjistí, zda zadaný adresář existuje.
        /// </summary>
        /// <param name="Cesta">Cesta k adresáři</param>
        /// <returns>True - složka existuje, False - složka neexistuje</returns>
        private bool SlozkaSouboru(string Cesta)
        {
            return Directory.Exists(Cesta);
        }

        /// <summary>
        ///     Uloží přijatý soubor do příslušné složky.
        /// </summary>
        /// <param name="Data">Přijatá data souboru</param>
        /// <param name="Druh">Určuje, zda se jedná o obrázek nebo o soubor jiného druhu.</param>
        private void ZpracovaniSouboru(byte[] Data, string Druh)
        {
            var Nazev = new byte[300];

            Array.Copy(Data, 0, Nazev, 0, 300);

            var Prevod = Encoding.Unicode.GetString(Nazev).TrimEnd('\0');
            var NazevSouboru = Prevod.Split('φ');
            var DelkaSouboru = int.Parse(NazevSouboru[3]);
            var SlozkaServer = Path.Combine(Slozka, "Klient");
            var SlozkaDruh = Path.Combine(SlozkaServer, Druh);
            var Soubor = new byte[DelkaSouboru];

            Array.Copy(Data, 300, Soubor, 0, DelkaSouboru);

            if (!SlozkaSouboru(Slozka)) Directory.CreateDirectory(Slozka);

            if (!SlozkaSouboru(SlozkaServer)) Directory.CreateDirectory(SlozkaServer);

            if (!SlozkaSouboru(SlozkaDruh)) Directory.CreateDirectory(SlozkaDruh);

            var Cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "Stercore soubory", "Klient", Druh) + @"\" + NazevSouboru[1] + NazevSouboru[2];

            MessageBox.Show(Cesta);
            MessageBox.Show(DelkaSouboru.ToString());
            MessageBox.Show(Data.Length.ToString());

            if (File.Exists(Cesta))
            {
                var Index = 1;

                while (File.Exists(Cesta))
                {
                    Cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                "Stercore soubory", "Klient", Druh) + @"\" + NazevSouboru[1] + "(" + Index + ")" +
                            NazevSouboru[2];
                    ++Index;
                }
            }

            using (var UlozeniSoubour = new MemoryStream(Soubor))
            {
                File.WriteAllBytes(Cesta, Soubor);
            }
        }

        /// <summary>
        ///     Odešle žádost o odpojení na server a otevře okno se zadáváním údajů.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOdpojit_Click(object sender, EventArgs e)
        {
            var Zprava = Encoding.Unicode.GetBytes("4φ"); //Převedení zprávy na sériová data
            Odesilani.Write(Zprava, 0, Zprava.Length); //Odeslání sériových dat
            Odesilani.Flush(); //Vyprázdnění proudu

            UvodKlienta.ZmenaUdaju = true;
            Close();
        }

        /// <summary>
        ///     Odešle zprávu na server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOdeslat_Click(object sender, EventArgs e) //Odeslání zprávy
        {
            if (!string.IsNullOrWhiteSpace(TxtZprava.Text))
            {
                var Zprava = Encoding.Unicode.GetBytes("0φ" + TxtZprava.Text.Trim()); //Převedení zprávy na sériová data
                Odesilani.Write(Zprava, 0, Zprava.Length); //Odeslání sériových dat
                Odesilani.Flush(); //Vyprázdnění proudu

                TxtZprava.Text = null; //Vyprázdnění textového pole
                TxtZprava.Focus();
                TxtZprava.SelectAll();
            }
        }

        /// <summary>
        ///     Zjednodušení odeslání zprávy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtZprava_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter) BtnOdeslat_Click(null, null);
        }

        private void OdeslaniObrazku_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = "Obrázky|*.jpg;*.png;*.gif;*.jpeg;*.jpe;*.bmp";

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                var Obrazek = File.ReadAllBytes(VolbaSouboru.FileName);

                if (Obrazek.Length < 4194004)
                    try
                    {
                        var Nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) +
                                    Path.GetExtension(VolbaSouboru.FileName);
                        var MetaData = "1φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" +
                                       Path.GetExtension(VolbaSouboru.FileName) + "φ" + Obrazek.Length + "φ";
                        var Znacka = Encoding.Unicode.GetBytes(MetaData);
                        var Zprava = new byte[1024 * 1024 * 4];

                        Array.Copy(Znacka, 0, Zprava, 0, Znacka.Length);
                        Array.Copy(Obrazek, 0, Zprava, 300, Obrazek.Length);

                        VyslaniSouboru(Zprava);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.StackTrace);
                        MessageBox.Show(x.Message);
                    }
                else
                    MessageBox.Show("Zvolený soubor je pro přenos příliš velký!\nMaximum jsou 4 MB.", "Chyba!");
            }
        }

        /// <summary>
        ///     Při zapsání nové zprávy skočí na poslední zprávu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VypisChatu_TextChanged(object sender, EventArgs e)
        {
            VypisChatu.SelectionStart = VypisChatu.Text.Length;
            VypisChatu.ScrollToCaret();
        }

        /// <summary>
        ///     Po kliknutí na odkaz otevře webovou stránku v prohlížeči.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Odkaz webové stránky</param>
        private void VypisChatu_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void OknoKlienta_Load(object sender, EventArgs e)
        {
            VypisChatu.BackColor = Color.White;
        }

        private void OdeslatSoubor_Click(object sender, EventArgs e)
        {
            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                VolbaSouboru.Filter = null;

                if (VolbaSouboru.ShowDialog() == DialogResult.OK)
                {
                    var Soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                    if (Soubor.Length < 4194004)
                        try
                        {
                            var MetaData = "2φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" +
                                           Path.GetExtension(VolbaSouboru.FileName) + "φ" + Soubor.Length + "φ";
                            var Znacka = Encoding.Unicode.GetBytes(MetaData);
                            var Zprava = new byte[1024 * 1024 * 4];
                            MessageBox.Show(Soubor.Length.ToString());

                            Array.Copy(Znacka, 0, Zprava, 0, Znacka.Length);
                            Array.Copy(Soubor, 0, Zprava, 300, Soubor.Length);

                            VyslaniSouboru(Zprava);
                        }
                        catch (Exception x)
                        {
                            MessageBox.Show(x.StackTrace);
                            MessageBox.Show(x.Message);
                        }
                    else
                        MessageBox.Show("Zvolený soubor je pro přenos příliš velký!\nMaximum jsou 4 MB.", "Chyba!");
                }
            }
        }

        private void VyslaniSouboru(byte[] Data)
        {
            try
            {
                Odesilani.Write(Data, 0, Data.Length);
                Odesilani.Flush();
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " +
                                       "Objevila se chyba:"));
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
            }
        }
    }
}