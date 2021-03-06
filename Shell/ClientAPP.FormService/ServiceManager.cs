﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.VideoModule;
using ClientAPP.Core.Contract.Log;
using ClientAPP.Core.Contract.Websocket;
using System.Windows.Forms;
using System.Drawing;

namespace ClientAPP.FormService
{
    internal class ServiceManager
    {
        /// <summary>
        /// 视频管理器
        /// </summary>
        private VideoManager videoManager;

        public IAppLog LogModule { get; set; }

        /// <summary>
        /// 视频命令处理
        /// </summary>
        private Video.VideoRequesProc videoRequesProc;

        /// <summary>
        /// websocket处理器
        /// </summary>
        private WebsocketHelper WebsocketHelper;

        /// <summary>
        /// 主窗口
        /// </summary>
        private Form frmMain;

        private UploadFacePic.FrmPickFace frmPickFace;
        

        public bool Init(Form frm)
        {
            this.frmMain = frm;
            this.LogModule.Info("初始化app");

            bool ret= this.initVideoModule();
            if(ret==false)
            {
                this.LogModule.Error("初始化视频模块失败");
                return false;
            }
            videoRequesProc = new Video.VideoRequesProc() { LogModule = this.LogModule, VideoManager = this.videoManager };

            this.WebsocketHelper = new WebsocketHelper() { LogModule = this.LogModule, Manager = this };
            this.WebsocketHelper.Init();

            this.frmPickFace = new UploadFacePic.FrmPickFace();
            this.frmPickFace.FaceDetected += FrmPickFace_FaceDetected;

            return true;
        }

        private void FrmPickFace_FaceDetected(object sender, UploadFacePic.FaceDetectEventArgs args)
        {
            args.FaceImage.Save("uploadtemp.jgp");
            var wsp = Plugins.FileUpload.GetUploadPicWSP("uploadtemp.jgp");
            this.EventUploadProc(wsp);
        }

        /// <summary>
        /// 初始化视频模块
        /// </summary>
        /// <returns></returns>
        private bool initVideoModule()
        {
            this.LogModule.Info("初始化视频模块");
            videoManager = new VideoManager() { LogModule = this.LogModule };
            videoManager.SnapEvent += VideoManager_SnapEvent;
            return videoManager.Init();
            
        }

        private void VideoManager_SnapEvent(object sender, VideoModule.Event.SnapEventArgs args)
        {
            Image img = default;
            try
            {
                img = Image.FromFile(args.FileName);
            }
            catch(Exception ex)
            {
                //todo 
                this.LogModule.Error($"文件未能转化为图片: {args.FileName}  错误: {ex.Message}");
                return;
            }
            this.frmPickFace.SetPicFile(img);
            this.frmPickFace.ShowDialog();
        }

        public void WebSocketRequestProc(WSRequest request)
        {
            switch (request.Module)
            {
                case WSDefine.VideoModule:
                    this.frmMain.Invoke(new Action(() =>
                    this.videoRequesProc.WSRequestProc(request)));
                    //this.videoRequesProc.WSRequestProc(request);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 事件上报处理
        /// </summary>
        /// <param name="wsp"></param>
        public void EventUploadProc(WSProtocol wsp)
        {
            this.WebsocketHelper.SendEvent(wsp);
        }
    }
}
