using System;
using System.Collections;
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

namespace Klient
{
    public partial class OknoKlienta : MaterialForm
    {
        private readonly IPEndPoint _adresa;
        private readonly string _prezdivka;

        private readonly string _slozka = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Stercore soubory");

        private TcpClient _komunikace;
        private NetworkStream _prijem, _odesilani = default(NetworkStream);
        private Thread _prijmani;
        private byte[] _obrazek;
        private bool _prijemObrazku;
        private int _pozice;
 
        public OknoKlienta()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = UvodKlienta.Tema;
            materialSkinManager.ColorScheme = UvodKlienta.Vzhled;

            _prezdivka = UvodKlienta.Prezdivka;
            _adresa = new IPEndPoint(UvodKlienta.AdresaServeru, UvodKlienta.Port);
        }

        /// <summary>
        ///     Připojí se na server podle zadané adresy.
        /// </summary>
        public void Pripojeni()
        {
            _komunikace = new TcpClient();
            bool pripojeno = false;

            try
            {
                _komunikace.Connect(_adresa); //Pokus o připojení na zadanou adresu a port

                if (_komunikace.Connected)
                {
                    _odesilani = _komunikace.GetStream(); //Nastavení proudu na adresu
                    VypisChatu.Text += DateTime.Now.ToShortTimeString() + " Připojení bylo úspěšné!";
                    var Jmeno = Encoding.Unicode.GetBytes(_prezdivka); //Převedení přezdívky na byty
                    _odesilani.Write(Jmeno, 0, Jmeno.Length); //Odeslání přezdívky
                    _odesilani.Flush(); //Vyprázdnění proudu

                    pripojeno = true;

                    _prijmani = new Thread(PrijmaniZprav)
                    {
                        IsBackground = true
                    }; //Nastaví thread pro přijímání zpráv a nastaví jej do pozadí

                    _prijmani.Start(); //Zapnutí poslouchání zpráv
                }
            }
            catch
            {
                if (!pripojeno)
                {
                    MessageBox.Show("K zadanému serveru se nepodařilo připojit.", "Chyba");
                }
                Close();
            }
        }

        /// <summary>
        ///     Přijímá zprávy od serveru.
        /// </summary>
        private void PrijmaniZprav()
        {
            try
            {
                using (_prijem = _komunikace.GetStream())
                {
                    while (_komunikace.Connected)
                    {
                        if (_prijem.CanRead)
                        {
                            var data = new byte[1024 * 1024 * 4]; //Pole pro příjem sériových dat
                            var znak = new byte[3];
                            var potvrzeni = Encoding.Unicode.GetBytes("9φ");

                            _prijem.Read(data, 0, _komunikace.ReceiveBufferSize); //Načtení sériových dat

                            Array.Copy(data, znak, 3);

                            var uprava = Encoding.Unicode.GetString(znak);

                            switch (uprava[0])
                            {
                                case '0': //Běžná zpráva
                                {
                                    var dekodovani = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                    var zprava = dekodovani.Split('φ');
                                    Vypsani(zprava[1] + zprava[2]);
                                    _odesilani.Write(potvrzeni, 0, potvrzeni.Length);
                                   break;
                                }
                                case '1': //TODO: Zpracování obrázku
                                {
                                    ZpracovaniSouboru(data, "Obrazek");
                                    _odesilani.Write(potvrzeni, 0, potvrzeni.Length);
                                    break;
                                }
                                case '2': //TODO: Zpracování souboru
                                {
                                    ZpracovaniSouboru(data, "Soubor");
                                    _odesilani.Write(potvrzeni, 0, potvrzeni.Length);
                                    break;
                                }
                                case '3': //TODO: Seznam klientů
                                {
                                    var dekodovani = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                    var seznam = dekodovani.Split('φ');
                                    var jmena = seznam[1].Split(',');

                                    foreach (var jmeno in jmena)
                                        if (InvokeRequired)
                                            Invoke((MethodInvoker)(() => LstPripojeni.Items.Add(jmeno)));
                                    _odesilani.Write(potvrzeni, 0, potvrzeni.Length);
                                    break;
                                }
                                case '4'://Odpojení 
                                {
                                    throw new ApplicationException();
                                }
                                case '5': //Historie
                                {
                                    var dekodovani = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                    var historie = dekodovani.Split('φ');
                                    VypisChatu.Text = historie[1].TrimEnd('\n');
                                    _odesilani.Write(potvrzeni, 0, potvrzeni.Length);
                                    break;
                                }
                                case '9': //Kontrola přijatých dat - klient nepřijímá
                                {
                                    break;
                                }
                            }

                            
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Server ukončil spojení.", "Konec spojení");

                if (InvokeRequired) Invoke((MethodInvoker) (() => Close()));

                _prijmani.Join();
            }
        }

        /// <summary>
        ///     Vypíše přijatou zprávu do okna chatu.
        /// </summary>
        /// <param name="zprava">Přijatá zpráva</param>
        private void Vypsani(string zprava)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker) (() => Vypsani(zprava)));
            else
                VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + zprava;
        }

        /// <summary>
        ///     Zjistí, zda zadaný adresář existuje.
        /// </summary>
        /// <param name="cesta">Cesta k adresáři</param>
        /// <returns>True - složka existuje, False - složka neexistuje</returns>
        private bool SlozkaSouboru(string cesta)
        {
            return Directory.Exists(cesta);
        }

