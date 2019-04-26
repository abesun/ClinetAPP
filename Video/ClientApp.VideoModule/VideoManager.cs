using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.VideoModule.Config;
using ClientAPP.Uti;
using ClientAPP.Core;
using ClientAPP.Core.Contract.Log;
using System.Collections;
using System.Collections.Concurrent;
using System.Windows.Forms;
using ClientAPP.Core.DataModel.Video;
using System.ComponentModel.Composition;
using ClientAPP.VideoModule.VideoSource;
using System.Threading;
namespace ClientAPP.VideoModule
{
    /// <summary>
    /// 视频管理器
    /// </summary>
    public class VideoManager
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        private VideoConfig config;

        /// <summary>
        /// 布局配置
        /// </summary>
        private LayoutConfig layoutConfig;

        /// <summary>
        /// 日志
        /// </summary>
        public IAppLog LogModule { get; set; }

        /// <summary>
        /// 面板哈希表
        /// </summary>
        private Hashtable m_VideoPanelTable = new Hashtable();

        /// <summary>
        /// 控件列表
        /// </summary>
        private ConcurrentBag<VideoControl> m_VideoControlList = new ConcurrentBag<VideoControl>();

        /// <summary>
        /// 当前选中的视频控件
        /// </summary>
        private VideoControl m_CurVideoControl;

        /// <summary>
        /// 当前选中的视频控件
        /// </summary>
        public VideoControl CurVideoControl
        {
            get { return this.m_CurVideoControl; }
            set
            {
                this.m_CurVideoControl = value;
                if (this.m_CurVideoControl.Selected == false)
                {
                    this.m_CurVideoControl.Selected = true;
                }
            }
        }

        #region 外部接口
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Init(string ConfigFile = "VideoModuleConfig.xml", string layoutConfigFile = "layoutConfig.xml")
        {
            //todo
            config = ConfigHelper<VideoConfig>.LoadXML(ConfigFile, out string err);
            if(config is null)
            {
                this.LogModule.Error($"读取配置文件失败: {err}");
                return false;
            }

            this.layoutConfig = LayoutConfig.LoadConfig(layoutConfigFile);
            if (layoutConfig is null)
            {
                this.LogModule.Error($"读取布局配置文件失败: {err}");
                return false;
            }

         

            return true;
        }

        
        public Control CreateVideoPanel(string key, int VideoControlCount = 16, string layoutName = "1")
        {
            VideoPanel panel = this.m_VideoPanelTable[key] as VideoPanel;
            if (panel != null)
                return panel;

            panel = new VideoPanel();

            List<VideoControl> vcList = new List<VideoControl>();
            for (int i = 0; i < VideoControlCount; i++)
            {
                VideoControl vc = this.CreateVideoControl($"{key}_{i}");
                vcList.Add(vc);

            }
            panel.VCList = vcList;
            panel.ControlChanged += Panel_ControlChanged;
            panel.CurLayout = this.layoutConfig.Layouts.First(l => l.Name == layoutName) ?? this.layoutConfig.Layouts[0] ?? Layout.GetNormalLayou("1", 1, 1);            
            panel.Init();
            this.m_VideoPanelTable[key] = panel;
            return panel;
        }
             

        public VideoControl CreateVideoControl(string key="")
        {
            VideoControl vc = new VideoControl();       
            vc.SetVideoControlConfig(this.config.VideoControlConfig);
            vc.CanControl = true;
            vc.Mode = ShowMode.Stop;
            vc.CanSelected = true;
            vc.Visible = false;
            vc.Name = key;
            vc.ActionEvent += Vc_ActionEvent;
            vc.PtzEvent += Vc_PtzEvent;
            this.m_VideoControlList.Add(vc);

            return vc;
        }
        
        public bool StartPreview(CameraInfo camera, VideoSourceInfo videoSourceInfo, VideoControl vc, int streamIndex = 0)
        {
            if (vc.Locked == true)//图像被锁定
                return false;

            vc.ErrorMessage = "";

            this.LogModule?.Debug($"准备打开实时预览: ({camera.ID}){camera.Name} 设备:{videoSourceInfo.IP}:{videoSourceInfo.Port} 连接码:{camera.CameraCode}");

            //关闭已经打开的视频
            if(vc.Mode != ShowMode.Stop)
            {
                this.StopVideo(vc);
            }

            VideoSourceBase vs = this.getVideoSourceByType(videoSourceInfo.Type);
            if (vs == null)
            {
                this.LogModule?.Error("未支持的设备类型");
                return false;
            }
            camera.VideoSourceInfo = videoSourceInfo;
            bool ret= vs.StartPreview(camera, vc, streamIndex);
            if (ret == true)
            {
                vc.Mode = ShowMode.Real;
                this.m_VideoControlTable[vc] = vs;
                vc.CurrentCamera = camera;
                
            }
            return ret;

        }

