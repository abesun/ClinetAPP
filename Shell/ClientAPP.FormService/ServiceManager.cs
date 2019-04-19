using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.VideoModule;
using ClientAPP.Core.Contract.Log;
namespace ClientAPP.FormService
{
    internal class ServiceManager
    {
        
        private VideoManager videoManager;

        public IAppLog LogModule { get; set; }
        

        public bool Init()
        {
            this.LogModule.Info("初始化app");
            return true;
        }

        /// <summary>
        /// 初始化视频模块
        /// </summary>
        /// <returns></returns>
        private bool initVideoModule()
        {
            this.LogModule.Info("初始化视频模块");
            videoManager = new VideoManager();
            return videoManager.Init();
            
        }
    }
}
