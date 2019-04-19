using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientAPP.VideoModule
{
    public class VideoPanel:Panel
    {
        /// <summary>
        /// 当前布局
        /// </summary>
        private Layout m_Layout;
        /// <summary>
        /// 当前是否某个图像最大化
        /// </summary>
        private bool m_IsMax;
        /// <summary>
        /// 当前选中的控件
        /// </summary>
        private VideoControl m_CurControl;


        /// <summary>
        /// 所有视频控件（当前排列数少于控件数时，显示前n个）
        /// </summary>
        public List<VideoControl> VCList { get; set; }

        /// <summary>
        /// 选中的控件更改
        /// </summary>
        public event ControlChangedEvent ControlChanged;



        /// <summary>
        /// 可视控件的数量，由layout决定
        /// </summary>
        public int UsingControl
        {
            get
            {
                return this.CurLayout.ControlList.Count;
            }
        }

        /// <summary>
        /// 当前视频控件排列方式
        /// </summary>
        public Layout CurLayout
        {
            get { return this.m_Layout; }
            set
            {
                if (this.m_Layout == null)
                    this.m_Layout = value;// 如果第一次赋值，直接赋，否则将上一次的视图先恢复
                for (int i = 0; i < this.m_Layout.ControlList.Count; i++)//切换视图时全部显示
                {

                    this.VCList[i].Selected = false;
                    this.VCList[i].IsMax = false;
                    this.VCList[i].Visible = false;

                }
                this.VCList[0].Selected = true;

                this.m_Layout = value;
                this.RefreshPanel(this, new EventArgs());
            }
        }

        

        public void Init()
        {
          
            this.Controls.AddRange(this.VCList.ToArray());
            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                parentForm.ResizeEnd += new EventHandler(VideoPanelManager_ResizeEnd);
                parentForm.Resize += new EventHandler(VideoPanelManager_Resize);
            }
            else
            {
                this.Resize += new EventHandler(RefreshPanel);
            }
            foreach (VideoControl vc in this.VCList)
            {
                //vc.ControlSeleted += new EventHandler(vc_ControlSeleted);
                vc.ActionEvent += new VideoControlEventHandle(vc_ActionProc);
            }
            this.VCList[0].Selected = true;
            this.m_CurControl = this.VCList[0];
            if (parentForm != null)
            {
                parentForm.FindForm().BackColor = this.VCList[0].VControl.BackColor;
            }
            this.BackColor = this.VCList[0].VControl.BackColor;
        }

        void VideoPanelManager_Resize(object sender, EventArgs e)
        {
            this.RefreshPanel(sender, e);
            //if (FatherPanel.FindForm().WindowState == FormWindowState.Maximized)
            //    this.RefreshPanel(sender, e);
        }

        void VideoPanelManager_ResizeEnd(object sender, EventArgs e)
        {
            //this.RefreshPanel(sender, e);
        }

        /// <summary>
        /// 交换位置
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public void Transposition(int index1, int index2)
        {
            VideoControl vc1 = this.VCList[index1];
            VideoControl vc2 = this.VCList[index2];

            vc1.Name = index2.ToString();
            vc2.Name = index1.ToString();

            VideoControl temp = new VideoControl() { Location = new System.Drawing.Point(vc1.Location.X, vc1.Location.Y), Width = vc1.Width, Height = vc1.Height };

            vc1.Location = new System.Drawing.Point(vc2.Location.X, vc2.Location.Y);
            vc1.Width = vc2.Width;
            vc1.Height = vc2.Height;

            vc2.Location = new System.Drawing.Point(temp.Location.X, temp.Location.Y);
            vc2.Width = temp.Width;
            vc2.Height = temp.Height;

            this.VCList[index2] = vc1;
            this.VCList[index1] = vc2;

        }


        private void vc_ActionProc(object sender, VideoControlActionEventArgs e)
        {
            VideoControlActionEventArgs voe = e as VideoControlActionEventArgs;

            switch (voe.Type)
            {
                case VideoControlAction.FullScreen:
                case VideoControlAction.NormalScreen:
                    if (this.CurLayout.ScreenCount == 1)
                    {
                        foreach (VideoControlLayout vcl in this.CurLayout.ControlList)
                        {
                            if (vcl.Control != voe.VControl)
                                vcl.Control.IsMax = false;
                        }

                    }
                    this.RefreshPanel(this, new EventArgs());
                    break;
            }

        }

        void vc_ControlSeleted(object sender, EventArgs e)
        {
            ControlChangedEventArgs cce = new ControlChangedEventArgs();
            for (int i = 0; i < this.m_Layout.ControlList.Count; i++)
            {
                if (sender == this.VCList[i])
                {
                    cce.Sn = i;
                    cce.VControl = this.VCList[i];
                    this.m_CurControl = this.VCList[i];
                    continue;
                }
                this.VCList[i].Selected = false;
            }
            this.ControlChanged(this, cce);
        }


        /// <summary>
        /// 刷新面板内所有控件的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshPanel(object sender, EventArgs e)
        {
            if (this.m_CurControl != null)
            {
                if (this.m_CurControl.BackColor != System.Drawing.Color.Black)
                    this.m_CurControl.BackColor = System.Drawing.Color.Black;
            }

            int width = this.Width / this.m_Layout.ColumnCount;
            int height = this.Height / this.m_Layout.RowCount;

            int VideoSN = 0;
            foreach (VideoControlLayout vcl in this.m_Layout.ControlList)
            {
                vcl.Control = this.VCList[VideoSN];
                vcl.Control.Visible = true;
                if (vcl.Control.IsMax == false)//没有最大化的控件
                {
                    vcl.Control.Location = new System.Drawing.Point(vcl.Column * width, vcl.Row * height);
                    vcl.Control.Width = width * vcl.ColumnSpan;
                    vcl.Control.Height = height * vcl.RowSpan;
                    //vcl.Control.SendToBack();
                }
                else//最大化的控件
                {
                    switch (this.CurLayout.ScreenCount)
                    {
                        case 1://单屏

                            vcl.Control.Location = new System.Drawing.Point(vcl.MaxColumn * width, vcl.MaxRow * height);
                            vcl.Control.Width = width * vcl.MaxColumnSpan;
                            vcl.Control.Height = height * vcl.MaxRowSpan;

                            //vcl.Control.Location = new System.Drawing.Point(0, 0);
                            //vcl.Control.Width = this.FatherPanel.Width;
                            //vcl.Control.Height = this.FatherPanel.Height;

                            break;
                        case 2://双屏
                            if (vcl.Column < this.CurLayout.ColumnCount / 2)//左半边
                            {
                                vcl.Control.Location = new System.Drawing.Point(0, 0);
                                vcl.Control.Width = this.Width / 2;
                                vcl.Control.Height = this.Height;
                            }
                            else//右半边
                            {
                                vcl.Control.Location = new System.Drawing.Point(this.Width / 2 - 1, 0);
                                vcl.Control.Width = this.Width / 2;
                                vcl.Control.Height = this.Height;
                            }
                            break;
                        default:
                            break;
                    }
                    vcl.Control.BringToFront();

                }
                VideoSN++;
            }

            foreach (VideoControlLayout vcl in this.m_Layout.ControlList.Where(vc => vc.IsTop == true))
            {
                vcl.Control.BringToFront();
                if (vcl.Control.ShowTitle == true)
                {
                    vcl.Control.ShowTitle = false;
                    vcl.Control.VideoPanelMargin = 0;
                }
            }

            if (this.m_CurControl != null)
            {
                if (this.m_CurControl.BackColor != this.m_CurControl.BackgroundColor)
                    this.m_CurControl.BackColor = this.m_CurControl.BackgroundColor;
            }
        }


        /// <summary>
        /// 刷新面板内所有控件的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RefreshPanelAll(int Height, int Width)
        {
            if (this.m_CurControl != null)
            {
                if (this.m_CurControl.BackColor != System.Drawing.Color.Black)
                    this.m_CurControl.BackColor = System.Drawing.Color.Black;
            }
            int VideoSN = 0;
            int width = Width / this.m_Layout.ColumnCount;
            int height = Height / this.m_Layout.RowCount;
            foreach (VideoControlLayout vcl in this.m_Layout.ControlList)
            {
                vcl.Control = this.VCList[VideoSN];
                vcl.Control.Visible = true;
                if (vcl.Control.IsMax == false)//没有最大化的控件
                {
                    vcl.Control.Location = new System.Drawing.Point(vcl.Column * width, vcl.Row * height);
                    vcl.Control.Width = width * vcl.ColumnSpan;
                    vcl.Control.Height = height * vcl.RowSpan;
                    //vcl.Control.SendToBack();
                }
                else//最大化的控件
                {
                    switch (this.CurLayout.ScreenCount)
                    {
                        case 1://单屏

                            vcl.Control.Location = new System.Drawing.Point(vcl.MaxColumn * width, vcl.MaxRow * height);
                            vcl.Control.Width = width * vcl.MaxColumnSpan;
                            vcl.Control.Height = height * vcl.MaxRowSpan;

                            //vcl.Control.Location = new System.Drawing.Point(0, 0);
                            //vcl.Control.Width = this.FatherPanel.Width;
                            //vcl.Control.Height = this.FatherPanel.Height;

                            break;
                        case 2://双屏
                            if (vcl.Column < this.CurLayout.ColumnCount / 2)//左半边
                            {
                                vcl.Control.Location = new System.Drawing.Point(0, 0);
                                vcl.Control.Width = this.Width / 2;
                                vcl.Control.Height = this.Height;
                            }
                            else//右半边
                            {
                                vcl.Control.Location = new System.Drawing.Point(this.Width / 2 - 1, 0);
                                vcl.Control.Width = this.Width / 2;
                                vcl.Control.Height = this.Height;
                            }
                            break;
                        default:
                            break;
                    }
                    vcl.Control.BringToFront();

                }
                VideoSN++;
            }

            foreach (VideoControlLayout vcl in this.m_Layout.ControlList.Where(vc => vc.IsTop == true))
            {
                vcl.Control.BringToFront();
                if (vcl.Control.ShowTitle == true)
                {
                    vcl.Control.ShowTitle = false;
                    vcl.Control.VideoPanelMargin = 0;
                }
            }

            if (this.m_CurControl != null)
            {
                if (this.m_CurControl.BackColor != this.m_CurControl.BackgroundColor)
                    this.m_CurControl.BackColor = this.m_CurControl.BackgroundColor;
            }
        }

        internal void Uninit()
        {
            //throw new NotImplementedException();
        }
    }

    public delegate void ControlChangedEvent(object sender, ControlChangedEventArgs e);

    public class ControlChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Sn { get; set; }

        /// <summary>
        /// 控件
        /// </summary>
        public VideoControl VControl { get; set; }
    }
}
