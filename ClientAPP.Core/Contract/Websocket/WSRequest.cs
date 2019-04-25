using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Core.Contract.Websocket
{
    /// <summary>
    /// websocket请求
    /// </summary>
    public class WSRequest
    {
        /// <summary>
        /// 模块名
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object Params { get; set; }
        /// <summary>
        /// 同步/异步执行
        /// </summary>
        public bool Sync { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        public string RequestSN { get; set; }
    }
}
