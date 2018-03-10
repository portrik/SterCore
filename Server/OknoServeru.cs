using System;
using System.Collections.Generic;
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

namespace Server
{
    public partial class OknoServeru : MaterialForm
    {
        private readonly IPEndPoint _ipAdresa; //Adresa serveru, ke které se kliénti připojují.
        private readonly int _pocetKlientu; //Proměná portu serveru a maximální počet klientů(0 znamená neomezený počet)

        private readonly Dictionary<string, TcpClient>
            _seznamKlientu = new Dictionary<string, TcpClient>(); //Seznam připojených uživatelů a jejich adres

        private Thread _behServeru; //Thread pro běh serveru na pozadí nezávisle na hlavním okně
        private int _pocetPripojeni; //Počet aktuálně připojených uživatelů
        private TcpListener _prichoziKomunikace; //Poslouchá příchozí komunikaci a žádosti i připojení
        private byte[] _bufferSouboru = new byte[1024 * 1024* 16]; //Pole pro příjem souborů
        private int _poziceBufferu;

        private bool _stop; //Proměná pro zastavení běhu serveru
        private bool _odesilani;

        public OknoServeru()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = UvodServeru.Tema;
            materialSkinManager.ColorScheme = UvodServeru.Vzhled;

            _pocetKlientu = UvodServeru.PocetPripojeni;
            _ipAdresa = new IPEndPoint(UvodServeru.LokalniAdresa(), UvodServeru.Port);
        }

