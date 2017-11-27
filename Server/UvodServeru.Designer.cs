namespace SterCore
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
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.txtServerPort = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.SuspendLayout();
            // 
            // BtnServerStart
            // 
            this.BtnServerStart.Depth = 0;
            this.BtnServerStart.Location = new System.Drawing.Point(158, 356);
            this.BtnServerStart.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnServerStart.Name = "BtnServerStart";
            this.BtnServerStart.Primary = true;
            this.BtnServerStart.Size = new System.Drawing.Size(140, 50);
            this.BtnServerStart.TabIndex = 16;
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
            this.materialLabel1.Location = new System.Drawing.Point(90, 128);
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
            this.txtServerIP.Location = new System.Drawing.Point(94, 150);
            this.txtServerIP.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.PasswordChar = '\0';
            this.txtServerIP.SelectedText = "";
            this.txtServerIP.SelectionLength = 0;
            this.txtServerIP.SelectionStart = 0;
            this.txtServerIP.Size = new System.Drawing.Size(191, 23);
            this.txtServerIP.TabIndex = 11;
            this.txtServerIP.UseSystemPasswordChar = false;
            this.txtServerIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartServer_Enter);
            // 
            // txtPocetKlientu
            // 
            this.txtPocetKlientu.Depth = 0;
            this.txtPocetKlientu.Hint = "";
            this.txtPocetKlientu.Location = new System.Drawing.Point(94, 274);
            this.txtPocetKlientu.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtPocetKlientu.Name = "txtPocetKlientu";
            this.txtPocetKlientu.PasswordChar = '\0';
            this.txtPocetKlientu.SelectedText = "";
            this.txtPocetKlientu.SelectionLength = 0;
            this.txtPocetKlientu.SelectionStart = 0;
            this.txtPocetKlientu.Size = new System.Drawing.Size(112, 23);
            this.txtPocetKlientu.TabIndex = 15;
            this.txtPocetKlientu.Text = "5";
            this.txtPocetKlientu.UseSystemPasswordChar = false;
            this.txtPocetKlientu.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartServer_Enter);
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(90, 188);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(94, 19);
            this.materialLabel2.TabIndex = 12;
            this.materialLabel2.Text = "Port serveru:";
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel3.Location = new System.Drawing.Point(90, 252);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(173, 19);
            this.materialLabel3.TabIndex = 14;
            this.materialLabel3.Text = "Maximální počet klientů:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Depth = 0;
            this.txtServerPort.Hint = "";
            this.txtServerPort.Location = new System.Drawing.Point(94, 210);
            this.txtServerPort.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.PasswordChar = '\0';
            this.txtServerPort.SelectedText = "";
            this.txtServerPort.SelectionLength = 0;
            this.txtServerPort.SelectionStart = 0;
            this.txtServerPort.Size = new System.Drawing.Size(191, 23);
            this.txtServerPort.TabIndex = 13;
            this.txtServerPort.Text = "8888";
            this.txtServerPort.UseSystemPasswordChar = false;
            this.txtServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartServer_Enter);
            // 
            // UvodServeru
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 470);
            this.Controls.Add(this.BtnServerStart);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.txtPocetKlientu);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.materialLabel3);
            this.Controls.Add(this.txtServerPort);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(470, 470);
            this.MinimumSize = new System.Drawing.Size(470, 470);
            this.Name = "UvodServeru";
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
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtServerPort;
    }
}