using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using ClientAPP.Core.DataModel.Video;

namespace ClientAPP.VideoModule
{
    public partial class VideoControl : UserControl
    {

        #region 内部变量

        private bool m_Seleted;


        private bool m_ShowTitle;

        private bool m_ShowPlaybackBar;

        private ShowMode m_PlayMode;

        private CameraInfo m_CameraInfo;

        /// <summary>
        /// 正在拖动进度条，此时不能更新trackbar进度
        /// </summary>
        private bool m_TrackbarDraging;

        /// <summary>
        /// 回放状态
        /// </summary>
        private PB_Status m_Pb_Status;

        /// <summary>
        /// 码流
        /// </summary>
        private double m_DataRate = -1;

        /// <summary>
        /// 设置不显示标题栏时，是否在选中时显示，不选中时不显示。
        /// </summary>
        private bool m_AutoHideTitle = false;

        /// <summary>
        /// 是否锁定图像
        /// </summary>
        private bool m_Locked = false;

        /// <summary>
        /// 是否打开声音
        /// </summary>
        private bool m_OpenSound = false;

 
        /// <summary>
        /// 回放时间段
        /// </summary>
        private PlaybackInfo m_PbArgs;

        /// <summary>
        /// 上一个点位
        /// </summary>
        private CameraInfo m_LastCamera;

        private List<PictureBox> m_ControlButtonList = new List<PictureBox>();

        #region 画框相关
        /// <summary>
        /// 是否在回调函数中画矩形
        /// </summary>
        private bool m_DrawRectangle = true;
        /// <summary>
        /// 要画的矩形
        /// </summary>
        private Rectangle m_RectDraw = new Rectangle(0, 0, 0, 0);
        /// <summary>
        /// 鼠标按下时的原点时候的点
        /// </summary>
        private Point m_Orign = new Point(0, 0);
        /// <summary>
        /// WindowsApiDrawing
        /// </summary>
        //private WindowsApiDrawing m_WindowsApiDrawing;

        #endregion

        #endregion


        #region 事件
   
        /// <summary>
        /// 通用事件
        /// </summary>
        public event VideoControlEventHandle ActionEvent;

        /// <summary>
        /// 控制事件
        /// </summary>
        public event PTZEventHandle PtzEvent;

        #endregion

        #region 属性

        /// <summary>
        /// 所属面板id
        /// </summary>
        public string VideoPanelID { get; set; }
        /// <summary>
        /// 是否最大化
        /// </summary>
        public bool IsMax { get; set; }

        /// <summary>
        /// 是否显示标题栏
        /// </summary>
        public bool ShowTitle
        {
            get
            {
                return this.m_ShowTitle;
            }
            set
            {
                this.flowLayoutPanel_Title.Height = value == true ? 15 : 0;
                this.SetVCSize();
                this.Refresh();
                this.m_ShowTitle = value;
            }
        }

        /// <summary>
        /// 是否显示ptz按钮
        /// </summary>
        public bool ShowPtzButton { get; set; }

        /// <summary>
        /// 背景框颜色
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// 默认背景色
        /// </summary>
        public Color VCDefaultBackColor { get; set; }

        /// <summary>
        /// 视频窗口的边框大小
        /// </summary>
        public int VideoPanelMargin { get; set; }

        /// <summary>
        /// 是否显示回放进度条
        /// </summary>
        public bool ShowPlaybackBar
        {
            get
            {
                return this.m_ShowPlaybackBar;
            }
            set
            {
                this.m_ShowPlaybackBar = value;
                this.panel_PlaybackBar.Height = value == true ? 20 : 0;
                this.SetVCSize();
                this.Refresh();
            }
        }

        /// <summary>
        /// 是否能控制
        /// </summary>
        public bool CanControl { get; set; }


        /// <summary>
        /// 当前播放的摄像机
        /// </summary>
        public CameraInfo CurrentCamera
        {
            get
            {
                return this.m_CameraInfo;

            }
            set
            {
                this.m_CameraInfo = value;
                //if (value == null)
                //    this.VControl.Refresh();
                this.flowLayoutPanel_Title.Refresh();
            }

        }

        /// <summary>
        /// 模式（实时，回放等）
        /// </summary>
        public ShowMode Mode
        {
            get { return this.m_PlayMode; }
            set
            {
                this.m_PlayMode = value;
                switch (value)
                {
                    case ShowMode.Real:                        
                        this.ShowPlaybackBar = false;
                        //this.pictureBox_FastPlayback.Visible = true;
                        break;
                    case ShowMode.Playback:
                    case ShowMode.PlayFile:
                        this.ShowPlaybackBar = true;
                        //this.pictureBox_FastPlayback.Visible = false;
                        break;
                    default:
                        this.ShowPlaybackBar = false;
                        //this.pictureBox_FastPlayback.Visible = true;
                        break;
                }
            }
        }
        /// <summary>
        /// 视频控件
        /// </summary>
        public Control VControl
        {
            get
            {
                return this.panel_Video;
            }
        }

        /// <summary>
        /// 码率
        /// </summary>
        public double DataRate
        {
            get
            {
                return this.m_DataRate;
            }
            set
            {
                this.m_DataRate = value;
                if (m_DataRate >= 0)
                {
                    this.flowLayoutPanel_Title.Invoke(new Action(() =>
                        {
                            this.flowLayoutPanel_Title.Refresh();
                        }));
                }
            }
        }


        /// <summary>
        /// 显示时间选择
        /// </summary>
        public bool ShowTimePicker { get; set; }

        /// <summary>
        /// 标题栏菜单
        /// </summary>
        public ContextMenuStrip TitleContextMenu
        {
            get { return this.flowLayoutPanel_Title.ContextMenuStrip; }
            set { this.flowLayoutPanel_Title.ContextMenuStrip = value; }
        }

