namespace UploadFacePic
{
    partial class FrmPickFace
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
            this.pictureBox_SnapPic = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button_Upload = new System.Windows.Forms.Button();
            this.button_AutoDetect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SnapPic)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox_SnapPic
            // 
            this.pictureBox_SnapPic.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox_SnapPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_SnapPic.Location = new System.Drawing.Point(3, 17);
            this.pictureBox_SnapPic.Name = "pictureBox_SnapPic";
            this.pictureBox_SnapPic.Size = new System.Drawing.Size(575, 361);
            this.pictureBox_SnapPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_SnapPic.TabIndex = 0;
            this.pictureBox_SnapPic.TabStop = false;
            this.pictureBox_SnapPic.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox_SnapPic_Paint);
            this.pictureBox_SnapPic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox_SnapPic_MouseDown);
            this.pictureBox_SnapPic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox_SnapPic_MouseMove);
            this.pictureBox_SnapPic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox_SnapPic_MouseUp);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pictureBox_SnapPic);
            this.groupBox1.Location = new System.Drawing.Point(24, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(581, 381);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "截图";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(627, 29);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(150, 361);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // button_Upload
            // 
            this.button_Upload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Upload.Location = new System.Drawing.Point(702, 424);
            this.button_Upload.Name = "button_Upload";
            this.button_Upload.Size = new System.Drawing.Size(75, 23);
            this.button_Upload.TabIndex = 3;
            this.button_Upload.Text = "上传";
            this.button_Upload.UseVisualStyleBackColor = true;
            this.button_Upload.Click += new System.EventHandler(this.ButtonUpload_Click);
            // 
            // button_AutoDetect
            // 
            this.button_AutoDetect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_AutoDetect.Location = new System.Drawing.Point(527, 424);
            this.button_AutoDetect.Name = "button_AutoDetect";
            this.button_AutoDetect.Size = new System.Drawing.Size(75, 23);
            this.button_AutoDetect.TabIndex = 4;
            this.button_AutoDetect.Text = "自动检测";
            this.button_AutoDetect.UseVisualStyleBackColor = true;
            this.button_AutoDetect.Click += new System.EventHandler(this.Button_AutoDetect_Click);
            // 
            // FrmPickFace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 481);
            this.Controls.Add(this.button_AutoDetect);
            this.Controls.Add(this.button_Upload);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmPickFace";
            this.Text = "选择人脸";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPickFace_FormClosing);
            this.Load += new System.EventHandler(this.FrmPickFace_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SnapPic)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_SnapPic;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button_Upload;
        private System.Windows.Forms.Button button_AutoDetect;
    }
}