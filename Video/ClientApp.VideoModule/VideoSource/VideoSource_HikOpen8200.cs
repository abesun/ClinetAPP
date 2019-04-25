using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.Core.DataModel.Video;
using ClientAPP.API;
using System.Runtime.InteropServices;

namespace ClientAPP.VideoModule.VideoSource
{
    /// <summary>
    /// 8200开放平台
    /// </summary>
    internal class VideoSource_HikOpen8200:VideoSourceBase
    {

        public override bool Init()
        {
            return base.Init();
        }

        #region Preview

        public override bool StartPreview(CameraInfo camera, VideoControl vc, int StreamIndex = 0)
        {

            VideoSourceInfo videoSourceInfo = camera.VideoSourceInfo;
            int loginID = Hik_Open8200API.GetLoginID(videoSourceInfo.IP, (int)videoSourceInfo.Port, videoSourceInfo.User, videoSourceInfo.Password);
            if (loginID == -1)
            {
                string error = Hik_Open8200API.GetLastError();
                this.LogModule.Error($"初始化登录服务器失败, 错误码:{error}, 设备{videoSourceInfo.IP}:{videoSourceInfo.Port} ");
                return false;
            }

            int playHandle = Hik_Open8200API.Std_StartRealPlay(loginID, camera.CameraCode, vc.VControl.Handle, StreamIndex, null, IntPtr.Zero);
            if (playHandle < 0)
            {
                string error = Hik_Open8200API.GetLastError();
                Hik_Open8200API.Std_StopRealPlay(playHandle);
                vc.VControl.Refresh();
                this.LogModule.Error($"打开图像失败: 错误码:{error}, 摄像机标识: {camera.CameraCode}");
                Hik_Open8200API.Std_StopRealPlay(playHandle);
                return false;

            }

            ControlInfo_Preview cm = new ControlInfo_Preview() { LoginHandle = loginID, Camera = camera, PlayHandle = playHandle, VideoSource = videoSourceInfo, VC = vc, StreamIndex = StreamIndex };
            this.m_ControlTable[vc] = cm;
            return true;
        }

        public override void StopPreview(VideoControl vc)
        {
            ControlInfo_Preview info = this.m_ControlTable[vc] as ControlInfo_Preview;
            if (info == null)
                return;
            bool ret = Hik_Open8200API.Std_StopRealPlay((int) info.PlayHandle ) >= 0 ? true : false;
            vc.VControl.Refresh();
            return ;

        }

        public override bool Snap(VideoControl vc, string fileName, string ext = "jpg")
        {
            ControlInfo_Preview info = this.m_ControlTable[vc] as ControlInfo_Preview;
            if (info == null)
                return false;
            if (fileName.EndsWith(".jpg") == true)
                fileName = fileName.Replace(".jpg", "");
            IntPtr pFileName = Hik_Open8200API.GetUTF8StringPtr(fileName);
            int ret = Hik_Open8200API.Std_Capture2((int)info.PlayHandle, pFileName, 0);
            Marshal.FreeHGlobal(pFileName);
            return ret >= 0 ? true : false;
        }

        public override bool OpenSound(VideoControl vc)
        {
            ControlInfo_Preview info = this.m_ControlTable[vc] as ControlInfo_Preview;
            if (info == null)
                return false;

            return Hik_Open8200API.Std_OpenSound((int)info.PlayHandle) >= 0 ? true : false;
        }

        public override bool CloseSound(VideoControl vc)
        {

            ControlInfo_Preview info = this.m_ControlTable[vc] as ControlInfo_Preview;
            if (info == null)
                return false;

            return Hik_Open8200API.Std_CloseSound((int)info.PlayHandle) >= 0 ? true : false;
        }

        public override bool Ptz_DirCamera(VideoControl vc, PTZ.DirDirection dirDirection, int hSpeed, int vSpeed)
        {
            ControlInfo_Preview info = this.m_ControlTable[vc] as ControlInfo_Preview;
            if (info == null)
                return false;

            int hspeed = hSpeed * 64 / 256;
            int vspeed = vSpeed * 64 / 256;
            string ptzCmd = "";
            PTZ.DirDirection d = dirDirection;
            if (d == PTZ.DirDirection.Stop)
                d = (PTZ.DirDirection)info.LastDirection;
            switch (d)
            {
                case PTZ.DirDirection.Left:
                    ptzCmd = "LEFT";
                    break;
                case PTZ.DirDirection.Right:
                    ptzCmd = "RIGHT";
                    break;
                case PTZ.DirDirection.Down:
                    ptzCmd = "DOWN";
                    break;
                case PTZ.DirDirection.Up:
                    ptzCmd = "UP";
                    break;
                case PTZ.DirDirection.LeftUp:
                    ptzCmd = "LEFT_UP";
                    break;
                case PTZ.DirDirection.LeftDown:
                    ptzCmd = "LEFT_DOWN";
                    break;
                case PTZ.DirDirection.RightUp:
                    ptzCmd = "RIGHT_UP";
                    break;
                case PTZ.DirDirection.RightDown:
                    ptzCmd = "RIGHT_DOWN";
                    break;

            }
            if (dirDirection == PTZ.DirDirection.Stop)
                ptzCmd += "_STOP";
            info.LastDirection = (int)dirDirection;

            return Hik_Open8200API.Std_PtzCtrl((int)info.LoginHandle, info.Camera.CameraCode, ptzCmd, hspeed, 0, 0) >= 0 ? true : false;
        }

