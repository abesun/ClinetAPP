using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.VideoModule.Event
{
    public class SnapEventArgs:EventArgs
    {
        /// <summary>
        /// 抓图文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 抓图控件
        /// </summary>
        public VideoControl VC { get; set; }
    }

    public delegate void SnapEventHandle(object sender, SnapEventArgs args);
}
