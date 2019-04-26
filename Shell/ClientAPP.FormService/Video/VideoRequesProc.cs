using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.Core.Contract.Websocket;
using Newtonsoft.Json;
using ClientAPP.Core.Contract.Log;
using ClientAPP.VideoModule;
using System.Windows.Forms;
using System.Collections;
using ClientAPP.Core.DataModel.Video;
namespace ClientAPP.FormService.Video
{
    /// <summary>
    /// 视频模块
    /// </summary>
    public class VideoRequesProc
    {
        /// <summary>
        /// 日志
        /// </summary>
        public IAppLog LogModule { get; set; }

        /// <summary>
        /// 视频管理器
        /// </summary>
        public VideoManager VideoManager { get; set; }


        /// <summary>
        /// 保存视频面板
        /// </summary>
        private Dictionary<string, FrmVideo> m_VideoFrmList = new Dictionary<string, FrmVideo>();
            

        /// <summary>
        /// Websocket命令请求处理
        /// </summary>
        /// <param name="request"></param>
        public void WSRequestProc(WSRequest request)
        {
            switch (request.Command)
            {
                case WSVideoRequest.OpenWindow:
                    this.procVideo_OpenWindow(request);
                    break;
                case WSVideoRequest.CloseWindow:
                    break;
                case WSVideoRequest.StartPreview:
                    this.procVideo_StartPreview(request);
                    break;
                case WSVideoRequest.StopPreview:
                    break;
                case WSVideoRequest.StartPlayback:
                    this.procVideo_StartPlayback(request);
                    break;
                case WSVideoRequest.StopPlayback:
                    break;
                default:
                    this.LogModule.Error($"不支持的视频指令: {request.Command}");
                    break;


            }
        }

   



        /// <summary>
        /// 打开视频窗口
        /// </summary>
        /// <param name="request"></param>
        private void procVideo_OpenWindow(WSRequest request)
        {
            var req = JsonConvert.DeserializeObject<WSVideoRequest_OpenWindow>(request.Params.ToString());



            FrmVideo frmVideo = this.getVideoFormByID(req.PanelID);

            int screenid = req.ScreenID ?? 0;
            if (screenid > Screen.AllScreens.Length)
                screenid = 0;
            var screen = Screen.AllScreens[screenid];

            frmVideo.Width = req.Width??frmVideo.Width;
            frmVideo.Height = req.Height?? frmVideo.Height;
            frmVideo.Location = new System.Drawing.Point(req.LocationX?? frmVideo.Location.X, req.LocationY?? frmVideo.Location.Y);
            frmVideo.TopMost = req.TopMost??true;
            frmVideo.FormBorderStyle = req.ShowWindowBorder == true ? FormBorderStyle.Sizable : FormBorderStyle.None;

            this.VideoManager.SetLayout(req.PanelID, req.LayoutName);

            frmVideo.WindowState = FormWindowState.Normal;
            frmVideo.Show();

        }

        private void procVideo_CloseWindow(WSRequest request)
        {

            var req = JsonConvert.DeserializeObject<WSVideoRequest>(request.Params.ToString());


            FrmVideo frmVideo = this.getVideoFormByID(req.PanelID);

            frmVideo.Hide();
        }

        private void procVideo_StartPreview(WSRequest request)
        {
            var req = JsonConvert.DeserializeObject<WSVideoRequest_StartPreview>(request.Params.ToString());
            VideoSourceInfo videoSourceInfo = new VideoSourceInfo()
            {
                ID = req.SourceID,
                IP = req.SourceIP,
                Port = req.SourcePort,
                User = req.SourceUser,
                Password = req.SourcePassword,
                Type = (VideoSourceType)req.SourceType,
                Name = req.SourceName
            };

            CameraInfo cameraInfo = new CameraInfo()
            {
                ID = req.CameraID,
                CameraCode = req.CameraCode,
                Name = req.CameraName
            };

            VideoControl vc = this.VideoManager.GetVideoControl(req.PanelID, req.VCIndex);
            if(vc==null)
            {
                this.LogModule.Error("找不到指定控件");
                return;
            }
            this.VideoManager.StartPreview(cameraInfo, videoSourceInfo, vc, req.StreamIndex);
        }

        private void procVideo_Stop(WSRequest request)
        {
            var req = JsonConvert.DeserializeObject<WSVideoRequest_StopVideo>(request.Params.ToString());

            VideoControl vc = this.VideoManager.GetVideoControl(req.PanelID, req.VCIndex);
            if (vc == null)
            {
                this.LogModule.Error("找不到指定控件");
                return;
            }
            this.VideoManager.StopVideo(vc);
        }

        private void procVideo_StartPlayback(WSRequest request)
        {
            var req = JsonConvert.DeserializeObject<WSVideoRequest_StartPlayback>(request.Params.ToString());

            VideoSourceInfo videoSourceInfo = new VideoSourceInfo()
            {
                ID = req.SourceID,
                IP = req.SourceIP,
                Port = req.SourcePort,
                User = req.SourceUser,
                Password = req.SourcePassword,
                Type = (VideoSourceType)req.SourceType,
                Name = req.SourceName
            };

            CameraInfo cameraInfo = new CameraInfo()
            {
                ID = req.CameraID,
                CameraCode = req.CameraCode,
                Name = req.CameraName
            };

            VideoControl vc = this.VideoManager.GetVideoControl(req.PanelID, req.VCIndex);
            if (vc == null)
            {
                this.LogModule.Error("找不到指定控件");
                return;
            }
            this.VideoManager.StartPlayback(cameraInfo, videoSourceInfo, vc, req.StartTime, req.EndTime);
            
        }



        #region 内部方法
        /// <summary>
        /// 获取指定的窗口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private FrmVideo getVideoFormByID(string id)
        {
            if (this.m_VideoFrmList.ContainsKey(id))
                return this.m_VideoFrmList[id];
            var panel = this.VideoManager.CreateVideoPanel(id);
            FrmVideo ret = new FrmVideo();
            ret.Show();
            ret.SetVideoPanel(panel);
            this.m_VideoFrmList.Add(id, ret);
            return ret;
        }
        #endregion

    }
}
