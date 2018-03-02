namespace Server
{
    partial class OknoServeru
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OknoServeru));
            this.VypisKlientu = new System.Windows.Forms.ListBox();
            this.TxtZprava = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnZprava = new MaterialSkin.Controls.MaterialRaisedButton();
            this.BtnServerStop = new MaterialSkin.Controls.MaterialRaisedButton();
            this.VypisChatu = new System.Windows.Forms.RichTextBox();
            this.OdeslaniObrazku = new System.Windows.Forms.PictureBox();
            this.OdeslaniSouboru = new System.Windows.Forms.PictureBox();
            this.VolbaSouboru = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.OdeslaniObrazku)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OdeslaniSouboru)).BeginInit();
            this.SuspendLayout();
            // 
            // VypisKlientu
            // 
            this.VypisKlientu.FormattingEnabled = true;
            this.VypisKlientu.Location = new System.Drawing.Point(23, 114);
            this.VypisKlientu.Name = "VypisKlientu";
            this.VypisKlientu.Size = new System.Drawing.Size(117, 446);
            this.VypisKlientu.TabIndex = 9;
            this.VypisKlientu.TabStop = false;
            // 
            // TxtZprava
            // 
            this.TxtZprava.Depth = 0;
            this.TxtZprava.Font = new System.Drawing.Font("Gill Sans MT", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtZprava.Hint = "";
            this.TxtZprava.Location = new System.Drawing.Point(159, 583);
            this.TxtZprava.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtZprava.Name = "TxtZprava";
            this.TxtZprava.PasswordChar = '\0';
            this.TxtZprava.SelectedText = "";
            this.TxtZprava.SelectionLength = 0;
            this.TxtZprava.SelectionStart = 0;
            this.TxtZprava.Size = new System.Drawing.Size(641, 23);
            this.TxtZprava.TabIndex = 0;
            this.TxtZprava.UseSystemPasswordChar = false;
            this.TxtZprava.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtZprava_KeyPress);
            // 
            // BtnZprava
            // 
            this.BtnZprava.Depth = 0;
            this.BtnZprava.Location = new System.Drawing.Point(806, 565);
            this.BtnZprava.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnZprava.Name = "BtnZprava";
            this.BtnZprava.Primary = true;
            this.BtnZprava.Size = new System.Drawing.Size(133, 41);
            this.BtnZprava.TabIndex = 1;
            this.BtnZprava.Text = "Odeslat zprávu";
            this.BtnZprava.UseVisualStyleBackColor = true;
            this.BtnZprava.Click += new System.EventHandler(this.BtnZprava_Click);
            // 
            // BtnServerStop
            // 
            this.BtnServerStop.Depth = 0;
            this.BtnServerStop.Location = new System.Drawing.Point(23, 74);
            this.BtnServerStop.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnServerStop.Name = "BtnServerStop";
            this.BtnServerStop.Primary = true;
            this.BtnServerStop.Size = new System.Drawing.Size(117, 34);
            this.BtnServerStop.TabIndex = 10;
            this.BtnServerStop.TabStop = false;
            this.BtnServerStop.Text = "Stop serveru";
            this.BtnServerStop.UseVisualStyleBackColor = true;
            this.BtnServerStop.Click += new System.EventHandler(this.BtnServerStop_Click);
            // 
            // VypisChatu
            // 
            this.VypisChatu.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VypisChatu.Location = new System.Drawing.Point(147, 74);
            this.VypisChatu.Name = "VypisChatu";
            this.VypisChatu.ReadOnly = true;
            this.VypisChatu.Size = new System.Drawing.Size(791, 485);
            this.VypisChatu.TabIndex = 13;
            this.VypisChatu.TabStop = false;
            this.VypisChatu.Text = "";
            this.VypisChatu.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.VypisChatu_LinkClicked);
            this.VypisChatu.TextChanged += new System.EventHandler(this.VypisChatu_TextChanged);
            // 
            // OdeslaniObrazku
            // 
            this.OdeslaniObrazku.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OdeslaniObrazku.Image = global::Server.Properties.Resources.ObrazekServeru;
            this.OdeslaniObrazku.Location = new System.Drawing.Point(92, 566);
            this.OdeslaniObrazku.Name = "OdeslaniObrazku";
            this.OdeslaniObrazku.Size = new System.Drawing.Size(48, 48);
            this.OdeslaniObrazku.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.OdeslaniObrazku.TabIndex = 15;
            this.OdeslaniObrazku.TabStop = false;
            this.OdeslaniObrazku.Click += new System.EventHandler(this.OdeslaniObrazku_Click);
            // 
            // OdeslaniSouboru
            // 
            this.OdeslaniSouboru.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OdeslaniSouboru.Image = global::Server.Properties.Resources.SouborServeru;
            this.OdeslaniSouboru.Location = new System.Drawing.Point(23, 566);
            this.OdeslaniSouboru.Name = "OdeslaniSouboru";
            this.OdeslaniSouboru.Size = new System.Drawing.Size(48, 48);
            this.OdeslaniSouboru.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.OdeslaniSouboru.TabIndex = 14;
            this.OdeslaniSouboru.TabStop = false;
            this.OdeslaniSouboru.Click += new System.EventHandler(this.OdeslaniSouboru_Click);
            // 
            // VolbaSouboru
            // 
            this.VolbaSouboru.FileName = "openFileDialog1";
            // 
            // OknoServeru
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 620);
            this.ControlBox = false;
            this.Controls.Add(this.OdeslaniObrazku);
            this.Controls.Add(this.OdeslaniSouboru);
            this.Controls.Add(this.VypisChatu);
            this.Controls.Add(this.BtnServerStop);
            this.Controls.Add(this.BtnZprava);
            this.Controls.Add(this.TxtZprava);
            this.Controls.Add(this.VypisKlientu);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(950, 620);
            this.MinimumSize = new System.Drawing.Size(950, 620);
            this.Name = "OknoServeru";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SterCore Server";
            this.Load += new System.EventHandler(this.OknoServeru_Load);
            ((System.ComponentModel.ISupportInitialize)(this.OdeslaniObrazku)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OdeslaniSouboru)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox VypisKlientu;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtZprava;
        private MaterialSkin.Controls.MaterialRaisedButton BtnZprava;
        private MaterialSkin.Controls.MaterialRaisedButton BtnServerStop;
        private System.Windows.Forms.RichTextBox VypisChatu;
        private System.Windows.Forms.PictureBox OdeslaniSouboru;
        private System.Windows.Forms.PictureBox OdeslaniObrazku;
        private System.Windows.Forms.OpenFileDialog VolbaSouboru;
    }
}

