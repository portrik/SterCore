namespace Klient
{
    partial class UvodKlienta
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UvodKlienta));
            this.TxtPrezdivka = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnPripojit = new MaterialSkin.Controls.MaterialRaisedButton();
            this.TxtIP = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.ChckUlozitNast = new MaterialSkin.Controls.MaterialCheckBox();
            this.BtnRozsNastaveni = new MaterialSkin.Controls.MaterialFlatButton();
            this.SuspendLayout();
            // 
            // TxtPrezdivka
            // 
            this.TxtPrezdivka.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TxtPrezdivka.Depth = 0;
            this.TxtPrezdivka.Hint = "";
            this.TxtPrezdivka.Location = new System.Drawing.Point(54, 112);
            this.TxtPrezdivka.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtPrezdivka.Name = "TxtPrezdivka";
            this.TxtPrezdivka.PasswordChar = '\0';
            this.TxtPrezdivka.SelectedText = "";
            this.TxtPrezdivka.SelectionLength = 0;
            this.TxtPrezdivka.SelectionStart = 0;
            this.TxtPrezdivka.Size = new System.Drawing.Size(120, 23);
            this.TxtPrezdivka.TabIndex = 0;
            this.TxtPrezdivka.UseSystemPasswordChar = false;
            this.TxtPrezdivka.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtPrezdivka_KeyPress);
            // 
            // BtnPripojit
            // 
            this.BtnPripojit.Depth = 0;
            this.BtnPripojit.Location = new System.Drawing.Point(258, 340);
            this.BtnPripojit.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnPripojit.Name = "BtnPripojit";
            this.BtnPripojit.Primary = true;
            this.BtnPripojit.Size = new System.Drawing.Size(130, 48);
            this.BtnPripojit.TabIndex = 4;
            this.BtnPripojit.Text = "Připojit se";
            this.BtnPripojit.UseVisualStyleBackColor = true;
            this.BtnPripojit.Click += new System.EventHandler(this.BtnPripojit_Click);
            // 
            // TxtIP
            // 
            this.TxtIP.Depth = 0;
            this.TxtIP.Hint = "";
            this.TxtIP.Location = new System.Drawing.Point(54, 179);
            this.TxtIP.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtIP.Name = "TxtIP";
            this.TxtIP.PasswordChar = '\0';
            this.TxtIP.SelectedText = "";
            this.TxtIP.SelectionLength = 0;
            this.TxtIP.SelectionStart = 0;
            this.TxtIP.Size = new System.Drawing.Size(120, 23);
            this.TxtIP.TabIndex = 1;
            this.TxtIP.UseSystemPasswordChar = false;
            this.TxtIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtIP_KeyPress);
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(50, 90);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(73, 19);
            this.materialLabel1.TabIndex = 5;
            this.materialLabel1.Text = "Přezdívka";
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(50, 157);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(75, 19);
            this.materialLabel2.TabIndex = 6;
            this.materialLabel2.Text = "IP adresa:";
            // 
            // ChckUlozitNast
            // 
            this.ChckUlozitNast.AutoSize = true;
            this.ChckUlozitNast.Checked = true;
            this.ChckUlozitNast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChckUlozitNast.Depth = 0;
            this.ChckUlozitNast.Font = new System.Drawing.Font("Roboto", 10F);
            this.ChckUlozitNast.Location = new System.Drawing.Point(54, 226);
            this.ChckUlozitNast.Margin = new System.Windows.Forms.Padding(0);
            this.ChckUlozitNast.MouseLocation = new System.Drawing.Point(-1, -1);
            this.ChckUlozitNast.MouseState = MaterialSkin.MouseState.HOVER;
            this.ChckUlozitNast.Name = "ChckUlozitNast";
            this.ChckUlozitNast.Ripple = true;
            this.ChckUlozitNast.Size = new System.Drawing.Size(174, 30);
            this.ChckUlozitNast.TabIndex = 2;
            this.ChckUlozitNast.Text = "Pamatovat si nastavení";
            this.ChckUlozitNast.UseVisualStyleBackColor = true;
            // 
            // BtnRozsNastaveni
            // 
            this.BtnRozsNastaveni.AutoSize = true;
            this.BtnRozsNastaveni.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnRozsNastaveni.Depth = 0;
            this.BtnRozsNastaveni.Location = new System.Drawing.Point(54, 271);
            this.BtnRozsNastaveni.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnRozsNastaveni.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnRozsNastaveni.Name = "BtnRozsNastaveni";
            this.BtnRozsNastaveni.Primary = false;
            this.BtnRozsNastaveni.Size = new System.Drawing.Size(164, 36);
            this.BtnRozsNastaveni.TabIndex = 3;
            this.BtnRozsNastaveni.Text = "Rozšířená nastavení";
            this.BtnRozsNastaveni.UseVisualStyleBackColor = true;
            this.BtnRozsNastaveni.Click += new System.EventHandler(this.BtnRozsNastaveni_Click);
            // 
            // UvodKlienta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.ChckUlozitNast);
            this.Controls.Add(this.BtnRozsNastaveni);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.TxtIP);
            this.Controls.Add(this.BtnPripojit);
            this.Controls.Add(this.TxtPrezdivka);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(400, 400);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "UvodKlienta";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SterCore";
            this.Load += new System.EventHandler(this.KlientUvod_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialSingleLineTextField TxtPrezdivka;
        private MaterialSkin.Controls.MaterialRaisedButton BtnPripojit;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtIP;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialCheckBox ChckUlozitNast;
        private MaterialSkin.Controls.MaterialFlatButton BtnRozsNastaveni;
    }
}