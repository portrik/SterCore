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
using Server;

namespace Server
{
    public partial class OknoServeru : MaterialForm
    {
        private Thread _behServeru; //Thread pro běh serveru na pozadí nezávisle na hlavním okně

        private readonly IPEndPoint _ipAdresa;
        private readonly int _pocetKlientu; //Proměná portu serveru a maximální počet klientů(0 znamená neomezený počet)
        private int _pocetPripojeni; //Počet aktuálně připojených uživatelů
        private TcpListener _prichoziKomunikace; //Poslouchá příchozí komunikaci a žádosti i připojení
        private readonly Hashtable _seznamKlientu = new Hashtable(); //Seznam připojených uživatelů a jejich adres

        private readonly string _slozka = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Stercore soubory");

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
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += DateTime.Now.ToShortTimeString() + " " + "Server byl spuštěn."));

                while (!_stop)
                    if (_prichoziKomunikace.Pending() && MaximalniPocet())
                    {
                        var Klient = _prichoziKomunikace.AcceptTcpClient(); //Přijme žádost o připojení
                        ++_pocetPripojeni;
                        var ByteJmeno = new byte[1024 * 1024 * 2]; //Bytové pole pro načtení jména
                        var CteniJmena = Klient.GetStream(); //Připojení načítání na správný socket
                        CteniJmena.Read(ByteJmeno, 0, Klient.ReceiveBufferSize); //Načtení sériových dat
                        var Jmeno = Encoding.Unicode.GetString(ByteJmeno)
                            .TrimEnd('\0'); //Dekódování dat a vymazání prázdných znaků

                        if (KontrolaJmena(Jmeno))
                        {
                            _seznamKlientu.Add(Jmeno, Klient);
                            Invoke((MethodInvoker) (() => VypisKlientu.Items.Add(Jmeno)));
                            Vysilani("SERVER", Jmeno + " se připojil(a)");
                            Invoke((MethodInvoker) (() => AktualizaceSeznamu()));

                            var VlaknoKlienta = new Thread(() => ObsluhaKlienta(Jmeno, Klient))
                            {
                                IsBackground = true
                            };

                            VlaknoKlienta.Start();
                        }
                        else
                        {
                            Vysilani("SERVER",
                                Jmeno + " se pokusil(a) připojit. Pokus byl zamítnut - duplikátní jméno");
                            CteniJmena.Flush();
                            OdebratKlienta(Klient);
                        }
                    }
            }
            catch (Exception x)
            {
                if (!_stop)
                {
                    Invoke((MethodInvoker) (() =>
                        VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + "Objevila se chyba:"));
                    Invoke((MethodInvoker) (() =>
                        VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
                }
            }
        }

        /// <summary>
        ///     Naslouchá příchozím zprávám od klienta.
        /// </summary>
        /// <param name="jmeno">Jméno klienta</param>
        /// <param name="Pripojeni">Připojení klienta</param>
        private void ObsluhaKlienta(string jmeno, TcpClient Pripojeni) //Naslouchá příchozím zprávám od klienta
        {
            using (var Cteni = Pripojeni.GetStream()) //Nastaví naslouchání na správnou adresu
            {
                byte[] HrubaData; //Pole pro přijímání zpráv

                try
                {
                    while (!_stop)
                    {
                        HrubaData = new byte[1024 * 1024 * 4];
                        Cteni.Read(HrubaData, 0, Pripojeni.ReceiveBufferSize); //Načtení sériových dat     

                        var Znacka = new byte[4];
                        Array.Copy(HrubaData, Znacka, 4); //Zkopíruje první tři bajty z hrubých dat  
                        var Uprava = Encoding.Unicode.GetString(Znacka);
                        string NazevSouboru = null;

                        switch (Uprava[0])
                        {
                            case '0': //Běžná zpráva
                            {
                                var Data = Encoding.Unicode.GetString(HrubaData).TrimEnd('\0');
                                var Zprava = Data.Split('φ');
                                Vysilani(jmeno, Zprava[1]);
                                break;
                            }
                            case '1': //Obrázek
                            {
                                
                                VysilaniObrazku(HrubaData, jmeno, NazevSouboru);

                                break;
                            }
                            case '2': //Soubor
                            {
                                
                                VysilaniSouboru(HrubaData, jmeno, NazevSouboru);

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
                        }
                    }
                }
                catch
                {
                    OdebratKlienta(jmeno);
                }
            }
        }

        /// <summary>
        ///     Odešle soubor všem klientům.
        /// </summary>
        /// <param name="Data">Data souboru</param>
        /// <param name="Tvurce">Odesílatel souboru</param>
        private void VysilaniObrazku(byte[] Data, string Tvurce, string Nazev)
        {
            try
            {
                foreach (DictionaryEntry Klient in _seznamKlientu)
                {
                    var VysilaniSocket = (TcpClient) Klient.Value;
                    var VysilaniProud = VysilaniSocket.GetStream();
                    VysilaniProud.Write(Data, 0, Data.Length);
                    VysilaniProud.Flush();
                }

                Vysilani(Tvurce, "Poslal obrázek (" + Nazev + ")");
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + "Objevila se chyba:"));
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
            }
        }

        private void VysilaniSouboru(byte[] Data, string Tvurce, string Nazev)
        {
            try
            {
                foreach (DictionaryEntry Klient in _seznamKlientu)
                {
                    var VysilaniSocket = (TcpClient) Klient.Value;
                    var VysilaniProud = VysilaniSocket.GetStream();
                    VysilaniProud.Write(Data, 0, Data.Length);
                    VysilaniProud.Flush();
                }

                Vysilani(Tvurce, "Poslal soubor (" + Nazev + ")");
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + "Objevila se chyba:"));
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + x.Message));
            }
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
        ///     Odešle zprávu všem připojeným klientům.
        /// </summary>
        /// <param name="Tvurce">Jméno odesílatele</param>
        /// <param name="Text">Obsah zprávy</param>
        private void Vysilani(string Tvurce, string Text) //Odeslání zprávy všem klientům
        {
            try
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text +=
                        "\n" + DateTime.Now.ToShortTimeString() + " " + Tvurce + ": " +
                        Text)); //Vypíše zprávu na serveru
                Text = "0φ" + Tvurce + "φ: " + Text; //Naformátuje zprávu před odesláním                
                var Data = Encoding.Unicode.GetBytes(Text); //Převede zprávu na byty

                foreach (DictionaryEntry Klient in _seznamKlientu)
                {
                    var VysilaniSocket = (TcpClient) Klient.Value; //Nastavení adresy k odeslání
                    var VysilaniProud =
                        VysilaniSocket.GetStream(); //Nastaví odesílací stream na adresu                        
                    VysilaniProud.Write(Data, 0, Data.Length); //Odeslání sériových dat
                    VysilaniProud.Flush(); //Ukončení odesílání
                }
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + "Objevila se chyba:"));
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
                string Jmena = null;

                foreach (DictionaryEntry Klient in _seznamKlientu) Jmena += Klient.Key + ",";

                var Seznam = "3φ" + Jmena; //Naformátuje zprávu před odesláním                
                var Data = Encoding.Unicode.GetBytes(Seznam); //Převede zprávu na byty

                foreach (DictionaryEntry Klient in _seznamKlientu)
                {
                    var VysilaniSocket = (TcpClient) Klient.Value; //Nastavení adresy k odeslání
                    var VysilaniProud =
                        VysilaniSocket.GetStream(); //Nastaví odesílací stream na adresu                        
                    VysilaniProud.Write(Data, 0, Data.Length); //Odeslání sériových dat
                    VysilaniProud.Flush(); //Ukončení odesílání
                }
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += "\n" + DateTime.Now.ToShortTimeString() + " " + "Objevila se chyba:"));
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
            foreach (DictionaryEntry Klient in _seznamKlientu) OdebratKlienta(Klient.Key as string);
            _stop = true;
            _prichoziKomunikace.Stop();
            UvodServeru.ZmenaUdaju = true;
            Close();
        }

        /// <summary>
        ///     Zkontroluje, jestli není připojený uživatel se stejným jménem.
        /// </summary>
        /// <param name="Jmeno">Jméno ke kontrole</param>
        /// <returns>Jméno je v pořádku</returns>
        private bool KontrolaJmena(string Jmeno) //Zkontroluje, zda se jméno již nevyskytuje
        {
            foreach (DictionaryEntry Klient in _seznamKlientu)
                if ((string) Klient.Key == Jmeno)
                    return false;

            return true;
        }

        /// <summary>
        ///     Odpojí klienta ze serveru.
        /// </summary>
        /// <param name="Jmeno">Jméno klienta</param>
        private void OdebratKlienta(string Jmeno)
        {
            --_pocetPripojeni;

            if (InvokeRequired) //Odstraní klienta z výpisu
                Invoke((MethodInvoker) (() => VypisKlientu.Items.Remove(Jmeno)));
            else
                VypisKlientu.Items.Remove(Jmeno);

            Invoke((MethodInvoker) (() => _seznamKlientu.Remove(Jmeno))); //Odstraní klienta ze seznamu
            VypisKlientu.Items.Remove(Jmeno);
            Vysilani("SERVER", Jmeno + " se odpojil(a)"); //Ohlasí odpojení ostatním klientům
            Invoke((MethodInvoker) (() => AktualizaceSeznamu()));
        }

        /// <summary>
        ///     Odpojí klienta ze serveru bez zásahu do seznamů.
        /// </summary>
        /// <param name="Klient">Připojení klienta</param>
        private void OdebratKlienta(TcpClient Klient)
        {
            --_pocetPripojeni;

            Klient.Close();
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
            //VypisChatu.BackColor = Color.
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
                var Soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                if (Soubor.Length < 4194004)
                    try
                    {
                        var Nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) +
                                    Path.GetExtension(VolbaSouboru.FileName);
                        var MetaData = "2φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" +
                                       Path.GetExtension(VolbaSouboru.FileName) + "φ" + Soubor.Length + "φ";
                        var Znacka = Encoding.Unicode.GetBytes(MetaData);
                        var Zprava = new byte[1024 * 1024 * 4];

                        Array.Copy(Znacka, 0, Zprava, 0, Znacka.Length);
                        Array.Copy(Soubor, 0, Zprava, 300, Soubor.Length);

                       

                        
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

            if (data.Length > 1024 * 1024 * 4)
            {

            }
            else
            {
                var metaData = Encoding.Unicode.GetBytes("1φ" + "0φ" + data.Length + "φ" + nazev + "φ" + pripona);
                byte[] odesilanaData = new byte[128 + data.Length];
                
                Array.Copy(metaData, odesilanaData, metaData.Length);
                Array.Copy(data, 0, odesilanaData, 128, data.Length);

                foreach (DictionaryEntry klient in _seznamKlientu)
                {
                    var vysilaniSocket = (TcpClient)klient.Value; 
                    var vysilaniProud = vysilaniSocket.GetStream();
                    vysilaniProud.Write(odesilanaData, 0, odesilanaData.Length);
                    vysilaniProud.Flush();
                }

                Vysilani(odesilatel, "Poslal obrázek " + nazev + pripona);
            }
        }

        private void UlozitObrazek(byte[] data, string nazev, string pripona)
        {
            var slozkaServer = Path.Combine(_slozka, "Server");
            var slozkaDruh = Path.Combine(slozkaServer, "Obrazek");

            if (!SlozkaSouboru(_slozka)) Directory.CreateDirectory(_slozka);

            if (!SlozkaSouboru(slozkaServer)) Directory.CreateDirectory(slozkaServer);

            if (!SlozkaSouboru(slozkaDruh)) Directory.CreateDirectory(slozkaDruh);

            var cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "Stercore soubory", "Server", "Obrazek") + @"\" + nazev + pripona;

            if (File.Exists(cesta))
            {
                var index = 1;

                while (File.Exists(cesta))
                {
                    cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                "Stercore soubory", "Server", "Obrazek") + @"\" + nazev + "(" + index + ")" +
                            pripona;
                    ++index;
                }
            }

            File.WriteAllBytes(cesta, data);
        }
    }
}