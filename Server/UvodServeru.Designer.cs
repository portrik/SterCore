namespace Server
{
    partial class UvodServeru
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UvodServeru));
            this.BtnServerStart = new MaterialSkin.Controls.MaterialRaisedButton();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.txtServerIP = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.txtPocetKlientu = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.BtnRozsNastaveni = new MaterialSkin.Controls.MaterialFlatButton();
            this.ChckUlozitNast = new MaterialSkin.Controls.MaterialCheckBox();
            this.SuspendLayout();
            // 
            // BtnServerStart
            // 
            this.BtnServerStart.Depth = 0;
            this.BtnServerStart.Location = new System.Drawing.Point(248, 338);
            this.BtnServerStart.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnServerStart.Name = "BtnServerStart";
            this.BtnServerStart.Primary = true;
            this.BtnServerStart.Size = new System.Drawing.Size(140, 50);
            this.BtnServerStart.TabIndex = 5;
            this.BtnServerStart.Text = "Start serveru";
            this.BtnServerStart.UseVisualStyleBackColor = true;
            this.BtnServerStart.Click += new System.EventHandler(this.BtnServerStart_Click);
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
            this.materialLabel1.Size = new System.Drawing.Size(79, 19);
            this.materialLabel1.TabIndex = 10;
            this.materialLabel1.Text = "IP serveru:";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Depth = 0;
            this.txtServerIP.Hint = "";
            this.txtServerIP.Location = new System.Drawing.Point(54, 112);
            this.txtServerIP.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.PasswordChar = '\0';
            this.txtServerIP.SelectedText = "";
            this.txtServerIP.SelectionLength = 0;
            this.txtServerIP.SelectionStart = 0;
            this.txtServerIP.Size = new System.Drawing.Size(100, 23);
            this.txtServerIP.TabIndex = 1;
            this.txtServerIP.UseSystemPasswordChar = false;
            this.txtServerIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartServer_Enter);
            // 
            // txtPocetKlientu
            // 
            this.txtPocetKlientu.Depth = 0;
            this.txtPocetKlientu.Hint = "";
            this.txtPocetKlientu.Location = new System.Drawing.Point(54, 184);
            this.txtPocetKlientu.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtPocetKlientu.Name = "txtPocetKlientu";
            this.txtPocetKlientu.PasswordChar = '\0';
            this.txtPocetKlientu.SelectedText = "";
            this.txtPocetKlientu.SelectionLength = 0;
            this.txtPocetKlientu.SelectionStart = 0;
            this.txtPocetKlientu.Size = new System.Drawing.Size(50, 23);
            this.txtPocetKlientu.TabIndex = 2;
            this.txtPocetKlientu.Text = "5";
            this.txtPocetKlientu.UseSystemPasswordChar = false;
            this.txtPocetKlientu.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartServer_Enter);
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel3.Location = new System.Drawing.Point(50, 162);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(173, 19);
            this.materialLabel3.TabIndex = 14;
            this.materialLabel3.Text = "Maximální počet klientů:";
            // 
            // BtnRozsNastaveni
            // 
            this.BtnRozsNastaveni.AutoSize = true;
            this.BtnRozsNastaveni.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnRozsNastaveni.Depth = 0;
            this.BtnRozsNastaveni.Location = new System.Drawing.Point(54, 273);
            this.BtnRozsNastaveni.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnRozsNastaveni.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnRozsNastaveni.Name = "BtnRozsNastaveni";
            this.BtnRozsNastaveni.Primary = false;
            this.BtnRozsNastaveni.Size = new System.Drawing.Size(164, 36);
            this.BtnRozsNastaveni.TabIndex = 4;
            this.BtnRozsNastaveni.Text = "Rozšířená nastavení";
            this.BtnRozsNastaveni.UseVisualStyleBackColor = true;
            this.BtnRozsNastaveni.Click += new System.EventHandler(this.BtnRozsNastaveni_click);
            // 
            // ChckUlozitNast
            // 
            this.ChckUlozitNast.AutoSize = true;
            this.ChckUlozitNast.Checked = true;
            this.ChckUlozitNast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChckUlozitNast.Depth = 0;
            this.ChckUlozitNast.Font = new System.Drawing.Font("Roboto", 10F);
            this.ChckUlozitNast.Location = new System.Drawing.Point(54, 228);
            this.ChckUlozitNast.Margin = new System.Windows.Forms.Padding(0);
            this.ChckUlozitNast.MouseLocation = new System.Drawing.Point(-1, -1);
            this.ChckUlozitNast.MouseState = MaterialSkin.MouseState.HOVER;
            this.ChckUlozitNast.Name = "ChckUlozitNast";
            this.ChckUlozitNast.Ripple = true;
            this.ChckUlozitNast.Size = new System.Drawing.Size(174, 30);
            this.ChckUlozitNast.TabIndex = 3;
            this.ChckUlozitNast.Text = "Pamatovat si nastavení";
            this.ChckUlozitNast.UseVisualStyleBackColor = true;
            // 
            // UvodServeru
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.ChckUlozitNast);
            this.Controls.Add(this.BtnRozsNastaveni);
            this.Controls.Add(this.BtnServerStart);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.txtPocetKlientu);
            this.Controls.Add(this.materialLabel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(400, 400);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "UvodServeru";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SterCore Server";
            this.Load += new System.EventHandler(this.UvodServeru_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialRaisedButton BtnServerStart;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtServerIP;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtPocetKlientu;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialFlatButton BtnRozsNastaveni;
        private MaterialSkin.Controls.MaterialCheckBox ChckUlozitNast;
    }
}