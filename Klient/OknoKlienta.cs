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

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        public OknoKlienta(string Jmeno, IPEndPoint AdresaServeru)
        {
            InitializeComponent();

            Prezdivka = Jmeno;
            Adresa = AdresaServeru;
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
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

                VypisChatu.Items.Add("Připojuji se k serveru...");
                BtnOdpojit.Enabled = true;

                if (Komunikace.Connected)
                {
                    Odesilani = Komunikace.GetStream();//Nastavení proudu na adresu
                    VypisChatu.Items.Add("Připojení bylo úspěšné!");
                    byte[] Jmeno = Encoding.UTF8.GetBytes(Prezdivka);//Převedení přezdívky na byty
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
                VypisChatu.Items.Add("Objevila se chyba: ");
                VypisChatu.Items.Add(x.Message);
            }
        }

        /// <summary>
        /// Přijímá zprávy od serveru.
        /// </summary>
        private void PrijmaniZprav()
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
                        case "3"://TODO: Seznam klientů
                            {
                                if(bool.Parse(Uprava[2]))
                                {
                                    Invoke((MethodInvoker)(() => LstPripojeni.Items.Add(Uprava[1])));
                                }
                                else
                                {
                                    Invoke((MethodInvoker)(() => LstPripojeni.Items.Remove(Uprava[1])));
                                }
                                break;
                            }
                    }                    
                }
            }      
            catch
            {
                Komunikace.Close();
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
                VypisChatu.Items.Add(Text);
            }
        }

        /// <summary>
        /// Odešle žádost o odpojení na server a otevře okno se zadáváním údajů.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOdpojit_Click(object sender, EventArgs e)
        {
            byte[] Zprava = Encoding.UTF8.GetBytes("4φ");//Převedení zprávy na sériová data
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
                byte[] Zprava = Encoding.UTF8.GetBytes("0φ" + TxtZprava.Text.Trim());//Převedení zprávy na sériová data
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

        private void BtnOdeslatObrazek_Click(object sender, EventArgs e)
        {
            VolbaSouboru.Filter = "Obrázky|*.jpg;*.png;*.gif;*.jpeg;*.jpe;*.bmp";

            if (VolbaSouboru.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("1φ" + Path.GetFileName(VolbaSouboru.FileName + "φ" + VolbaSouboru.FileName.Length));

                byte[] Informace = Encoding.UTF8.GetBytes("1φ" + Path.GetFileName(VolbaSouboru.FileName + "φ" + VolbaSouboru.FileName.Length));

                using (FileStream Obrazek = new FileStream(VolbaSouboru.FileName, FileMode.Open, FileAccess.Read))
                {
                    Obrazek.CopyTo(Odesilani);
                }
            }            
        }
    }
}