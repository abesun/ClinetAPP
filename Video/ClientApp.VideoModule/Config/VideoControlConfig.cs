using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.VideoModule.Config
{
    public class VideoControlConfig
    {
        /// <summary>
        /// 是否隐藏视频控件的标题栏
        /// </summary>
        public bool HideVCTitle { get; set; }

        /// <summary>
        /// 是否隐藏视频控件下载按钮
        /// </summary>
        public bool ShowDownload { get; set; }

        /// <summary>
        /// 显示快速回放按钮
        /// </summary>
        public bool ShowFastPlayback { get; set; }

        /// <summary>
        /// 隐藏标题栏时是否自动显示和隐藏
        /// </summary>
        public bool AutoHideVCTitle { get; set; }

        /// <summary>
        /// 边框粗细
        /// </summary>
        public int Margin { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public string BackColorRGB { get; set; }

        /// <summary>
        /// 选中色
        /// </summary>
        public string SelectedColorRGB { get; set; }

        /// <summary>
        /// 视频面板颜色
        /// </summary>
        public string VideoPanelColorRGB { get; set; }

        /// <summary>
        /// 关闭图像时刷新控件
        /// </summary>
        public bool AutoRefreshControl { get; set; }
    }
}
