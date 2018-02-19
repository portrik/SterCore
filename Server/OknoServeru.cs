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
using SterCore;

namespace SterCore
{
    public partial class OknoServeru : MaterialForm
    {
        private Thread BehServeru; //Thread pro běh serveru na pozadí nezávisle na hlavním okně

        private readonly IPEndPoint IPAdresa;
        private readonly int PocetKlientu; //Proměná portu serveru a maximální počet klientů(0 znamená neomezený počet)
        private int PocetPripojeni; //Počet aktuálně připojených uživatelů
        private TcpListener PrichoziKomunikace; //Poslouchá příchozí komunikaci a žádosti i připojení
        private readonly Hashtable SeznamKlientu = new Hashtable(); //Seznam připojených uživatelů a jejich adres

        private readonly string Slozka = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Stercore soubory");

        private bool Stop; //Proměná pro zastavení běhu serveru

        public OknoServeru()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = UvodServeru.Tema;
            materialSkinManager.ColorScheme = UvodServeru.Vzhled;

            PocetKlientu = UvodServeru.PocetPripojeni;
            IPAdresa = new IPEndPoint(UvodServeru.LokalniAdresa(), UvodServeru.Port);
        }

        /// <summary>
        ///     Přijímá připojení klientů. Duplikátní jména jsou odpojena.
        /// </summary>
        private void PrijmaniKlientu()
        {
            try
            {
                PrichoziKomunikace.Start();
                Invoke((MethodInvoker) (() =>
                    VypisChatu.Text += DateTime.Now.ToShortTimeString() + " " + "Server byl spuštěn."));

                while (!Stop)
                    if (PrichoziKomunikace.Pending() && MaximalniPocet())
                    {
                        var Klient = PrichoziKomunikace.AcceptTcpClient(); //Přijme žádost o připojení
                        ++PocetPripojeni;
                        var ByteJmeno = new byte[1024 * 1024 * 2]; //Bytové pole pro načtení jména
                        var CteniJmena = Klient.GetStream(); //Připojení načítání na správný socket
                        CteniJmena.Read(ByteJmeno, 0, Klient.ReceiveBufferSize); //Načtení sériových dat
                        var Jmeno = Encoding.Unicode.GetString(ByteJmeno)
                            .TrimEnd('\0'); //Dekódování dat a vymazání prázdných znaků

                        if (KontrolaJmena(Jmeno))
                        {
                            SeznamKlientu.Add(Jmeno, Klient);
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
                if (!Stop)
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
                    while (!Stop)
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
                                ZpracovaniSouboru(HrubaData, "Obrazek", out NazevSouboru);
                                VysilaniObrazku(HrubaData, jmeno, NazevSouboru);

                                break;
                            }
                            case '2': //Soubor
                            {
                                ZpracovaniSouboru(HrubaData, "Soubor", out NazevSouboru);
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
                catch (Exception x) //Při chybě je klient odpojen
                {
                    MessageBox.Show(x.Message);
                    MessageBox.Show(x.StackTrace);
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
                foreach (DictionaryEntry Klient in SeznamKlientu)
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
                foreach (DictionaryEntry Klient in SeznamKlientu)
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
            var SlozkaServer = Path.Combine(Slozka, "Server");
            var SlozkaDruh = Path.Combine(SlozkaServer, Druh);
            var Soubor = new byte[DelkaSouboru];

            Array.Copy(Data, 300, Soubor, 0, DelkaSouboru);

            if (!SlozkaSouboru(Slozka)) Directory.CreateDirectory(Slozka);

            if (!SlozkaSouboru(SlozkaServer)) Directory.CreateDirectory(SlozkaServer);

            if (!SlozkaSouboru(SlozkaDruh)) Directory.CreateDirectory(SlozkaDruh);

            var Cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "Stercore soubory", "Server", Druh) + @"\" + NazevSouboru[1] + NazevSouboru[2];

            MessageBox.Show(Cesta);
            MessageBox.Show(DelkaSouboru.ToString());
            MessageBox.Show(Data.Length.ToString());

            if (File.Exists(Cesta))
            {
                var Index = 1;

                while (File.Exists(Cesta))
                {
                    Cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                "Stercore soubory", "Server", Druh) + @"\" + NazevSouboru[1] + "(" + Index + ")" +
                            NazevSouboru[2];
                    ++Index;
                }
            }

            File.WriteAllBytes(Cesta, Soubor);
        }

        private void ZpracovaniSouboru(byte[] Data, string Druh, out string nazevsouboru)
        {
            var Hlavicka = new byte[300];

            Array.Copy(Data, 0, Hlavicka, 0, 300);

            var Prevod = Encoding.Unicode.GetString(Hlavicka).TrimEnd('\0');
            var NazevSouboru = Prevod.Split('φ');
            nazevsouboru = NazevSouboru[1] + NazevSouboru[2];
            var DelkaSouboru = int.Parse(NazevSouboru[3]);
            var SlozkaServer = Path.Combine(Slozka, "Klient");
            var SlozkaDruh = Path.Combine(SlozkaServer, Druh);
            var Soubor = new byte[DelkaSouboru];

            MessageBox.Show(DelkaSouboru.ToString());
            MessageBox.Show(Prevod.Length.ToString());

            Array.Copy(Data, 300, Soubor, 0, DelkaSouboru);

            if (!SlozkaSouboru(Slozka)) Directory.CreateDirectory(Slozka);

            if (!SlozkaSouboru(SlozkaServer)) Directory.CreateDirectory(SlozkaServer);

            if (!SlozkaSouboru(SlozkaDruh)) Directory.CreateDirectory(SlozkaDruh);

            var Cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "Stercore soubory", "Server", Druh) + @"\" + NazevSouboru[1] + NazevSouboru[2];

            if (File.Exists(Cesta))
            {
                var Index = 1;

                while (File.Exists(Cesta))
                {
                    Cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                "Stercore soubory", "Server", Druh) + @"\" + NazevSouboru[1] + "(" + Index + ")" +
                            NazevSouboru[2];
                    ++Index;
                }
            }

            File.WriteAllBytes(Cesta, Soubor);
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

                foreach (DictionaryEntry Klient in SeznamKlientu)
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

                foreach (DictionaryEntry Klient in SeznamKlientu) Jmena += Klient.Key + ",";

                var Seznam = "3φ" + Jmena; //Naformátuje zprávu před odesláním                
                var Data = Encoding.Unicode.GetBytes(Seznam); //Převede zprávu na byty

                foreach (DictionaryEntry Klient in SeznamKlientu)
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
            foreach (DictionaryEntry Klient in SeznamKlientu) (Klient.Value as TcpClient).Close();

            Stop = true;
            PrichoziKomunikace.Stop();
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
            foreach (DictionaryEntry Klient in SeznamKlientu)
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
            --PocetPripojeni;

            if (InvokeRequired) //Odstraní klienta z výpisu
                Invoke((MethodInvoker) (() => VypisKlientu.Items.Remove(Jmeno)));
            else
                VypisKlientu.Items.Remove(Jmeno);

            Invoke((MethodInvoker) (() => SeznamKlientu.Remove(Jmeno))); //Odstraní klienta ze seznamu
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
            --PocetPripojeni;

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
            Stop = false;
            PrichoziKomunikace = new TcpListener(IPAdresa);

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

        BehServeru = new Thread(PrijmaniKlientu)
            {
                IsBackground = true
            };

            BehServeru.Start();
        }

        /// <summary>
        ///     Rozhodne, zda nebyl překročen počet připojení, pokud byl nastaven.
        ///     0 = neomezený počet připojení.
        /// </summary>
        /// <returns>True = je možné přidat další připojení</returns>
        private bool MaximalniPocet()
        {
            if (PocetKlientu == 0 || PocetPripojeni < PocetKlientu)
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

                        ZpracovaniSouboru(Zprava, "Obrazek");

                        VysilaniObrazku(Zprava, "SERVER", Nazev);
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

                        ZpracovaniSouboru(Zprava, "Soubor");

                        VysilaniSouboru(Zprava, "SERVER", Nazev);
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
}