        /// <summary>
        ///     Uloží přijatý soubor do příslušné složky.
        /// </summary>
        /// <param name="data">Přijatá data souboru</param>
        /// <param name="druh">Určuje, zda se jedná o obrázek nebo o soubor jiného druhu.</param>
        private void ZpracovaniSouboru(byte[] data, string druh)
        {
            byte[] metaData = new byte[128];
            Array.Copy(data, 0, metaData, 0, 128);
            string[] split = Encoding.Unicode.GetString(metaData).Split('φ');
            var velikostDat = Convert.ToInt32(split[2]);
            var delkaSouboru = Convert.ToInt32(split[3]);

            if (Convert.ToInt32(split[1]) >= 0)
            {
                if (!_prijemObrazku)
                {
                    _obrazek = new byte[delkaSouboru];
                    Array.Copy(data, 128, _obrazek, 0, velikostDat - 128);
                    _prijemObrazku = true;
                    _pozice += velikostDat;
                }
                else
                {
                    Array.Copy(data, 128, _obrazek, _pozice, velikostDat - 128);
                    _pozice += velikostDat;
                }
            }
            else
            {
                var slozkaServer = Path.Combine(_slozka, "Klient");
                var slozkaDruh = Path.Combine(slozkaServer, druh);

                if (!SlozkaSouboru(_slozka)) Directory.CreateDirectory(_slozka);

                if (!SlozkaSouboru(slozkaServer)) Directory.CreateDirectory(slozkaServer);

                if (!SlozkaSouboru(slozkaDruh)) Directory.CreateDirectory(slozkaDruh);


                var cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                "Stercore soubory", "Klient", druh) + @"\" + split[4] + split[5].Trim('\0');

                if (File.Exists(cesta))
                {
                    var index = 1;

                    while (File.Exists(cesta))
                    {
                        cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    "Stercore soubory", "Klient", druh) + @"\" + split[4] + "(" + index + ")" +
                                split[5].Trim('\0');
                        ++index;
                    }
                }

                File.WriteAllBytes(cesta, _obrazek);
                _obrazek = null;
                _prijemObrazku = false;
                _pozice = 0;
            }
        }

        /// <summary>
        ///     Odešle žádost o odpojení na server a otevře okno se zadáváním údajů.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOdpojit_Click(object sender, EventArgs e)
        {
            var zprava = Encoding.Unicode.GetBytes("4φ"); //Převedení zprávy na sériová data
            _odesilani.Write(zprava, 0, zprava.Length); //Odeslání sériových dat
            _odesilani.Flush(); //Vyprázdnění proudu

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
                var zprava = Encoding.Unicode.GetBytes("0φ" + TxtZprava.Text.Trim()); //Převedení zprávy na sériová data
                _odesilani.Write(zprava, 0, zprava.Length); //Odeslání sériových dat
                _odesilani.Flush(); //Vyprázdnění proudu

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
                var obrazek = File.ReadAllBytes(VolbaSouboru.FileName);

                if (obrazek.Length < 4194004)
                    try
                    {
                        var metaData = "1φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" +
                                       Path.GetExtension(VolbaSouboru.FileName) + "φ" + obrazek.Length + "φ";
                        var znacka = Encoding.Unicode.GetBytes(metaData);
                        var zprava = new byte[1024 * 1024 * 4];

                        Array.Copy(znacka, 0, zprava, 0, znacka.Length);
                        Array.Copy(obrazek, 0, zprava, 300, obrazek.Length);

                        VyslaniSouboru(zprava);
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
            if (UvodKlienta.Tema == MaterialSkinManager.Themes.LIGHT)
            {
                VypisChatu.BackColor = Color.White;
                VypisChatu.ForeColor = Color.Black;
                LstPripojeni.BackColor = Color.White;
                LstPripojeni.ForeColor = Color.Black;
            }
            else
            {
                VypisChatu.BackColor = ColorTranslator.FromHtml("#333333");
                VypisChatu.ForeColor = Color.White;
                LstPripojeni.BackColor = ColorTranslator.FromHtml("#333333");
                LstPripojeni.ForeColor = Color.White;
            }

            _prijemObrazku = false;
            _pozice = 0;
            Pripojeni();
        }

        private void OdeslatSoubor_Click(object sender, EventArgs e)
        {
            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                VolbaSouboru.Filter = null;

                if (VolbaSouboru.ShowDialog() == DialogResult.OK)
                {
                    var soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                    if (soubor.Length < 4194004)
                        try
                        {
                            var metaData = "2φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" +
                                           Path.GetExtension(VolbaSouboru.FileName) + "φ" + soubor.Length + "φ";
                            var znacka = Encoding.Unicode.GetBytes(metaData);
                            var zprava = new byte[1024 * 1024 * 4];
                            MessageBox.Show(soubor.Length.ToString());

                            Array.Copy(znacka, 0, zprava, 0, znacka.Length);
                            Array.Copy(soubor, 0, zprava, 300, soubor.Length);

                            VyslaniSouboru(zprava);
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

        private void VyslaniSouboru(byte[] data)
        {
            try
            {
                _odesilani.Write(data, 0, data.Length);
                _odesilani.Flush();
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() +
                                       " Objevila se chyba:"));
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
            }
        }
    }
}