        public bool StartPlayback(CameraInfo camera, VideoSourceInfo videoSourceInfo, VideoControl vc, DateTime start, DateTime end)
        {
            if (vc.Locked == true)//图像被锁定
                return false;
            this.LogModule?.Debug($"准备打开回放: ({camera.ID}){camera.Name} 设备:{videoSourceInfo.IP}:{videoSourceInfo.Port} 连接码:{camera.CameraCode} 时间:{start}--{end}");

            vc.ErrorMessage = "";
            //关闭已经打开的视频
            if (vc.Mode != ShowMode.Stop)
            {
                this.StopVideo(vc);
            }

            VideoSourceBase vs = this.getVideoSourceByType(videoSourceInfo.Type);
            if (vs == null)
            {
                this.LogModule?.Error("未支持的设备类型");
                return false;
            }
            camera.VideoSourceInfo = videoSourceInfo;
            bool ret= vs.StartPlaybackByTime(camera, vc, start, end);
            if (ret == true)
            {
                vc.Mode = ShowMode.Playback;
                this.m_VideoControlTable[vc] = vs;
                vc.CurrentCamera = camera;
            }
            return ret;
        }

        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <param name="vc"></param>
        public void StopVideo(VideoControl vc)
        {
            VideoSourceBase vs = this.getVideoSourceByVideoControl(vc);
            switch(vc.Mode)
            {
                case ShowMode.Real:
                    vs?.StopPreview(vc);
                    break;
                case ShowMode.Playback:
                case ShowMode.PlayFile:
                    vs?.StopPlayback(vc);
                    break;
                default:break;
                    
            }
            vc.Mode = ShowMode.Stop;
            vc.CurrentCamera = null;
            this.m_VideoControlTable[vc] = null;
            vc.ErrorMessage = "";
        }

        /// <summary>
        /// 设置布局
        /// </summary>
        /// <param name="panelID"></param>
        /// <param name="layoutName"></param>
        public void SetLayout(string panelID, string layoutName)
        {

            VideoPanel panel = this.m_VideoPanelTable[panelID] as VideoPanel;
            if(panel==null)
            {
                this.LogModule?.Error($"未找到相应的视频面板: {panelID}");
                return;
            }

            var layout = this.layoutConfig.Layouts.FirstOrDefault(l => l.Name == layoutName);
            if(layout==null)
            {

                this.LogModule?.Error($"未找到相应的预案名称: {layoutName}");
                return;
            }
            if(panel.CurLayout.Name!= layout.Name)
                panel.CurLayout = layout;
        }

        /// <summary>
        /// 获得面板的布局
        /// </summary>
        /// <param name="panelID"></param>
        /// <returns></returns>
        public Layout GetPanelLayout(string panelID)
        {
            Layout ret = default;
            VideoPanel panel = this.m_VideoPanelTable[panelID] as VideoPanel;
            if (panel == null)
            {
                this.LogModule?.Error($"未找到相应的视频面板: {panelID}");
                return ret;
            }
            return panel.CurLayout;
        }

        public VideoControl GetVideoControl(string panelID, int index)
        {
            VideoControl ret = default;
            VideoPanel panel = this.m_VideoPanelTable[panelID] as VideoPanel;
            if(panel==null)
            {
                this.LogModule.Error($"未找到相应的视频面板: {panelID}");
                return ret;
            }
            if(index<0||index>=panel.CurLayout.ControlList.Count)
            {
                this.LogModule.Error($"索引有误: {index}");
                return ret;
            }
            return panel.CurLayout.ControlList[index].Control;
        }

        #endregion

        #region 视频面板
        /// <summary>
        /// 选中的控件改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_ControlChanged(object sender, ControlChangedEventArgs e)
        {
            
            //throw new NotImplementedException();
        }
        #endregion

