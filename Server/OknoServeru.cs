using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Animations;
using MaterialSkin.Controls;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Collections;

namespace SterCore
{
    public partial class OknoServeru : MaterialForm
    {
        public OknoServeru()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        public OknoServeru(IPAddress Adresa, int Port, int Pocet)
        {
            InitializeComponent();

            IPAdresa = new IPEndPoint(Adresa, Port);
            PocetKlientu = Pocet;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red700, Primary.Red900, Primary.Red100, Accent.Red400, TextShade.WHITE);
        }

        IPEndPoint IPAdresa;
        Hashtable SeznamKlientu = new Hashtable();//Seznam připojených uživatelů a jejich adres
        int PocetKlientu;//Proměná portu serveru a maximální počet klientů(0 znamená neomezený počet)
        int PocetPripojeni = 0;//Počet aktuálně připojených uživatelů
        bool Stop = false;//Proměná pro zastavení běhu serveru
        TcpListener PrichoziKomunikace;//Poslouchá příchozí komunikaci a žádosti i připojení
        Thread BehServeru;//Thread pro běh serveru na pozadí nezávisle na hlavním okně
        string Slozka = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stercore soubory");

        /// <summary>
        /// Přijímá připojení klientů. Duplikátní jména jsou odpojena.
        /// </summary>
        private void PrijmaniKlientu()
        {           
            try
            {
                PrichoziKomunikace.Start();
                Invoke((MethodInvoker)(() => VypisChatu.Text += DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + "Server byl spuštěn."));

                while (!Stop)
                {
                    if(PrichoziKomunikace.Pending() && MaximalniPocet())
                    {
                        TcpClient Klient = PrichoziKomunikace.AcceptTcpClient();//Přijme žádost o připojení
                        ++PocetPripojeni;
                        byte[] ByteJmeno = new byte[1024 * 1024 * 2];//Bytové pole pro načtení jména
                        NetworkStream CteniJmena = Klient.GetStream();//Připojení načítání na správný socket
                        CteniJmena.Read(ByteJmeno, 0, Klient.ReceiveBufferSize);//Načtení sériových dat
                        string Jmeno = Encoding.Unicode.GetString(ByteJmeno).TrimEnd('\0');//Dekódování dat a vymazání prázdných znaků

                        if (KontrolaJmena(Jmeno))
                        {
                            SeznamKlientu.Add(Jmeno, Klient);
                            Invoke((MethodInvoker)(() => VypisKlientu.Items.Add(Jmeno)));
                            Vysilani("SERVER", Jmeno + " se připojil(a)"); 
                            Invoke((MethodInvoker)(() => AktualizaceSeznamu()));

                            Thread VlaknoKlienta = new Thread(() => ObsluhaKlienta(Jmeno, Klient))
                            {
                                IsBackground = true
                            };

                            VlaknoKlienta.Start();
                        }
                        else
                        {
                            Vysilani("SERVER", Jmeno + " se pokusil(a) připojit. Pokus byl zamítnut - duplikátní jméno");
                            CteniJmena.Flush();
                            OdebratKlienta(Klient);
                        }
                    }                    
                }
            }
            catch(Exception x)
            {
                if(!Stop)
                {
                    Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + "Objevila se chyba:"));
                    Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + x.Message));
                }                
            }
        }

        /// <summary>
        /// Naslouchá příchozím zprávám od klienta.
        /// </summary>
        /// <param name="jmeno">Jméno klienta</param>
        /// <param name="Pripojeni">Připojení klienta</param>
        private void ObsluhaKlienta(string jmeno, TcpClient Pripojeni)//Naslouchá příchozím zprávám od klienta
        {
            using (NetworkStream Cteni = Pripojeni.GetStream())//Nastaví naslouchání na správnou adresu
            {
                byte[] HrubaData;//Pole pro přijímání zpráv

                try
                {
                    while (!Stop)
                    {
                        HrubaData = new byte[1024 * 1024 * 4];
                        Cteni.Read(HrubaData, 0, Pripojeni.ReceiveBufferSize);//Načtení sériových dat     

                        byte[] Znacka = new byte[4];
                        Array.Copy(HrubaData, Znacka, 4); //Zkopíruje první tři bajty z hrubých dat  
                        string Uprava = Encoding.Unicode.GetString(Znacka);

                        switch (Uprava[0])
                        {
                            case '0'://Běžná zpráva
                                {
                                    string Data = Encoding.Unicode.GetString(HrubaData).TrimEnd('\0');
                                    string[] Zprava = Data.Split('φ');
                                    Vysilani(jmeno, Zprava[1]);//Vyslání zprávy všem klientům
                                    break;
                                }
                            case '1'://TODO: Obrázek
                                {
                                    ZpracovaniSouboru(HrubaData, "Obrazek");


                                    break;
                                }
                            case '2'://TODO: Soubor
                                {
                                    ZpracovaniSouboru(HrubaData, "Soubor");

                                    break;
                                }
                            case '3'://Seznam klientů - server nevyužívá
                                {
                                    break;
                                }
                            case '4'://TODO: Odpojení
                                {
                                    OdebratKlienta(jmeno);
                                    break;
                                }
                        }
                    }
                }
                catch(Exception x)//Při chybě je klient odpojen
                {
                    MessageBox.Show(x.Message);
                    MessageBox.Show(x.StackTrace);
                    OdebratKlienta(jmeno);
                }
            }                
        }

        /// <summary>
        /// Odešle soubor všem klientům.
        /// </summary>
        /// <param name="Data">Data souboru</param>
        /// <param name="Tvurce">Odesílatel souboru</param>
        private void VysilaniObrazku(byte[] Data, string Tvurce, string Nazev)
        {
            try
            {
                foreach (DictionaryEntry Klient in SeznamKlientu)
                {
                    TcpClient VysilaniSocket = (TcpClient)Klient.Value;
                    NetworkStream VysilaniProud = VysilaniSocket.GetStream();                   
                    VysilaniProud.Write(Data, 0, Data.Length);
                    VysilaniProud.Flush();
                }

                Vysilani(Tvurce, "Poslal obrázek (" + Nazev + ")");
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + "Objevila se chyba:"));
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + x.Message));
            }
        }

        private void VysilaniSouboru(byte[] Data, string Tvurce, string Nazev)
        {
            try
            {
                foreach (DictionaryEntry Klient in SeznamKlientu)
                {
                    TcpClient VysilaniSocket = (TcpClient)Klient.Value;
                    NetworkStream VysilaniProud = VysilaniSocket.GetStream();
                    VysilaniProud.Write(Data, 0, Data.Length);
                    VysilaniProud.Flush();
                }

                Vysilani(Tvurce, "Poslal soubor (" + Nazev + ")");
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + "Objevila se chyba:"));
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + x.Message));
            }
        }

        /// <summary>
        /// Zjistí, zda zadaný adresář existuje.
        /// </summary>
        /// <param name="Cesta">Cesta k adresáři</param>
        /// <returns>True - složka existuje, False - složka neexistuje</returns>
        private bool SlozkaSouboru(string Cesta)
        {
            if (Directory.Exists(Cesta))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Uloží přijatý soubor do příslušné složky.
        /// </summary>
        /// <param name="Data">Přijatá data souboru</param>
        /// <param name="Druh">Určuje, zda se jedná o obrázek nebo o soubor jiného druhu.</param>
        private void ZpracovaniSouboru(byte[] Data, string Druh)
        {
            byte[] Soubor = new byte[1024 * 1024 * 4];
            byte[] Nazev = new byte[264];


            Array.Copy(Data, 264, Soubor, 0, 4194040);
            Array.Copy(Data, 0, Nazev, 0, 264);

            string Prevod = Encoding.Unicode.GetString(Nazev).TrimEnd('\0');
            string[] NazevSouboru = Prevod.Split('φ');
            string SlozkaServer = Path.Combine(Slozka, "Server");
            string SlozkaDruh = Path.Combine(SlozkaServer, Druh);

            if (!SlozkaSouboru(Slozka))
            {
                Directory.CreateDirectory(Slozka);
            }

            if(!SlozkaSouboru(SlozkaServer))
            {
                Directory.CreateDirectory(SlozkaServer);
            }

            if(!SlozkaSouboru(SlozkaDruh))
            {
                Directory.CreateDirectory(SlozkaDruh);
            }

            string Cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stercore soubory", "Server", Druh) + @"\" + NazevSouboru[1] + NazevSouboru[2];

            if (File.Exists(Cesta))
            {
                int Index = 1;

                while (File.Exists(Cesta))
                {
                    Cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stercore soubory", "Server", Druh) + @"\" + NazevSouboru[1] + "(" + Index.ToString() + ")" + NazevSouboru[2];
                    ++Index;
                }
            }

            using (MemoryStream UlozeniSoubour = new MemoryStream(Soubor))
            {
                File.WriteAllBytes(Cesta, Soubor);
            }                
        }

        /// <summary>
        /// Odešle zprávu všem připojeným klientům.
        /// </summary>
        /// <param name="Tvurce">Jméno odesílatele</param>
        /// <param name="Text">Obsah zprávy</param>
        private void Vysilani(string Tvurce, string Text)//Odeslání zprávy všem klientům
        {
            try
            {
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + Tvurce + ": " + Text));//Vypíše zprávu na serveru
                Text = ("0φ" + Tvurce + "φ: " + Text);//Naformátuje zprávu před odesláním                
                byte[] Data = Encoding.Unicode.GetBytes(Text);//Převede zprávu na byty

                foreach (DictionaryEntry Klient in SeznamKlientu)
                {
                    TcpClient VysilaniSocket = (TcpClient)Klient.Value;//Nastavení adresy k odeslání
                    NetworkStream VysilaniProud = VysilaniSocket.GetStream();//Nastaví odesílací stream na adresu                        
                    VysilaniProud.Write(Data, 0, Data.Length);//Odeslání sériových dat
                    VysilaniProud.Flush();//Ukončení odesílání
                }
            }
            catch(Exception x)
            {
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + "Objevila se chyba:"));
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + x.Message));
            }
        }

        /// <summary>
        /// Odešle všem klientům aktualizovaný seznam všech připojených klientů.
        /// </summary>
        private void AktualizaceSeznamu()
        {
            try
            {
                string Jmena = null;

                foreach (DictionaryEntry Klient in SeznamKlientu)
                {
                    Jmena += Klient.Key + ",";
                }

                string Seznam = ("3φ" + Jmena);//Naformátuje zprávu před odesláním                
                byte[] Data = Encoding.Unicode.GetBytes(Seznam);//Převede zprávu na byty

                foreach (DictionaryEntry Klient in SeznamKlientu)
                {
                    TcpClient VysilaniSocket = (TcpClient)Klient.Value;//Nastavení adresy k odeslání
                    NetworkStream VysilaniProud = VysilaniSocket.GetStream();//Nastaví odesílací stream na adresu                        
                    VysilaniProud.Write(Data, 0, Data.Length);//Odeslání sériových dat
                    VysilaniProud.Flush();//Ukončení odesílání
                }
            }
            catch (Exception x)
            {
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + "Objevila se chyba:"));
                Invoke((MethodInvoker)(() => VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " +  x.Message));
            }
        }

        /// <summary>
        /// Ukončí všechna připojení, vypne server a vrtáí se na úvodní form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnServerStop_Click(object sender, EventArgs e)//Ukončení běhu serveru
        {
            foreach (DictionaryEntry Klient in SeznamKlientu)
            {
                (Klient.Value as TcpClient).Close();
            }

            Stop = true;
            PrichoziKomunikace.Stop();
            UvodServeru.ZmenaUdaju = true;
            Close();    
        }

        /// <summary>
        /// Zkontroluje, jestli není připojený uživatel se stejným jménem.
        /// </summary>
        /// <param name="Jmeno">Jméno ke kontrole</param>
        /// <returns>Jméno je v pořádku</returns>
        private bool KontrolaJmena(string Jmeno)//Zkontroluje, zda se jméno již nevyskytuje
        {
            foreach(DictionaryEntry Klient in SeznamKlientu)
            {
                if ((string)Klient.Key == Jmeno)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Odpojí klienta ze serveru.
        /// </summary>
        /// <param name="Jmeno">Jméno klienta</param>
        private void OdebratKlienta(string Jmeno)
        {
            --PocetPripojeni;

            if (InvokeRequired)//Odstraní klienta z výpisu
            {
                Invoke((MethodInvoker)(() => VypisKlientu.Items.Remove(Jmeno)));
            }
            else
            {
                VypisKlientu.Items.Remove(Jmeno);
            }

            Invoke((MethodInvoker)(() => SeznamKlientu.Remove(Jmeno)));//Odstraní klienta ze seznamu
            VypisKlientu.Items.Remove(Jmeno);
            Vysilani("SERVER", Jmeno + " se odpojil(a)");//Ohlasí odpojení ostatním klientům
            Invoke((MethodInvoker)(() => AktualizaceSeznamu()));
        }

        /// <summary>
        /// Odpojí klienta ze serveru bez zásahu do seznamů.
        /// </summary>
        /// <param name="Klient">Připojení klienta</param>
        private void OdebratKlienta(TcpClient Klient)
        {
            --PocetPripojeni;

            Klient.Close();
            
        }

        /// <summary>
        /// Odešle zprávu ze serveru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnZprava_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(TxtZprava.Text))
            {
                Vysilani("Server", TxtZprava.Text);
                TxtZprava.Text = null;
                TxtZprava.Focus();
                TxtZprava.SelectAll();
            }           
        }

        /// <summary>
        /// Odeslání zprávy pomocí enteru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtZprava_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnZprava_Click(null, null);
            }
        }

        /// <summary>
        /// Nastaví vzhled okna a spustí server po načtení.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OknoServeru_Load(object sender, EventArgs e)
        {
            VypisChatu.BackColor = Color.White;
            Stop = false;
            PrichoziKomunikace = new TcpListener(IPAdresa);

            BehServeru = new Thread(PrijmaniKlientu)
            {
                IsBackground = true
            };

            BehServeru.Start();
        }

        /// <summary>
        /// Rozhodne, zda nebyl překročen počet připojení, pokud byl nastaven.
        /// 0 = neomezený počet připojení.
        /// </summary>
        /// <returns>True = je možné přidat další připojení</returns>
        private bool MaximalniPocet()
        {
            if(PocetKlientu == 0 || PocetPripojeni < PocetKlientu)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Při zapsání nové zprávy skočí na poslední zprávu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VypisChatu_TextChanged(object sender, EventArgs e)
        {
            VypisChatu.SelectionStart = VypisChatu.Text.Length;
            VypisChatu.ScrollToCaret();
        }

        /// <summary>
        /// Po kliknutí na odkaz otevře webovou stránku v prohlížeči.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Odkaz webové stránky</param>
        private void VypisChatu_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void OdeslaniObrazku_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = "Obrázky|*.jpg;*.png;*.gif;*.jpeg;*.jpe;*.bmp";

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                byte[] Obrazek = File.ReadAllBytes(VolbaSouboru.FileName);

                if (Obrazek.Length < 4194040)
                {
                    try
                    {
                        string Nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + Path.GetExtension(VolbaSouboru.FileName);
                        string Cesta = "1φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" + Path.GetExtension(VolbaSouboru.FileName) + "φ";
                        byte[] Znacka = Encoding.Unicode.GetBytes(Cesta);
                        byte[] Zprava = new byte[1024 * 1024 * 4];

                        Array.Copy(Znacka, 0, Zprava, 0, Znacka.Length);
                        Array.Copy(Obrazek, 0, Zprava, 264, Obrazek.Length);

                        ZpracovaniSouboru(Zprava, "Obrazek");

                        VysilaniObrazku(Zprava, "SERVER", Nazev);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.StackTrace);
                        MessageBox.Show(x.Message);
                    }

                }
                else
                {
                    MessageBox.Show("Zvolený obrázek je pro přenos příliš velký!", "Chyba!");
                }
            }
        }

        private void OdeslaniSouboru_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = null;

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                byte[] Soubor = File.ReadAllBytes(VolbaSouboru.FileName);

                if (Soubor.Length < 4194040)
                {
                    try
                    {
                        string Nazev = Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + Path.GetExtension(VolbaSouboru.FileName);
                        string Cesta = "1φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" + Path.GetExtension(VolbaSouboru.FileName) + "φ";
                        byte[] Znacka = Encoding.Unicode.GetBytes(Cesta);
                        byte[] Zprava = new byte[1024 * 1024 * 4];

                        Array.Copy(Znacka, 0, Zprava, 0, Znacka.Length);
                        Array.Copy(Soubor, 0, Zprava, 264, Soubor.Length);

                        ZpracovaniSouboru(Zprava, "Soubor");

                        VysilaniSouboru(Zprava, "SERVER", Nazev);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.StackTrace);
                        MessageBox.Show(x.Message);
                    }

                }
                else
                {
                    MessageBox.Show("Zvolený soubor je pro přenos příliš velký!", "Chyba!");
                }
            }
        }
    }
}
