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
            this.txtServerPort = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.txtServerIP = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.BtnKlientPripojeni = new MaterialSkin.Controls.MaterialFlatButton();
            this.VypisChatu = new System.Windows.Forms.ListBox();
            this.BtnOdeslat = new MaterialSkin.Controls.MaterialFlatButton();
            this.txtZprava = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.txtPrezdivka = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnPrezdivka = new MaterialSkin.Controls.MaterialFlatButton();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.GrpPrezdivka = new System.Windows.Forms.GroupBox();
            this.GrpPripojeni = new System.Windows.Forms.GroupBox();
            this.GrpZpravy = new System.Windows.Forms.GroupBox();
            this.GrpPrezdivka.SuspendLayout();
            this.GrpPripojeni.SuspendLayout();
            this.GrpZpravy.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtServerPort
            // 
            this.txtServerPort.Depth = 0;
            this.txtServerPort.Hint = "";
            this.txtServerPort.Location = new System.Drawing.Point(106, 63);
            this.txtServerPort.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.PasswordChar = '\0';
            this.txtServerPort.SelectedText = "";
            this.txtServerPort.SelectionLength = 0;
            this.txtServerPort.SelectionStart = 0;
            this.txtServerPort.Size = new System.Drawing.Size(248, 23);
            this.txtServerPort.TabIndex = 7;
            this.txtServerPort.Text = "8888";
            this.txtServerPort.UseSystemPasswordChar = false;
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
            // txtServerIP
            // 
            this.txtServerIP.Depth = 0;
            this.txtServerIP.Hint = "";
            this.txtServerIP.Location = new System.Drawing.Point(106, 34);
            this.txtServerIP.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.PasswordChar = '\0';
            this.txtServerIP.SelectedText = "";
            this.txtServerIP.SelectionLength = 0;
            this.txtServerIP.SelectionStart = 0;
            this.txtServerIP.Size = new System.Drawing.Size(248, 23);
            this.txtServerIP.TabIndex = 5;
            this.txtServerIP.Text = "127.0.0.1";
            this.txtServerIP.UseSystemPasswordChar = false;
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
            // txtZprava
            // 
            this.txtZprava.Depth = 0;
            this.txtZprava.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtZprava.Hint = "";
            this.txtZprava.Location = new System.Drawing.Point(4, 19);
            this.txtZprava.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtZprava.Name = "txtZprava";
            this.txtZprava.PasswordChar = '\0';
            this.txtZprava.SelectedText = "";
            this.txtZprava.SelectionLength = 0;
            this.txtZprava.SelectionStart = 0;
            this.txtZprava.Size = new System.Drawing.Size(449, 23);
            this.txtZprava.TabIndex = 11;
            this.txtZprava.UseSystemPasswordChar = false;
            // 
            // txtPrezdivka
            // 
            this.txtPrezdivka.Depth = 0;
            this.txtPrezdivka.Hint = "";
            this.txtPrezdivka.Location = new System.Drawing.Point(93, 27);
            this.txtPrezdivka.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtPrezdivka.Name = "txtPrezdivka";
            this.txtPrezdivka.PasswordChar = '\0';
            this.txtPrezdivka.SelectedText = "";
            this.txtPrezdivka.SelectionLength = 0;
            this.txtPrezdivka.SelectionStart = 0;
            this.txtPrezdivka.Size = new System.Drawing.Size(261, 23);
            this.txtPrezdivka.TabIndex = 12;
            this.txtPrezdivka.UseSystemPasswordChar = false;
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
            this.GrpPrezdivka.Controls.Add(this.txtPrezdivka);
            this.GrpPrezdivka.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.GrpPrezdivka.Location = new System.Drawing.Point(12, 127);
            this.GrpPrezdivka.Name = "GrpPrezdivka";
            this.GrpPrezdivka.Size = new System.Drawing.Size(360, 100);
            this.GrpPrezdivka.TabIndex = 15;
            this.GrpPrezdivka.TabStop = false;
            // 
            // GrpPripojeni
            // 
            this.GrpPripojeni.Controls.Add(this.materialLabel1);
            this.GrpPripojeni.Controls.Add(this.txtServerIP);
            this.GrpPripojeni.Controls.Add(this.materialLabel2);
            this.GrpPripojeni.Controls.Add(this.txtServerPort);
            this.GrpPripojeni.Controls.Add(this.BtnKlientPripojeni);
            this.GrpPripojeni.Location = new System.Drawing.Point(12, 233);
            this.GrpPripojeni.Name = "GrpPripojeni";
            this.GrpPripojeni.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.GrpPripojeni.Size = new System.Drawing.Size(360, 159);
            this.GrpPripojeni.TabIndex = 16;
            this.GrpPripojeni.TabStop = false;
            // 
            // GrpZpravy
            // 
            this.GrpZpravy.Controls.Add(this.BtnOdeslat);
            this.GrpZpravy.Controls.Add(this.txtZprava);
            this.GrpZpravy.Location = new System.Drawing.Point(379, 554);
            this.GrpZpravy.Name = "GrpZpravy";
            this.GrpZpravy.Size = new System.Drawing.Size(594, 57);
            this.GrpZpravy.TabIndex = 17;
            this.GrpZpravy.TabStop = false;
            // 
            // Klient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 680);
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

        }

        #endregion

        private MaterialSkin.Controls.MaterialSingleLineTextField txtServerPort;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtServerIP;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialFlatButton BtnKlientPripojeni;
        private System.Windows.Forms.ListBox VypisChatu;
        private MaterialSkin.Controls.MaterialFlatButton BtnOdeslat;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtZprava;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtPrezdivka;
        private MaterialSkin.Controls.MaterialFlatButton BtnPrezdivka;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private System.Windows.Forms.GroupBox GrpPrezdivka;
        private System.Windows.Forms.GroupBox GrpPripojeni;
        private System.Windows.Forms.GroupBox GrpZpravy;
    }
}

