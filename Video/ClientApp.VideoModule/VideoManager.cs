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
        private IAppLog LogModule;

        /// <summary>
        /// 面板哈希表
        /// </summary>
        private Hashtable m_VideoPanelTable = new Hashtable();

        /// <summary>
        /// 控件列表
        /// </summary>
        private ConcurrentBag<VideoControl> m_VideoControlList = new ConcurrentBag<VideoControl>();

        #region 外部接口
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Init(string ConfigFile = "VideoModule.config", string layoutConfigFile = "layoutConfig.xml")
        {
            //todo
            config = ConfigHelper<VideoConfig>.LoadXML(ConfigFile, out string err);
            if(config is null)
            {
                this.LogModule.Error($"读取配置文件失败: {err}");
                return false;
            }

            this.layoutConfig = ConfigHelper<LayoutConfig>.LoadXML(layoutConfigFile, out  err);
            if (layoutConfig is null)
            {
                this.LogModule.Error($"读取布局配置文件失败: {err}");
                return false;
            }

         

            return true;
        }

        
        public Control CreateVideoPanel(string key, int VideoControlCount = 1, string layoutName = "1")
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

            return null;
        }
             

        public VideoControl CreateVideoControl(string key="")
        {
            VideoControl vc = new VideoControl();       
            vc.SetVideoControlConfig(this.config.VideoControlConfig);
            vc.CanControl = true;
            vc.Mode = ShowMode.Real;
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



            this.LogModule?.Debug($"准备打开实时预览: ({camera.ID}){camera.Name} 设备:{videoSourceInfo.IP}:{videoSourceInfo.Port} 连接码:{camera.CameraCode}");

            VideoSourceBase vs = this.getVideoSourceByType(videoSourceInfo.Type);
            if (vs == null)
            {
                this.LogModule?.Error("未支持的设备类型");
                return false;
            }

            return vs.StartPreview(camera, videoSourceInfo, vc, streamIndex);

        }


        #endregion

        #region 视频面板
        private void Panel_ControlChanged(object sender, ControlChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 视频控件
        private void Vc_ActionEvent(object sender, VideoControlActionEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void Vc_PtzEvent(object sender, PTZControlArgs args)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 视频源

        private Hashtable m_VidoeSourceTable = new Hashtable();

        

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

            return ret;
        }


        #endregion

       

    }
}
