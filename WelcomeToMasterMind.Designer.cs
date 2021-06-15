
namespace MasterMind56955_JRN
{
    partial class WelcomeToMasterMind
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
            this.buttonNewClient = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonHelp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonNewClient
            // 
            this.buttonNewClient.BackColor = System.Drawing.Color.Silver;
            this.buttonNewClient.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonNewClient.FlatAppearance.BorderSize = 6;
            this.buttonNewClient.Location = new System.Drawing.Point(160, 188);
            this.buttonNewClient.Name = "buttonNewClient";
            this.buttonNewClient.Padding = new System.Windows.Forms.Padding(6);
            this.buttonNewClient.Size = new System.Drawing.Size(75, 75);
            this.buttonNewClient.TabIndex = 1;
            this.buttonNewClient.Text = "Start Game";
            this.buttonNewClient.UseVisualStyleBackColor = false;
            this.buttonNewClient.Click += new System.EventHandler(this.buttonNewClient_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MasterMind56955_JRN.Properties.Resources.brainwelcome1;
            this.pictureBox1.Location = new System.Drawing.Point(-33, -13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(508, 465);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.BackColor = System.Drawing.Color.Silver;
            this.buttonHelp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonHelp.FlatAppearance.BorderSize = 6;
            this.buttonHelp.Location = new System.Drawing.Point(249, 188);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Padding = new System.Windows.Forms.Padding(6);
            this.buttonHelp.Size = new System.Drawing.Size(75, 75);
            this.buttonHelp.TabIndex = 2;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = false;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // WelcomeToMasterMind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(23)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(475, 427);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonNewClient);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(491, 465);
            this.MinimumSize = new System.Drawing.Size(491, 465);
            this.Name = "WelcomeToMasterMind";
            this.Text = "WelcomeToMasterMind";
            this.Load += new System.EventHandler(this.WelcomeToMasterMind_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonNewClient;
        private System.Windows.Forms.Button buttonHelp;
    }
}