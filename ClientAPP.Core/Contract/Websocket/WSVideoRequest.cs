using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Core.Contract.Websocket
{
    /// <summary>
    /// 视频请求
    /// </summary>
    public class WSVideoRequest
    {
        /// <summary>
        /// 面板id
        /// </summary>
        public string PanelID { get; set; } = "Default";

        #region 命令常量
        public const string StartPreview = "StartPreview";

        public const string StopPreview = "StopPreview";

        public const string StartPlayback = "StartPlayback";

        public const string StopPlayback = "StopPlayback";

        public const string OpenWindow = "OpenWindow";

        public const string CloseWindow = "CloseWindow";
        #endregion
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    public class WSVideoRequest_OpenWindow : WSVideoRequest
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public int? Width { get; set; } = 800;

        /// <summary>
        /// 高度
        /// </summary>
        public int? Height { get; set; } = 600;

        /// <summary>
        /// 起始X坐标
        /// </summary>
        public int? LocationX { get; set; } = 100;

        /// <summary>
        /// 起始Y坐标
        /// </summary>
        public int? LocationY { get; set; } = 100;

        /// <summary>
        /// 屏幕号
        /// </summary>
        public int? ScreenID { get; set; } = 0;

        /// <summary>
        /// 总在最前
        /// </summary>
        public bool? TopMost { get; set; } = true;
        /// <summary>
        /// 显示窗口边框
        /// </summary>
        public bool? ShowWindowBorder { get; set; } = true;
        /// <summary>
        /// 显示视频画面的标题栏
        /// </summary>
        public bool? ShowVCTitle { get; set; } = true;

        /// <summary>
        /// 布局名称
        /// </summary>
        public string LayoutName { get; set; } = "1";
    }

    /// <summary>
    /// 打开视频预览参数
    /// </summary>
    public class WSVideoRequest_StartPreview:WSVideoRequest
    {

        /// <summary>
        /// 控件索引
        /// </summary>
        public int VCIndex { get; set; } = 0;
        /// <summary>
        /// 摄像机ID
        /// </summary>
        public string CameraID { get; set; }
        /// <summary>
        /// 名称（用于显示）
        /// </summary>
        public string CameraName { get; set; }
        /// <summary>
        /// 视频源ID
        /// </summary>
        public string SourceID { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string SourceIP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int SourcePort { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string SourceUser { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string SourcePassword { get; set; }

        /// <summary>
        /// 视频源名称
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 视频源类型
        /// </summary>
        public int SourceType { get; set; }

        /// <summary>
        /// 通道号/第三方系统编号/打开视频需要的编码
        /// </summary>
        public string CameraCode { get; set; }
        /// <summary>
        /// 码流类型（0主码流 1 子码流 ）
        /// </summary>
        public int StreamIndex { get; set; }
    }

    /// <summary>
    /// 关闭视频
    /// </summary>
    public class WSVideoRequest_StopVideo: WSVideoRequest
    {
        public int VCIndex { get; set; }
    }

    /// <summary>
    /// 开始回放
    /// </summary>
    public class WSVideoRequest_StartPlayback:WSVideoRequest
    {
        /// <summary>
        /// 控件索引
        /// </summary>
        public int VCIndex { get; set; } = 0;
        /// <summary>
        /// 摄像机ID
        /// </summary>
        public string CameraID { get; set; }
        /// <summary>
        /// 名称（用于显示）
        /// </summary>
        public string CameraName { get; set; }
        /// <summary>
        /// 数据源ID
        /// </summary>
        public string SourceID { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string SourceIP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int SourcePort { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string SourceUser { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string SourcePassword { get; set; }

        /// <summary>
        /// 视频源名称
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 视频源类型
        /// </summary>
        public int SourceType { get; set; }

        /// <summary>
        /// 通道号/第三方系统编号/打开视频需要的编码
        /// </summary>
        public string CameraCode { get; set; }
        
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
