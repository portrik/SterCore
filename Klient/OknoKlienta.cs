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
        TcpClient Komunikace;//TcpClient pro komunkaci se serverem
        NetworkStream Prijem, Odesilani = default(NetworkStream);//Proudy pro přijímání a odesílání informací

        Thread Prijmani;

        private void BtnPrezdivka_Click(object sender, EventArgs e) //Nastavení přezdívky klienta
        {
            if(TxtPrezdivka.Text.Length != 0 && TxtPrezdivka.Text.Length <= 30 && !string.IsNullOrWhiteSpace(TxtPrezdivka.Text))
            {
                Prezdivka = TxtPrezdivka.Text;
                GrpPrezdivka.Enabled = false; //Vypnutí zadávání přezdívky
                GrpPripojeni.Enabled = true; //Zapnutí možností připojení
                TxtServerIP.Focus();
                TxtServerIP.SelectAll();
            }
            else
            {
                MessageBox.Show("Přezdívka se musí skládat ze znaků a nesmí být delší než 30 znaků!", "Chyba!");
                TxtPrezdivka.Focus();
                TxtPrezdivka.SelectAll();
            }
        }

        private void BtnKlientPripojeni_Click(object sender, EventArgs e) //Připojení klienta k serveru
        {
            Komunikace = new TcpClient();
            IPEndPoint AdresaServeru = null;

            try
            {
                try
                {
                    IPAddress IP = IPAddress.Parse(TxtServerIP.Text);
                    int Port = int.Parse(TxtServerPort.Text);
                    AdresaServeru = new IPEndPoint(IP, Port);//Zpracování adresy a portu
                }
                catch
                {
                    MessageBox.Show("Adresa IP nebo portu byla špatně napsána!", "Chyba!");
                    TxtServerIP.Focus();
                    TxtServerIP.SelectAll();
                }
                
                if(!(AdresaServeru == null))
                {
                    Komunikace.Connect(AdresaServeru);//Pokus o připojení na zadanou adresu a port

                    VypisChatu.Items.Add("Připojuji se k serveru...");
                    BtnOdpojit.Enabled = true;

                    if (Komunikace.Connected)
                    {
                        Odesilani = Komunikace.GetStream();//Nastavení proudu na adresu
                        VypisChatu.Items.Add("Připojení bylo úspěšné!");
                        byte[] Jmeno = Encoding.UTF8.GetBytes(Prezdivka);//Převedení přezdívky na byty
                        Odesilani.Write(Jmeno, 0, Jmeno.Length);//Odeslání přezdívky
                                                                //Odesilani.Flush();//Vyprázdnění proudu

                        Povoleni(GrpPripojeni, false);//Vypne možnosti pro připojení
                        Povoleni(GrpZpravy, true);//Zapne odesílání zpráv

                        Prijmani = new Thread(PrijmaniZprav)
                        {
                            IsBackground = true
                        };//Nastaví thread pro přijímání zpráv a nastaví jej do pozadí

                        Prijmani.Start();//Zapnutí poslouchání zpráv
                    }                
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
            try
            {
                while (Komunikace.Connected)
                {
                    Prijem = Komunikace.GetStream();//Nastaví proud na adresu

                    byte[] Data = new byte[1024 * 1024 * 2];//Pole pro příjem sériových dat

                    Prijem.Read(Data, 0, Komunikace.ReceiveBufferSize);//Načtení sériových dat
                    string Zprava = Encoding.UTF8.GetString(Data).TrimEnd('\0');//Dekódování sériových dat
                    string[] Uprava = Zprava.Split('φ');

                    switch (Uprava[0])
                    {
                        case "0"://Běžná zpráva
                            {
                                Vypsani(Uprava[1] + Uprava[2]);
                                break;
                            }
                        case "1"://TODO: Zpracování obrázku
                            {
                                break;
                            }
                        case "2"://TODO: Zpracování souboru
                            {
                                break;
                            }
                        case "3"://TODO: Obsluha
                            {
                                break;
                            }
                        case "4":
                            {
                                Komunikace.Dispose();
                                break;
                            }
                    }

                    
                }
            }      
            catch
            {
                Komunikace.Close();
                Vypsani("Spojení bylo ukončeno");
                Povoleni(GrpZpravy, true);
                Povoleni(GrpZpravy, false);
                Prijmani.Join();                
            }           
        }

        private void Povoleni(GroupBox Skupina, bool Volba)
        {
            if(InvokeRequired)
            {
                Invoke((MethodInvoker)(() => Povoleni(Skupina, Volba)));
            }
            else
            {
                Skupina.Enabled = Volba;
            }
        }//Změní, zda je možné daný groupbox používat

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
            Povoleni(GrpPripojeni, false);//Zablokování prvků po spuštění okna
            Povoleni(GrpZpravy, false);
            BtnOdpojit.Enabled = false;
        }

        private void TxtPrezdivka_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnPrezdivka_Click(null, null);
            }
        }//Povolí enter pro rychlé zadávání

        private void TxtServerIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnKlientPripojeni_Click(null, null);
            }
        }//Povolí enter pro rychlé zadávání

        private void TxtZprava_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                BtnOdeslat_Click(null, null);
            }
        }//Povolí enter pro rychlé zadávání

        private void BtnOdpojit_Click(object sender, EventArgs e)
        {
            byte[] Zprava = Encoding.UTF8.GetBytes("4φ");//Převedení zprávy na sériová data
            Odesilani.Write(Zprava, 0, Zprava.Length);//Odeslání sériových dat
            Odesilani.Flush();//Vyprázdnění proudu

            Komunikace.Close();
            GrpPripojeni.Enabled = true;
            BtnOdpojit.Enabled = false;
        }

        private void BtnOdeslat_Click(object sender, EventArgs e) //Odeslání zprávy
        {
            if(!string.IsNullOrWhiteSpace(TxtZprava.Text))
            {
                byte[] Zprava = Encoding.UTF8.GetBytes("0φ" + TxtZprava.Text.Trim());//Převedení zprávy na sériová data
                Odesilani.Write(Zprava, 0, Zprava.Length);//Odeslání sériových dat
                Odesilani.Flush();//Vyprázdnění proudu

                TxtZprava.Text = null;//Vyprázdnění textového pole
                TxtZprava.Focus();
                TxtZprava.SelectAll();
            }            
        }
    }
}