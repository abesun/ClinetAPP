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
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            return true;
        }

        /// <summary>
        /// 开始实时预览
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="StreamIndex"></param>
        /// <returns></returns>
        public virtual bool StartPreview(CameraInfo camera,  VideoControl vc ,int StreamIndex=0 )
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
        /// <param name="vc"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        public virtual bool StartPlaybackByTime(CameraInfo camera,  VideoControl vc, DateTime Start, DateTime End)
        {
            this.LogModule?.Error("不支持按时间回放");
            return false;
        }

        /// <summary>
        /// 停止回放
        /// </summary>
        /// <param name="vc"></param>
        public virtual bool StopPlayback(VideoControl vc)
        {
            this.LogModule?.Error("不支持关闭回放");
            return false;
        }

        public virtual bool PB_Pause(VideoControl vc)
        {

            this.LogModule?.Error("不支持暂停");
            return false;
        }

        public virtual bool PB_Resume(VideoControl vc)
        {
            this.LogModule?.Error("不支持继续");
            return false;

        }

        public virtual bool PB_Fast(VideoControl vc)
        {

            this.LogModule?.Error("不支持快进");
            return false;
        }

        public virtual bool PB_Slow(VideoControl vc)
        {

            this.LogModule?.Error("不支持慢进");
            return false;
        }

        public virtual bool PB_Step(VideoControl vc)
        {

            this.LogModule?.Error("不支持单帧");
            return false;
        }

        public virtual bool PB_SetPos(VideoControl vc, int pos)
        {

            this.LogModule?.Error("不支持设置进度");
            return false;
        }

        public virtual bool PB_GetPos(VideoControl vc, out int pos)
        {
            pos = 0;
            this.LogModule?.Error("不支持获取进度");
            return false;

        }

        public virtual bool PB_Snap(VideoControl vc, string fileName)
        {
            this.LogModule?.Error("不支持回放截图");
            return false;
        }

        public virtual bool PB_GetCurTime(VideoControl vc, out DateTime dateTime)
        {
            dateTime = default;
            this.LogModule?.Error("不支持获取时间");
            return false;
        }
            

        /// <summary>
        /// 开始按时间下载
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="fileName"></param>
        /// <param name="downloadHandle"></param>
        /// <returns></returns>
        public virtual bool StartDownloadByTime(CameraInfo camera, DateTime Start, DateTime End, string fileName, out string downloadHandle)
        {
            downloadHandle = "";
            this.LogModule?.Error("不支持按时间下载");
            return false;
        }

        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="fileName"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public virtual bool Snap(VideoControl vc, string fileName, string ext = "jpg")
        {
            this.LogModule?.Error("不支持抓图");
            return false;
        }

        public virtual bool OpenSound(VideoControl vc)
        {
            this.LogModule?.Error("不支持打开声音");
            return false;

        }

        public virtual bool CloseSound(VideoControl vc)
        {

            this.LogModule?.Error("不支持关闭声音");
            return false;
        }

        public virtual bool Ptz_DirCamera(VideoControl vc, PTZ.DirDirection dirDirection, int hSpeed, int vSpeed)
        {
            this.LogModule?.Error("不支持云台控制");
            return false;
        }

        public virtual bool Ptz_LenCamera(VideoControl vc, PTZ.LenType lenType)
        {

            this.LogModule?.Error("不支持镜头控制");
            return false;
        }

        public virtual bool PTZ_CameraAutoFindDirection(VideoControl videoControl, int oldWidth, int oldHeight, int newX, int newY, int newWidth, int newHeight)
        {
            this.LogModule?.Error("不支持三维定位");
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

        /// <summary>
        /// 最后一次云台控制得方向
        /// </summary>
        public int LastDirection { get; set; }

        /// <summary>
        /// 最后一次镜头控制
        /// </summary>
        public int LastLenType { get; set; }
    }

    internal class ControlInfo_Playback:ControlInfo
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        /// <summary>
        /// 播放文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 播放速度
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// 是否再运行
        /// </summary>
        public bool Running { get; set; }
    }


}