        //显示错误信息
        public string ErrorMessage
        {
            get { return this.label_Error.Text; }
            set
            {
                this.label_Error.Invoke(new Action(() =>
                    {
                        this.label_Error.Text = value;
                        if (string.IsNullOrEmpty(value) == true)
                        {
                            this.label_Error.Dock = DockStyle.None;
                            this.label_Error.Size = new Size(0, 0);
                        }
                        else
                        {
                            this.label_Error.Dock = DockStyle.Fill;
                        }
                    }
                    ));
            }
        }

        /// <summary>
        /// 视频面板菜单
        /// </summary>
        public ContextMenuStrip VCContextMenu
        {
            get { return this.panel_Video.ContextMenuStrip; }
            set { this.panel_Video.ContextMenuStrip = value; }
        }

        /// <summary>
        /// 是否锁定图像
        /// </summary>
        public bool Locked
        {
            get
            {

                return this.m_Locked;
            }
            set
            {
                this.m_Locked = value;
                if (value == true)
                {
                    this.pictureBox_Lock.Image = ImgResource.锁住k;
                }
                else
                {
                    this.pictureBox_Lock.Image = ImgResource.打开k;
                }
            }
        }

        /// <summary>
        /// 是否打开声音
        /// </summary>
        public bool IsOpenSound
        {
            get { return this.m_OpenSound; }
            set
            {
                this.m_OpenSound = value;
                if (value == true)
                {
                    this.pictureBox_Sound.Image = ImgResource.打开声音k;
                }
                else
                {
                    this.pictureBox_Sound.Image = ImgResource.关闭声音k;
                }
            }
        }
     

        /// <summary>
        /// 是否可以被选中(会有红色边框)
        /// </summary>
        public bool CanSelected { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool Selected
        {
            get
            {
                return this.m_Seleted;
            }
            set
            {
                if (this.m_Seleted == value)
                    return;

                this.m_Seleted = value;
                if (this.m_Seleted == true)
                {
                    VideoControlActionEventArgs voeSelectedArgs = new VideoControlActionEventArgs() { Camera = this.CurrentCamera, VControl = this, Type = VideoControlAction.Seleced };
                    
                    this.ActionEvent?.Invoke(this, voeSelectedArgs);


                    if (this.CanSelected)
                        this.BackColor = this.BackgroundColor;
                }
                else
                    this.BackColor = this.VCDefaultBackColor;

                if (this.ShowTitle == value)//控制自动隐藏标题栏
                {
                    if (this.m_AutoHideTitle == true)
                    {
                        this.ShowTitle = value;
                    }
                }
            }
        }

        /// <summary>
        /// 是否自动隐藏标题栏
        /// </summary>
        public bool AutoHideTitle
        {
            get { return this.m_AutoHideTitle; }
            set { this.m_AutoHideTitle = value; }
        }

        /// <summary>
        /// 回放状态
        /// </summary>
        public PB_Status PBStatus
        {
            get
            {
                return this.m_Pb_Status;
            }
            set
            {
                this.m_Pb_Status = value;
                switch (value)
                {
                    case VideoControl.PB_Status.BeforeStart:
                    case PB_Status.Pause:
                    case PB_Status.Step:
                        this.pictureBox_Play.Image = ImgResource.播放k;
                        break;
                    case PB_Status.Play:
                        this.pictureBox_Play.Image = ImgResource.暂停k;
                        break;
                }
            }
        }



        /// <summary>
        /// 回放信息
        /// </summary>
        public PlaybackInfo PBInfo
        {
            get
            {
                return this.m_PbArgs;
            }
            set
            {
                this.m_PbArgs = value;
                if (value != null)
                {
                    this.dateTimePicker_Start.Value = value.Start;
                    this.dateTimePicker_End.Value = value.End;
                }
            }
        }

        private bool m_ShowDownloadButton;
        /// <summary>
        /// 是否显示下载按钮
        /// </summary>
        public bool ShowDownLoadButton
        {
            get
            {
                return this.m_ShowDownloadButton;
            }
            set
            {
                this.m_ShowDownloadButton = value;
                this.pictureBox_Download.Visible = value;
                this.pictureBox_StopDownload.Visible = value;
            }
        }

        private bool m_ShowFastPlaybackButton;
        /// <summary>
        /// 是否显示快速回放按钮
        /// </summary>
        public bool ShowFastPlaybackButton
        {
            get { return this.m_ShowFastPlaybackButton; }
            set
            {
                this.m_ShowFastPlaybackButton = value;
                this.pictureBox_FastPlayback.Visible = value;
            }
        }

        /// <summary>
        /// true用于服务模式在屏幕拖动，false用于在客户端中交换图像
        /// </summary>
        public bool CanDragOnScreen { get; set; }

      

        /// <summary>
        /// 是否电子放大状态
        /// </summary>
        public bool DigitalZoom { get; set; }

        #endregion


        internal VideoControl()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;

            //this.ShowTimePicker = true;//调试

            this.IsMax = false;
            this.m_ShowPlaybackBar = false;
            this.BackgroundColor = Color.Red;
            this.VCDefaultBackColor = Color.White;
            this.VideoPanelMargin = 1;

            this.panel_Video.DragEnter += new DragEventHandler(panel_Video_DragEnter);
            this.panel_Video.DragDrop += new DragEventHandler(panel_Video_DragDrop);

            //设置悬浮图片
            this.SetPicboxProp();
            this.PBStatus = PB_Status.BeforeStart;
            this.PBInfo = new PlaybackInfo() { Start = DateTime.Now.AddHours(-1), End = DateTime.Now };
            this.label_Error.Text = "";

            foreach (Control c in this.panel_Video.Controls)
            {
                c.MouseMove += new MouseEventHandler(this.panel_Video_MouseMove);
                c.DragEnter+=new DragEventHandler(this.panel_Video_DragEnter);
                c.Click+=new EventHandler(this.panel_Video_Click);
                c.DragDrop+=new DragEventHandler(this.panel_Video_DragDrop);
                c.MouseDoubleClick += new MouseEventHandler(this.panel_Video_MouseDoubleClick);
                c.MouseDown += new MouseEventHandler(this.panel_Video_MouseDown);
                c.MouseUp += new MouseEventHandler(this.panel_Video_MouseUp);
                c.MouseWheel += new MouseEventHandler(this.panel_Video_MouseWheel);
                c.PreviewKeyDown += new PreviewKeyDownEventHandler(this.panel_Video_PreviewKeyDown);
            }

            this.m_ControlButtonList = new List<PictureBox>() { this.pictureBoxLeft, this.pictureBoxLeftDown, this.pictureBoxLeftUp,
            this.pictureBoxUp, this.pictureBoxDown, this.pictureBoxRight,this.pictureBoxRightDown,this.pictureBoxRightUp};
        }

