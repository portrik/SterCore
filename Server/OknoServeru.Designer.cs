namespace SterCore
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
            this.VypisChatu = new System.Windows.Forms.ListBox();
            this.VypisKlientu = new System.Windows.Forms.ListBox();
            this.TxtZprava = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.BtnZprava = new MaterialSkin.Controls.MaterialRaisedButton();
            this.BtnServerStop = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SuspendLayout();
            // 
            // VypisChatu
            // 
            this.VypisChatu.FormattingEnabled = true;
            this.VypisChatu.Location = new System.Drawing.Point(345, 117);
            this.VypisChatu.Name = "VypisChatu";
            this.VypisChatu.Size = new System.Drawing.Size(594, 446);
            this.VypisChatu.TabIndex = 6;
            // 
            // VypisKlientu
            // 
            this.VypisKlientu.FormattingEnabled = true;
            this.VypisKlientu.Location = new System.Drawing.Point(23, 382);
            this.VypisKlientu.Name = "VypisKlientu";
            this.VypisKlientu.Size = new System.Drawing.Size(303, 212);
            this.VypisKlientu.TabIndex = 9;
            // 
            // TxtZprava
            // 
            this.TxtZprava.Depth = 0;
            this.TxtZprava.Hint = "";
            this.TxtZprava.Location = new System.Drawing.Point(345, 570);
            this.TxtZprava.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtZprava.Name = "TxtZprava";
            this.TxtZprava.PasswordChar = '\0';
            this.TxtZprava.SelectedText = "";
            this.TxtZprava.SelectionLength = 0;
            this.TxtZprava.SelectionStart = 0;
            this.TxtZprava.Size = new System.Drawing.Size(455, 23);
            this.TxtZprava.TabIndex = 11;
            this.TxtZprava.UseSystemPasswordChar = false;
            this.TxtZprava.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtZprava_KeyPress);
            // 
            // BtnZprava
            // 
            this.BtnZprava.Depth = 0;
            this.BtnZprava.Location = new System.Drawing.Point(806, 569);
            this.BtnZprava.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnZprava.Name = "BtnZprava";
            this.BtnZprava.Primary = true;
            this.BtnZprava.Size = new System.Drawing.Size(133, 41);
            this.BtnZprava.TabIndex = 12;
            this.BtnZprava.Text = "Odeslat zprávu";
            this.BtnZprava.UseVisualStyleBackColor = true;
            this.BtnZprava.Click += new System.EventHandler(this.BtnZprava_Click);
            // 
            // BtnServerStop
            // 
            this.BtnServerStop.Depth = 0;
            this.BtnServerStop.Location = new System.Drawing.Point(33, 324);
            this.BtnServerStop.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnServerStop.Name = "BtnServerStop";
            this.BtnServerStop.Primary = true;
            this.BtnServerStop.Size = new System.Drawing.Size(117, 34);
            this.BtnServerStop.TabIndex = 10;
            this.BtnServerStop.Text = "Stop serveru";
            this.BtnServerStop.UseVisualStyleBackColor = true;
            this.BtnServerStop.Click += new System.EventHandler(this.BtnServerStop_Click);
            // 
            // OknoServeru
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 622);
            this.Controls.Add(this.BtnServerStop);
            this.Controls.Add(this.BtnZprava);
            this.Controls.Add(this.TxtZprava);
            this.Controls.Add(this.VypisKlientu);
            this.Controls.Add(this.VypisChatu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OknoServeru";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SterCore Server";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox VypisChatu;
        private System.Windows.Forms.ListBox VypisKlientu;
        private MaterialSkin.Controls.MaterialSingleLineTextField TxtZprava;
        private MaterialSkin.Controls.MaterialRaisedButton BtnZprava;
        private MaterialSkin.Controls.MaterialRaisedButton BtnServerStop;
    }
}

