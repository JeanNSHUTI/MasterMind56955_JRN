
namespace MasterMind56955_JRN
{
    partial class MasterMindHelp
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
            this.buttonHBack = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.help1 = new System.Windows.Forms.PictureBox();
            this.helpSideBar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.help1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpSideBar)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonHBack
            // 
            this.buttonHBack.BackColor = System.Drawing.Color.Silver;
            this.buttonHBack.Location = new System.Drawing.Point(788, 441);
            this.buttonHBack.Name = "buttonHBack";
            this.buttonHBack.Size = new System.Drawing.Size(71, 82);
            this.buttonHBack.TabIndex = 4;
            this.buttonHBack.Text = "Back";
            this.buttonHBack.UseVisualStyleBackColor = false;
            this.buttonHBack.Click += new System.EventHandler(this.buttonHBack_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MasterMind56955_JRN.Properties.Resources.Help2;
            this.pictureBox2.Location = new System.Drawing.Point(442, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(313, 537);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MasterMind56955_JRN.Properties.Resources.Victoria8;
            this.pictureBox1.Location = new System.Drawing.Point(749, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(313, 537);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // help1
            // 
            this.help1.Image = global::MasterMind56955_JRN.Properties.Resources.Help1;
            this.help1.Location = new System.Drawing.Point(132, 0);
            this.help1.Name = "help1";
            this.help1.Size = new System.Drawing.Size(313, 537);
            this.help1.TabIndex = 1;
            this.help1.TabStop = false;
            // 
            // helpSideBar
            // 
            this.helpSideBar.Image = global::MasterMind56955_JRN.Properties.Resources.Victoria8;
            this.helpSideBar.Location = new System.Drawing.Point(1, 0);
            this.helpSideBar.Name = "helpSideBar";
            this.helpSideBar.Size = new System.Drawing.Size(313, 537);
            this.helpSideBar.TabIndex = 0;
            this.helpSideBar.TabStop = false;
            // 
            // MasterMindHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(252)))), ((int)(((byte)(211)))));
            this.ClientSize = new System.Drawing.Size(885, 535);
            this.Controls.Add(this.buttonHBack);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.help1);
            this.Controls.Add(this.helpSideBar);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(862, 573);
            this.Name = "MasterMindHelp";
            this.Text = "MasterMindHelp";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.help1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpSideBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox helpSideBar;
        private System.Windows.Forms.PictureBox help1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button buttonHBack;
    }
}