        void panel_Video_DragDrop(object sender, DragEventArgs e)
        {

            int cameraID = 0;
            string data = (string)e.Data.GetData(DataFormats.Text);
            bool ret = Int32.TryParse(data, out cameraID);
            if (ret == true)//直接传输了摄像机ID过来
            {
                
                //this.OpenStream(this, new OpenStreamArgs() { CameraID = cameraID, ChangeControlIndex = -1 });
            }
            else
            {
                if (data.IndexOf("Transposition") >= 0)
                {
                    VideoControlActionEventArgs vae = new VideoControlActionEventArgs() { Arg = data.Split(' ')[1], VControl = this, Type = VideoControlAction.Transposition };
                    if (this.ActionEvent != null)
                        this.ActionEvent(this, vae);
                }
            }
        }

        void panel_Video_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;

        }


        /// <summary>
        /// 设置进度条的进度值(0-100)
        /// </summary>
        /// <param name="pos"></param>
        public void SetTarckbarPos(int pos)
        {
            if (pos < 0 || pos > 100 || this.m_TrackbarDraging == true)
                return;

            //if (System.Math.Abs(pos - this.trackBarControl_Pb.Value) > 10)//防止移动过快
            //    return;

            this.Invoke(new Action(()=> this.trackBarControl_Pb.Value = pos));
        }

        /// <summary>
        /// 播放速度
        /// </summary>
        public string m_PlaySpeed = "";

        /// <summary>
        /// 设置播放速度
        /// </summary>
        /// <param name="pos"></param>
        public void SetPlaySpeed(string playSpeed)
        {
            this.m_PlaySpeed = playSpeed;
            
            
            this.Invoke(new Action(()=> this.flowLayoutPanel_Title.Refresh()));
        }

        /// <summary>
        /// 重置进度条
        /// </summary>
        public void ResetTrackBar()
        {
            this.trackBarControl_Pb.Value = 0;
        }

        #region 鼠标事件
        private void panel_Video_Click(object sender, EventArgs e)
        {

            //this.Selected = true;

        }

