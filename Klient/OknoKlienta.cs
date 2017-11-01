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

        string Prezdivka;//Přezdívka klienta
        TcpClient Komunikace = new TcpClient();//TcpClient pro komunkaci se serverem
        NetworkStream Prijem, Odesilani = default(NetworkStream);//Proudy pro přijímání a odesílání informací

        Thread Prijmani;

        private void BtnPrezdivka_Click(object sender, EventArgs e) //Nastavení přezdívky klienta
        {
            if(txtPrezdivka.Text.Length != 0)
            {
                Prezdivka = txtPrezdivka.Text;
                GrpPrezdivka.Enabled = false; //Vypnutí zadávání přezdívky
                GrpPripojeni.Enabled = true; //Zapnutí možností připojení
            }
            else
            {
                MessageBox.Show("Musí být zadána přezdívka!", "Chyba!");
            }
        }

        private void BtnKlientPripojeni_Click(object sender, EventArgs e) //Připojení klienta k serveru
        {            
            try
            {
                IPEndPoint AdresaServeru = new IPEndPoint(IPAddress.Parse(txtServerIP.Text), int.Parse(txtServerPort.Text));//Zpracování adresy a portu
                Komunikace.Connect(AdresaServeru);//Pokus o připojení na zadanou adresu a port

                VypisChatu.Items.Add("Připojuji se k serveru...");

                if(Komunikace.Connected)
                {
                    Odesilani = Komunikace.GetStream();//Nastavení proudu na adresu
                    VypisChatu.Items.Add("Připojení bylo úspěšné!");
                    byte[] Jmeno = Encoding.UTF8.GetBytes(Prezdivka);//Převedení přezdívky na byty
                    Odesilani.Write(Jmeno, 0, Jmeno.Length);//Odeslání přezdívky
                    Odesilani.Flush();//Vyprázdnění proudu

                    GrpPripojeni.Enabled = false;//Vypne možnosti pro připojení
                    GrpZpravy.Enabled = true;//Zapne odesílání zpráv

                    Prijmani = new Thread(PrijmaniZprav)
                    {
                        IsBackground = true
                    };//Nastaví thread pro přijímání zpráv a nastaví jej do pozadí

                    Prijmani.Start();//Zapnutí poslouchání zpráv
                }
            }
            catch(Exception x)
            {
                VypisChatu.Items.Add("Objevila se chyba: ");
                VypisChatu.Items.Add(x.Message);
            }
        }

        private void PrijmaniZprav()//Příjmá zprávy od serveru
        {
            while(Komunikace.Connected)
            {
                Prijem = Komunikace.GetStream();//Nastaví proud na adresu

                byte[] Data = new byte[1024 * 1024 * 2];//Pole pro příjem sériových dat

                Prijem.Read(Data, 0, Komunikace.ReceiveBufferSize);//Načtení sériových dat
                string Zprava = Encoding.UTF8.GetString(Data).TrimEnd('\0');//Dekódování sériových dat

                Vypsani(Zprava);
            }

            Komunikace.Close();
        }

        private void Vypsani(string Text)//Vypsání zprávy do okna
        {
            if(InvokeRequired)//Invoke pro ovládání z jiného vlákna
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
            GrpPripojeni.Enabled = false;//Zablokování prvků po spuštění okna
            GrpZpravy.Enabled = false;
        }

        private void txtPrezdivka_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnPrezdivka_Click(null, null);
            }
        }//Povolí enter pro rychlé zadávání

        private void txtServerIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnKlientPripojeni_Click(null, null);
            }
        }//Povolí enter pro rychlé zadávání

        private void txtZprava_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnOdeslat_Click(null, null);
            }
        }//Povolí enter pro rychlé zadávání

        private void BtnOdpojit_Click(object sender, EventArgs e)
        {
            Prijmani.Join();
            Komunikace.Dispose();
        }

        private void BtnOdeslat_Click(object sender, EventArgs e) //Odeslání zprávy
        {
            byte[] Zprava = Encoding.UTF8.GetBytes(txtZprava.Text.Trim());//Převedení zprávy na sériová data
            Odesilani.Write(Zprava, 0, Zprava.Length);//Odeslání sériových dat
            Odesilani.Flush();//Vyprázdnění proudu

            txtZprava.Text = null;//Vyprázdnění textového pole
        }
    }
}