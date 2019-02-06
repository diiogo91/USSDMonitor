namespace USSDMonitor
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.btnMcel = new System.Windows.Forms.Button();
            this.btnMovitel = new System.Windows.Forms.Button();
            this.btnVodacom = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.DeepPink;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F);
            this.label1.Location = new System.Drawing.Point(208, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(779, 46);
            this.label1.TabIndex = 10;
            this.label1.Text = "Monitorização do Serviço Mobile IZI USSD";
            // 
            // btnMcel
            // 
            this.btnMcel.BackgroundImage = global::USSDMonitor.Properties.Resources.mcel;
            this.btnMcel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMcel.Location = new System.Drawing.Point(216, 281);
            this.btnMcel.Name = "btnMcel";
            this.btnMcel.Size = new System.Drawing.Size(223, 200);
            this.btnMcel.TabIndex = 11;
            this.btnMcel.UseVisualStyleBackColor = true;
            this.btnMcel.Click += new System.EventHandler(this.btnMcel_Click);
            // 
            // btnMovitel
            // 
            this.btnMovitel.BackgroundImage = global::USSDMonitor.Properties.Resources.movitel2;
            this.btnMovitel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMovitel.Location = new System.Drawing.Point(764, 281);
            this.btnMovitel.Name = "btnMovitel";
            this.btnMovitel.Size = new System.Drawing.Size(223, 200);
            this.btnMovitel.TabIndex = 13;
            this.btnMovitel.UseVisualStyleBackColor = true;
            this.btnMovitel.Click += new System.EventHandler(this.btnMovitel_Click);
            // 
            // btnVodacom
            // 
            this.btnVodacom.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnVodacom.BackgroundImage = global::USSDMonitor.Properties.Resources.vodacom;
            this.btnVodacom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVodacom.Location = new System.Drawing.Point(490, 281);
            this.btnVodacom.Name = "btnVodacom";
            this.btnVodacom.Size = new System.Drawing.Size(223, 200);
            this.btnVodacom.TabIndex = 12;
            this.btnVodacom.UseVisualStyleBackColor = false;
            this.btnVodacom.Click += new System.EventHandler(this.btnVodacom_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.BackgroundImage = global::USSDMonitor.Properties.Resources.exit;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Location = new System.Drawing.Point(1, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 61);
            this.button1.TabIndex = 27;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F);
            this.label2.Location = new System.Drawing.Point(287, 497);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 26);
            this.label2.TabIndex = 28;
            this.label2.Text = "mCel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F);
            this.label3.Location = new System.Drawing.Point(542, 497);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 26);
            this.label3.TabIndex = 29;
            this.label3.Text = "VodaCom";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F);
            this.label4.Location = new System.Drawing.Point(832, 497);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 26);
            this.label4.TabIndex = 30;
            this.label4.Text = "Movitel";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnMcel);
            this.Controls.Add(this.btnMovitel);
            this.Controls.Add(this.btnVodacom);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMcel;
        private System.Windows.Forms.Button btnVodacom;
        private System.Windows.Forms.Button btnMovitel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}