        private void panel_Video_MouseDown(object sender, MouseEventArgs e)
        {

            this.Selected = true;


            if (this.CanControl == true)
            {
                #region 画框
                if (e.Button == MouseButtons.Left && e.Clicks == 1)
                {
                    this.m_RectDraw = new Rectangle(e.X, e.Y, 0, 0);
                    this.m_DrawRectangle = true;
                    this.m_Orign = new Point(e.X, e.Y);
                }
                #endregion

                Panel pPanel = this.panel_Video;
                pPanel.BorderStyle = BorderStyle.None;
                pPanel.Focus();
                //如果不是当前选中的，则先选中
                if (this.Selected == false)
                {
                    this.Selected = true;
                }

                if (this.CurrentCamera == null)//未播放
                {
                    return;
                }


                //有右键菜单时，不使用鼠标控制
                if (this.ContextMenuStrip != null)
                    return;

                if (e.Button == MouseButtons.Left)
                {
                    return;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PTZ.DirDirection pDirDirection = this.GetDirByXY(pPanel, e.X, e.Y);
                    int pVSpeed = this.GetVspeedByY(pPanel, e.Y);
                    int pHSpeed = this.GetHSpeedByX(pPanel, e.X);
                    try
                    {
                        this.PtzEvent?.Invoke(this, new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = pDirDirection, hSpeed = pHSpeed, vSpeed = pVSpeed, VideoControl = this, Type = PTZControlArgs.PtzType.Direction });
                      }
                    catch { }
                    //this.ControlHandle.DirCamera(this.CurrentCamera.ID, pDirDirection, pHSpeed, pVSpeed);
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    try
                    {
                        this.PtzEvent?.Invoke(this, new PTZControlArgs() { cameraID = this.CurrentCamera.ID, lenType = PTZ.LenType.LensWide, VideoControl = this ,Type= PTZControlArgs.PtzType.Len});
                    }
                    catch { }
                    //this.ControlHandle.LenCamera(this.CurrentCamera.ID, LenType.LensWide);
                }
            }
            else
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (this.ActionEvent != null)
                    {
                        this.ActionEvent(this, new VideoControlActionEventArgs() { Type = VideoControlAction.ErrorReport, Camera = this.CurrentCamera, VControl = this, Arg = "crtl" });
                    }
                }
            }


        }

        private void panel_Video_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.CanControl == true)
            {


                #region 画框
                if (e.Button == MouseButtons.Left && e.Clicks == 1)
                {
                    if (this.m_DrawRectangle)
                    {
                        this.m_DrawRectangle = false;
                        if (this.CurrentCamera != null)
                        {
                            Task.Factory.StartNew(new Action<object>(this.CameraAutoFindDirection), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, VideoControl=this });
                        }
                    }
                }
                #endregion


                Panel pPanel = this.panel_Video;
                if (this.CurrentCamera == null)//未播放
                {
                    return;
                }


                //有右键菜单时，不使用鼠标控制
                if (this.ContextMenuStrip != null)
                    return;

                string pCamID = this.CurrentCamera.ID;
                if (e.Button == MouseButtons.Left)
                {
                    return;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    try
                    {
                        this.PtzEvent?.Invoke(this, new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection =  PTZ.DirDirection.Stop,  VideoControl = this, Type = PTZControlArgs.PtzType.Direction });

                       // Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = pCamID, dirDirection = PTZ.DirDirection.Stop, hSpeed = 255, vSpeed = 255, Control = this.VControl, VideoControl = this });
                    }
                    catch { }
                    //this.ControlHandle.DirCamera(pCamID, DirDirection.Stop, 255, 255);
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    try
                    {
                        this.PtzEvent?.Invoke(this, new PTZControlArgs() { cameraID = this.CurrentCamera.ID, lenType = PTZ.LenType.Stop, VideoControl = this, Type = PTZControlArgs.PtzType.Len });

                        //Task.Factory.StartNew(new Action<object>(this.LenCamera), new ControlArgs() { cameraID = pCamID, lenType = PTZ.LenType.Stop, Control = this.VControl, VideoControl = this });
                    }
                    catch { }
                    //this.ControlHandle.LenCamera(pCamID, LenType.Stop);
                }
            }
        }



        private void panel_Video_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (this.m_DrawRectangle)
                {

                    //重新计算要画的Rectangle
                    int left = (int)Math.Min(m_Orign.X, e.X);
                    int top = (int)Math.Min(m_Orign.Y, e.Y);
                    int width = (int)Math.Abs(m_Orign.X - e.X);
                    int height = (int)Math.Abs(m_Orign.Y - e.Y);
                    this.m_RectDraw = new Rectangle(left, top, width, height);

                }
            }
        }

        private void panel_Video_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
            {
                return;
            }

            this.IsMax ^= true;
            VideoControlAction type = VideoControlAction.FullScreen;
            if (this.IsMax == false)
                type = VideoControlAction.NormalScreen;
            if (this.ActionEvent != null)
                this.ActionEvent(this, new VideoControlActionEventArgs() { Camera = this.CurrentCamera, Type = type, VControl = this });

        }

        void panel_Video_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.CanControl == true)
            {
                //有右键菜单时，不使用鼠标控制
                if (this.ContextMenuStrip != null)
                    return;

                if (this.CurrentCamera == null)//未播放
                {
                    return;
                }
                PTZ.LenType lenType = PTZ.LenType.LensWide;

                if (e.Delta < 0)
                {
                    lenType = PTZ.LenType.LensWide;
                }
                else
                {
                    lenType = PTZ.LenType.LensTele;
                }

                try
                {
                    Task.Factory.StartNew(new Action<object>(this.LenCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, lenType = lenType,  VideoControl = this });
                    string stopTag = Guid.NewGuid().ToString();
                    this.m_TeleStopTag = stopTag;
                    Task.Factory.StartNew(new Action<object>(this.LenCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, lenType = PTZ.LenType.Stop, Delay = 800, GuidString = stopTag,  VideoControl = this });

                }
                catch { }


            }
        }

        private void trackBarControl_Pb_MouseDown(object sender, MouseEventArgs e)
        {
            this.m_TrackbarDraging = true;
            this.linkLabel_CurTime.Visible = true;
        }

        private void trackBarControl_Pb_MouseUp(object sender, MouseEventArgs e)
        {
            VideoControlActionEventArgs voe = new VideoControlActionEventArgs() { Camera = this.CurrentCamera, VControl = this };
            voe.Arg = this.trackBarControl_Pb.Value;
            voe.Type = VideoControlAction.PB_SetPos;

            this.ActionEvent(this, voe);
            this.m_TrackbarDraging = false;
            this.linkLabel_CurTime.Visible = false;
        }

        private void trackBarControl_Pb_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.m_TrackbarDraging == false)
                return;
            double totalSeconds = (this.PBInfo.End - this.PBInfo.Start).TotalSeconds;
            if (totalSeconds < 0)
                return;
            this.linkLabel_CurTime.Text = this.PBInfo.Start.AddSeconds(totalSeconds * this.trackBarControl_Pb.Value / 100).ToLongTimeString();
            
        }

        #endregion

        #region 计算控制方向及速度
        //根据鼠标点击VideoControl的位置确定控制移动方向
        private PTZ.DirDirection GetDirByXY(Panel panel, int x, int y)
        {
            float pWidth = (float)panel.Width;
            float pHeight = (float)panel.Height;
            PTZ.DirDirection pDirDirection;
            if (x < pWidth * 40 / 100 && y < pHeight * 40 / 100)
            {
                pDirDirection = PTZ.DirDirection.LeftUp;
            }
            else if (x < pWidth * 40 / 100 && y > pHeight * 60 / 100)
            {
                pDirDirection = PTZ.DirDirection.LeftDown;
            }
            else if (x > pWidth * 60 / 100 && y < pHeight * 40 / 100)
            {
                pDirDirection = PTZ.DirDirection.RightUp;
            }
            else if (x > pWidth * 60 / 100 && y > pHeight * 60 / 100)
            {
                pDirDirection = PTZ.DirDirection.RightDown;
            }
            else if (x >= pWidth * 40 / 100 && x <= pWidth * 60 / 100 && y < pHeight * 50 / 100 && Math.Abs(pWidth / 2 - x) / pWidth < Math.Abs(pHeight / 2 - y) / pHeight)
            {
                pDirDirection = PTZ.DirDirection.Up;
            }
            else if (x >= pWidth * 40 / 100 && x <= pWidth * 60 / 100 && y > pHeight * 50 / 100 && Math.Abs(pWidth / 2 - x) / pWidth < Math.Abs(pHeight / 2 - y) / pHeight)
            {
                pDirDirection = PTZ.DirDirection.Down;
            }
            else if (y >= pHeight * 40 / 100 && y <= pHeight * 60 / 100 && x < pWidth * 50 / 100 && Math.Abs(pWidth / 2 - x) / pWidth > Math.Abs(pHeight / 2 - y) / pHeight)
            {
                pDirDirection = PTZ.DirDirection.Left;
            }
            else if (y >= pHeight * 40 / 100 && y <= pHeight * 60 / 100 && x > pWidth * 50 / 100 && Math.Abs(pWidth / 2 - x) / pWidth > Math.Abs(pHeight / 2 - y) / pHeight)
            {
                pDirDirection = PTZ.DirDirection.Right;
            }
            else
            {
                pDirDirection = PTZ.DirDirection.Stop;
            }
            return pDirDirection;
        }

        //根据鼠标点击VideoControl的位置确定水平移动速度
        private int GetHSpeedByX(Panel panel, int x)
        {
            float pWidth = panel.Width;
            int pHSpeed = (int)(Math.Abs(pWidth / 2 - x) / pWidth * 255 * 2);
            return pHSpeed;
        }

        //根据鼠标点击VideoControl的位置确定垂直移动速度
        private int GetVspeedByY(Panel panel, int y)
        {
            float pHeight = panel.Height;
            int pVSpeed = (int)(Math.Abs(pHeight / 2 - y) / pHeight * 255 * 2);
            return pVSpeed;
        }

        #endregion

        #region 控制方法

        private void DirCamera(object argObj)
        {
            try
            {

                PTZControlArgs arg = argObj as PTZControlArgs;

                this.PtzEvent?.Invoke(this, arg);

            }
            catch { }
        }

        private void LenCamera(object argObj)
        {
            try
            {
                PTZControlArgs arg = argObj as PTZControlArgs;
                if (arg.Delay != 0)
                {
                    System.Threading.Thread.Sleep(arg.Delay);
                    if (arg.GuidString != this.m_TeleStopTag)
                        return;
                }
                this.PtzEvent?.Invoke(this, arg);
               
            }
            catch { }

        }

        private string m_TeleStopTag;

        private void CameraAutoFindDirection(object argObj)
        {
            try
            {

                PTZControlArgs arg = argObj as PTZControlArgs;

                this.PtzEvent?.Invoke(this, arg);

            }
            catch { }
        }

        #endregion



        private void VideoControl_Resize(object sender, EventArgs e)
        {
            this.SetVCSize();
        }

        private void SetVCSize()
        {
           

            this.panel_Video.Location = new Point(this.VideoPanelMargin, this.flowLayoutPanel_Title.Height);


            if (this.m_ShowPlaybackBar == true)
            {
                if (this.ShowTimePicker == true)
                {
                    this.panel_PlaybackBar.Height = 44;
                }
                else
                {
                    this.panel_PlaybackBar.Height = 20;
                }
            }
            else
                this.panel_PlaybackBar.Height = 0;

            if (this.m_ShowPlaybackBar == false)
                this.panel_Video.Size = new Size(this.Width - 2 * this.VideoPanelMargin, this.Height - this.VideoPanelMargin - this.flowLayoutPanel_Title.Height);
            else
                this.panel_Video.Size = new Size(this.Width - 2 * this.VideoPanelMargin, this.Height - this.VideoPanelMargin - this.flowLayoutPanel_Title.Height - this.panel_PlaybackBar.Height);



            this.trackBarControl_Pb.Width = this.panel_PlaybackBar.Width;
            this.trackBarControl_Pb.Location = new Point(this.pictureBox_StopDownload.Location.X + this.pictureBox_StopDownload.Width + 1, 1);
            //this.trackBarControl_Pb.Location = new Point(this.linkLabel_CurTime.Location.X + this.linkLabel_CurTime.Width + 1, 1);
            this.trackBarControl_Pb.Height = 20;
            this.trackBarControl_Pb.Width = this.panel_PlaybackBar.Width - this.trackBarControl_Pb.Location.X;

            this.linkLabel_CurTime.Location = new Point(this.trackBarControl_Pb.Location.X + this.trackBarControl_Pb.Width / 2-30, this.trackBarControl_Pb.Location.Y);
            this.linkLabel_CurTime.BringToFront();
       
        }

        /// <summary>
        /// 写入标题文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (this.m_PlayMode == ShowMode.Real)
                {
                    if (this.CurrentCamera != null)
                    {
                        SolidBrush brush = new SolidBrush(Color.FromArgb(40, 91, 135));
                        string title = this.CurrentCamera.Name;
                        if (this.DataRate >= 0)
                            title += string.Format(" ({0}k)", this.m_DataRate / 125);
                        e.Graphics.DrawString(title, this.Font, brush, new Point(1, 1));
                    }
                    else
                    {
                        SolidBrush brush = new SolidBrush(Color.FromArgb(40, 91, 135));
                        e.Graphics.DrawString("未打开图像", this.Font, brush, new Point(1, 1));
                    }
                }
                else if (this.m_PlayMode == ShowMode.Playback)
                {
                    if (this.CurrentCamera != null)
                    {
                        if (string.IsNullOrEmpty(this.m_PlaySpeed) == true)
                            return;
                        SolidBrush brush = new SolidBrush(Color.FromArgb(40, 91, 135));
                        string title = this.CurrentCamera.Name;                        
                        title += string.Format(" (回放速度：{0} 倍)", this.m_PlaySpeed);
                        e.Graphics.DrawString(title, this.Font, brush, new Point(1, 1));
                    }
                }


            }
            catch { }
        }


        #region 按钮效果



        /// <summary>
        /// 临时
        /// </summary>
        private Image m_tempImage;
        private void pictureBox_Play_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb.Image == null)
                return;
            //pb.Image = this.RePic((Bitmap)pb.Image, pb.Image.Width, pb.Image.Height);
            Image temp = null;
            if (this.m_tempImage != null)
                temp = (Image)this.m_tempImage.Clone();
            this.m_tempImage = (Image)pb.Image.Clone();
            //pb.Image = (Image)temp.Clone();

        }

        private void SetMoveInPic(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb.Tag == null)
                return;
            switch (pb.Tag.ToString())
            {
                case "播放":
                    {
                        switch (this.m_Pb_Status)
                        {
                            case PB_Status.BeforeStart:
                            case PB_Status.Pause:
                            case PB_Status.Step:
                                pb.Image = ImgResource.播放b;
                                break;
                            case PB_Status.Play:
                                pb.Image = ImgResource.暂停b;
                                break;
                        }
                        break;
                    }
                case "暂停":
                    pb.Image = ImgResource.暂停b;
                    break;
                case "停止":
                    pb.Image = ImgResource.停止b;
                    break;
                case "快进":
                    pb.Image = ImgResource.快b;
                    break;
                case "慢进":
                    pb.Image = ImgResource.慢b;
                    break;
                case "单帧":
                    pb.Image = ImgResource.单帧b;
                    break;
                case "关闭":
                    pb.Image = ImgResource.关闭b;
                    break;
                case "放大/恢复":
                    pb.Image = ImgResource.缩放b;
                    break;
                case "抓图":
                    pb.Image = ImgResource.抓图b;
                    break;
                case "下载":
                    pb.Image = ImgResource.下载b;
                    break;
                case "停止下载":
                    pb.Image = ImgResource.停止下载b;
                    break;
                case "隐藏/显示":
                    pb.Image = ImgResource.隐藏显示b;
                    break;
                case "左":
                    pb.Image = ImgResource.左e;
                    break;
                case "右":
                    pb.Image = ImgResource.右e;
                    break;
                case "上":
                    pb.Image = ImgResource.上e;
                    break;
                case "下":
                    pb.Image = ImgResource.下e;
                    break;
                case "左上":
                    pb.Image = ImgResource.左上e;
                    break;
                case "右上":
                    pb.Image = ImgResource.右上e;
                    break;
                case "左下":
                    pb.Image = ImgResource.左下e;
                    break;
                case "右下":
                    pb.Image = ImgResource.右下e;
                    break;
                case "锁定":
                    if (this.m_Locked == false)
                    {
                        pb.Image = ImgResource.打开b;
                    }
                    else
                    {
                        pb.Image = ImgResource.锁住b;
                    }
                    break;
                case "声音":
                    if (this.m_OpenSound == false)
                    {
                        this.pictureBox_Sound.Image = ImgResource.关闭声音b;
                    }
                    else
                    {
                        this.pictureBox_Sound.Image = ImgResource.打开声音b;
                    }
                    break;
            }

        }

        private void SetMoveOutPic(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb.Tag == null)
                return;
            switch (pb.Tag.ToString())
            {
                case "播放":
                    {
                        switch (this.m_Pb_Status)
                        {
                            case PB_Status.BeforeStart:
                            case PB_Status.Pause:
                            case PB_Status.Step:
                                pb.Image = ImgResource.播放k;
                                break;
                            case PB_Status.Play:
                                pb.Image = ImgResource.暂停k;
                                break;
                        }
                        break;
                    }
                case "暂停":
                    pb.Image = ImgResource.暂停k;
                    break;
                case "停止":
                    pb.Image = ImgResource.停止k;
                    break;
                case "快进":
                    pb.Image = ImgResource.快k;
                    break;
                case "慢进":
                    pb.Image = ImgResource.慢k;
                    break;
                case "单帧":
                    pb.Image = ImgResource.单帧k;
                    break;
                case "关闭":
                    pb.Image = ImgResource.关闭k;
                    break;
                case "放大/恢复":
                    pb.Image = ImgResource.缩放k;
                    break;
                case "抓图":
                    pb.Image = ImgResource.抓图k;
                    break;
                case "下载":
                    pb.Image = ImgResource.下载k;
                    break;
                case "停止下载":
                    pb.Image = ImgResource.停止下载k;
                    break;
                case "隐藏/显示":
                    pb.Image = ImgResource.隐藏显示k;
                    break;
                case "左":
                    pb.Image = ImgResource.左;
                    break;
                case "右":
                    pb.Image = ImgResource.右;
                    break;
                case "上":
                    pb.Image = ImgResource.上;
                    break;
                case "下":
                    pb.Image = ImgResource.下;
                    break;
                case "左上":
                    pb.Image = ImgResource.左上;
                    break;
                case "右上":
                    pb.Image = ImgResource.右上;
                    break;
                case "左下":
                    pb.Image = ImgResource.左下;
                    break;
                case "右下":
                    pb.Image = ImgResource.右下;
                    break;
                case "锁定":
                    if (this.m_Locked == false)
                    {
                        pb.Image = ImgResource.打开k;
                    }
                    else
                    {
                        pb.Image = ImgResource.锁住k;
                    }
                    break;
                case "声音":
                    if (this.m_OpenSound == false)
                    {
                        this.pictureBox_Sound.Image = ImgResource.关闭声音k;
                    }
                    else
                    {
                        this.pictureBox_Sound.Image = ImgResource.打开声音k;
                    }
                    break;
            }
        }

        private void SetPicboxProp()
        {
            this.pictureBox_Play.Tag = "播放";
            this.pictureBox_Pause.Tag = "暂停";
            this.pictureBox_Stop.Tag = "停止";
            this.pictureBox_Fast.Tag = "快进";
            this.pictureBox_Slow.Tag = "慢进";
            this.pictureBox_Step.Tag = "单帧";

            this.pictureBox_Close.Tag = "关闭";
            this.pictureBox_Zoom.Tag = "放大/恢复";
            this.pictureBox_Snap.Tag = "抓图";
            this.pictureBox_Download.Tag = "下载";
            this.pictureBox_StopDownload.Tag = "停止下载";
            this.pictureBoxControl.Tag = "隐藏/显示";
            this.pictureBoxLeft.Tag = "左";
            this.pictureBoxRight.Tag = "右";
            this.pictureBoxUp.Tag = "上";
            this.pictureBoxDown.Tag = "下";
            this.pictureBoxLeftUp.Tag = "左上";
            this.pictureBoxRightUp.Tag = "右上";
            this.pictureBoxLeftDown.Tag = "左下";
            this.pictureBoxRightDown.Tag = "右下";
            this.pictureBox_Lock.Tag = "锁定";
            this.pictureBox_Sound.Tag = "声音";

            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;

            toolTip.SetToolTip(this.pictureBox_Play, "播放");
            toolTip.SetToolTip(this.pictureBox_Pause, "暂停");
            toolTip.SetToolTip(this.pictureBox_Stop, "停止");
            toolTip.SetToolTip(this.pictureBox_Fast, "快进");
            toolTip.SetToolTip(this.pictureBox_Slow, "慢进");
            toolTip.SetToolTip(this.pictureBox_Step, "单帧");

            toolTip.SetToolTip(this.pictureBox_Close, "关闭");
            toolTip.SetToolTip(this.pictureBox_Zoom, "放大/恢复");
            toolTip.SetToolTip(this.pictureBox_Snap, "抓图");
            toolTip.SetToolTip(this.pictureBox_Download, "下载");
            toolTip.SetToolTip(this.pictureBox_StopDownload, "停止下载");
            toolTip.SetToolTip(this.pictureBoxControl, "隐藏/显示");

            toolTip.SetToolTip(this.pictureBoxLeft, "左");
            toolTip.SetToolTip(this.pictureBoxRight, "右");
            toolTip.SetToolTip(this.pictureBoxUp, "上");
            toolTip.SetToolTip(this.pictureBoxDown, "下");
            toolTip.SetToolTip(this.pictureBoxLeftUp, "左上");
            toolTip.SetToolTip(this.pictureBoxRightUp, "右上");
            toolTip.SetToolTip(this.pictureBoxLeftDown, "左下");
            toolTip.SetToolTip(this.pictureBoxRightDown, "右下");
            toolTip.SetToolTip(this.pictureBox_Lock, "锁定");
            toolTip.SetToolTip(this.pictureBox_Sound, "声音");
        }



        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.ActionEvent == null)
                return;
            
            PictureBox pb = sender as PictureBox;

            VideoControlActionEventArgs voe = new VideoControlActionEventArgs() { Camera = this.CurrentCamera, VControl = this };

            if (pb == this.pictureBox_Close)
            {
                voe.Type = VideoControlAction.CloseVideo;
                this.Locked = false;
            }
            else if (pb == this.pictureBox_Zoom)
            {
                this.IsMax ^= true;
                if (this.IsMax == true)
                    voe.Type = VideoControlAction.FullScreen;
                else
                    voe.Type = VideoControlAction.NormalScreen;
            }
            else if (pb == this.pictureBox_Lock)
            {
                if (this.m_Locked == true)
                {
                    this.m_Locked = false;
                    voe.Type = VideoControlAction.UnLocked;
                }
                else
                {
                    this.m_Locked = true;
                    voe.Type = VideoControlAction.Locked;
                }
            }
            else if (pb == this.pictureBox_Play)
            {
                switch (this.PBStatus)
                {
                    case PB_Status.Pause:
                    case PB_Status.Step:
                        voe.Type = VideoControlAction.PB_Resume;
                        break;
                    case PB_Status.Play:
                        voe.Type = VideoControlAction.PB_Pause;
                        break;
                    case PB_Status.BeforeStart:
                        voe.Type = VideoControlAction.PB_Play;
                        this.m_PbArgs.Start = this.dateTimePicker_Start.Value;
                        this.m_PbArgs.End = this.dateTimePicker_End.Value;
                        voe.Arg = this.PBInfo;
                        if (voe.Camera == null)//回放已经关闭的情况下，播放之前的点位
                            voe.Camera = this.m_LastCamera;
                        break;
                }
            }
            else if (pb == this.pictureBox_Pause)
                voe.Type = VideoControlAction.PB_Pause;
            else if (pb == this.pictureBox_Stop)
            {
                this.PBStatus = PB_Status.BeforeStart;
                voe.Type = VideoControlAction.PB_Stop;
                this.m_LastCamera = this.CurrentCamera;
            }
            else if (pb == this.pictureBox_Fast)
                voe.Type = VideoControlAction.PB_Fast;
            else if (pb == this.pictureBox_Slow)
                voe.Type = VideoControlAction.PB_Slow;
            else if (pb == this.pictureBox_Step)
                voe.Type = VideoControlAction.PB_Step;
            else if (pb == this.pictureBox_Snap)
                voe.Type = VideoControlAction.Snap;
            else if (pb == this.pictureBox_Download)
                voe.Type = VideoControlAction.PB_StratDownload;
            else if (pb == this.pictureBox_StopDownload)
                voe.Type = VideoControlAction.PB_StopDownload;
            else if (pb == this.pictureBox_Sound)
            {
                if (this.m_OpenSound == true)
                {
                    this.m_OpenSound = false;
                    voe.Type = VideoControlAction.CloseSound;
                }
                else
                {
                    this.m_OpenSound = true;
                    voe.Type = VideoControlAction.OpenSound;
                }
            }
            else if (pb == this.pictureBox_FastPlayback)
                voe.Type = VideoControlAction.FastPlayback;
            else
                voe.Type = VideoControlAction.Unknown;

            this.ActionEvent(this, voe);


        }

        #endregion

        #region 拖动标题栏
        private void flowLayoutPanel_Title_MouseDown(object sender, MouseEventArgs e)
        {
            this.Selected = true;
            VideoControlActionEventArgs voe = new VideoControlActionEventArgs() { Camera = this.CurrentCamera, VControl = this, Type = VideoControlAction.Title_MouseDown, Arg = e };

            if (this.ActionEvent != null)
            {
                this.ActionEvent(this, voe);
            }


            if (this.CurrentCamera == null)
                return;
            if (this.CanDragOnScreen == false)
            {
                string data = "Transposition " + this.Name;
                this.flowLayoutPanel_Title.DoDragDrop(data, DragDropEffects.Link);
            }
        }

        private void flowLayoutPanel_Title_MouseMove(object sender, MouseEventArgs e)
        {
            VideoControlActionEventArgs voe = new VideoControlActionEventArgs() { Camera = this.CurrentCamera, VControl = this, Type = VideoControlAction.Title_MouseMove, Arg = e };
            if (this.ActionEvent != null)
                this.ActionEvent(this, voe);
        }

        private void flowLayoutPanel_Title_MouseUp(object sender, MouseEventArgs e)
        {
            VideoControlActionEventArgs voe = new VideoControlActionEventArgs() { Camera = this.CurrentCamera, VControl = this, Type = VideoControlAction.Title_MouseUp, Arg = e };
            if (this.ActionEvent != null)
                this.ActionEvent(this, voe);
        }

        #endregion

        private void VideoControl_Load(object sender, EventArgs e)
        {
            this.SetVCSize();
        }

        private void panel_Video_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    this.ActionEvent(this, new VideoControlActionEventArgs() { Type = VideoControlAction.Escape });
                    break;
            }
        }


        /// <summary>
        /// 回放状态
        /// </summary>
        public enum PB_Status
        {
            BeforeStart = 1,
            Play = 2,
            Pause = 3,
            Step = 4
        }

        private void pictureBoxDirControl_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            int speed = 64;
            switch (pb.Tag.ToString())
            {
                case "左":
                    try
                    {
                        Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.Left, hSpeed = speed, vSpeed = 0,  VideoControl = this });
                    }
                    catch { }
                    break;
                case "右":
                    try
                    {
                        Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.Right, hSpeed = speed, vSpeed = 0,  VideoControl = this });
                    }
                    catch { }
                    break;
                case "上":
                    try
                    {
                        Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.Up, hSpeed = 0, vSpeed = speed,  VideoControl = this });
                    }
                    catch { }
                    break;
                case "下":
                    try
                    {
                        Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.Down, hSpeed = 0, vSpeed = speed,  VideoControl = this });
                    }
                    catch { }
                    break;
                case "左上":
                    try
                    {
                        Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.LeftUp, hSpeed = speed, vSpeed = speed,  VideoControl = this });
                    }
                    catch { }
                    break;
                case "右上":
                    try
                    {
                        Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.RightUp, hSpeed = speed, vSpeed = speed,  VideoControl = this });
                    }
                    catch { }
                    break;
                case "左下":
                    try
                    {
                        Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.LeftDown, hSpeed = speed, vSpeed = speed,  VideoControl = this });
                    }
                    catch { }
                    break;
                case "右下":
                    try
                    {
                        Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.RightDown, hSpeed = speed, vSpeed = speed,  VideoControl = this });
                    }
                    catch { }
                    break;
            }
        }

        private void pictureBoxDirControl_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(new Action<object>(this.DirCamera), new PTZControlArgs() { cameraID = this.CurrentCamera.ID, dirDirection = PTZ.DirDirection.Stop, hSpeed = 255, vSpeed = 255, VideoControl = this });
                Console.WriteLine("停");
            }
            catch { }
        }


        private ContextMenuStrip m_TempMenuStrip;
        private void pictureBoxControl_Click(object sender, EventArgs e)
        {

            this.ShowPtzButton =! this.ShowPtzButton;
            setArrowButtonSize();

            this.m_ControlButtonList.ForEach(c => { c.Visible = !c.Visible; c.BringToFront(); });
            //this.pictureBoxLeft.Visible = !this.pictureBoxLeft.Visible;
            //this.pictureBoxRight.Visible = !this.pictureBoxRight.Visible;
            //this.pictureBoxUp.Visible = !this.pictureBoxUp.Visible;
            //this.pictureBoxDown.Visible = !this.pictureBoxDown.Visible;
            //this.pictureBoxLeftUp.Visible = !this.pictureBoxLeftUp.Visible;
            //this.pictureBoxRightUp.Visible = !this.pictureBoxRightUp.Visible;
            //this.pictureBoxLeftDown.Visible = !this.pictureBoxLeftDown.Visible;
            //this.pictureBoxRightDown.Visible = !this.pictureBoxRightDown.Visible;


            if (ShowPtzButton == true)
            {
                this.m_TempMenuStrip = this.ContextMenuStrip;
                this.ContextMenuStrip = null;
            }
            else
            {
                this.ContextMenuStrip = this.m_TempMenuStrip;
            }
        }

        private void panel_Video_Resize(object sender, EventArgs e)
        {
            if (this.ShowPtzButton == false)
                return;
            setArrowButtonSize();
        }

        /// <summary>
        /// 设置箭头按钮
        /// </summary>
        private void setArrowButtonSize()
        {
            int w = this.panel_Video.Width;
            int h = this.panel_Video.Height;
            int l = w / 20;
            this.pictureBoxLeft.Width = l;
            this.pictureBoxLeft.Height = l;
            this.pictureBoxRight.Width = l;
            this.pictureBoxRight.Height = l;
            this.pictureBoxUp.Width = l;
            this.pictureBoxUp.Height = l;
            this.pictureBoxDown.Width = l;
            this.pictureBoxDown.Height = l;
            this.pictureBoxLeftUp.Width = l;
            this.pictureBoxLeftUp.Height = l;
            this.pictureBoxRightUp.Width = l;
            this.pictureBoxRightUp.Height = l;
            this.pictureBoxLeftDown.Width = l;
            this.pictureBoxLeftDown.Height = l;
            this.pictureBoxRightDown.Width = l;
            this.pictureBoxRightDown.Height = l;

            this.pictureBoxLeft.Location = new Point(0, (h - l) / 2);
            this.pictureBoxRight.Location = new Point(w - l, (h - l) / 2);
            this.pictureBoxUp.Location = new Point((w - l) / 2, 0);
            this.pictureBoxDown.Location = new Point((w - l) / 2, h - l);
            this.pictureBoxLeftUp.Location = new Point(0, 0);
            this.pictureBoxRightUp.Location = new Point(w - l, 0);
            this.pictureBoxLeftDown.Location = new Point(0, h - l);
            this.pictureBoxRightDown.Location = new Point(w - l, h - l);
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            return base.PreProcessMessage(ref msg);
        }

   

     
    }
}