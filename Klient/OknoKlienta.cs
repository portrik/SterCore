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
using System.Drawing.Imaging;

namespace SterCore
{
    public partial class OknoKlienta : MaterialForm
    {
        string Prezdivka;//Přezdívka klienta
        TcpClient Komunikace;//TcpClient pro komunkaci se serverem
        IPEndPoint Adresa = null;
        NetworkStream Prijem, Odesilani = default(NetworkStream);//Proudy pro přijímání a odesílání informací
        Thread Prijmani;

        public OknoKlienta()
        {
            InitializeComponent();
        }

        public OknoKlienta(string Jmeno, IPEndPoint AdresaServeru)
        {
            InitializeComponent();

            Prezdivka = Jmeno;
            Adresa = AdresaServeru;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue400, Primary.LightBlue900, Primary.Cyan100, Accent.LightBlue400, TextShade.WHITE);

            Pripojeni();
        }

        /// <summary>
        /// Připojí se na server podle zadané adresy.
        /// </summary>
        public void Pripojeni()
        {
            Komunikace = new TcpClient();

            try
            {
                Komunikace.Connect(Adresa);//Pokus o připojení na zadanou adresu a port

                if (Komunikace.Connected)
                {
                    Odesilani = Komunikace.GetStream();//Nastavení proudu na adresu
                    VypisChatu.Text += DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + "Připojení bylo úspěšné!";
                    byte[] Jmeno = Encoding.Unicode.GetBytes(Prezdivka);//Převedení přezdívky na byty
                    Odesilani.Write(Jmeno, 0, Jmeno.Length);//Odeslání přezdívky
                    Odesilani.Flush();//Vyprázdnění proudu

                    Prijmani = new Thread(PrijmaniZprav)
                    {
                        IsBackground = true
                    };//Nastaví thread pro přijímání zpráv a nastaví jej do pozadí

                    Prijmani.Start();//Zapnutí poslouchání zpráv
                }
            }
            catch (Exception x)
            {
                VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + "Objevila se chyba: ";
                VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + x.Message;
            }
        }

        /// <summary>
        /// Přijímá zprávy od serveru.
        /// </summary>
        private void PrijmaniZprav()
        {
            try
            {
                using (Prijem = Komunikace.GetStream())
                {
                    while (Komunikace.Connected)
                    {
                        byte[] Data = new byte[1024 * 1024 * 4];//Pole pro příjem sériových dat
                        byte[] Znak = new byte[3];

                        Prijem.Read(Data, 0, Komunikace.ReceiveBufferSize);//Načtení sériových dat

                        Array.Copy(Data, Znak, 3);

                        string Uprava = Encoding.Unicode.GetString(Znak);

                        switch (Uprava[0])
                        {
                            case '0'://Běžná zpráva
                                {
                                    string Dekodovani = Encoding.Unicode.GetString(Data).TrimEnd('\0');
                                    string[] Zprava = Dekodovani.Split('φ');
                                    Vypsani(Zprava[1] + Zprava[2]);
                                    break;
                                }
                            case '1'://TODO: Zpracování obrázku
                                {
                                    break;
                                }
                            case '2'://TODO: Zpracování souboru
                                {
                                    break;
                                }
                            case '3'://TODO: Seznam klientů
                                {
                                    string Dekodovani = Encoding.Unicode.GetString(Data).TrimEnd('\0');
                                    string[] Seznam = Dekodovani.Split('φ');
                                    string[] Jmena = Seznam[1].Split(',');

                                    foreach (string Jmeno in Jmena)
                                    {
                                        if (InvokeRequired)
                                        {
                                            Invoke((MethodInvoker)(() => LstPripojeni.Items.Add(Jmeno)));
                                        }
                                    }

                                    break;
                                }
                        }                        
                    }                    
                }
            }      
            catch(Exception x)
            {
                MessageBox.Show(x.Message);
                MessageBox.Show(x.StackTrace);
                Vypsani("Spojení bylo ukončeno");
                UvodKlienta.ZmenaUdaju = true;

                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => Close()));
                }

                Prijmani.Join();                
            }           
        }

        /// <summary>
        /// Vypíše přijatou zprávu do okna chatu.
        /// </summary>
        /// <param name="Text">Přijatá zpráva</param>
        private void Vypsani(string Text)
        {
            if(InvokeRequired)
            {
                Invoke((MethodInvoker)(() => Vypsani(Text)));
            }
            else
            {
                VypisChatu.Text += "\n" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + " " + Text;
            }
        }

        /// <summary>
        /// Odešle žádost o odpojení na server a otevře okno se zadáváním údajů.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOdpojit_Click(object sender, EventArgs e)
        {
            byte[] Zprava = Encoding.Unicode.GetBytes("4φ");//Převedení zprávy na sériová data
            Odesilani.Write(Zprava, 0, Zprava.Length);//Odeslání sériových dat
            Odesilani.Flush();//Vyprázdnění proudu

            UvodKlienta.ZmenaUdaju = true;
            Close();
        }

        /// <summary>
        /// Odešle zprávu na server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOdeslat_Click(object sender, EventArgs e) //Odeslání zprávy
        {
            if(!string.IsNullOrWhiteSpace(TxtZprava.Text))
            {
                byte[] Zprava = Encoding.Unicode.GetBytes("0φ" + TxtZprava.Text.Trim());//Převedení zprávy na sériová data
                Odesilani.Write(Zprava, 0, Zprava.Length);//Odeslání sériových dat
                Odesilani.Flush();//Vyprázdnění proudu

                TxtZprava.Text = null;//Vyprázdnění textového pole
                TxtZprava.Focus();
                TxtZprava.SelectAll();
            }            
        }

        /// <summary>
        /// Zjednodušení odeslání zprávy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtZprava_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnOdeslat_Click(null, null);
            }
        }

        private void OdeslaniObrazku_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = "Obrázky|*.jpg;*.png;*.gif;*.jpeg;*.jpe;*.bmp";

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                byte[] Obrazek = File.ReadAllBytes(VolbaSouboru.FileName); 

                if(Obrazek.Length < 4194040)
                {
                    try
                    {
                        string Cesta = "1φ" + Path.GetFileNameWithoutExtension(VolbaSouboru.FileName) + "φ" + Path.GetExtension(VolbaSouboru.FileName) + "φ";
                        byte[] Znacka = Encoding.Unicode.GetBytes(Cesta);
                        byte[] Zprava = new byte[1024 * 1024 * 4];

                        Array.Copy(Znacka, 0, Zprava, 0, Znacka.Length);
                        Array.Copy(Obrazek, 0, Zprava, 264, Obrazek.Length);

                        Odesilani.Write(Zprava, 0, Zprava.Length);
                        Odesilani.Flush();
                    }
                    catch(Exception x)
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

        private void OknoKlienta_Load(object sender, EventArgs e)
        {
            VypisChatu.BackColor = Color.White;
        }

        private void OdeslatSoubor_Click(object sender, EventArgs e)
        {
            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                
            }
        }
    }
}