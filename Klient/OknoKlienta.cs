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

namespace Klient
{
    public partial class OknoKlienta : MaterialForm
    {
        private readonly IPEndPoint _adresa;
        private readonly string _prezdivka;

        private TcpClient _komunikace;
        private int _pozice;
        private NetworkStream _prijem, _odesilani = default(NetworkStream);
        private bool _prijemSouboru;
        private Thread _prijmani;
        private byte[] _soubor;


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
            var pripojeno = false;

            try
            {
                _komunikace.Connect(_adresa); //Pokus o připojení na zadanou adresu a port

                if (_komunikace.Connected)
                {
                    _odesilani = _komunikace.GetStream(); //Nastavení proudu na adresu
                    VypisChatu.Text += DateTime.Now.ToShortTimeString() + " Připojení bylo úspěšné!";
                    var jmeno = Encoding.Unicode.GetBytes(_prezdivka); //Převedení přezdívky na byty
                    _odesilani.Write(jmeno, 0, jmeno.Length); //Odeslání přezdívky
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
                if (!pripojeno) MessageBox.Show("K zadanému serveru se nepodařilo připojit.", "Chyba");

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
                        if (_prijem.CanRead)
                        {
                            var data = new byte[1024 * 1024]; //Pole pro příjem sériových dat
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
                                    UlozitSoubor(data, "Obrazek");
                                    _odesilani.Write(potvrzeni, 0, potvrzeni.Length);
                                    break;
                                }
                                case '2': //TODO: Zpracování souboru
                                {
                                    UlozitSoubor(data, "Soubor");
                                    _odesilani.Write(potvrzeni, 0, potvrzeni.Length);
                                    break;
                                }
                                case '3': //TODO: Seznam klientů
                                {
                                    LstPripojeni.Items.Clear();

                                    var dekodovani = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                    var seznam = dekodovani.Split('φ');
                                    var jmena = seznam[1].Split(',');

                                    foreach (var jmeno in jmena)
                                        if (InvokeRequired)
                                            Invoke((MethodInvoker) (() => LstPripojeni.Items.Add(jmeno)));
                                    _odesilani.Write(potvrzeni, 0, potvrzeni.Length);
                                    break;
                                }
                                case '4': //Odpojení 
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


        private void UlozitSoubor(byte[] data, string druh)
        {
            var metaData = new byte[128];
            Array.Copy(data, 0, metaData, 0, 128);
            var split = Encoding.Unicode.GetString(metaData).Split('φ');
            var velikostDat = Convert.ToInt32(split[2]);
            var delkaSouboru = Convert.ToInt32(split[3]);

            if (Convert.ToInt32(split[1]) >= 0)
            {
                if (!_prijemSouboru)
                {
                    _soubor = new byte[delkaSouboru];
                    Array.Copy(data, 128, _soubor, 0, velikostDat - 128);
                    _prijemSouboru = true;
                    _pozice += velikostDat;
                }
                else
                {
                    Array.Copy(data, 128, _soubor, _pozice, velikostDat - 128);
                    _pozice += velikostDat;
                }
            }
            else
            {
                var slozkaDruh = Path.Combine(UvodKlienta.SlozkaSouboru, druh);

                if (!UvodKlienta.SlozkaExistuje(UvodKlienta.SlozkaSouboru))
                    Directory.CreateDirectory(UvodKlienta.SlozkaSouboru);

                if (!UvodKlienta.SlozkaExistuje(slozkaDruh)) Directory.CreateDirectory(slozkaDruh);


                var cesta = slozkaDruh + @"\" + split[4] + split[5].Trim('\0');

                if (File.Exists(cesta))
                {
                    var index = 1;

                    while (File.Exists(cesta))
                    {
                        cesta = slozkaDruh + @"\" + split[4] + "(" + index + ")" +
                                split[5].Trim('\0');
                        ++index;
                    }
                }

                File.WriteAllBytes(cesta, _soubor);
                _soubor = null;
                _prijemSouboru = false;
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

                try
                {
                    var nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName);
                    var pripona = Path.GetExtension(VolbaSouboru.FileName);

                    OdeslaniSouboru(obrazek, nazev, pripona, 2);
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.StackTrace);
                    MessageBox.Show(x.Message);
                }
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

            _prijemSouboru = false;
            _pozice = 0;
            Pripojeni();
        }

        private void OdeslatSoubor_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = null;

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                var soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                try
                {
                    var nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName);
                    var pripona = Path.GetExtension(VolbaSouboru.FileName);

                    OdeslaniSouboru(soubor, nazev, pripona, 1);
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.StackTrace);
                    MessageBox.Show(x.Message);
                }
            }
        }

        private void OdeslaniSouboru(byte[] data, string nazev, string pripona, int druh)
        {
            var pozice = 0;
            var pocet = 0;

            var odesilanaData = new byte[1500];

            while (pozice < data.Length)
            {
                byte[] metaData;

                if (data.Length - pozice > 1372)
                {
                    metaData = Encoding.Unicode.GetBytes(druh + "φ" + pocet + "φ" + 1372 + "φ" + data.Length + "φ" +
                                                         nazev + "φ" + pripona);
                    Array.Copy(data, pozice, odesilanaData, 128, 1372);
                    pozice += 1372;
                }
                else
                {
                    metaData = Encoding.Unicode.GetBytes(druh + "φ" + pocet + "φ" + (data.Length - pozice) + "φ" +
                                                         data.Length + "φ" + nazev + "φ" + pripona);
                    Array.Copy(data, pozice, odesilanaData, 128, data.Length - pozice);
                    pozice += data.Length - pozice;
                }

                Array.Copy(metaData, 0, odesilanaData, 0, metaData.Length);

                _odesilani.Write(odesilanaData, 0, 1500);
                _odesilani.Flush();

                ++pocet;
            }
        }
    }
}