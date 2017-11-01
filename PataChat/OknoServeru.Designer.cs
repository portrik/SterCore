namespace PataChat
{
    partial class Server
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
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.txtServerPort = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnServerStart = new MaterialSkin.Controls.MaterialFlatButton();
            this.BtnServerStop = new MaterialSkin.Controls.MaterialFlatButton();
            this.VypisChatu = new System.Windows.Forms.ListBox();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.txtServerIP = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.txtPocetKlientu = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.VypisKlientu = new System.Windows.Forms.ListBox();
            this.GrpOvladace = new System.Windows.Forms.GroupBox();
            this.GrpOvladace.SuspendLayout();
            this.SuspendLayout();
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(6, 70);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(94, 19);
            this.materialLabel2.TabIndex = 2;
            this.materialLabel2.Text = "Port serveru:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Depth = 0;
            this.txtServerPort.Hint = "";
            this.txtServerPort.Location = new System.Drawing.Point(106, 70);
            this.txtServerPort.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.PasswordChar = '\0';
            this.txtServerPort.SelectedText = "";
            this.txtServerPort.SelectionLength = 0;
            this.txtServerPort.SelectionStart = 0;
            this.txtServerPort.Size = new System.Drawing.Size(191, 23);
            this.txtServerPort.TabIndex = 3;
            this.txtServerPort.Text = "8888";
            this.txtServerPort.UseSystemPasswordChar = false;
            this.txtServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServerIP_KeyPress);
            // 
            // BtnServerStart
            // 
            this.BtnServerStart.AutoSize = true;
            this.BtnServerStart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnServerStart.Depth = 0;
            this.BtnServerStart.Location = new System.Drawing.Point(10, 148);
            this.BtnServerStart.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnServerStart.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnServerStart.Name = "BtnServerStart";
            this.BtnServerStart.Primary = false;
            this.BtnServerStart.Size = new System.Drawing.Size(117, 36);
            this.BtnServerStart.TabIndex = 4;
            this.BtnServerStart.Text = "Start serveru";
            this.BtnServerStart.UseVisualStyleBackColor = true;
            this.BtnServerStart.Click += new System.EventHandler(this.BtnServerStart_Click);
            // 
            // BtnServerStop
            // 
            this.BtnServerStop.AutoSize = true;
            this.BtnServerStop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnServerStop.Depth = 0;
            this.BtnServerStop.Location = new System.Drawing.Point(33, 324);
            this.BtnServerStop.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnServerStop.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnServerStop.Name = "BtnServerStop";
            this.BtnServerStop.Primary = false;
            this.BtnServerStop.Size = new System.Drawing.Size(109, 36);
            this.BtnServerStop.TabIndex = 5;
            this.BtnServerStop.Text = "Stop serveru";
            this.BtnServerStop.UseVisualStyleBackColor = true;
            this.BtnServerStop.Click += new System.EventHandler(this.BtnServerStop_Click);
            // 
            // VypisChatu
            // 
            this.VypisChatu.FormattingEnabled = true;
            this.VypisChatu.Location = new System.Drawing.Point(345, 117);
            this.VypisChatu.Name = "VypisChatu";
            this.VypisChatu.Size = new System.Drawing.Size(594, 485);
            this.VypisChatu.TabIndex = 6;
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(6, 33);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(79, 19);
            this.materialLabel1.TabIndex = 0;
            this.materialLabel1.Text = "IP serveru:";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Depth = 0;
            this.txtServerIP.Hint = "";
            this.txtServerIP.Location = new System.Drawing.Point(106, 33);
            this.txtServerIP.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.PasswordChar = '\0';
            this.txtServerIP.SelectedText = "";
            this.txtServerIP.SelectionLength = 0;
            this.txtServerIP.SelectionStart = 0;
            this.txtServerIP.Size = new System.Drawing.Size(191, 23);
            this.txtServerIP.TabIndex = 1;
            this.txtServerIP.UseSystemPasswordChar = false;
            this.txtServerIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServerIP_KeyPress);
            // 
            // txtPocetKlientu
            // 
            this.txtPocetKlientu.Depth = 0;
            this.txtPocetKlientu.Hint = "";
            this.txtPocetKlientu.Location = new System.Drawing.Point(185, 100);
            this.txtPocetKlientu.MouseState = MaterialSkin.MouseState.HOVER;
            this.txtPocetKlientu.Name = "txtPocetKlientu";
            this.txtPocetKlientu.PasswordChar = '\0';
            this.txtPocetKlientu.SelectedText = "";
            this.txtPocetKlientu.SelectionLength = 0;
            this.txtPocetKlientu.SelectionStart = 0;
            this.txtPocetKlientu.Size = new System.Drawing.Size(112, 23);
            this.txtPocetKlientu.TabIndex = 8;
            this.txtPocetKlientu.Text = "5";
            this.txtPocetKlientu.UseSystemPasswordChar = false;
            this.txtPocetKlientu.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServerIP_KeyPress);
            // 
            // materialLabel3
            // 
            this.materialLabel3.AutoSize = true;
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel3.Location = new System.Drawing.Point(6, 96);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(173, 19);
            this.materialLabel3.TabIndex = 7;
            this.materialLabel3.Text = "Maximální počet klientů:";
            // 
            // VypisKlientu
            // 
            this.VypisKlientu.FormattingEnabled = true;
            this.VypisKlientu.Location = new System.Drawing.Point(23, 382);
            this.VypisKlientu.Name = "VypisKlientu";
            this.VypisKlientu.Size = new System.Drawing.Size(303, 212);
            this.VypisKlientu.TabIndex = 9;
            // 
            // GrpOvladace
            // 
            this.GrpOvladace.Controls.Add(this.materialLabel1);
            this.GrpOvladace.Controls.Add(this.txtServerIP);
            this.GrpOvladace.Controls.Add(this.txtPocetKlientu);
            this.GrpOvladace.Controls.Add(this.materialLabel2);
            this.GrpOvladace.Controls.Add(this.BtnServerStart);
            this.GrpOvladace.Controls.Add(this.materialLabel3);
            this.GrpOvladace.Controls.Add(this.txtServerPort);
            this.GrpOvladace.Location = new System.Drawing.Point(23, 117);
            this.GrpOvladace.Name = "GrpOvladace";
            this.GrpOvladace.Size = new System.Drawing.Size(303, 188);
            this.GrpOvladace.TabIndex = 10;
            this.GrpOvladace.TabStop = false;
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 622);
            this.Controls.Add(this.GrpOvladace);
            this.Controls.Add(this.VypisKlientu);
            this.Controls.Add(this.VypisChatu);
            this.Controls.Add(this.BtnServerStop);
            this.Name = "Server";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server";
            this.Load += new System.EventHandler(this.Server_Load);
            this.GrpOvladace.ResumeLayout(false);
            this.GrpOvladace.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtServerPort;
        private MaterialSkin.Controls.MaterialFlatButton BtnServerStart;
        private MaterialSkin.Controls.MaterialFlatButton BtnServerStop;
        private System.Windows.Forms.ListBox VypisChatu;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtServerIP;
        private MaterialSkin.Controls.MaterialSingleLineTextField txtPocetKlientu;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private System.Windows.Forms.ListBox VypisKlientu;
        private System.Windows.Forms.GroupBox GrpOvladace;
    }
}

