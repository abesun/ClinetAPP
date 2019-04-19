using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Core.DataModel.Video
{
    /// <summary>
    /// 视频源类型
    /// </summary>
    public enum VideoSourceType
    {
        /// <summary>
        /// 大华设备
        /// </summary>
        Dahua = 1,
        /// <summary>
        /// 海康设备
        /// </summary>
        Hik=8,

        /// <summary>
        /// 海康8200开放平台
        /// </summary>
        Hik8200Open=200,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 1000,

        /// <summary>
        /// 其他
        /// </summary>
        Other = 0
    }

    /// <summary>
    /// 协议类型
    /// </summary>
    public enum Protocol
    {
        SDK=0,
        RTSP=1,
        HLS=2,
        RTMP=3
    }

}