        /// <summary>
        ///     Přijímá připojení klientů. Duplikátní jména jsou odpojena.
        /// </summary>
        private void PrijmaniKlientu()
        {
            try
            {
                _prichoziKomunikace.Start();

                while (!_stop)
                    if (_prichoziKomunikace.Pending() && MaximalniPocet())
                    {
                        var klient = _prichoziKomunikace.AcceptTcpClient(); //Přijme žádost o připojení
                        ++_pocetPripojeni;
                        var byteJmeno = new byte[1024 * 1024 * 2]; //Bytové pole pro načtení jména
                        var cteniJmena = klient.GetStream(); //Připojení načítání na správný socket
                        cteniJmena.Read(byteJmeno, 0, klient.ReceiveBufferSize); //Načtení sériových dat
                        var jmeno = Encoding.Unicode.GetString(byteJmeno)
                            .TrimEnd('\0'); //Dekódování dat a vymazání prázdných znaků

                        if (KontrolaJmena(jmeno))
                        {
                            _seznamKlientu.Add(jmeno, klient);
                            Invoke((MethodInvoker) (() => VypisKlientu.Items.Add(jmeno)));
                            Invoke((MethodInvoker) (() => Vysilani("SERVER", jmeno + " se připojil(a)")));
                            AktualizaceSeznamu();

                            OdeslatHistorii(jmeno);

                            var vlaknoKlienta = new Thread(() => ObsluhaKlienta(jmeno, klient))
                            {
                                IsBackground = true
                            };

                            vlaknoKlienta.Start();
                        }
                        else
                        {
                            Vysilani("SERVER",
                                jmeno + " se pokusil(a) připojit. Pokus byl zamítnut - duplikátní jméno");
                            cteniJmena.Flush();
                            OdebratKlienta(klient);
                        }
                    }
            }
            catch (Exception x)
            {
                if (!_stop)
                {
                    Invoke((MethodInvoker) (() =>
                        VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " Objevila se chyba:"));
                    Invoke((MethodInvoker) (() =>
                        VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
                }
            }
        }

        /// <summary>
        ///     Naslouchá příchozím zprávám od klienta.
        /// </summary>
        /// <param name="jmeno">Jméno klienta</param>
        /// <param name="pripojeni">Připojení klienta</param>
        private void ObsluhaKlienta(string jmeno, TcpClient pripojeni) 
        {
            using (var cteni = pripojeni.GetStream()) 
            {
                try
                {
                    while (!_stop)
                    {
                        var hrubaData = new byte[1024 * 1024 * 2];
                        cteni.Read(hrubaData, 0, pripojeni.ReceiveBufferSize); 

                        var znacka = new byte[4];
                        Array.Copy(hrubaData, znacka, 4); 
                        var uprava = Encoding.Unicode.GetString(znacka);

                        switch (uprava[0])
                        {
                            case '0': //Běžná zpráva
                            {
                                var data = Encoding.Unicode.GetString(hrubaData).TrimEnd('\0');
                                var zprava = data.Split('φ');
                                Vysilani(jmeno, zprava[1]);
                                break;
                            }
                            case '1': //Obrázek
                            {
                                PrijmaniSouboru(hrubaData, jmeno, "Obrazek");
                                break;
                            }
                            case '2': //Soubor
                            {
                                PrijmaniSouboru(hrubaData, jmeno, "Soubor");
                                break;
                            }
                            case '3': //Seznam klientů - server nepřijímá
                            {
                                break;
                            }
                            case '4': //Odpojení
                            {
                                OdebratKlienta(jmeno);
                                break;
                            }
                            case '5': //Historie - server nepřijímá
                            {
                                break;
                            }
                        }
                    }
                }
                catch //V případě chyby je klient odpojen
                {
                    if (!_stop) OdebratKlienta(jmeno);
                }
            }
        }

        /// <summary>
        ///     Odešle zprávu všem připojeným klientům.
        /// </summary>
        /// <param name="tvurce">Jméno odesílatele</param>
        /// <param name="text">Obsah zprávy</param>
        private void Vysilani(string tvurce, string text) //Odeslání zprávy všem klientům
        {
            try
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + tvurce + ": " +
                                       text)); //Vypíše zprávu na serveru
                text = "0φ" + tvurce + "φ: " + text; //Naformátuje zprávu před odesláním                
                var data = Encoding.Unicode.GetBytes(text); //Převede zprávu na byty

                foreach (var klient in _seznamKlientu)
                {
                    var odeslani = new Thread(() => OdeslaniDat(data, klient.Key));
                    odeslani.Start();
                }
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " Objevila se chyba:"));
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
            }
        }

        /// <summary>
        ///     Odešle všem klientům aktualizovaný seznam všech připojených klientů.
        /// </summary>
        private void AktualizaceSeznamu()
        {
            try
            {
                string jmena = null;

                foreach (var klient in _seznamKlientu) jmena += klient.Key + ",";

                var seznam = "3φ" + jmena; //Naformátuje zprávu před odesláním                
                var data = Encoding.Unicode.GetBytes(seznam); //Převede zprávu na byty

                foreach (var klient in _seznamKlientu)
                {
                    var odeslani = new Thread(() => OdeslaniDat(data, klient.Key));
                    odeslani.Start();
                }
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " Objevila se chyba:"));
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
            }
        }

        /// <summary>
        ///     Ukončí všechna připojení, vypne server a vrtáí se na úvodní form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnServerStop_Click(object sender, EventArgs e) //Ukončení běhu serveru
        {
            _stop = true;

            VypisChatu.SaveFile(UvodServeru.SlozkaSouboru + "\\Historie.txt", RichTextBoxStreamType.UnicodePlainText);

            OdebratKlienty();
            _prichoziKomunikace.Stop();
            Close();
        }

        /// <summary>
        ///     Zkontroluje, jestli není připojený uživatel se stejným jménem.
        /// </summary>
        /// <param name="jmeno">Jméno ke kontrole</param>
        /// <returns>Jméno je v pořádku</returns>
        private bool KontrolaJmena(string jmeno) //Zkontroluje, zda se jméno již nevyskytuje
        {
            foreach (var klient in _seznamKlientu)
                if (klient.Key == jmeno)
                    return false;

            return true;
        }

        /// <summary>
        ///     Odpojí klienta ze serveru.
        /// </summary>
        /// <param name="jmeno">Jméno klienta</param>
        private void OdebratKlienta(string jmeno)
        {
            --_pocetPripojeni;

            if (InvokeRequired) //Odstraní klienta z výpisu
                Invoke((MethodInvoker) (() => VypisKlientu.Items.Remove(jmeno)));
            else
                VypisKlientu.Items.Remove(jmeno);

            if (InvokeRequired)
                Invoke((MethodInvoker) (() => _seznamKlientu.Remove(jmeno)));
            else
                _seznamKlientu.Remove(jmeno);

            if (InvokeRequired)
                Invoke((MethodInvoker)(() => AktualizaceSeznamu()));
            else
                AktualizaceSeznamu();

            Vysilani("SERVER", jmeno + " se odpojil(a)");
        }

        /// <summary>
        ///     Odpojí klienta ze serveru bez zásahu do seznamů.
        /// </summary>
        /// <param name="klient">Připojení klienta</param>
        private void OdebratKlienta(TcpClient klient)
        {
            --_pocetPripojeni;

            klient.Close();
        }

        private void OdebratKlienty()
        {
            var seznam = new List<string>();

            foreach (var klient in _seznamKlientu) seznam.Add(klient.Key);

            var data = Encoding.Unicode.GetBytes("4φ");

            foreach (var klient in _seznamKlientu)
            {
                var odeslani = new Thread(() => OdeslaniDat(data, klient.Key));
                odeslani.Start();
            }

            for (var i = 0; i < _seznamKlientu.Count; ++i) _seznamKlientu[seznam[i]].Close();
        }

        /// <summary>
        ///     Odešle zprávu ze serveru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnZprava_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtZprava.Text))
            {
                Vysilani("Server", TxtZprava.Text);
                TxtZprava.Text = null;
                TxtZprava.Focus();
                TxtZprava.SelectAll();
            }
        }

        /// <summary>
        ///     Odeslání zprávy pomocí enteru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtZprava_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter) BtnZprava_Click(null, null);
        }

        /// <summary>
        ///     Nastaví vzhled okna a spustí server po načtení.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OknoServeru_Load(object sender, EventArgs e)
        {
            _stop = false;
            _prichoziKomunikace = new TcpListener(_ipAdresa);

            if (UvodServeru.Tema == MaterialSkinManager.Themes.LIGHT)
            {
                VypisChatu.BackColor = Color.White;
                VypisChatu.ForeColor = Color.Black;
                VypisKlientu.BackColor = Color.White;
                VypisKlientu.ForeColor = Color.Black;
            }
            else
            {
                VypisChatu.BackColor = ColorTranslator.FromHtml("#333333");
                VypisChatu.ForeColor = Color.White;
                VypisKlientu.BackColor = ColorTranslator.FromHtml("#333333");
                VypisKlientu.ForeColor = Color.White;
            }

            if (File.Exists(UvodServeru.SlozkaSouboru + "\\Historie.txt"))
                VypisChatu.LoadFile(UvodServeru.SlozkaSouboru + "\\Historie.txt",
                    RichTextBoxStreamType.UnicodePlainText);

            _behServeru = new Thread(PrijmaniKlientu)
            {
                IsBackground = true
            };

            _odesilani = true;
            _poziceBufferu = 0;
            _behServeru.Start();
        }

        /// <summary>
        ///     Rozhodne, zda nebyl překročen počet připojení, pokud byl nastaven.
        ///     0 = neomezený počet připojení.
        /// </summary>
        /// <returns>True = je možné přidat další připojení</returns>
        private bool MaximalniPocet()
        {
            return _pocetKlientu == 0 || _pocetPripojeni < _pocetKlientu;
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
        
        /// <summary>
        /// Vybere obrázek k odeslání a pošle ho všem klientům.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    ZpracovaniSouboru(obrazek, nazev, pripona, "SERVER", "Obrazek");
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.StackTrace);
                    MessageBox.Show(x.Message);
                }
            }
        }

        /// <summary>
        /// Vybere soubor k odeslání a pošle ho všem klientům.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OdeslaniSouboru_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = null;

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                var soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                try
                {
                    var nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName);
                    var pripona = Path.GetExtension(VolbaSouboru.FileName);

                    ZpracovaniSouboru(soubor, nazev, pripona, "SERVER", "Soubor");
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.StackTrace);
                    MessageBox.Show(x.Message);
                }
            }
        }

        private void ZpracovaniSouboru(byte[] data, string nazev, string pripona, string odesilatel, string znak)
        {
            const int velikostDat = 1024 * 512;
            var pozice = 0;
            var pocet = 0;
            var druh = 1;

            if (znak == "Soubor")
            {
                druh = 2;
            }

            UlozitSoubor(data, nazev, pripona, znak);

            var odesilanaData = new byte[velikostDat + 128];

            while (pozice < data.Length)
            {
                byte[] metaData;

                if (data.Length - pozice > velikostDat + 128)
                {
                    metaData = Encoding.Unicode.GetBytes(druh + "φ" + pocet + "φ" + velikostDat + "φ" + nazev + "φ" + pripona);
                    Array.Copy(data, pozice, odesilanaData, 128, velikostDat);
                    pozice += velikostDat;
                }
                else
                {
                    metaData = Encoding.Unicode.GetBytes(druh + "φ" + pocet + "φ" + (data.Length - pozice) + "φ" + nazev + "φ" + pripona);
                    Array.Copy(data, pozice, odesilanaData, 128, data.Length - pozice);
                    pozice += data.Length - pozice;
                }

                Array.Copy(metaData, 0, odesilanaData, 0, metaData.Length);

                foreach (var klient in _seznamKlientu)
                {
                    var odeslani = new Thread(() => OdeslaniDat(odesilanaData, klient.Key));
                    odeslani.Start();
                }

                ++pocet;
            }

            odesilanaData = Encoding.Unicode.GetBytes("1φ" + -1 + "φ" + data.Length + "φ" + nazev + "φ" + pripona);

            foreach (var klient in _seznamKlientu)
            {
                var odeslani = new Thread(() => OdeslaniDat(odesilanaData, klient.Key));
                odeslani.Start();
            }

            Vysilani(odesilatel, "Poslal obrázek " + nazev + pripona);
        }

        private void PrijmaniSouboru(byte[] data, string jmeno, string druh)
        {
            var hlavicka = new byte[128];
            Array.Copy(data, 0, hlavicka, 0, 128);
            var prevod = Encoding.Unicode.GetString(hlavicka).Split('φ');


            if (Convert.ToInt32(prevod[1]) >= 0)
            {
                Array.Copy(data, 0, _bufferSouboru, _poziceBufferu, Convert.ToInt32(prevod[2]));
                _poziceBufferu += Convert.ToInt32(prevod[2]);
            }
            else
            {
                var pole = new byte[Convert.ToInt32(prevod[2])];
                Array.Copy(_bufferSouboru, 0, pole, 0, Convert.ToInt32(prevod[2]));
                ZpracovaniSouboru(pole, prevod[3], prevod[4], jmeno, druh);
                _bufferSouboru = new byte[1024 * 1024 * 16];
                _poziceBufferu = 0;
            }
        }

        /// <summary>
        /// Uloží soubor na disk do příslušné složky.
        /// </summary>
        /// <param name="data">Pole bajtů souboru</param>
        /// <param name="nazev">Název souboru</param>
        /// <param name="pripona">Přípona souboru</param>
        /// <param name="druh">Druh souboru - Obrázek nebo Soubor</param>
        private static void UlozitSoubor(byte[] data, string nazev, string pripona, string druh)
        {
            var slozkaDruh = Path.Combine(UvodServeru.SlozkaSouboru, druh);

            if (!UvodServeru.SlozkaExistuje(UvodServeru.SlozkaSouboru))
                Directory.CreateDirectory(UvodServeru.SlozkaSouboru);

            if (!UvodServeru.SlozkaExistuje(slozkaDruh)) Directory.CreateDirectory(slozkaDruh);

            var cesta = Path.Combine(UvodServeru.SlozkaSouboru, druh) + @"\" + nazev + pripona;

            if (File.Exists(cesta))
            {
                var index = 1;

                while (File.Exists(cesta))
                {
                    cesta = Path.Combine(UvodServeru.SlozkaSouboru, druh) + @"\" + nazev + "(" + index + ")" +
                            pripona;
                    ++index;
                }
            }

            File.WriteAllBytes(cesta, data);
        }

        /// <summary>
        ///     Odešle historii chatu novému klientovi.
        /// </summary>
        private void OdeslatHistorii(string jmeno)
        {
            var pomocna = "5φ";
            Invoke((MethodInvoker) (() => pomocna += VypisChatu.Text));
            var historie = Encoding.Unicode.GetBytes(pomocna);
            var odeslani = new Thread(() => OdeslaniDat(historie, jmeno));
            odeslani.Start();
        }

        /// <summary>
        /// Odešela zadaná data klientovi.
        /// </summary>
        /// <param name="data">Odesílaná data</param>
        /// <param name="jmeno">Jméno klienta</param>
        private void OdeslaniDat(byte[] data, string jmeno)
        {
            try
            {
                while (!_odesilani)
                {
                    Thread.Sleep(UvodServeru.Kontrola);
                }

                if (!_stop)
                {
                    _odesilani = false;
                    var vysilaniSocket = _seznamKlientu[jmeno];
                    var vysilaniStream = vysilaniSocket.GetStream();
                    vysilaniStream.Write(data, 0, data.Length);
                    vysilaniStream.Flush();
                    Thread.Sleep(UvodServeru.Kontrola);
                    _odesilani = true;
                }
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + "Objevila se chyba:"));
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
            }
        }

        private void BtnZobrazitSoubory_Click(object sender, EventArgs e)
        {
            Process.Start(UvodServeru.SlozkaSouboru);
        }
    }
}