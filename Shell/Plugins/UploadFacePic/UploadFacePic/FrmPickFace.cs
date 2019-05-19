using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UploadFacePic
{
    public partial class FrmPickFace : Form
    {

        private FaceDetect faceDetect;

        public event FaceDetectEventHandle FaceDetected;

        private Image m_SelectedImage;

        private Image m_SourceImage;


        public FrmPickFace()
        {
            InitializeComponent();
            this.faceDetect = new FaceDetect();
        }

        private void FrmPickFace_Load(object sender, EventArgs e)
        {
            this.pictureBox_SnapPic.Image = this.m_SourceImage;
        }

        public void SetPicFile(Image img)
        {
            this.m_SourceImage = img;
        }


        #region 画框

        private Rectangle rectangle;

        /// <summary>
        /// 鼠标按下的点
        /// </summary>
        private Point StartPoint;

        private bool isDraw = false;

        private void PictureBox_SnapPic_MouseDown(object sender, MouseEventArgs e)
        {
            isDraw = true;
            rectangle = new Rectangle(e.X, e.Y, e.X, e.Y);
            this.StartPoint = new Point(e.X, e.Y);
        }

        private void PictureBox_SnapPic_MouseUp(object sender, MouseEventArgs e)
        {

            isDraw = false;
            Image img = this.pictureBox_SnapPic.Image;

      

            
            int width = rectangle.Width * img.Width / this.pictureBox_SnapPic.Width;
            int height = rectangle.Height * img.Height / this.pictureBox_SnapPic.Height;
            int x = rectangle.X * img.Width / this.pictureBox_SnapPic.Width;
            int y = rectangle.Y * img.Height / this.pictureBox_SnapPic.Height;
            this.addImageToList(this.pictureBox_SnapPic.Image, new Rectangle(x, y, width, height));
        }

        private void PictureBox_SnapPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraw == false)
                return;
            int x = e.X > 0 ? e.X : 0;
            int y = e.Y > 0 ? e.Y : 0;
            x = e.X < this.pictureBox_SnapPic.Width ? x : this.pictureBox_SnapPic.Width;
            y = e.Y < this.pictureBox_SnapPic.Height ? y : this.pictureBox_SnapPic.Height;

            rectangle.Height =Math.Abs( y- this.StartPoint.Y);
            rectangle.Width = Math.Abs( x - this.StartPoint.X);
            rectangle.X = this.StartPoint.X < x ? this.StartPoint.X : x;
            rectangle.Y = this.StartPoint.Y < y ? this.StartPoint.Y : y;
                
            this.pictureBox_SnapPic.Refresh();
        }

        private void PictureBox_SnapPic_Paint(object sender, PaintEventArgs e)
        {
            Console.WriteLine($"{rectangle.X}, {rectangle.Y}, {rectangle.Width}, {rectangle.Height}");
            e.Graphics.DrawRectangle(new Pen(Color.Red), rectangle);
            
        }



        #endregion

        #region 人脸检测
        private void Button_AutoDetect_Click(object sender, EventArgs e)
        {
            string fileName = "FaceDetectTemp.jpg";
            this.pictureBox_SnapPic.Image.Save(fileName);

            Mat img1 = CvInvoke.Imread(fileName, Emgu.CV.CvEnum.LoadImageType.AnyColor);

            FaceDetect.faceDetectedObj pfaceDetectedObj = faceDetect.GetFaceRectangle(img1);

            pfaceDetectedObj.facesRectangle.ForEach((f) =>
            {
                var rec = FixedRect(f, this.pictureBox_SnapPic.Image.Width, this.pictureBox_SnapPic.Image.Height);
                this.addImageToList(this.pictureBox_SnapPic.Image, rec);
            });
            //using (Graphics g = Graphics.FromImage(this.pictureBox1.Image))
            //{
            //    foreach (Rectangle face in pfaceDetectedObj.facesRectangle)
            //    {
            //        g.DrawRectangle(new Pen(Color.Red, 2), face);//给识别出的人脸画矩形框
            //    }
            //}

        }

        private Rectangle FixedRect(Rectangle src, int picWidth, int picHeight)
        {
            int x = src.X - src.Width * 10 / 100;
            int y = src.Y - src.Height * 15 / 100;
            int width = src.Width * 115 / 100;
            int height = src.Height * 130 / 100;
            x = x >= 0 ? x : 0;
            y = y >= 0 ? y : 0;

            width = width <= picWidth ? width : picWidth;
            height = height <= picHeight ? height : picHeight;

            return new Rectangle(x, y, width, height);


        }

        /// <summary>
        /// 截取图片区域，返回所截取的图片
        /// </summary>
        /// <param name="SrcImage"></param>
        /// <param name="pos"></param>
        /// <param name="cutWidth"></param>
        /// <param name="cutHeight"></param>
        /// <returns></returns>
        private Image cutImage(Image SrcImage, Point pos, int cutWidth, int cutHeight)
        {

            Image cutedImage = null;
            if (cutWidth <= 0 || cutHeight <= 0)
                return null;

            //先初始化一个位图对象，来存储截取后的图像
            Bitmap bmpDest = new Bitmap(cutWidth, cutHeight, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(bmpDest);

            //矩形定义,将要在被截取的图像上要截取的图像区域的左顶点位置和截取的大小
            Rectangle rectSource = new Rectangle(pos.X, pos.Y, cutWidth, cutHeight);


            //矩形定义,将要把 截取的图像区域 绘制到初始化的位图的位置和大小
            //rectDest说明，将把截取的区域，从位图左顶点开始绘制，绘制截取的区域原来大小
            Rectangle rectDest = new Rectangle(0, 0, cutWidth, cutHeight);

            //第一个参数就是加载你要截取的图像对象，第二个和第三个参数及如上所说定义截取和绘制图像过程中的相关属性，第四个属性定义了属性值所使用的度量单位
            g.DrawImage(SrcImage, rectDest, rectSource, GraphicsUnit.Pixel);

            //在GUI上显示被截取的图像
            cutedImage = (Image)bmpDest;

            g.Dispose();

            return cutedImage;

        }

        /// <summary>
        /// 添加指定区域到列表
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rect"></param>
        private void addImageToList(Image source, Rectangle rect)
        {
            Image dst = this.cutImage(source, new Point(rect.Left, rect.Top), rect.Width, rect.Height);
            if (dst == null)
                return;
            Panel panel = new Panel() { Height = 100, Width = 120, Padding = new Padding(0), BackColor = Color.Red };

            PictureBox pb = new PictureBox() { Dock =  DockStyle.Fill,SizeMode= PictureBoxSizeMode.StretchImage };
            pb.Click += new EventHandler((s, e) =>
            {
                Panel parent = (s as PictureBox).Parent as Panel;
                foreach(Panel p in this.flowLayoutPanel1.Controls)
                {
                    if (p == parent)
                        p.Padding = new Padding(3);
                    else
                        p.Padding = new Padding(0);
                }
                this.m_SelectedImage = pb.Image;
            });
            pb.Image = dst;
            panel.Controls.Add(pb);
            this.flowLayoutPanel1.Controls.Add(panel);
        }

        #endregion

        private void ButtonUpload_Click(object sender, EventArgs e)
        {
            this.FaceDetected?.Invoke(this, new FaceDetectEventArgs() { FaceImage = this.m_SelectedImage });
        }

        private void FrmPickFace_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.rectangle = new Rectangle();
            this.flowLayoutPanel1.Controls.Clear();
            e.Cancel = true;
            this.Hide();
        }

      
    }
}
