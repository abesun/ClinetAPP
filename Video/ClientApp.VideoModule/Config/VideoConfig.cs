using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.VideoModule.Config
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public class VideoConfig
    {

        /// <summary>
        /// 屏幕数 0表示自动检测
        /// </summary>
        public int ScreenCount { get; set; }

        /// <summary>
        /// 截图默认路径
        /// </summary>
        public string SanpPath { get; set; }

        /// <summary>
        /// 截图文件名（支持通配符）
        /// </summary>
        public string SnapFilePattern { get; set; }

       
        /// <summary>
        /// 录像默认路径
        /// </summary>
        public string RecordPath { get; set; }

        /// <summary>
        /// 录像文件名（支持通配符）
        /// </summary>
        public string RecordFilePattern { get; set; }

        /// <summary>
        /// 是否自动关闭相同的图像
        /// </summary>
        public bool AutoCloseSameVideo { get; set; }

        /// <summary>
        /// 小窗口时自动切换子码流
        /// </summary>
        public bool AutoChangeStreamIndex { get; set; }

        /// <summary>
        /// 视频控件配置
        /// </summary>
        public VideoControlConfig VideoControlConfig { get; set; }

        /// <summary>
        /// 窗体属性配置
        /// </summary>
        public FormStyleConfig FormStyleConfig { get; set; }
    }


    public class FormStyleConfig
    {
        /// <summary>
        /// 是否启用本地配置
        /// </summary>
        public bool AutoLoad { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }

    
}
