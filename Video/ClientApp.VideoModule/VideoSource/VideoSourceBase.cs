using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.Core.DataModel.Video;
using ClientAPP.Core.Contract.Log;
using System.Collections;
namespace ClientAPP.VideoModule.VideoSource
{
    internal class VideoSourceBase
    {
        public IAppLog LogModule { get; set; }

        /// <summary>
        /// 控件信息列表
        /// </summary>
        protected Hashtable m_ControlTable = new Hashtable();

        /// <summary>
        /// 开始实时预览
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="videoSourceInfo"></param>
        /// <param name="StreamIndex"></param>
        /// <returns></returns>
        public virtual bool StartPreview(CameraInfo camera, VideoSourceInfo videoSourceInfo, VideoControl vc ,int StreamIndex=0 )
        {
            this.LogModule?.Error("不支持实时预览");
            return false;
        }

        /// <summary>
        /// 关闭实时预览
        /// </summary>
        /// <param name="vc"></param>
        public virtual void StopPreview(VideoControl vc)
        {
            this.LogModule?.Error("不支持的关闭实时预览");
        }


        /// <summary>
        /// 根据时间回放
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="videoSourceInfo"></param>
        /// <param name="vc"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        public virtual bool StartPlaybackByTime(CameraInfo camera, VideoSourceInfo videoSourceInfo, VideoControl vc, DateTime Start, DateTime End)
        {
            this.LogModule?.Error("不支持按时间回放");
            return false;
        }

        /// <summary>
        /// 停止回放
        /// </summary>
        /// <param name="vc"></param>
        public virtual void StopPlayback(VideoControl vc)
        {
            this.LogModule?.Error("不支持关闭回放");

        }

        /// <summary>
        /// 开始按时间下载
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="videoSourceInfo"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="fileName"></param>
        /// <param name="downloadHandle"></param>
        /// <returns></returns>
        public virtual bool StartDownloadByTime(CameraInfo camera, VideoSourceInfo videoSourceInfo, DateTime Start, DateTime End, string fileName, out string downloadHandle)
        {
            downloadHandle = "";
            this.LogModule?.Error("不支持按时间下载");
            return false;
        }

        public virtual bool Snap(VideoControl vc, string fileName, string ext = "jpg")
        {
            this.LogModule?.Error("不支持抓图");
            return false;
        }
    }

    /// <summary>
    /// 控件相关的播放信息
    /// </summary>
    internal class ControlInfo
    {
        public enum PlayStatus
        {
            Preview=0,
            Playback=1,
            PlayLocalfile=2,
            Stop=10
        }

        public CameraInfo Camera { get; set; }

        public VideoSourceInfo VideoSource { get; set; }

        /// <summary>
        /// 控件
        /// </summary>
        public VideoControl VC { get; set; }

        /// <summary>
        /// 播放句柄
        /// </summary>
        public object PlayHandle { get; set; }

        /// <summary>
        /// 登录句柄
        /// </summary>
        public object LoginHandle { get; set; }

    }

    /// <summary>
    /// 实时预览控件信息
    /// </summary>
    internal class ControlInfo_Preview:ControlInfo
    {
        /// <summary>
        /// 码流编号 
        /// </summary>
        public int StreamIndex { get; set; }

        /// <summary>
        /// 码流大小
        /// </summary>
        public int DataRate { get; set; }
    }

    internal class ControlInfo_Playback:ControlInfo
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        /// <summary>
        /// 播放文件名
        /// </summary>
        public string FileName { get; set; }
    }


}
