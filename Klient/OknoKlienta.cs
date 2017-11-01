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
using System.Threading;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using SpolecneSoubory;

namespace PataChat
{
    public partial class Klient : MaterialForm
    {
        public Klient()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        } //Inicializuje okno a nastaví jeho vzhled

        string Prezdivka;
        TcpClient Komunikace = new TcpClient();
        NetworkStream Prijem, Odesilani = default(NetworkStream);

        private void BtnPrezdivka_Click(object sender, EventArgs e) //Nastaveni prezdivky klienta
        {
            if(txtPrezdivka.Text.Length != 0)
            {
                Prezdivka = txtPrezdivka.Text;
                GrpPrezdivka.Enabled = false; //Vypnuti zadavani prezdivky
                GrpPripojeni.Enabled = true; //Zapnuti tlacitka pro pripojeni
            }
            else
            {
                MessageBox.Show("Musí být zadána přezdívka!", "Chyba!");
            }
        }

        private void BtnKlientPripojeni_Click(object sender, EventArgs e) //Pripojeni klienta
        {            
            try
            {
                IPEndPoint AdresaServeru = new IPEndPoint(IPAddress.Parse(txtServerIP.Text), int.Parse(txtServerPort.Text));
                Komunikace.Connect(AdresaServeru);

                VypisChatu.Items.Add("Připojuji se k serveru...");

                if(Komunikace.Connected)
                {
                    Odesilani = Komunikace.GetStream();
                    VypisChatu.Items.Add("Připojení bylo úspěšné!");
                    byte[] Jmeno = Encoding.UTF8.GetBytes(Prezdivka);
                    Odesilani.Write(Jmeno, 0, Jmeno.Length);
                    Odesilani.Flush();

                    GrpPripojeni.Enabled = false;
                    GrpZpravy.Enabled = true;

                    Thread Prijmani = new Thread(PrijmaniZprav)
                    {
                        IsBackground = true
                    };
                    Prijmani.Start();
                }
            }
            catch(Exception x)
            {
                VypisChatu.Items.Add("Objevila se chyba: ");
                VypisChatu.Items.Add(x.Message);
            }
        }

        private void PrijmaniZprav()
        {
            while(true)
            {
                Prijem = Komunikace.GetStream();

                byte[] Data = new byte[1024 * 1024 * 2];

                Prijem.Read(Data, 0, Komunikace.ReceiveBufferSize);
                string Zprava = Encoding.UTF8.GetString(Data).TrimEnd('\0');

                Vypsani(Zprava);
            }
        }

        private void Vypsani(string Text)
        {
            if(InvokeRequired)
            {
                Invoke((MethodInvoker)(() => Vypsani(Text)));
            }
            else
            {
                VypisChatu.Items.Add(Text);
            }
        }

        private void Klient_Load(object sender, EventArgs e)
        {
            GrpPripojeni.Enabled = false;
            GrpZpravy.Enabled = false;
        }

        private void BtnOdeslat_Click(object sender, EventArgs e) //Odeslani zpravy
        {
            byte[] Zprava = Encoding.UTF8.GetBytes(txtZprava.Text.Trim());
            Odesilani.Write(Zprava, 0, Zprava.Length);
            Odesilani.Flush();
        }
    }
}