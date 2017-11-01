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

namespace PataChat
{
    public partial class Server : MaterialForm
    {
        public Server()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        } //Inicializuje okno a nastaví jeho vzhled

        public static Hashtable SeznamKlientu = new Hashtable();//Seznam připojených uživatelů a jejich adres
        public static IPAddress AdresaServeru = LokalniAdresa();//Nastavení lokální adresy
        public static int PortServeru, PocetKlientu;//Proměná portu serveru a maximální počet klientů(0 znamená neomezený počet)
        public static int PocetPripojeni = 0;//Počet aktuálně připojených uživatelů
        bool Stop = false;//Proměná pro zastavení běhu serveru
        TcpListener PrichoziKomunikace;//Poslouchá příchozí komunikaci a žádosti i připojení
        Thread BehServeru;//Thread pro běh serveru na pozadí nezávisle na hlavním okně

        private void BtnServerStart_Click(object sender, EventArgs e)//Spustí běh serveru
        {
            Stop = false;//Povolí běh serveru
            PortServeru = int.Parse(txtServerPort.Text);//Zadá hodnotu proměné z textboxu
            PrichoziKomunikace = new TcpListener(AdresaServeru, PortServeru);//Nastaví poslouchání žádostí a komunikace na adresu a port
            PocetKlientu = int.Parse(txtPocetKlientu.Text);//Načte maximální počet klientů
            GrpOvladace.Enabled = false;//Vypne úpravu nastavení serveru

            BehServeru = new Thread(PrijmaniKlientu)
            {
                IsBackground = true
            };//Připraví thread a nastaví jej do pozadí

            BehServeru.Start();
        }

        private void PrijmaniKlientu()//Funkce pro přijímaní připojení
        {           

            try
            {
                PrichoziKomunikace.Start();//Spustí poslouchání

                while (PocetKlientu > PocetPripojeni && !Stop)
                {
                    TcpClient Klient = PrichoziKomunikace.AcceptTcpClient();//Přijme žádost o připojení
                    ++PocetPripojeni;
                    byte[] ByteJmeno = new byte[1024 * 1024 * 2];//Bytové pole pro načtení jména
                    NetworkStream CteniJmena = Klient.GetStream();//Připojení načítání na správný socket
                    CteniJmena.Read(ByteJmeno, 0, Klient.ReceiveBufferSize);//Načtení sériových dat
                    string Jmeno = Encoding.UTF8.GetString(ByteJmeno).TrimEnd('\0');//Dekódování dat a vymazání prázdných znaků
                    
                    if(KontrolaJmena(Jmeno))//Kontrola duplikátního jména
                    {
                        SeznamKlientu.Add(Jmeno, Klient);//Přidá klienta do seznamu
                        Invoke((MethodInvoker)(() => VypisKlientu.Items.Add(Jmeno)));//Vypíše klienta do seznamu na serveru
                        Vysilani("SERVER", Jmeno + " se připojil(a)"); //Oznámí všem klientům, že se připojil někdo připojil

                        Thread VlaknoKlienta = new Thread(() => ObsluhaKlienta(Jmeno, Klient))
                        {
                            IsBackground = true
                        };//Připraví thread pro obsluhu klienta a nastaví jej do pozadí

                        VlaknoKlienta.Start();
                    }
                    else
                    {
                        Vysilani("SERVER", Jmeno + " se pokusil(a) připojit. Pokus byl zamítnut - duplikátní jméno");
                        byte[] Oznameni = Encoding.UTF8.GetBytes("Pokus o připojení byl zamítnut - uživatel se stejným jménem je již připojen!");
                        CteniJmena.Write(Oznameni, 0, Oznameni.Length);
                        CteniJmena.Flush();
                        Klient.Close();
                        --PocetPripojeni;
                    }
                }
            }
            catch(Exception x)//Kontrola chyb
            {
                Invoke((MethodInvoker)(() => VypisChatu.Items.Add("Objevila se chyba:")));
                Invoke((MethodInvoker)(() => VypisChatu.Items.Add(x.Message)));//Vypíše chybu na server
            }

            PrichoziKomunikace.Stop();//Konec naslouchání
        }

        private void ObsluhaKlienta(string jmeno, TcpClient Pripojeni)//Naslouchá příchozím zprávám od klienta
        {
            NetworkStream Cteni = Pripojeni.GetStream();//Nastaví naslouchání na správnou adresu
            byte[] HrubaData = new byte[1024 * 1024 * 2];//Pole pro přijímání zpráv
            string Zprava;
            
            try
            {
                while (true)
                {
                    Cteni.Read(HrubaData, 0, Pripojeni.ReceiveBufferSize);//Načtení sériových dat
                    Zprava = Encoding.UTF8.GetString(HrubaData).TrimEnd('\0');//Dekódování a vymazání prázdných znaků
                    Vysilani(jmeno, Zprava);//Vyslání zprávy všem klientům
                }
            }
            catch//Při chybě je klient odpojen
            {
                --PocetPripojeni;
                Pripojeni.Close();//Zavře socket klienta
                Invoke((MethodInvoker)(() => VypisKlientu.Items.Remove(jmeno)));//Odstraní klienta z výpisu
                SeznamKlientu.Remove(jmeno);//Odstraní klienta ze seznamu
                Vysilani("SERVER", jmeno + " se odpojil(a)");//Ohlasí odpojení ostatním klientům
            }            
        }

        private void Vysilani(string Tvurce, string Text)//Odeslání zprávy všem klientům
        {
            try
            {
                Text = string.Concat(Tvurce, ": ", Text);//Naformátuje zprávu před odesláním
                Invoke((MethodInvoker)(() => VypisChatu.Items.Add(Text)));//Vypíše zprávu na serveru
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

        private void BtnServerStop_Click(object sender, EventArgs e)//Ukončení běhu serveru
        {
            Vysilani("SERVER", "Vypínání serveru...");

            foreach(DictionaryEntry Klient in SeznamKlientu)
            {
                (Klient.Value as TcpClient).GetStream().Close();
                (Klient.Value as TcpClient).Close();
            }

            if(InvokeRequired)
            {
                Invoke((MethodInvoker)(() => PrichoziKomunikace.Stop()));
            }
            GrpOvladace.Enabled = true;//Povolí používání nastavení
        }

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

        private void Server_Load(object sender, EventArgs e)
        {
            AdresaServeru = LokalniAdresa();//
            txtServerIP.Text = AdresaServeru.ToString(); //Nastaví lokální adresu do textboxu
        }

        private void txtServerIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnServerStart_Click(null, null);
            }
        }

        public static IPAddress LokalniAdresa()//Získá lokální adresu serveru
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var IP in host.AddressList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                {
                    return IP;
                }
            }

            return IPAddress.Parse("127.0.0.1"); //Pokud není počítač připojen k síti, vrátí loopback adresu
        } //Zjistí lokální adresu klienta
    }
}
