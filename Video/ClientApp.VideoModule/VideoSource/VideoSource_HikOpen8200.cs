using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.Core.DataModel.Video;

namespace ClientAPP.VideoModule.VideoSource
{
    /// <summary>
    /// 8200开放平台
    /// </summary>
    internal class VideoSource_HikOpen8200:VideoSourceBase
    {
        public override bool StartPreview(CameraInfo camera, VideoSourceInfo videoSourceInfo, VideoControl vc, int StreamIndex = 0)
        {
            return base.StartPreview(camera, videoSourceInfo, vc, StreamIndex);
        }

        public override void StopPreview(VideoControl vc)
        {
            ControlInfo_Preview info = this.m_ControlTable[vc] as ControlInfo_Preview;
            
        }
    }
}
