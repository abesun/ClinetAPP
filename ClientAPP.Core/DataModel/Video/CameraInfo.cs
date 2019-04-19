using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Core.DataModel.Video
{
    /// <summary>
    /// 摄像机
    /// </summary>
    public class CameraInfo
    {
        /// <summary>
        /// 编号（内部编号）
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 国标编码
        /// </summary>
        public string GBCode { get; set; }

        /// <summary>
        /// 所属组织名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 通道号/第三方系统编号/打开视频需要的编码
        /// </summary>
        public string CameraCode { get; set; }

    }
}