        public override bool Ptz_LenCamera(VideoControl vc, PTZ.LenType lenType)
        {
            ControlInfo_Preview info = this.m_ControlTable[vc] as ControlInfo_Preview;
            if (info == null)
                return false;


            string ptzCmd = "";
            PTZ.LenType d = lenType;
            if (d == PTZ.LenType.Stop)
                d = (PTZ.LenType)info.LastLenType;
            switch (d)
            {
                case PTZ.LenType.LensTele:
                    ptzCmd = "ZOOMIN";
                    break;
                case PTZ.LenType.LensWide:
                    ptzCmd = "ZOOMOUT";
                    break;
                default:
                    break;

            }
            if (lenType == PTZ.LenType.Stop)
                ptzCmd += "_STOP";
            info.LastLenType = (int)lenType;

            return Hik_Open8200API.Std_PtzCtrl((int)info.LoginHandle, info.Camera.CameraCode, ptzCmd, 49, 0, 0) >= 0 ? true : false;
        }

        public override bool PTZ_CameraAutoFindDirection(VideoControl vc, int oldWidth, int oldHeight, int newX, int newY, int newWidth, int newHeight)
        {
            ControlInfo_Preview info = this.m_ControlTable[vc] as ControlInfo_Preview;
            if (info == null)
                return false;
            int height = 288, width = 352;
            //HikPlayAPI.PlayM4_GetPictureSize(cm.PlayHandle, ref width, ref height);
            int xTop = width * newX / oldWidth;
            int yTop = height * newY / oldHeight;
            int xBottom = width * (newX + newWidth) / oldWidth;
            int yBottom = height * (newY + newHeight) / oldHeight;
            Hik_Open8200API.Std_PtzCtrl3D((int)info.LoginHandle, info.Camera.CameraCode, xTop, yTop, xBottom, yBottom, 4);
            return true;
        }

        #endregion

        #region playback

        public override bool StartPlaybackByTime(CameraInfo camera,  VideoControl vc, DateTime Start, DateTime End)
        {
            VideoSourceInfo videoSourceInfo = camera.VideoSourceInfo;
            int loginID = Hik_Open8200API.GetLoginID(videoSourceInfo.IP, (int)videoSourceInfo.Port, videoSourceInfo.User, videoSourceInfo.Password);
            if (loginID < 0)
            {
                string error = Hik_Open8200API.GetLastError();
                vc.ErrorMessage = $"初始化登录服务器失败, 错误码:{error}, 设备{videoSourceInfo.IP}:{videoSourceInfo.Port} ";
                this.LogModule.Error(vc.ErrorMessage);
                return false;
            }
            int playHandle = Hik_Open8200API.Std_StreamReplayByTime(loginID, camera.CameraCode, this.getTimeString(Start), this.getTimeString(End), 0, vc.VControl.Handle, null, IntPtr.Zero);
            if (playHandle < 0)
            {
                string error = Hik_Open8200API.GetLastError();
                vc.ErrorMessage = $"回放图像失败: 错误码:{error}, 摄像机标识: {camera.CameraCode}";
                this.LogModule.Error(vc.ErrorMessage);
                return false;

            }

            ControlInfo_Playback cm = new ControlInfo_Playback()
            {
                Camera = camera,
                VideoSource = videoSourceInfo,
                Start = Start,
                End = End,
                LoginHandle = loginID,
                PlayHandle = playHandle,
                VC = vc, Speed = (int)Hik_Open8200API.ReplayMode.PLAYMODE_1_FORWARD, Running = true
            };      

            this.m_ControlTable[vc] = cm;
            return true;
        }

        public override bool StopPlayback(VideoControl vc)
        {
            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;
            Hik_Open8200API.Std_StopStreamReplay((int)cm.PlayHandle);
            this.m_ControlTable[vc] = null;
            cm.Running = false;
            return true;
        }

        public override bool PB_Pause(VideoControl vc)
        {
            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;
            int ret = Hik_Open8200API.Std_StreamReplayControl((int)cm.PlayHandle, Hik_Open8200API.ReplayMode.PLAYMODE_PAUSE);
            cm.Running = false;
            return ret >= 0 ? true : false;
        }

