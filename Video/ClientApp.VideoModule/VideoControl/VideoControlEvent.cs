using ClientAPP.Core.DataModel.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientAPP.VideoModule
{
    public class VideoControlEvent
    {
    }

    /// <summary>
    /// 控制命令参数
    /// </summary>
    public class PTZControlArgs:EventArgs
    {
        /// <summary>
        /// 摄像机ID
        /// </summary>
        public string cameraID { get; set; }
        public PtzType Type { get; set; } = PtzType.Direction;

        public PTZ.DirDirection dirDirection { get; set; } = PTZ.DirDirection.Stop;
        public int hSpeed { get; set; } = 64;
        public int vSpeed { get; set; } = 64;
        public PTZ.LenType lenType { get; set; } = PTZ.LenType.Stop;
        public Rectangle Rect { get; set; }
        /// <summary>
        /// 延迟执行时间(毫秒)
        /// </summary>
        public int Delay { get; set; }
        /// <summary>
        /// 控制标识，用于识别结束控制
        /// </summary>
        public string GuidString { get; set; }

        /// <summary>
        /// 面板
        /// </summary>
        public VideoControl VideoControl { get; set; }

        /// <summary>
        /// 控制类型
        /// </summary>
        public enum PtzType
        {
            Direction=1,
            Len=2,
            AutoFindDirection=3
        }
    }

    /// <summary>
    /// 控制委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void PTZEventHandle(object sender, PTZControlArgs args);
}