        #region 视频控件
        private void Vc_ActionEvent(object sender, VideoControlActionEventArgs args)
        {
            var vs = this.getVideoSourceByVideoControl(args.VControl);
            switch (args.Type)
            {
                case VideoControlAction.CloseVideo:
                    this.closeVideo(args.VControl);
                    break;
                case VideoControlAction.PB_Play:
                    if (args.Arg != null && args.Camera != null)
                    {
                        this.closeVideo(args.VControl);
                        vs?.StartPlaybackByTime(args.Camera, args.VControl, ((PlaybackInfo)args.Arg).Start, ((PlaybackInfo)args.Arg).End);                     
                    }
                    break;
                case VideoControlAction.PB_Resume:
                    vs?.PB_Resume(args.VControl);
                    break;
                case VideoControlAction.PB_Pause:
                    vs?.PB_Pause(args.VControl);
                    break;
                case VideoControlAction.PB_Stop:
                    vs?.StopPreview(args.VControl);
                    break;
                case VideoControlAction.PB_Fast:
                    vs?.PB_Fast(args.VControl);
                    break;
                case VideoControlAction.PB_Slow:
                    vs?.PB_Slow(args.VControl);
                    break;
                case VideoControlAction.PB_Step:
                    vs?.PB_Step(args.VControl);
                    break;
                case VideoControlAction.Snap:
                    this.snap(args.VControl);
                    break;
                case VideoControlAction.Seleced:
                    this.setSelected(args.VControl);
                    break;
                case VideoControlAction.PB_SetPos:
                    vs?.PB_SetPos(args.VControl, (int)args.Arg);
                    break;
                case VideoControlAction.PB_StratDownload:

                    this.startDownload(args.VControl);
                    break;
                case VideoControlAction.PB_StopDownload:
                    this.stopDownload(args.VControl);
                    break;
                case VideoControlAction.ErrorReport:
                    this.LogModule.Error(args.Arg.ToString());                   
                    break;
                case VideoControlAction.Transposition:
                    try
                    {
                        int oldIndex = Convert.ToInt32(args.Arg.ToString());
                        int newIndex = Convert.ToInt32(args.VControl.Name);
                        VideoPanel panel = this.m_VideoPanelTable[args.VControl.VideoPanelID] as VideoPanel;
                        panel?.Transposition(oldIndex, newIndex);
                    }
                    catch { }
                    break;
                case VideoControlAction.FullScreen:
                case VideoControlAction.NormalScreen:
                    //this.changeStreamIndex(vca);
                    break;
                case VideoControlAction.OpenSound:
                    this.openSound_VC(args.VControl);
                    break;
                case VideoControlAction.CloseSound:
                    this.closeSound_VC(args.VControl);
                    break;
            }

        }

      

        private void Vc_PtzEvent(object sender, PTZControlArgs args)
        {
            VideoSourceBase vs = this.m_VideoControlTable[args.VideoControl] as VideoSourceBase;
            
            switch(args.Type)
            {
                case PTZControlArgs.PtzType.Direction:
                    vs?.Ptz_DirCamera(args.VideoControl, args.dirDirection, args.hSpeed, args.vSpeed);
                    break;
                case PTZControlArgs.PtzType.Len:
                    vs?.Ptz_LenCamera(args.VideoControl, args.lenType);
                    break;
                case PTZControlArgs.PtzType.AutoFindDirection:
                    //vs?.Client_CameraAutoFindDirection(args.VideoControl,args.ol)
                    //todo
                    break;
                default:break;

            }
        }

        /// <summary>
        /// 设置控件选中
        /// </summary>
        /// <param name="vc"></param>
        private void setSelected(VideoControl vc)
        {

            this.m_VideoControlList.Where(c => c != vc).ToList().ForEach(c => c.Selected = false);            

        }
        #endregion

        #region 视频源
        /// <summary>
        /// 类型索引视频源
        /// </summary>
        private Hashtable m_VidoeSourceTable = new Hashtable();
        /// <summary>
        /// 视频控件索引视频源
        /// </summary>
        private Hashtable m_VideoControlTable = new Hashtable();

        

        /// <summary>
        /// 根据类型获取视频源
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private VideoSourceBase getVideoSourceByType(VideoSourceType type)
        {
            var ret = this.m_VidoeSourceTable[type] as VideoSourceBase;
            if (ret != null)
                return ret;

            switch(type)
            {
                case VideoSourceType.Hik8200Open:
                    ret = new VideoSource_HikOpen8200();                    
                    break;
            }

            ret.LogModule = this.LogModule;
            if (ret.Init() == false)
                return null;
            return ret;
        }

