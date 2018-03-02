namespace Server
{
    partial class RozsNastaveni
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RozsNastaveni));
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.txtServerPort = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnPotvrdit = new MaterialSkin.Controls.MaterialRaisedButton();
            this.RadioSv = new MaterialSkin.Controls.MaterialRadioButton();
            this.RadioTm = new MaterialSkin.Controls.MaterialRadioButton();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.ChckUlozitHistorii = new MaterialSkin.Controls.MaterialCheckBox();
            this.BtnZrusit = new MaterialSkin.Controls.MaterialFlatButton();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.TxtKontrola = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.SuspendLayout();
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(37, 88);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(94, 19);
            this.materialLabel2.TabIndex = 14;
            this.materialLabel2.Text = "Port serveru:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Depth = 0;
            this.txtServerPort.Hint = "";
            this.txtServerPort.Location = new System.Drawing.Point(40, 110);
            this.txtServerPort.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.PasswordChar = '\0';
            this.txtServerPort.SelectedText = "";
            this.txtServerPort.SelectionLength = 0;
            this.txtServerPort.SelectionStart = 0;
            this.txtServerPort.Size = new System.Drawing.Size(90, 23);
            this.txtServerPort.TabIndex = 0;
            this.txtServerPort.UseSystemPasswordChar = false;
            // 
            // BtnPotvrdit
            // 
            this.BtnPotvrdit.Depth = 0;
            this.BtnPotvrdit.Location = new System.Drawing.Point(298, 388);
            this.BtnPotvrdit.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnPotvrdit.Name = "BtnPotvrdit";
            this.BtnPotvrdit.Primary = true;
            this.BtnPotvrdit.Size = new System.Drawing.Size(140, 50);
            this.BtnPotvrdit.TabIndex = 5;
            this.BtnPotvrdit.Text = "Potvrdit nastavení";
            this.BtnPotvrdit.UseVisualStyleBackColor = true;
            this.BtnPotvrdit.Click += new System.EventHandler(this.BtnPotvrdit_Click);
            // 
            // RadioSv
            // 
            this.RadioSv.AutoSize = true;
            this.RadioSv.Depth = 0;
            this.RadioSv.Font = new System.Drawing.Font("Roboto", 10F);
            this.RadioSv.Location = new System.Drawing.Point(41, 179);
            this.RadioSv.Margin = new System.Windows.Forms.Padding(0);
            this.RadioSv.MouseLocation = new System.Drawing.Point(-1, -1);
            this.RadioSv.MouseState = MaterialSkin.MouseState.HOVER;
            this.RadioSv.Name = "RadioSv";
            this.RadioSv.Ripple = true;
            this.RadioSv.Size = new System.Drawing.Size(66, 30);
            this.RadioSv.TabIndex = 1;
            this.RadioSv.Text = "Světlý";
            this.RadioSv.UseVisualStyleBackColor = true;
            // 
            // RadioTm
            // 
            this.RadioTm.AutoSize = true;
            this.RadioTm.Depth = 0;
            this.RadioTm.Font = new System.Drawing.Font("Roboto", 10F);
            this.RadioTm.Location = new System.Drawing.Point(41, 218);
            this.RadioTm.Margin = new System.Windows.Forms.Padding(0);
            this.RadioTm.MouseLocation = new System.Drawing.Point(-1, -1);
            this.RadioTm.MouseState = MaterialSkin.MouseState.HOVER;
            this.RadioTm.Name = "RadioTm";
            this.RadioTm.Ripple = true;
            this.RadioTm.Size = new System.Drawing.Size(70, 30);
            this.RadioTm.TabIndex = 2;
            this.RadioTm.Text = "Tmavý";
            this.RadioTm.UseVisualStyleBackColor = true;
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(37, 150);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(54, 19);
            this.materialLabel1.TabIndex = 20;
            this.materialLabel1.Text = "Vzhed:";
            // 
            // ChckUlozitHistorii
            // 
            this.ChckUlozitHistorii.AutoSize = true;
            this.ChckUlozitHistorii.Depth = 0;
            this.ChckUlozitHistorii.Font = new System.Drawing.Font("Roboto", 10F);
            this.ChckUlozitHistorii.Location = new System.Drawing.Point(40, 352);
            this.ChckUlozitHistorii.Margin = new System.Windows.Forms.Padding(0);
            this.ChckUlozitHistorii.MouseLocation = new System.Drawing.Point(-1, -1);
            this.ChckUlozitHistorii.MouseState = MaterialSkin.MouseState.HOVER;
            this.ChckUlozitHistorii.Name = "ChckUlozitHistorii";
            this.ChckUlozitHistorii.Ripple = true;
            this.ChckUlozitHistorii.Size = new System.Drawing.Size(111, 30);
            this.ChckUlozitHistorii.TabIndex = 4;
            this.ChckUlozitHistorii.Text = "Uložit historii";
            this.ChckUlozitHistorii.UseVisualStyleBackColor = true;
            // 
            // BtnZrusit
            // 
            this.BtnZrusit.AutoSize = true;
            this.BtnZrusit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnZrusit.Depth = 0;
            this.BtnZrusit.Location = new System.Drawing.Point(41, 388);
            this.BtnZrusit.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnZrusit.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnZrusit.Name = "BtnZrusit";
            this.BtnZrusit.Primary = false;
            this.BtnZrusit.Size = new System.Drawing.Size(104, 36);
            this.BtnZrusit.TabIndex = 6;
            this.BtnZrusit.Text = "Zrušit výběr";
            this.BtnZrusit.UseVisualStyleBackColor = true;
            this.BtnZrusit.Click += new System.EventHandler(this.BtnZrusit_Click);
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel3.Location = new System.Drawing.Point(37, 257);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(161, 19);
            this.materialLabel3.TabIndex = 23;
            this.materialLabel3.Text = "Rychlost kontroly (ms)";
            // 
            // TxtKontrola
            // 
            this.TxtKontrola.Depth = 0;
            this.TxtKontrola.Hint = "";
            this.TxtKontrola.Location = new System.Drawing.Point(41, 279);
            this.TxtKontrola.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtKontrola.Name = "TxtKontrola";
            this.TxtKontrola.PasswordChar = '\0';
            this.TxtKontrola.SelectedText = "";
            this.TxtKontrola.SelectionLength = 0;
            this.TxtKontrola.SelectionStart = 0;
            this.TxtKontrola.Size = new System.Drawing.Size(90, 23);
            this.TxtKontrola.TabIndex = 3;
            this.TxtKontrola.UseSystemPasswordChar = false;
            // 
            // RozsNastaveni
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 450);
            this.ControlBox = false;
            this.Controls.Add(this.TxtKontrola);
            this.Controls.Add(this.materialLabel3);
            this.Controls.Add(this.BtnZrusit);
            this.Controls.Add(this.ChckUlozitHistorii);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.RadioTm);
            this.Controls.Add(this.RadioSv);
            this.Controls.Add(this.BtnPotvrdit);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.txtServerPort);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RozsNastaveni";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rozšířená nastavení";
            this.Load += new System.EventHandler(this.RozsNastaveni_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtServerPort;
        private MaterialSkin.Controls.MaterialRaisedButton BtnPotvrdit;
        private MaterialSkin.Controls.MaterialRadioButton RadioSv;
        private MaterialSkin.Controls.MaterialRadioButton RadioTm;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialCheckBox ChckUlozitHistorii;
        private MaterialSkin.Controls.MaterialFlatButton BtnZrusit;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtKontrola;
    }
}