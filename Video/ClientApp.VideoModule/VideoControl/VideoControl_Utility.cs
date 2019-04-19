using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using ClientAPP.Uti;
using ClientAPP.VideoModule.Config;
using ClientAPP.Core.DataModel.Video;
namespace ClientAPP.VideoModule
{
    public partial class VideoControl 
    {
        /// <summary>
        /// 将图片进行反色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        /// <returns>被反色后的图片</returns>
        public Bitmap RePic(Bitmap mybm, int width, int height)
        {
            Bitmap bm = new Bitmap(width, height);//初始化一个记录处理后的图片的对象
            int x, y, resultR, resultG, resultB;
            Color pixel;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    resultR = 255 - pixel.R;//反红
                    resultG = 255 - pixel.G;//反绿
                    resultB = 255 - pixel.B;//反蓝
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));//绘图
                }
            }
            return bm;//返回经过反色处理后的图片
        }

        /// <summary>
        /// 根据配置文件设置视频面板
        /// </summary>
        /// <param name="config"></param>
        public void SetVideoControlConfig(VideoControlConfig config)
        {
            if (config != null)
            {
                this.ShowTitle = !config.HideVCTitle;
                this.AutoHideTitle = config.AutoHideVCTitle;

                if (string.IsNullOrEmpty(config.BackColorRGB) == false)
                {
                    this.VCDefaultBackColor = ColorHelper.colorHx16toRGB(config.BackColorRGB);

                }
                if (string.IsNullOrEmpty(config.SelectedColorRGB) == false)
                {
                    this.BackgroundColor = ColorHelper.colorHx16toRGB(config.SelectedColorRGB);
                }
                if (string.IsNullOrEmpty(config.VideoPanelColorRGB) == false)
                {
                    this.VControl.BackColor = ColorHelper.colorHx16toRGB(config.VideoPanelColorRGB);
                    Color color = this.panel_Video.BackColor;
                    Color newColor = Color.FromArgb(~color.ToArgb());
                    this.label_Error.ForeColor = newColor;
                }
                this.VideoPanelMargin = config.Margin;
            }
        }

    }

   
    /// <summary>
    /// 视频控件动作事件参数
    /// </summary>
    public class VideoControlActionEventArgs : EventArgs
    {
        /// <summary>
        /// 摄像机
        /// </summary>
        public CameraInfo Camera { get; set; }
        /// <summary>
        /// 控件
        /// </summary>
        public VideoControl VControl { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public VideoControlAction Type { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public object Arg { get; set; }

        /// <summary>
        /// 码流 0-主码流 1-辅码流
        /// </summary>
        public int StreamIndex { get; set; }


    }

    /// <summary>
    /// 视频控件事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void VideoControlEventHandle(object sender, VideoControlActionEventArgs args);

    


    public enum ShowMode
    {
        /// <summary>
        /// 实时视频
        /// </summary>
        Real = 0,
        /// <summary>
        /// 回放
        /// </summary>
        Playback = 1,
        /// <summary>
        /// 播放文件
        /// </summary>
        PlayFile = 2
    }

  

    public enum VideoControlAction
    {
        /// <summary>
        /// 关闭
        /// </summary>
        CloseVideo = 1,
        /// <summary>
        /// 选中
        /// </summary>
        Seleced = 2,
        /// <summary>
        /// 全屏
        /// </summary>
        FullScreen = 10,
        /// <summary>
        /// 恢复普通
        /// </summary>
        NormalScreen = 11,
        /// <summary>
        /// 锁定
        /// </summary>
        Locked = 12,
        /// <summary>
        /// 解锁
        /// </summary>
        UnLocked = 13,
        /// <summary>
        /// 错误报告
        /// </summary>
        ErrorReport = 14,
        /// <summary>
        /// 打开声音
        /// </summary>
        OpenSound = 20,
        /// <summary>
        /// 关闭声音
        /// </summary>
        CloseSound = 21,
        /// <summary>
        /// 抓图
        /// </summary>
        Snap = 30,
        /// <summary>
        /// 开始录像
        /// </summary>
        StartRecord = 40,
        /// <summary>
        /// 停止录像
        /// </summary>
        StopRecord = 41,
        /// <summary>
        /// 快速回放按钮
        /// </summary>
        FastPlayback=50,

        /// <summary>
        /// 打开实时视频
        /// </summary>
        Real_Play = 81,
        /// <summary>
        /// 关闭实时视频
        /// </summary>
        Real_Stop = 82,

        /// <summary>
        /// 播放
        /// </summary>
        PB_Play = 101,
        /// <summary>
        /// 停止
        /// </summary>
        PB_Stop = 102,
        /// <summary>
        /// 暂停
        /// </summary>
        PB_Pause = 103,
        /// <summary>
        /// 继续
        /// </summary>
        PB_Resume = 104,
        /// <summary>
        /// 快进
        /// </summary>
        PB_Fast = 105,
        /// <summary>
        /// 慢进
        /// </summary>
        PB_Slow = 106,
        /// <summary>
        /// 单帧进
        /// </summary>
        PB_Step = 107,
        /// <summary>
        /// 单帧退
        /// </summary>
        PB_StepBack = 108,
        /// <summary>
        /// 获得进度
        /// </summary>
        PB_GetPos = 109,
        /// <summary>
        /// 设置进度(0-100%)
        /// </summary>
        PB_SetPos = 110,
        /// <summary>
        /// 获得当前播放时间
        /// </summary>
        PB_GetPos_Time = 111,
        /// <summary>
        /// 设置播放时间
        /// </summary>
        PB_SetPos_Time = 112,
        /// <summary>
        /// 开始下载录像
        /// </summary>
        PB_StratDownload = 113,
        /// <summary>
        /// 停止下载录像
        /// </summary>
        PB__StopDownload = 114,

        /// <summary>
        /// 标题栏按下
        /// </summary>
        Title_MouseDown = 201,

        /// <summary>
        /// 标题栏松开
        /// </summary>
        Title_MouseUp = 202,
        /// <summary>
        /// 标题栏拖动
        /// </summary>
        Title_MouseMove = 203,
        /// <summary>
        /// 交换控件位置
        /// </summary>
        Transposition = 204,
        /// <summary>
        /// 退出键
        /// </summary>
        Escape = 301,


        /// <summary>
        /// 未知
        /// </summary>
        Unknown = -1
    }


    

    /// <summary>
    /// 回放信息
    /// </summary>
    public class PlaybackInfo
    {

        /// <summary>
        /// 摄像机信息
        /// </summary>
        public CameraInfo Camera { get; set; }

        /// <summary>
        /// 播放速度（-16 - 16）
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End { get; set; }
    }
}