        private VideoSourceBase getVideoSourceByVideoControl(VideoControl vc)
        {
            var ret = this.m_VideoControlTable[vc] as VideoSourceBase;
            return ret;
        }

        private void closeVideo(VideoControl vc)
        {
            VideoSourceBase vs = this.getVideoSourceByVideoControl(vc);
           
            switch (vc.Mode)
            {
                case ShowMode.Real:
                    vs?.StopPreview(vc);
                    break;
                case ShowMode.Playback:
                case ShowMode.PlayFile:
                    vs?.StopPlayback(vc);
                    break;
                default:break;
                    
            }
        }

        private void snap(VideoControl vc)
        {
            VideoSourceBase vs = this.getVideoSourceByVideoControl(vc);
            switch (vc.Mode)
            {
                case ShowMode.Real:
                    vs?.Snap(vc, getFileName(DateTime.Now));
                    //this.SnapPicture(vca.VControl);
                    break;
                case ShowMode.Playback:
                    DateTime time = DateTime.Now;
                    vs.PB_GetCurTime(vc, out time);
                    vs?.PB_Snap(vc, getFileName(time));                 
                    break;
            }

            //获取文件名
            string getFileName(DateTime time)
            {
                CameraInfo camera = vc.CurrentCamera;

                string fileName = $"{ FileHelper.GetCorrectFileName(camera.Name)}{time.ToString("yyyyMMddhhmmss")}.jpg";
                if ( string.IsNullOrEmpty( this.config.SnapPath )==true)
                {
                    System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "JPG(*.jpg)|*.jpg|BMP(*.bmp)|*.bmp|AllFile(*.*)|*.*";
                    sfd.FilterIndex = 1;
                    sfd.DefaultExt = "jpg";
                    sfd.FileName = fileName;
                    if (sfd.ShowDialog() == DialogResult.OK)
                        fileName = sfd.FileName;                    
                }
                else
                {
                    if (this.config.SnapPath.EndsWith("\\") == false)
                        this.config.SnapPath += "\\";
                    fileName = $"{this.config.SnapPath}{fileName}";
                }
                return fileName;
            }
        }

        private void startDownload(VideoControl vc)
        {
            //todo
        }

        private void stopDownload(VideoControl vc)
        {
            //todo
        }

        private void closeSound_VC(VideoControl vc)
        {
            var vs = this.getVideoSourceByVideoControl(vc);
            switch(vc.Mode)
            {
                case ShowMode.Real:
                    vs?.OpenSound(vc);
                    break;
                case ShowMode.Playback:
                case ShowMode.PlayFile:
                    
                    break;
            }
        }

        private void openSound_VC(VideoControl vc)
        {
            var vs = this.getVideoSourceByVideoControl(vc);
            switch (vc.Mode)
            {
                case ShowMode.Real:
                    vs?.CloseSound(vc);
                    break;
                case ShowMode.Playback:
                case ShowMode.PlayFile:

                    break;
            }
        }

        private void changeStreamIndex(VideoControlActionEventArgs vca)
        {
            if (this.config.AutoChangeStreamIndex == false)
                return;
            if (vca.Camera == null)
                return;
            if (vca.VControl.CurrentCamera == null)
                return;
            if (vca.VControl.Mode != ShowMode.Real)
                return;
            VideoSourceBase vs = this.getVideoSourceByVideoControl(vca.VControl);
            VideoPanel panel = this.m_VideoPanelTable[vca.VControl.VideoPanelID] as VideoPanel;
            //if (vca.VControl.IsMax)
            //    vs?.ChangeStreamIndex(vca.VControl.VControl, 0);
            //else
            //{
            //    if (this.CurLayout.ControlList.Count > 4)
            //    {
            //        this.m_StreamManager.ChangeStreamIndex(vca.VControl.VControl, 1);
            //    }
            //}
        }
        /// <summary>
        /// 切换码流
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="streamIndex"></param>
        public void ChangeStreamIndex(VideoControl vc, int streamIndex)
        {
            if (vc.CurrentCamera == null)
                return;
            if (vc.Mode != ShowMode.Real)
                return;
            //this.m_StreamManager.ChangeStreamIndex(vc.VControl, streamIndex);
        }
        #endregion



    }
}
