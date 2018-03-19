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
        private bool _odesilani;
        private int _pocetPripojeni; //Počet aktuálně připojených uživatelů
        private TcpListener _prichoziKomunikace; //Poslouchá příchozí komunikaci a žádosti i připojení

        private bool _stop; //Proměná pro zastavení běhu serveru

        public OknoServeru()
        {
            InitializeComponent();

            //Nastavení vzhledu
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
                    if (_prichoziKomunikace.Pending() && MaximalniPocet()
                    ) //Pokud není přesažen povolený počet klientů a existuje žádost o připojení, server ji zpracuje
                    {
                        var klient = _prichoziKomunikace.AcceptTcpClient();
                        ++_pocetPripojeni;
                        var byteJmeno = new byte[1024 * 1024 * 2];
                        var cteniJmena = klient.GetStream();
                        cteniJmena.Read(byteJmeno, 0, klient.ReceiveBufferSize);
                        var jmeno = Encoding.Unicode.GetString(byteJmeno)
                            .TrimEnd('\0');

                        //Pokud má klient duplikátní jméno, je okamžitě odpojen, v opačném případě je spuštěn nový thread
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
                        VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message + "\n"));
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
                        var hrubaData = new byte[1024 * 1024 * 2 + 128];
                        cteni.Read(hrubaData, 0, pripojeni.ReceiveBufferSize);

                        var znacka = new byte[4];
                        Array.Copy(hrubaData, znacka, 4);
                        var uprava = Encoding.Unicode.GetString(znacka);

                        //Podle znaku v hlavičce dat jsou data zpracována
                        switch (uprava[0])
                        {
                            case '0': //Běžná zpráva
                            {
                                var zprava = Encoding.Unicode.GetString(hrubaData).TrimEnd('\0').Split('φ');
                                Vysilani(jmeno, zprava[1]);
                                break;
                            }
                            case '1': //Obrázek
                            {
                                var pomocna = new byte[128];
                                Array.Copy(hrubaData, 0, pomocna, 0, 128);
                                var hlavicka = Encoding.Unicode.GetString(pomocna).TrimEnd('\0').Split('φ');
                                ZpracovaniSouboru(hrubaData, hlavicka[2], hlavicka[3], jmeno, "Obrazek");
                                break;
                            }
                            case '2': //Soubor
                            {
                                var pomocna = new byte[128];
                                Array.Copy(hrubaData, 0, pomocna, 0, 128);
                                var hlavicka = Encoding.Unicode.GetString(pomocna).TrimEnd('\0').Split('φ');
                                ZpracovaniSouboru(hrubaData, hlavicka[2], hlavicka[3], jmeno, "Soubor");
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
        private void Vysilani(string tvurce, string text)
        {
            try
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += DateTime.Now.ToShortTimeString() + " " + tvurce + ": " +
                                       text + "\n"));
                text = "0φ" + tvurce + "φ: " + text;
                var data = Encoding.Unicode.GetBytes(text);

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
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message + "\n"));
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

                var seznam = "3φ" + jmena;
                var data = Encoding.Unicode.GetBytes(seznam);

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
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message + "\n"));
            }
        }

        /// <summary>
        ///     Ukončí všechna připojení, vypne server a vrtáí se na úvodní okno.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnServerStop_Click(object sender, EventArgs e)
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
        private bool KontrolaJmena(string jmeno)
        {
            foreach (var klient in _seznamKlientu)
                if (klient.Key == jmeno)
                    return false;

            return true;
        }

        /// <summary>
        ///     Odpojí klienta ze serveru.
        /// </summary>
        /// <param name="jmeno">Jméno odebíraného klienta</param>
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
                Invoke((MethodInvoker) (() => AktualizaceSeznamu()));
            else
                AktualizaceSeznamu();

            Vysilani("SERVER", jmeno + " se odpojil(a)");
        }

        /// <summary>
        ///     Odpojí klienta ze serveru bez zásahu do seznamů.
        /// </summary>
        /// <param name="klient">Připojení odebíraného klienta</param>
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

            //Nastaví vzhled podle zvoleného nastavení
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

            //Pokud existuje soubor s historií, načte jej do okna
            if (File.Exists(UvodServeru.SlozkaSouboru + "\\Historie.txt"))
                VypisChatu.LoadFile(UvodServeru.SlozkaSouboru + "\\Historie.txt",
                    RichTextBoxStreamType.UnicodePlainText);

            _behServeru = new Thread(PrijmaniKlientu) //Spustí přijímání a obsluhu klientů
            {
                IsBackground = true
            };

            _odesilani = true;
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
        ///     Vybere obrázek k odeslání a pošle ho všem klientům.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OdeslaniObrazku_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = "Obrázky|*.jpg;*.png;*.gif;*.jpeg;*.jpe;*.bmp";

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                var obrazek = File.ReadAllBytes(VolbaSouboru.FileName);

                if (obrazek.Length < 2048 * 1024)
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
                else
                    MessageBox.Show("Vybraný soubor je větší než stanovený limit (2 MB).", "Chyba");
            }
        }

        /// <summary>
        ///     Vybere soubor k odeslání a pošle ho všem klientům.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OdeslaniSouboru_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = null;

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                var obrazek = File.ReadAllBytes(VolbaSouboru.FileName);

                if (obrazek.Length < 2048 * 1024)
                    try
                    {
                        var nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName);
                        var pripona = Path.GetExtension(VolbaSouboru.FileName);

                        ZpracovaniSouboru(obrazek, nazev, pripona, "SERVER", "Soubor");
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
        ///     Uloží data souboru a pošle je všem klientům
        /// </summary>
        /// <param name="data">Data souboru</param>
        /// <param name="nazev">Název souboru</param>
        /// <param name="pripona">Přípona souboru</param>
        /// <param name="odesilatel">Odesílatel souboru</param>
        /// <param name="znak">Příznak, zda se jedná o obrázek nebo o soubor</param>
        private void ZpracovaniSouboru(byte[] data, string nazev, string pripona, string odesilatel, string znak)
        {
            var druh = 1;

            if (znak == "Soubor") druh = 2;

            UlozitSoubor(data, nazev, pripona, znak);

            var hlavicka = Encoding.Unicode.GetBytes(druh + "φ" + data.Length + "φ" + nazev + "φ" + pripona);
            var odesilanaData = new byte[2048 * 2048 + 128];

            Array.Copy(hlavicka, 0, odesilanaData, 0, hlavicka.Length);
            Array.Copy(data, 0, odesilanaData, 128, data.Length);

            foreach (var klient in _seznamKlientu)
            {
                var odeslani = new Thread(() => OdeslaniDat(odesilanaData, klient.Key));
                odeslani.Start();
            }

            Vysilani(odesilatel, "Poslal obrázek " + nazev + pripona);
        }

        /// <summary>
        ///     Uloží soubor na disk do příslušné složky.
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
        ///     Odešela zadaná data klientovi.
        /// </summary>
        /// <param name="data">Odesílaná data</param>
        /// <param name="jmeno">Jméno klienta</param>
        private void OdeslaniDat(byte[] data, string jmeno)
        {
            try
            {
                //Server počká, než je možné znovu odesílat data
                while (!_odesilani) Thread.Sleep(UvodServeru.Kontrola);

                //Pokud se server nevypíná, pošle zadaná data všem klientům.
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
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message + "\n"));
            }
        }

        /// <summary>
        ///     Otevře složku se soubory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnZobrazitSoubory_Click(object sender, EventArgs e)
        {
            Process.Start(UvodServeru.SlozkaSouboru);
        }
    }
}