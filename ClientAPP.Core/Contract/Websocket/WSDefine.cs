using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Core.Contract.Websocket
{
    /// <summary>
    /// websocket常量定义
    /// </summary>
    public class WSDefine
    {
        #region 模块类型
        /// <summary>
        /// 视频
        /// </summary>
        public const string VideoModule = "video";
        /// <summary>
        /// 对讲
        /// </summary>
        public const string TalkModule = "talk";
            

        #endregion
    }

    /// <summary>
    /// 事件类型
    /// </summary>
    public class WSEventDefine
    {
        /// <summary>
        /// 人脸图像
        /// </summary>
        public const string FaceImg = "FaceImg";
    }
}
