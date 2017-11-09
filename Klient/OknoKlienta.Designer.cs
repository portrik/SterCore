namespace PataChat
{
    partial class Klient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TxtServerPort = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.TxtServerIP = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.BtnKlientPripojeni = new MaterialSkin.Controls.MaterialFlatButton();
            this.VypisChatu = new System.Windows.Forms.ListBox();
            this.BtnOdeslat = new MaterialSkin.Controls.MaterialFlatButton();
            this.TxtZprava = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.TxtPrezdivka = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnPrezdivka = new MaterialSkin.Controls.MaterialFlatButton();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.GrpPrezdivka = new System.Windows.Forms.GroupBox();
            this.GrpPripojeni = new System.Windows.Forms.GroupBox();
            this.GrpZpravy = new System.Windows.Forms.GroupBox();
            this.BtnOdpojit = new MaterialSkin.Controls.MaterialFlatButton();
            this.GrpPrezdivka.SuspendLayout();
            this.GrpPripojeni.SuspendLayout();
            this.GrpZpravy.SuspendLayout();
            this.SuspendLayout();
            // 
            // TxtServerPort
            // 
            this.TxtServerPort.Depth = 0;
            this.TxtServerPort.Hint = "";
            this.TxtServerPort.Location = new System.Drawing.Point(106, 63);
            this.TxtServerPort.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtServerPort.Name = "TxtServerPort";
            this.TxtServerPort.PasswordChar = '\0';
            this.TxtServerPort.SelectedText = "";
            this.TxtServerPort.SelectionLength = 0;
            this.TxtServerPort.SelectionStart = 0;
            this.TxtServerPort.Size = new System.Drawing.Size(248, 23);
            this.TxtServerPort.TabIndex = 7;
            this.TxtServerPort.Text = "8888";
            this.TxtServerPort.UseSystemPasswordChar = false;
            this.TxtServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtServerIP_KeyPress);
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(6, 60);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(94, 19);
            this.materialLabel2.TabIndex = 6;
            this.materialLabel2.Text = "Port serveru:";
            // 
            // TxtServerIP
            // 
            this.TxtServerIP.Depth = 0;
            this.TxtServerIP.Hint = "";
            this.TxtServerIP.Location = new System.Drawing.Point(106, 34);
            this.TxtServerIP.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtServerIP.Name = "TxtServerIP";
            this.TxtServerIP.PasswordChar = '\0';
            this.TxtServerIP.SelectedText = "";
            this.TxtServerIP.SelectionLength = 0;
            this.TxtServerIP.SelectionStart = 0;
            this.TxtServerIP.Size = new System.Drawing.Size(248, 23);
            this.TxtServerIP.TabIndex = 5;
            this.TxtServerIP.Text = "127.0.0.1";
            this.TxtServerIP.UseSystemPasswordChar = false;
            this.TxtServerIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtServerIP_KeyPress);
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(6, 34);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(79, 19);
            this.materialLabel1.TabIndex = 4;
            this.materialLabel1.Text = "IP serveru:";
            // 
            // BtnKlientPripojeni
            // 
            this.BtnKlientPripojeni.AutoSize = true;
            this.BtnKlientPripojeni.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnKlientPripojeni.Depth = 0;
            this.BtnKlientPripojeni.Location = new System.Drawing.Point(10, 95);
            this.BtnKlientPripojeni.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnKlientPripojeni.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnKlientPripojeni.Name = "BtnKlientPripojeni";
            this.BtnKlientPripojeni.Primary = false;
            this.BtnKlientPripojeni.Size = new System.Drawing.Size(91, 36);
            this.BtnKlientPripojeni.TabIndex = 8;
            this.BtnKlientPripojeni.Text = "Připojit se";
            this.BtnKlientPripojeni.UseVisualStyleBackColor = true;
            this.BtnKlientPripojeni.Click += new System.EventHandler(this.BtnKlientPripojeni_Click);
            // 
            // VypisChatu
            // 
            this.VypisChatu.FormattingEnabled = true;
            this.VypisChatu.Location = new System.Drawing.Point(378, 102);
            this.VypisChatu.Name = "VypisChatu";
            this.VypisChatu.Size = new System.Drawing.Size(594, 446);
            this.VypisChatu.TabIndex = 9;
            // 
            // BtnOdeslat
            // 
            this.BtnOdeslat.AutoSize = true;
            this.BtnOdeslat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnOdeslat.Depth = 0;
            this.BtnOdeslat.Location = new System.Drawing.Point(460, 12);
            this.BtnOdeslat.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnOdeslat.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnOdeslat.Name = "BtnOdeslat";
            this.BtnOdeslat.Primary = false;
            this.BtnOdeslat.Size = new System.Drawing.Size(127, 36);
            this.BtnOdeslat.TabIndex = 10;
            this.BtnOdeslat.Text = "Odeslat zprávu";
            this.BtnOdeslat.UseVisualStyleBackColor = true;
            this.BtnOdeslat.Click += new System.EventHandler(this.BtnOdeslat_Click);
            // 
            // TxtZprava
            // 
            this.TxtZprava.Depth = 0;
            this.TxtZprava.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtZprava.Hint = "";
            this.TxtZprava.Location = new System.Drawing.Point(4, 19);
            this.TxtZprava.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtZprava.Name = "TxtZprava";
            this.TxtZprava.PasswordChar = '\0';
            this.TxtZprava.SelectedText = "";
            this.TxtZprava.SelectionLength = 0;
            this.TxtZprava.SelectionStart = 0;
            this.TxtZprava.Size = new System.Drawing.Size(449, 23);
            this.TxtZprava.TabIndex = 11;
            this.TxtZprava.UseSystemPasswordChar = false;
            this.TxtZprava.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtZprava_KeyPress);
            // 
            // TxtPrezdivka
            // 
            this.TxtPrezdivka.Depth = 0;
            this.TxtPrezdivka.Hint = "";
            this.TxtPrezdivka.Location = new System.Drawing.Point(93, 27);
            this.TxtPrezdivka.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtPrezdivka.Name = "TxtPrezdivka";
            this.TxtPrezdivka.PasswordChar = '\0';
            this.TxtPrezdivka.SelectedText = "";
            this.TxtPrezdivka.SelectionLength = 0;
            this.TxtPrezdivka.SelectionStart = 0;
            this.TxtPrezdivka.Size = new System.Drawing.Size(261, 23);
            this.TxtPrezdivka.TabIndex = 12;
            this.TxtPrezdivka.UseSystemPasswordChar = false;
            this.TxtPrezdivka.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtPrezdivka_KeyPress);
            // 
            // BtnPrezdivka
            // 
            this.BtnPrezdivka.AutoSize = true;
            this.BtnPrezdivka.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnPrezdivka.Depth = 0;
            this.BtnPrezdivka.Location = new System.Drawing.Point(10, 59);
            this.BtnPrezdivka.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnPrezdivka.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnPrezdivka.Name = "BtnPrezdivka";
            this.BtnPrezdivka.Primary = false;
            this.BtnPrezdivka.Size = new System.Drawing.Size(134, 36);
            this.BtnPrezdivka.TabIndex = 13;
            this.BtnPrezdivka.Text = "Zvolit přezdívku";
            this.BtnPrezdivka.UseVisualStyleBackColor = true;
            this.BtnPrezdivka.Click += new System.EventHandler(this.BtnPrezdivka_Click);
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel3.Location = new System.Drawing.Point(6, 27);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(81, 19);
            this.materialLabel3.TabIndex = 14;
            this.materialLabel3.Text = "Přezdívka: ";
            // 
            // GrpPrezdivka
            // 
            this.GrpPrezdivka.Controls.Add(this.materialLabel3);
            this.GrpPrezdivka.Controls.Add(this.BtnPrezdivka);
            this.GrpPrezdivka.Controls.Add(this.TxtPrezdivka);
            this.GrpPrezdivka.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.GrpPrezdivka.Location = new System.Drawing.Point(12, 102);
            this.GrpPrezdivka.Name = "GrpPrezdivka";
            this.GrpPrezdivka.Size = new System.Drawing.Size(360, 100);
            this.GrpPrezdivka.TabIndex = 15;
            this.GrpPrezdivka.TabStop = false;
            // 
            // GrpPripojeni
            // 
            this.GrpPripojeni.Controls.Add(this.materialLabel1);
            this.GrpPripojeni.Controls.Add(this.TxtServerIP);
            this.GrpPripojeni.Controls.Add(this.materialLabel2);
            this.GrpPripojeni.Controls.Add(this.TxtServerPort);
            this.GrpPripojeni.Controls.Add(this.BtnKlientPripojeni);
            this.GrpPripojeni.Location = new System.Drawing.Point(12, 208);
            this.GrpPripojeni.Name = "GrpPripojeni";
            this.GrpPripojeni.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.GrpPripojeni.Size = new System.Drawing.Size(360, 159);
            this.GrpPripojeni.TabIndex = 16;
            this.GrpPripojeni.TabStop = false;
            // 
            // GrpZpravy
            // 
            this.GrpZpravy.Controls.Add(this.BtnOdeslat);
            this.GrpZpravy.Controls.Add(this.TxtZprava);
            this.GrpZpravy.Location = new System.Drawing.Point(379, 554);
            this.GrpZpravy.Name = "GrpZpravy";
            this.GrpZpravy.Size = new System.Drawing.Size(594, 57);
            this.GrpZpravy.TabIndex = 17;
            this.GrpZpravy.TabStop = false;
            // 
            // BtnOdpojit
            // 
            this.BtnOdpojit.AutoSize = true;
            this.BtnOdpojit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnOdpojit.Depth = 0;
            this.BtnOdpojit.Location = new System.Drawing.Point(22, 376);
            this.BtnOdpojit.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnOdpojit.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnOdpojit.Name = "BtnOdpojit";
            this.BtnOdpojit.Primary = false;
            this.BtnOdpojit.Size = new System.Drawing.Size(88, 36);
            this.BtnOdpojit.TabIndex = 18;
            this.BtnOdpojit.Text = "Odpojit se";
            this.BtnOdpojit.UseVisualStyleBackColor = true;
            this.BtnOdpojit.Click += new System.EventHandler(this.BtnOdpojit_Click);
            // 
            // Klient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 680);
            this.Controls.Add(this.BtnOdpojit);
            this.Controls.Add(this.GrpZpravy);
            this.Controls.Add(this.GrpPripojeni);
            this.Controls.Add(this.GrpPrezdivka);
            this.Controls.Add(this.VypisChatu);
            this.Name = "Klient";
            this.Text = "Klient";
            this.Load += new System.EventHandler(this.Klient_Load);
            this.GrpPrezdivka.ResumeLayout(false);
            this.GrpPrezdivka.PerformLayout();
            this.GrpPripojeni.ResumeLayout(false);
            this.GrpPripojeni.PerformLayout();
            this.GrpZpravy.ResumeLayout(false);
            this.GrpZpravy.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialSingleLineTextField TxtServerPort;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtServerIP;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialFlatButton BtnKlientPripojeni;
        private System.Windows.Forms.ListBox VypisChatu;
        private MaterialSkin.Controls.MaterialFlatButton BtnOdeslat;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtZprava;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtPrezdivka;
        private MaterialSkin.Controls.MaterialFlatButton BtnPrezdivka;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private System.Windows.Forms.GroupBox GrpPrezdivka;
        private System.Windows.Forms.GroupBox GrpPripojeni;
        private System.Windows.Forms.GroupBox GrpZpravy;
        private MaterialSkin.Controls.MaterialFlatButton BtnOdpojit;
    }
}

