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
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            Shown += new EventHandler(OknoServeru_Shown);
        }

        IPEndPoint IPAdresa;
        Hashtable SeznamKlientu = new Hashtable();//Seznam připojených uživatelů a jejich adres
        int PocetKlientu;//Proměná portu serveru a maximální počet klientů(0 znamená neomezený počet)
        int PocetPripojeni = 0;//Počet aktuálně připojených uživatelů
        bool Stop = false;//Proměná pro zastavení běhu serveru
        TcpListener PrichoziKomunikace;//Poslouchá příchozí komunikaci a žádosti i připojení
        Thread BehServeru;//Thread pro běh serveru na pozadí nezávisle na hlavním okně

        /// <summary>
        /// Po načtení formuláře spustí server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OknoServeru_Shown(Object sender, EventArgs e)
        {
            StartServeru();
        }

        /// <summary>
        /// Nastaví IP adresu a port. Spustí naslouchání serveru pro připojení ve vlastním vlákně.
        /// </summary>
        private void StartServeru()
        {
            Stop = false;//Povolí běh serveru
            PrichoziKomunikace = new TcpListener(IPAdresa);//Nastaví poslouchání žádostí a komunikace na adresu a port

            BehServeru = new Thread(PrijmaniKlientu)
            {
                IsBackground = true
            };

            BehServeru.Start();
        }

        /// <summary>
        /// Přijímá připojení klientů. Duplikátní jména jsou odpojena.
        /// </summary>
        private void PrijmaniKlientu()
        {           
            try
            {
                PrichoziKomunikace.Start();
                Invoke((MethodInvoker)(() => VypisChatu.Items.Add("Server byl spuštěn.")));

                while (!Stop)
                {
                    TcpClient Klient = PrichoziKomunikace.AcceptTcpClient();//Přijme žádost o připojení
                    ++PocetPripojeni;
                    byte[] ByteJmeno = new byte[1024 * 1024 * 2];//Bytové pole pro načtení jména
                    NetworkStream CteniJmena = Klient.GetStream();//Připojení načítání na správný socket
                    CteniJmena.Read(ByteJmeno, 0, Klient.ReceiveBufferSize);//Načtení sériových dat
                    string Jmeno = Encoding.UTF8.GetString(ByteJmeno).TrimEnd('\0');//Dekódování dat a vymazání prázdných znaků

                    if (KontrolaJmena(Jmeno))//Kontrola duplikátního jména
                    {
                        SeznamKlientu.Add(Jmeno, Klient);//Přidá klienta do seznamu
                        Invoke((MethodInvoker)(() => VypisKlientu.Items.Add(Jmeno)));//Vypíše klienta do seznamu na serveru
                        ZmenavSeznamu(Jmeno, true);
                        Vysilani("SERVER", Jmeno + " se připojil(a)"); //Oznámí všem klientům, že se připojil někdo připojil

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
            catch(Exception x)
            {
                if(!Stop)
                {
                    Invoke((MethodInvoker)(() => VypisChatu.Items.Add("Objevila se chyba:")));
                    Invoke((MethodInvoker)(() => VypisChatu.Items.Add(x.Message)));
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
                string Zprava;

                try
                {
                    while (!Stop)
                    {
                        HrubaData = new byte[1024 * 1024 * 2];
                        Cteni.Read(HrubaData, 0, Pripojeni.ReceiveBufferSize);//Načtení sériových dat     
                        Zprava = Encoding.UTF8.GetString(HrubaData).TrimEnd('\0');//Dekódování a vymazání prázdných znaků
                        string[] Uprava = Zprava.Split('φ');

                        switch (Uprava[0])
                        {
                            case "0"://Běžná zpráva
                                {
                                    Vysilani(jmeno, Uprava[1]);//Vyslání zprávy všem klientům
                                    break;
                                }
                            case "1"://TODO: Obrázek
                                {
                                    byte[] Data = new byte[int.Parse(Uprava[2])];
                                    MessageBox.Show(Uprava[1]);

                                    using (FileStream Obrazek = new FileStream("Obrazek.jpg", FileMode.Create, FileAccess.Write))
                                    {
                                       while(Cteni.Read(Data, 0, Data.Length) > 0)
                                       {
                                           Obrazek.Write(Data, 0, Data.Length);
                                       }
                                    }

                                    break;
                                }
                            case "2"://TODO: Soubor
                                {
                                    break;
                                }
                            case "3"://TODO: Seznam klientů
                                {
                                    break;
                                }
                            case "4"://TODO: Odpojení
                                {
                                    OdebratKlienta(jmeno);
                                    break;
                                }
                        }
                    }
                }
                catch//Při chybě je klient odpojen
                {
                    OdebratKlienta(jmeno);
                }
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
                Invoke((MethodInvoker)(() => VypisChatu.Items.Add(Tvurce + ": " + Text)));//Vypíše zprávu na serveru
                Text = ("0φ" + Tvurce + "φ: " + Text);//Naformátuje zprávu před odesláním                
                byte[] Data = Encoding.UTF8.GetBytes(Text);//Převede zprávu na byty

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
                Invoke((MethodInvoker)(() => VypisKlientu.Items.Add("Objevila se chyba:")));
                Invoke((MethodInvoker)(() => VypisKlientu.Items.Add(x.Message)));//Vypíše chybu na server
            }
        }

        private void ZmenavSeznamu(string Jmeno, bool Akce)
        {
            string Zmena;

            if (Akce)
            {
                Zmena = ("3φ" + Jmeno + "φ" + true);                
            }
            else
            {
                Zmena = ("3φ" + Jmeno + "φ" + false);
            }

            byte[] Data = Encoding.UTF8.GetBytes(Zmena);

            foreach (DictionaryEntry Klient in SeznamKlientu)
            {
                TcpClient VysilaniSocket = (TcpClient)Klient.Value;
                NetworkStream VysilaniProud = VysilaniSocket.GetStream();                      
                VysilaniProud.Write(Data, 0, Data.Length);
                VysilaniProud.Flush();
            }
        }

        private void PredaniSeznamu(string Jmeno)
        {
            using (TcpClient SocketSeznamu = (TcpClient)SeznamKlientu[Jmeno])
            {
                NetworkStream VysilaniSeznamu = SocketSeznamu.GetStream();

                foreach (DictionaryEntry Klient in SeznamKlientu)
                {
                    if (Klient.Key != Jmeno)
                    {
                        byte[] Data = Encoding.UTF8.GetBytes("3φ" + Klient.Key + "φ" + true);
                    }
                }
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
                    return false;//V seznamu se již jméno nachází
                }
            }

            return true;//V seznamu se nenachází
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

            ZmenavSeznamu(Jmeno, false);
            Invoke((MethodInvoker)(() => SeznamKlientu.Remove(Jmeno)));//Odstraní klienta ze seznamu
            VypisKlientu.Items.Remove(Jmeno);
            Vysilani("SERVER", Jmeno + " se odpojil(a)");//Ohlasí odpojení ostatním klientům
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
    }
}
