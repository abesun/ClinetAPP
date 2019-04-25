using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using ClientAPP.Core.Contract.Log;
using ClientAPP.Core.Contract.Websocket;
using Newtonsoft.Json;
using ClientAPP.VideoModule;

namespace ClientAPP.FormService
{
    /// <summary>
    /// websocket相关
    /// </summary>
    internal class WebsocketHelper
    {

        private WebSocketServer m_Server;

        /// <summary>
        /// 日志
        /// </summary>
        public IAppLog LogModule { get; set; }

        /// <summary>
        /// 视频管理器
        /// </summary>
        public ServiceManager Manager { get; set; }

        /// <summary>
        /// 客户端
        /// </summary>
        private List<IWebSocketConnection> m_ClientList;
        

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="localPort"></param>
        /// <returns></returns>
        public bool Init(int localPort = 4502)
        {

            this.m_ClientList = new List<IWebSocketConnection>();

            this.m_Server = new WebSocketServer($"ws://0.0.0.0:{localPort}");
            this.m_Server.Start(client =>
            {
                client.OnMessage = message => this.receiveMessage(client, message);
                client.OnError = error => this.LogModule.Error(error);
                client.OnBinary = bin => { this.LogModule.Error("接收到非文本数据"); };
                client.OnOpen = () => { lock (this.m_ClientList) { this.m_ClientList.Add(client); } };
                client.OnClose = () => { lock (this.m_ClientList) { this.m_ClientList.Remove(client); } };
            });

            return true;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(string msg)
        {
            try
            {
                this.m_ClientList.ToList().ForEach(c => c.Send(msg));
            }
            catch (Exception ex)
            {
                this.LogModule.Error(ex);
            }
        }

        /// <summary>
        /// 处理接收信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void receiveMessage(IWebSocketConnection client, string message)
        {
            WSProtocol wsp = default;
            this.LogModule.Debug($"接收到网络命令:{client.ConnectionInfo.ClientIpAddress}  内容:{message}");
            try
            {
                wsp = WSProtocol.FromJson(message);
            }
            catch (Exception ex)
            {
                this.LogModule.Error($"接收到无法解析的内容: {message}");
                return;
            }
            if(wsp==null||wsp?.Header==null||wsp?.Body==null)
            {
                this.LogModule.Error($"接收到无法解析的内容: {message}");
                return;
            }

            switch (wsp.Header.Ver)
            {
                case "1.0"://            
                default:
                    this.procWSPDefault(wsp);
                    break;
            }
        }

        /// <summary>
        /// 处理网络请求
        /// </summary>
        /// <param name="wsp"></param>
        private void procWSPDefault(WSProtocol wsp)
        {
            switch(wsp.Header.BodyType)
            {
                case BodyType.Request:
                    this.procRequest(wsp);
                    break;
                case BodyType.Response:
                    break;
                case BodyType.UploadEvent:
                    break;
                default:break;
            }
        }

        #region 请求处理

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="wsp"></param>
        private void procRequest(WSProtocol wsp)
        {
            //WSRequest wSRequest = JsonConvert.DeserializeObject<WSRequest>(wsp.Body);
            var wSRequest = JsonConvert.DeserializeObject< WSRequest>( wsp.Body .ToString()) ;
            this.Manager.WebSocketRequestProc(wSRequest);
          
        }


        #endregion
    }
}
