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
using SpolecneSoubory;
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

        public static Hashtable SeznamKlientu = new Hashtable();
        public static IPAddress AdresaServeru = LokalniAdresa();
        int PortServeru, PocetKlientu;
        bool Stop = false;
        TcpListener PrichoziKomunikace;
        Thread BehServeru;        

        private void BtnServerStart_Click(object sender, EventArgs e)
        {
            Stop = false;
            PortServeru = int.Parse(txtServerPort.Text);
            PrichoziKomunikace = new TcpListener(LokalniAdresa(), PortServeru);
            PocetKlientu = int.Parse(txtPocetKlientu.Text);
            GrpOvladace.Enabled = false;

            BehServeru = new Thread(PrijmaniKlientu)
            {
                IsBackground = true
            };

            BehServeru.Start();
        }

        private void PrijmaniKlientu()
        {
            int PocetPripojeni = 0;

            try
            {
                PrichoziKomunikace.Start();

                while (PocetKlientu > PocetPripojeni && !Stop)
                {
                    TcpClient Klient = PrichoziKomunikace.AcceptTcpClient();
                    ++PocetPripojeni;
                    byte[] ByteJmeno = new byte[1024 * 1024 * 2];
                    NetworkStream CteniJmena = Klient.GetStream();
                    CteniJmena.Read(ByteJmeno, 0, Klient.ReceiveBufferSize);
                    string Jmeno = Encoding.UTF8.GetString(ByteJmeno).TrimEnd('\0'); ;
                    SeznamKlientu.Add(Jmeno, Klient);
                    Invoke((MethodInvoker)(() => VypisKlientu.Items.Add(Jmeno)));

                    Thread VlaknoKlienta = new Thread(() => ObsluhaKlienta(Jmeno, Klient))
                    {
                        IsBackground = true
                    };
                    VlaknoKlienta.Start();
                }
            }
            catch(Exception x)
            {
                Invoke((MethodInvoker)(() => VypisKlientu.Items.Add("Objevila se chyba:")));
                Invoke((MethodInvoker)(() => VypisKlientu.Items.Add(x.Message)));
            }

            PrichoziKomunikace.Stop();
        }

        private void ObsluhaKlienta(string jmeno, TcpClient Pripojeni)
        {
            NetworkStream Cteni = Pripojeni.GetStream();
            byte[] HrubaData = new byte[1024 * 1024 * 2];
            string Zprava;

            while(true)
            {
                Cteni.Read(HrubaData, 0, Pripojeni.ReceiveBufferSize);
                Zprava = Encoding.UTF8.GetString(HrubaData).TrimEnd('\0');
                Vysilani(jmeno, Zprava);
            }
        }

        private void Vysilani(string Tvurce, string Text)
        {
            try
            {
                foreach (DictionaryEntry Klient in SeznamKlientu)
                {
                    TcpClient VysilaniSocket = (TcpClient)Klient.Value;
                    NetworkStream VysilaniProud = VysilaniSocket.GetStream();
                    Text = Text.Trim();
                    Text = string.Concat(Tvurce, ": ", Text);
                    Invoke((MethodInvoker)(() => VypisChatu.Items.Add(Text)));
                    byte[] Data = Encoding.UTF8.GetBytes(Text);
                    VysilaniProud.Write(Data, 0, Data.Length);
                    VysilaniProud.Flush();
                }
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message, "Průser, šéfe!");
            }
        }

        private void BtnServerStop_Click(object sender, EventArgs e)
        {
            GrpOvladace.Enabled = true;
            Stop = true;
        }

        private void Server_Load(object sender, EventArgs e)
        {
            AdresaServeru = LokalniAdresa();
            txtServerIP.Text = AdresaServeru.ToString(); //Nastaví lokální adresu do textboxu
        }

        public static IPAddress LokalniAdresa()
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
