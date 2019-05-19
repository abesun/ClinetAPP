using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadFacePic
{
    public class FaceDetectEventArgs:EventArgs
    {
        /// <summary>
        /// 人脸图片
        /// </summary>
        public Image FaceImage { get; set; }
    }

    public delegate void FaceDetectEventHandle(object sender, FaceDetectEventArgs args);
}
