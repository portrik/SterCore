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
            this.TxtZprava = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.GrpZpravy = new System.Windows.Forms.GroupBox();
            this.BtnOdeslat = new MaterialSkin.Controls.MaterialRaisedButton();
            this.BtnOdpojit = new MaterialSkin.Controls.MaterialRaisedButton();
            this.LstPripojeni = new System.Windows.Forms.ListBox();
            this.LabelPripojeni = new MaterialSkin.Controls.MaterialLabel();
            this.GrpZpravy.SuspendLayout();
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
            // BtnOdeslat
            // 
            this.BtnOdeslat.Depth = 0;
            this.BtnOdeslat.Location = new System.Drawing.Point(459, 12);
            this.BtnOdeslat.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnOdeslat.Name = "BtnOdeslat";
            this.BtnOdeslat.Primary = true;
            this.BtnOdeslat.Size = new System.Drawing.Size(128, 39);
            this.BtnOdeslat.TabIndex = 21;
            this.BtnOdeslat.Text = "Odeslat zprávu";
            this.BtnOdeslat.UseVisualStyleBackColor = true;
            this.BtnOdeslat.Click += new System.EventHandler(this.BtnOdeslat_Click);
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
            // OknoKlienta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 680);
            this.Controls.Add(this.LabelPripojeni);
            this.Controls.Add(this.LstPripojeni);
            this.Controls.Add(this.BtnOdpojit);
            this.Controls.Add(this.GrpZpravy);
            this.Controls.Add(this.VypisChatu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OknoKlienta";
            this.Text = "Klient";
            this.Load += new System.EventHandler(this.Klient_Load);
            this.GrpZpravy.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox VypisChatu;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtZprava;
        private System.Windows.Forms.GroupBox GrpZpravy;
        private MaterialSkin.Controls.MaterialRaisedButton BtnOdeslat;
        private MaterialSkin.Controls.MaterialRaisedButton BtnOdpojit;
        private System.Windows.Forms.ListBox LstPripojeni;
        private MaterialSkin.Controls.MaterialLabel LabelPripojeni;
    }
}

