using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Core.Contract.Websocket
{
    /// <summary>
    /// 事件上传
    /// </summary>
    public class WSEventUpload
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// 模块名
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public object Params { get; set; }
    }

    public class WSEvent_FileUpload
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// base64编码的文件数据
        /// </summary>
        public string FileData_Base64 { get; set; }
    }
}