        public override bool PB_Resume(VideoControl vc)
        {
            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;
            int ret = Hik_Open8200API.Std_StreamReplayControl((int)cm.PlayHandle, Hik_Open8200API.ReplayMode.PLAYMODE_RESUME);
            cm.Running = false;
            return ret >= 0 ? true : false;
        }

        public override bool PB_Fast(VideoControl vc)
        {
            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;
            Hik_Open8200API.ReplayMode newMode = Hik_Open8200API.ReplayMode.PLAYMODE_1_FORWARD;
            switch ((Hik_Open8200API.ReplayMode)cm.Speed)
            {
                case Hik_Open8200API.ReplayMode.PLAYMODE_1_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_2_FORWARD;
                    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_2_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_4_FORWARD;
                    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_4_FORWARD:
                case Hik_Open8200API.ReplayMode.PLAYMODE_8_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_8_FORWARD;
                    break;
                //case Hik_Open8200API.ReplayMode.PLAYMODE_8_FORWARD:
                //case Hik_Open8200API.ReplayMode.PLAYMODE_16_FORWARD:
                //    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_16_FORWARD;
                //    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_Eighth_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_QUARTER_FORWARD;
                    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_QUARTER_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_HALF_FORWARD;
                    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_HALF_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_1_FORWARD;
                    break;
            }
            int ret = Hik_Open8200API.Std_StreamReplayControl((int)cm.PlayHandle, newMode);
            cm.Speed = (int)newMode;
            return ret >= 0 ? true : false;
        }

        public override bool PB_Slow(VideoControl vc)
        {
            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;
            Hik_Open8200API.ReplayMode newMode = Hik_Open8200API.ReplayMode.PLAYMODE_1_FORWARD;
            switch ((Hik_Open8200API.ReplayMode)cm.Speed)
            {
                case Hik_Open8200API.ReplayMode.PLAYMODE_1_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_HALF_FORWARD;
                    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_2_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_1_FORWARD;
                    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_4_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_2_FORWARD;
                    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_8_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_4_FORWARD;
                    break;
                //case Hik_Open8200API.ReplayMode.PLAYMODE_16_FORWARD:
                //    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_8_FORWARD;
                //    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_Eighth_FORWARD:
                case Hik_Open8200API.ReplayMode.PLAYMODE_QUARTER_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_QUARTER_FORWARD;
                    break;
                case Hik_Open8200API.ReplayMode.PLAYMODE_HALF_FORWARD:
                    newMode = Hik_Open8200API.ReplayMode.PLAYMODE_QUARTER_FORWARD;
                    break;
            }
            int ret = Hik_Open8200API.Std_StreamReplayControl((int)cm.PlayHandle, newMode);
            cm.Speed = (int)newMode;
            return ret >= 0 ? true : false;
        }

        public override bool PB_Step(VideoControl vc)
        {
            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;
            int ret = Hik_Open8200API.Std_StreamReplayControl((int)cm.PlayHandle, Hik_Open8200API.ReplayMode.PLAYMODE_ONEBYONE);
            return ret >= 0 ? true : false;
        }

        public override bool PB_SetPos(VideoControl vc, int pos)
        {

            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;

            DateTime time = new DateTime();
            long allTick = cm.End.Ticks - cm.Start.Ticks;
            time = cm.Start + new TimeSpan(allTick * pos / 100);

            int seconds = (int)((time - cm.Start).TotalSeconds);
            int ret = Hik_Open8200API.Std_SetStreamReplayPos((int)cm.PlayHandle, seconds);
            return ret >= 0 ? true : false;
        }

        public override bool PB_GetPos(VideoControl vc, out int pos)
        {
            pos = default;
            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;
            int seconds = 0;
            int ret = Hik_Open8200API.Std_GetFileReplayPos((int)cm.PlayHandle, ref seconds);
            if (ret < 0)
                return false;
            pos= (int)(seconds * 100 / (cm.End - cm.Start).TotalSeconds);
            return true;
        }

        public override bool PB_GetCurTime(VideoControl vc, out DateTime dateTime)
        {
            dateTime = default;
            ControlInfo_Playback cm = this.m_ControlTable[vc] as ControlInfo_Playback;
            if (cm == null)
                return false;
            dateTime = cm.Start;
            int seconds = 0;
            int ret = Hik_Open8200API.Std_GetFileReplayPos((int)cm.PlayHandle, ref seconds);
            if (ret < 0)
                return false;
            dateTime = cm.Start.AddSeconds(seconds);
            return true;
        }

        private string getTimeString(DateTime time)
        {
            return time.ToString("yyyyMMddTHHmmssZ");
        }
        #endregion
    }
}
