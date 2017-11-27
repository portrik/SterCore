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
            this.VypisChatu = new System.Windows.Forms.ListBox();
            this.BtnOdpojit = new MaterialSkin.Controls.MaterialRaisedButton();
            this.LstPripojeni = new System.Windows.Forms.ListBox();
            this.LabelPripojeni = new MaterialSkin.Controls.MaterialLabel();
            this.TxtZprava = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnOdeslat = new MaterialSkin.Controls.MaterialRaisedButton();
            this.BtnOdeslatObrazek = new MaterialSkin.Controls.MaterialFlatButton();
            this.VolbaObrazku = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // VypisChatu
            // 
            this.VypisChatu.FormattingEnabled = true;
            this.VypisChatu.Location = new System.Drawing.Point(378, 102);
            this.VypisChatu.Name = "VypisChatu";
            this.VypisChatu.Size = new System.Drawing.Size(594, 446);
            this.VypisChatu.TabIndex = 9;
            // 
            // BtnOdpojit
            // 
            this.BtnOdpojit.Depth = 0;
            this.BtnOdpojit.Location = new System.Drawing.Point(12, 102);
            this.BtnOdpojit.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnOdpojit.Name = "BtnOdpojit";
            this.BtnOdpojit.Primary = true;
            this.BtnOdpojit.Size = new System.Drawing.Size(109, 36);
            this.BtnOdpojit.TabIndex = 19;
            this.BtnOdpojit.Text = "Odpojit se";
            this.BtnOdpojit.UseVisualStyleBackColor = true;
            this.BtnOdpojit.Click += new System.EventHandler(this.BtnOdpojit_Click);
            // 
            // LstPripojeni
            // 
            this.LstPripojeni.FormattingEnabled = true;
            this.LstPripojeni.Location = new System.Drawing.Point(12, 185);
            this.LstPripojeni.Name = "LstPripojeni";
            this.LstPripojeni.Size = new System.Drawing.Size(359, 420);
            this.LstPripojeni.TabIndex = 20;
            // 
            // LabelPripojeni
            // 
            this.LabelPripojeni.AutoSize = true;
            this.LabelPripojeni.Depth = 0;
            this.LabelPripojeni.Font = new System.Drawing.Font("Roboto", 11F);
            this.LabelPripojeni.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.LabelPripojeni.Location = new System.Drawing.Point(12, 163);
            this.LabelPripojeni.MouseState = MaterialSkin.MouseState.HOVER;
            this.LabelPripojeni.Name = "LabelPripojeni";
            this.LabelPripojeni.Size = new System.Drawing.Size(206, 19);
            this.LabelPripojeni.TabIndex = 21;
            this.LabelPripojeni.Text = "Seznam připojených uživatelů";
            // 
            // TxtZprava
            // 
            this.TxtZprava.Depth = 0;
            this.TxtZprava.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtZprava.Hint = "";
            this.TxtZprava.Location = new System.Drawing.Point(378, 582);
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
            // BtnOdeslat
            // 
            this.BtnOdeslat.Depth = 0;
            this.BtnOdeslat.Location = new System.Drawing.Point(844, 566);
            this.BtnOdeslat.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnOdeslat.Name = "BtnOdeslat";
            this.BtnOdeslat.Primary = true;
            this.BtnOdeslat.Size = new System.Drawing.Size(128, 39);
            this.BtnOdeslat.TabIndex = 21;
            this.BtnOdeslat.Text = "Odeslat zprávu";
            this.BtnOdeslat.UseVisualStyleBackColor = true;
            this.BtnOdeslat.Click += new System.EventHandler(this.BtnOdeslat_Click);
            // 
            // BtnOdeslatObrazek
            // 
            this.BtnOdeslatObrazek.AutoSize = true;
            this.BtnOdeslatObrazek.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnOdeslatObrazek.Depth = 0;
            this.BtnOdeslatObrazek.Location = new System.Drawing.Point(844, 612);
            this.BtnOdeslatObrazek.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnOdeslatObrazek.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnOdeslatObrazek.Name = "BtnOdeslatObrazek";
            this.BtnOdeslatObrazek.Primary = false;
            this.BtnOdeslatObrazek.Size = new System.Drawing.Size(135, 36);
            this.BtnOdeslatObrazek.TabIndex = 22;
            this.BtnOdeslatObrazek.Text = "Odeslat obrázek";
            this.BtnOdeslatObrazek.UseVisualStyleBackColor = true;
            this.BtnOdeslatObrazek.Click += new System.EventHandler(this.BtnOdeslatObrazek_Click);
            // 
            // VolbaObrazku
            // 
            this.VolbaObrazku.FileName = "openFileDialog1";
            // 
            // OknoKlienta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 680);
            this.Controls.Add(this.BtnOdeslatObrazek);
            this.Controls.Add(this.BtnOdeslat);
            this.Controls.Add(this.LabelPripojeni);
            this.Controls.Add(this.TxtZprava);
            this.Controls.Add(this.LstPripojeni);
            this.Controls.Add(this.BtnOdpojit);
            this.Controls.Add(this.VypisChatu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OknoKlienta";
            this.Text = "SterCore";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox VypisChatu;
        private MaterialSkin.Controls.MaterialRaisedButton BtnOdpojit;
        private System.Windows.Forms.ListBox LstPripojeni;
        private MaterialSkin.Controls.MaterialLabel LabelPripojeni;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtZprava;
        private MaterialSkin.Controls.MaterialRaisedButton BtnOdeslat;
        private MaterialSkin.Controls.MaterialFlatButton BtnOdeslatObrazek;
        private System.Windows.Forms.OpenFileDialog VolbaObrazku;
    }
}

