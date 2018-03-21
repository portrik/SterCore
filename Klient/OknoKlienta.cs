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
        private NetworkStream _prijem, _odesilani = default(NetworkStream);
        private Thread _prijmani;


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
                    _odesilani = _komunikace.GetStream();
                    VypisChatu.Text += DateTime.Now.ToShortTimeString() + " Připojení bylo úspěšné!";
                    var jmeno = Encoding.Unicode.GetBytes(_prezdivka);
                    _odesilani.Write(jmeno, 0, jmeno.Length);
                    _odesilani.Flush();

                    pripojeno = true;

                    _prijmani = new Thread(PrijmaniZprav)
                    {
                        IsBackground = true
                    };

                    _prijmani.Start();
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
                            var data = new byte[1024 * 1024 * 2 + 128];
                            var znak = new byte[3];

                            _prijem.Read(data, 0, _komunikace.ReceiveBufferSize);

                            Array.Copy(data, znak, 3);

                            var uprava = Encoding.Unicode.GetString(znak);

                            switch (uprava[0])
                            {
                                case '0': //Běžná zpráva
                                {
                                    var dekodovani = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                    var zprava = dekodovani.Split('φ');
                                    Vypsani(zprava[1] + zprava[2]);
                                    break;
                                }
                                case '1': //Zpracování obrázku
                                {
                                    PrijimaniSouboru(data, "Obrazek");
                                    break;
                                }
                                case '2': //Zpracování souboru
                                {
                                    PrijimaniSouboru(data, "Soubor");
                                    break;
                                }
                                case '3': //Seznam klientů
                                {
                                    if (InvokeRequired)
                                        Invoke((MethodInvoker) (() => LstPripojeni.Items.Clear()));
                                    else
                                        LstPripojeni.Items.Clear();

                                    var dekodovani = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                    var seznam = dekodovani.Split('φ');
                                    var jmena = seznam[1].Split(',');

                                    foreach (var jmeno in jmena)
                                        if (InvokeRequired)
                                            Invoke((MethodInvoker) (() => LstPripojeni.Items.Add(jmeno)));
                                        else
                                            LstPripojeni.Items.Add(jmeno);
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
                                    Invoke((MethodInvoker) (() => VypisChatu.Text = historie[1].TrimEnd('\n')));
                                    break;
                                }
                            }
                        }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Spojení bylo ukončeno.", "Konec spojení");

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
        ///     Odešle žádost o odpojení na server a otevře okno se zadáváním údajů.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOdpojit_Click(object sender, EventArgs e)
        {
            var zprava = Encoding.Unicode.GetBytes("4φ");
            _odesilani.Write(zprava, 0, zprava.Length);
            _odesilani.Flush();

            Close();
        }

        /// <summary>
        ///     Odešle zprávu na server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOdeslat_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtZprava.Text))
            {
                var zprava = Encoding.Unicode.GetBytes("0φ" + TxtZprava.Text.Trim());
                _odesilani.Write(zprava, 0, zprava.Length);
                _odesilani.Flush();

                TxtZprava.Text = null;
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

        /// <summary>
        ///     Otevře obrázek a pošle jej na server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OdeslaniObrazku_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = "Obrázky|*.jpg;*.png;*.gif;*.jpeg;*.jpe;*.bmp";

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                var soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                if (soubor.Length < 2048 * 1024)
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
                else
                    MessageBox.Show("Vybraný soubor je větší než stanovený limit (2 MB).", "Chyba");
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
        /// <param name="e"></param>
        private void VypisChatu_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        /// <summary>
        ///     Nastaví vzhled okna a připojí se na server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            Pripojeni();
        }

        /// <summary>
        ///     Otevře soubor a pošle jej na server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OdeslatSoubor_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = null;

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                var soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                if (soubor.Length < 2048 * 1024)
                    try
                    {
                        var nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName);
                        var pripona = Path.GetExtension(VolbaSouboru.FileName);

                        OdeslaniSouboru(soubor, nazev, pripona, 2);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.StackTrace);
                        MessageBox.Show(x.Message);
                    }
                else
                    MessageBox.Show("Vybraný soubor je větší než stanovený limit (2 MB).", "Chyba");
            }
        }

        /// <summary>
        ///     Dešifruje hlavičku souboru, uloží data a pošle je všem klientům
        /// </summary>
        /// <param name="data">Data souboru</param>
        /// <param name="druh">Příznak, zda se jedná o obrázek nebo o soubor</param>
        private static void PrijimaniSouboru(byte[] data, string druh)
        {
            var hlavicka = new byte[128];
            Array.Copy(data, 0, hlavicka, 0, 128);
            var prevod = Encoding.Unicode.GetString(hlavicka).Split('φ');

            var slozkaDruh = Path.Combine(UvodKlienta.SlozkaSouboru, druh);

            if (!UvodKlienta.SlozkaExistuje(UvodKlienta.SlozkaSouboru))
                Directory.CreateDirectory(UvodKlienta.SlozkaSouboru);

            if (!UvodKlienta.SlozkaExistuje(slozkaDruh)) Directory.CreateDirectory(slozkaDruh);

            var cesta = Path.Combine(UvodKlienta.SlozkaSouboru, druh) + @"\" + prevod[2] + prevod[3].Trim('\0');

            if (File.Exists(cesta))
            {
                var index = 1;

                while (File.Exists(cesta))
                {
                    cesta = Path.Combine(UvodKlienta.SlozkaSouboru, druh) + @"\" + prevod[2] + "(" + index + ")" +
                            prevod[3].Trim('\0');
                    ++index;
                }
            }

            var soubor = new byte[Convert.ToInt32(prevod[1])];
            Array.Copy(data, 128, soubor, 0, Convert.ToInt32(prevod[1]));

            File.WriteAllBytes(cesta, soubor);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnZobrazitSoubory_Click(object sender, EventArgs e)
        {
            Process.Start(UvodKlienta.SlozkaSouboru);
        }

        /// <summary>
        ///     Odešle soubor na server
        /// </summary>
        /// <param name="data">Data souboru</param>
        /// <param name="nazev">Název souboru</param>
        /// <param name="pripona">Přípona souboru</param>
        /// <param name="druh">Příznak, zda se jedná o soubor nebo obrázek</param>
        private void OdeslaniSouboru(byte[] data, string nazev, string pripona, int druh)
        {
            var hlavicka = Encoding.Unicode.GetBytes(druh + "φ" + data.Length + "φ" + nazev + "φ" + pripona);
            var odesilanaData = new byte[2048 * 2048 + 128];

            Array.Copy(hlavicka, 0, odesilanaData, 0, hlavicka.Length);
            Array.Copy(data, 0, odesilanaData, 128, data.Length);

            _odesilani.Write(odesilanaData, 0, odesilanaData.Length);
        }
    }
}
