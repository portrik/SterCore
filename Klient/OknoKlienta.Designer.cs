namespace SterCore
{
    partial class OknoKlienta
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OknoKlienta));
            this.BtnOdpojit = new MaterialSkin.Controls.MaterialRaisedButton();
            this.LstPripojeni = new System.Windows.Forms.ListBox();
            this.TxtZprava = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnOdeslat = new MaterialSkin.Controls.MaterialRaisedButton();
            this.VolbaSouboru = new System.Windows.Forms.OpenFileDialog();
            this.OdeslatSoubor = new System.Windows.Forms.PictureBox();
            this.OdeslaniObrazku = new System.Windows.Forms.PictureBox();
            this.VypisChatu = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.OdeslatSoubor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OdeslaniObrazku)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnOdpojit
            // 
            this.BtnOdpojit.Depth = 0;
            this.BtnOdpojit.Location = new System.Drawing.Point(23, 74);
            this.BtnOdpojit.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnOdpojit.Name = "BtnOdpojit";
            this.BtnOdpojit.Primary = true;
            this.BtnOdpojit.Size = new System.Drawing.Size(117, 36);
            this.BtnOdpojit.TabIndex = 19;
            this.BtnOdpojit.Text = "Odpojit se";
            this.BtnOdpojit.UseVisualStyleBackColor = true;
            this.BtnOdpojit.Click += new System.EventHandler(this.BtnOdpojit_Click);
            // 
            // LstPripojeni
            // 
            this.LstPripojeni.FormattingEnabled = true;
            this.LstPripojeni.Location = new System.Drawing.Point(23, 114);
            this.LstPripojeni.Name = "LstPripojeni";
            this.LstPripojeni.Size = new System.Drawing.Size(117, 446);
            this.LstPripojeni.TabIndex = 20;
            // 
            // TxtZprava
            // 
            this.TxtZprava.Depth = 0;
            this.TxtZprava.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtZprava.Hint = "";
            this.TxtZprava.Location = new System.Drawing.Point(159, 583);
            this.TxtZprava.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtZprava.Name = "TxtZprava";
            this.TxtZprava.PasswordChar = '\0';
            this.TxtZprava.SelectedText = "";
            this.TxtZprava.SelectionLength = 0;
            this.TxtZprava.SelectionStart = 0;
            this.TxtZprava.Size = new System.Drawing.Size(641, 23);
            this.TxtZprava.TabIndex = 11;
            this.TxtZprava.UseSystemPasswordChar = false;
            this.TxtZprava.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtZprava_KeyPress);
            // 
            // BtnOdeslat
            // 
            this.BtnOdeslat.Depth = 0;
            this.BtnOdeslat.Location = new System.Drawing.Point(806, 565);
            this.BtnOdeslat.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnOdeslat.Name = "BtnOdeslat";
            this.BtnOdeslat.Primary = true;
            this.BtnOdeslat.Size = new System.Drawing.Size(128, 39);
            this.BtnOdeslat.TabIndex = 21;
            this.BtnOdeslat.Text = "Odeslat zprávu";
            this.BtnOdeslat.UseVisualStyleBackColor = true;
            this.BtnOdeslat.Click += new System.EventHandler(this.BtnOdeslat_Click);
            // 
            // VolbaSouboru
            // 
            this.VolbaSouboru.FileName = "openFileDialog1";
            // 
            // OdeslatSoubor
            // 
            this.OdeslatSoubor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OdeslatSoubor.Image = global::Klient.Properties.Resources.SouborKlienta;
            this.OdeslatSoubor.Location = new System.Drawing.Point(23, 565);
            this.OdeslatSoubor.Name = "OdeslatSoubor";
            this.OdeslatSoubor.Size = new System.Drawing.Size(48, 48);
            this.OdeslatSoubor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.OdeslatSoubor.TabIndex = 23;
            this.OdeslatSoubor.TabStop = false;
            this.OdeslatSoubor.Click += new System.EventHandler(this.OdeslatSoubor_Click);
            // 
            // OdeslaniObrazku
            // 
            this.OdeslaniObrazku.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OdeslaniObrazku.Image = global::Klient.Properties.Resources.ObrazekKlienta;
            this.OdeslaniObrazku.Location = new System.Drawing.Point(92, 565);
            this.OdeslaniObrazku.Name = "OdeslaniObrazku";
            this.OdeslaniObrazku.Size = new System.Drawing.Size(48, 48);
            this.OdeslaniObrazku.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.OdeslaniObrazku.TabIndex = 22;
            this.OdeslaniObrazku.TabStop = false;
            this.OdeslaniObrazku.Click += new System.EventHandler(this.OdeslaniObrazku_Click);
            // 
            // VypisChatu
            // 
            this.VypisChatu.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VypisChatu.Location = new System.Drawing.Point(147, 74);
            this.VypisChatu.Name = "VypisChatu";
            this.VypisChatu.ReadOnly = true;
            this.VypisChatu.Size = new System.Drawing.Size(787, 485);
            this.VypisChatu.TabIndex = 24;
            this.VypisChatu.Text = "";
            this.VypisChatu.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.VypisChatu_LinkClicked);
            this.VypisChatu.TextChanged += new System.EventHandler(this.VypisChatu_TextChanged);
            // 
            // OknoKlienta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 620);
            this.Controls.Add(this.VypisChatu);
            this.Controls.Add(this.OdeslatSoubor);
            this.Controls.Add(this.OdeslaniObrazku);
            this.Controls.Add(this.BtnOdeslat);
            this.Controls.Add(this.TxtZprava);
            this.Controls.Add(this.LstPripojeni);
            this.Controls.Add(this.BtnOdpojit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(950, 620);
            this.MinimumSize = new System.Drawing.Size(950, 620);
            this.Name = "OknoKlienta";
            this.Text = "SterCore";
            this.Load += new System.EventHandler(this.OknoKlienta_Load);
            ((System.ComponentModel.ISupportInitialize)(this.OdeslatSoubor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OdeslaniObrazku)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private MaterialSkin.Controls.MaterialRaisedButton BtnOdpojit;
        private System.Windows.Forms.ListBox LstPripojeni;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtZprava;
        private MaterialSkin.Controls.MaterialRaisedButton BtnOdeslat;
        private System.Windows.Forms.OpenFileDialog VolbaSouboru;
        private System.Windows.Forms.PictureBox OdeslaniObrazku;
        private System.Windows.Forms.PictureBox OdeslatSoubor;
        private System.Windows.Forms.RichTextBox VypisChatu;
    }
}

