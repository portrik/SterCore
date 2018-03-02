using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private Thread _behServeru; //Thread pro běh serveru na pozadí nezávisle na hlavním okně

        private readonly IPEndPoint _ipAdresa;
        private readonly int _pocetKlientu; //Proměná portu serveru a maximální počet klientů(0 znamená neomezený počet)
        private int _pocetPripojeni; //Počet aktuálně připojených uživatelů
        private TcpListener _prichoziKomunikace; //Poslouchá příchozí komunikaci a žádosti i připojení
        private readonly Dictionary<string, TcpClient> _seznamKlientu = new Dictionary<string, TcpClient>(); //Seznam připojených uživatelů a jejich adres
        private readonly Dictionary<string, bool> _kontrola = new Dictionary<string, bool>();

        private bool _stop; //Proměná pro zastavení běhu serveru

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
                            _kontrola.Add(jmeno, true);
                            Invoke((MethodInvoker) (() => VypisKlientu.Items.Add(jmeno)));
                            Invoke((MethodInvoker)(() => Vysilani("SERVER", jmeno + " se připojil(a)")));
                            Invoke((MethodInvoker)(() => AktualizaceSeznamu()));

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
                        VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + "Objevila se chyba:"));
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
        private void ObsluhaKlienta(string jmeno, TcpClient pripojeni) //Naslouchá příchozím zprávám od klienta
        {
            using (var cteni = pripojeni.GetStream()) //Nastaví naslouchání na správnou adresu
            {
                try
                {
                    while (!_stop)
                    {
                        byte[] hrubaData = new byte[1024 * 1024 * 4];
                        cteni.Read(hrubaData, 0, pripojeni.ReceiveBufferSize); //Načtení sériových dat     

                        var znacka = new byte[4];
                        Array.Copy(hrubaData, znacka, 4); //Zkopíruje první tři bajty z hrubých dat  
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
                            case '1': //TODO: Obrázek
                            {
                                
                                break;
                            }
                            case '2': //TODO: Soubor
                            {
                                break;
                            }
                            case '3': //Seznam klientů - server nevyužívá
                            {
                                break;
                            }
                            case '4': //Odpojení
                            {
                                OdebratKlienta(jmeno);
                                break;
                            }
                            case '5': //Historie - server nevužívá
                            {
                                break;
                            }
                            case '9': //Kontrola, zda byla zpráva přijata a klient je schopný znovu přijímat.
                            {
                                if (!Kontrola(jmeno))
                                {
                                    _kontrola[jmeno] = true;
                                }
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    if (!_stop)
                    {
                        OdebratKlienta(jmeno);
                    }
                }
            }
        }

        /// <summary>
        ///     Odešle soubor všem klientům.
        /// </summary>
        /// <param name="data">Data souboru</param>
        /// <param name="tvurce">Odesílatel souboru</param>
        /// <param name="nazev"></param>
        private void VysilaniObrazku(byte[] data, string tvurce, string nazev)
        {
            try
            {
                foreach (KeyValuePair<string,TcpClient> klient in _seznamKlientu)
                {
                    var vysilaniSocket = klient.Value;
                    var vysilaniProud = vysilaniSocket.GetStream();
                    vysilaniProud.Write(data, 0, data.Length);
                    vysilaniProud.Flush();
                }

                Vysilani(tvurce, "Poslal obrázek (" + nazev + ")");
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() +  "Objevila se chyba:"));
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
            }
        }

        private void VysilaniSouboru(byte[] data, string tvurce, string nazev)
        {
            try
            {
                foreach (KeyValuePair<string,TcpClient> klient in _seznamKlientu)
                {
                    var vysilaniSocket = klient.Value;
                    var vysilaniProud = vysilaniSocket.GetStream();
                    vysilaniProud.Write(data, 0, data.Length);
                    vysilaniProud.Flush();
                }

                Vysilani(tvurce, "Poslal soubor (" + nazev + ")");
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
        ///     Odešle zprávu všem připojeným klientům.
        /// </summary>
        /// <param name="tvurce">Jméno odesílatele</param>
        /// <param name="text">Obsah zprávy</param>
        private void Vysilani(string tvurce, string text) //Odeslání zprávy všem klientům
        {
            try
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text +=  DateTime.Now.ToShortTimeString() + " " + tvurce + ": " +
                        text + "\n")); //Vypíše zprávu na serveru
                text = "0φ" + tvurce + "φ: " + text; //Naformátuje zprávu před odesláním                
                var data = Encoding.Unicode.GetBytes(text); //Převede zprávu na byty

                foreach (KeyValuePair<string, TcpClient> klient in _seznamKlientu)
                {
                    Thread odeslani = new Thread(() => OdeslaniDat(data, klient.Key));
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

                foreach (KeyValuePair<string, TcpClient> klient in _seznamKlientu) jmena += klient.Key + ",";

                var seznam = "3φ" + jmena; //Naformátuje zprávu před odesláním                
                var data = Encoding.Unicode.GetBytes(seznam); //Převede zprávu na byty

                foreach (KeyValuePair<string, TcpClient> klient in _seznamKlientu)
                {
                    Thread odeslani = new Thread(() => OdeslaniDat(data, klient.Key));
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
            foreach (KeyValuePair<string, TcpClient> klient in _seznamKlientu)
                if ( klient.Key == jmeno)
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

            Invoke((MethodInvoker) (() => _seznamKlientu.Remove(jmeno))); //Odstraní klienta ze seznamu
            Vysilani("SERVER", jmeno + " se odpojil(a)"); //Ohlasí odpojení ostatním klientům
            Invoke((MethodInvoker) (() => AktualizaceSeznamu()));
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
            List<string> seznam = new List<string>();

            foreach (KeyValuePair<string,TcpClient> klient in _seznamKlientu)
            {
                seznam.Add(klient.Key);
            }

            var data = Encoding.Unicode.GetBytes("4φ");

            foreach (KeyValuePair<string, TcpClient> klient in _seznamKlientu)
            {
                Thread odeslani = new Thread(() => OdeslaniDat(data, klient.Key));
                odeslani.Start();
            }

            for (int i = 0; i < _seznamKlientu.Count; ++i)
            {
                _seznamKlientu[seznam[i]].Close();
            }
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
            {
                VypisChatu.LoadFile(UvodServeru.SlozkaSouboru + "\\Historie.txt", RichTextBoxStreamType.UnicodePlainText);
            }

            _behServeru = new Thread(PrijmaniKlientu)
                {
                    IsBackground = true
                };

                _behServeru.Start();
        }

        /// <summary>
        ///     Rozhodne, zda nebyl překročen počet připojení, pokud byl nastaven.
        ///     0 = neomezený počet připojení.
        /// </summary>
        /// <returns>True = je možné přidat další připojení</returns>
        private bool MaximalniPocet()
        {
            if (_pocetKlientu == 0 || _pocetPripojeni < _pocetKlientu)
                return true;
            return false;
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

                    ZpracovaniSouboru(obrazek, nazev, pripona, "SERVER");
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.StackTrace);
                    MessageBox.Show(x.Message);
                }
            }
        }

        private void OdeslaniSouboru_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = null;

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                var soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                if (soubor.Length < 4194004)
                    try
                    {
                        var nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) +
                                    Path.GetExtension(VolbaSouboru.FileName);
                        var metaData = "2φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" +
                                       Path.GetExtension(VolbaSouboru.FileName) + "φ" + soubor.Length + "φ";
                        var znacka = Encoding.Unicode.GetBytes(metaData);
                        var zprava = new byte[1024 * 1024 * 4];

                        Array.Copy(znacka, 0, zprava, 0, znacka.Length);
                        Array.Copy(soubor, 0, zprava, 300, soubor.Length);

                       

                        
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

        private void ZpracovaniSouboru(byte[] data, string nazev, string pripona, string odesilatel)
        {
            UlozitObrazek(data, nazev, pripona);

            var pozicePuvodni = 0;
            var pocet = 0;

            byte[] odesilanaData = new byte[1500];

            while (pozicePuvodni < data.Length)
            {
                byte[] metaData;
                
                if (data.Length - pozicePuvodni > 1372)
                {
                    metaData = Encoding.Unicode.GetBytes("1φ" + pocet + "φ" + 1372 + "φ" + data.Length + "φ" + nazev + "φ" + pripona);
                    Array.Copy(data, pozicePuvodni, odesilanaData, 128, 1372);
                    pozicePuvodni += 1372;
                }
                else
                {
                    metaData = Encoding.Unicode.GetBytes("1φ" + pocet + "φ" + (data.Length - pozicePuvodni) + "φ" + data.Length + "φ" + nazev + "φ" + pripona);
                    Array.Copy(data, pozicePuvodni, odesilanaData, 128, data.Length - pozicePuvodni);
                    pozicePuvodni += data.Length - pozicePuvodni;
                }

                Array.Copy(metaData, 0, odesilanaData, 0, metaData.Length);

                foreach (KeyValuePair<string, TcpClient> klient in _seznamKlientu)
                {
                    Thread odeslani = new Thread(() => OdeslaniDat(odesilanaData, klient.Key));
                    odeslani.Start();
                }

                ++pocet;
            }

            odesilanaData = Encoding.Unicode.GetBytes("1φ" + -1 + "φ" + 0 + "φ" + data.Length + "φ" + nazev + "φ" + pripona);

            foreach (KeyValuePair<string, TcpClient> klient in _seznamKlientu)
            {
                Thread odeslani = new Thread(() => OdeslaniDat(odesilanaData, klient.Key));
                odeslani.Start();
            }

            Vysilani(odesilatel, "Poslal obrázek " + nazev + pripona);
        }

        private void UlozitObrazek(byte[] data, string nazev, string pripona)
        {
            var slozkaDruh = Path.Combine(UvodServeru.SlozkaSouboru, "Obrazek");

            if (!UvodServeru.SlozkaExistuje(UvodServeru.SlozkaSouboru)) Directory.CreateDirectory(UvodServeru.SlozkaSouboru);

            if (!UvodServeru.SlozkaExistuje(slozkaDruh)) Directory.CreateDirectory(slozkaDruh);

            var cesta = Path.Combine(UvodServeru.SlozkaSouboru, "Obrazek") + @"\" + nazev + pripona;

            if (File.Exists(cesta))
            {
                var index = 1;

                while (File.Exists(cesta))
                {
                    cesta = Path.Combine(UvodServeru.SlozkaSouboru, "Obrazek") + @"\" + nazev + "(" + index + ")" +
                            pripona;
                    ++index;
                }
            }

            File.WriteAllBytes(cesta, data);
        }

        /// <summary>
        /// Odešle historii chatu novému klientovi.
        /// </summary>
        private void OdeslatHistorii(string jmeno)
        {
            string pomocna = "5φ";
            Invoke((MethodInvoker) (() => pomocna += VypisChatu.Text));
            var historie = Encoding.Unicode.GetBytes(pomocna);
            Thread odeslani = new Thread(() => OdeslaniDat(historie, jmeno));
            odeslani.Start();
        }

        private bool Kontrola(string jmeno)
        {
            return _kontrola[jmeno];
        }

        private void OdeslaniDat(byte[] data, string jmeno)
        {
            if (!_stop)
            {
                while (!Kontrola(jmeno))
                {
                    Thread.Sleep(UvodServeru.Kontrola);
                }

                _kontrola[jmeno] = false;

                do
                {
                    var vysilaniSocket = _seznamKlientu[jmeno];
                    var vysilaniProud = vysilaniSocket.GetStream();
                    vysilaniProud.Write(data, 0, data.Length);
                    vysilaniProud.Flush();
                    Thread.Sleep(UvodServeru.Kontrola);
                } while (!Kontrola(jmeno));
            }
        }
    }
}