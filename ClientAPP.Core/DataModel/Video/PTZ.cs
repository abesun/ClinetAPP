using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Core.DataModel.Video
{
    public class PTZ
    {
        /// <summary>
        /// 方向
        /// </summary>
        public enum DirDirection
        {
            Left = 0,
            Up = 1,
            Right = 2,
            Down = 3,
            LeftUp = 4,
            LeftDown = 5,
            RightUp = 6,
            RightDown = 7,
            Stop = 8
        }

        /// <summary>
        /// 镜头
        /// </summary>
        public enum LenType
        {
            IrisOpen = 0,
            IrisClose = 1,
            LensTele = 2,
            LensWide = 3,
            FocusFar = 4,
            FocusNear = 5,
            Stop = 6
        }
    }
}
