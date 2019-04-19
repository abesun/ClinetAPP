using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ClientAPP.Core.Contract.Websocket
{
    /// <summary>
    /// websocket协议
    /// </summary>
    public class WSProtocol
    {
        /// <summary>
        /// 协议头
        /// </summary>
        public WSPHeader Header { get; set; }

        /// <summary>
        /// 协议内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 转化成json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()=> JsonConvert.SerializeObject(this);

        /// <summary>
        /// 从json字符串转化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static WSProtocol FromJson(string json) => JsonConvert.DeserializeObject<WSProtocol>(json);

      
    }
    
    /// <summary>
    /// 协议头
    /// </summary>
    public class WSPHeader
    {
        /// <summary>
        /// 版本号（默认最高版本）
        /// </summary>
        public string Ver { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 用户标签
        /// </summary>
        public string UserTag { get; set; }

        /// <summary>
        /// body类型
        /// </summary>
        public BodyType BodyType { get; set; }
    }

    /// <summary>
    /// body类型
    /// </summary>
    public enum BodyType
    {
        /// <summary>
        /// 请求
        /// </summary>
        Request = 0,
        /// <summary>
        /// 响应
        /// </summary>
        Response = 1,
        /// <summary>
        /// 事件上报
        /// </summary>
        UploadEvent = 2
    }


}
