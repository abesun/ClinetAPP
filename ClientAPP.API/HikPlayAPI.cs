using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.API
{

    /// <summary>
    /// 海康播放API函数
    /// </summary>
    public class HikPlayAPI
    {

        private const string DLLName = "\\DLL\\HIK\\PlayCtrl.dll";

        #region 结构体
        /// <summary>
        /// 区域
        /// </summary>
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        };

        #endregion

        /// <summary>
        /// 抓图回调函数
        /// nPort 通道号。
        /// pBuf 返回图像数据；
        /// nSize 返回图像数据大小。
        /// nWidth 画面宽，单位像素。
        /// nHeigh 画面高。
        /// nStamp 时标信息，单位毫秒。
        /// nType 数据类型， T_YV12，T_RGB32，T_UYVY，详见宏定义说明。
        /// nReceaved 保留。
        /// </summary>
        public delegate void DisplayCBFun(int nPort, IntPtr pBuf, int nSize, int nWidth, int nHeight, int nStamp, int nType, int nReceaved);
        /// <summary>
        /// 画图回调函数
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="hDc">句柄</param>
        /// <param name="nUser">用户数据</param>
        public delegate void DrawFun(int nPort, IntPtr hDc, int nUser);
        /// <summary>
        /// 打开流
        /// 返回-1表示失败，其他值表示成功
        /// </summary>
        /// <param name="nPort">端口号</param>
        /// <param name="pFileHeadBuf">用户从卡上得到文件头数据</param>
        /// <param name="nSize">文件头长度</param>
        /// <param name="nBufPoolSize">设置播放器中存放数据流的缓冲区大小</param>
        /// <returns>返回-1表示失败，其他值表示成功</returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_OpenStream(int nPort, byte[] pFileHeadBuf, UInt32 nSize, UInt32 nBufPoolSize);

        /// <summary>
        /// 播放文件
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="sFileName">文件名</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_OpenFile(int nPort, string sFileName);
        /// <summary>
        /// 关闭文件
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_CloseFile(int nPort);
        /// <summary>
        /// 开始播放
        /// 返回-1表示失败，其他值表示成功
        /// </summary>
        /// <param name="nPort">端口号</param>
        /// <param name="hWnd">播放容器句柄</param>
        /// <returns>返回-1表示失败，其他值表示成功</returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_Play(int nPort, IntPtr hWnd);

        /// <summary>
        /// 初始化DirectDraw
        /// 返回-1表示失败，其他值表示成功
        /// </summary>
        /// <param name="IntPtr">播放容器句柄</param>
        /// <returns>返回-1表示失败，其他值表示成功</returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_InitDDraw(IntPtr IntPtr);

        /// <summary>
        /// 输入流
        /// </summary>
        /// <param name="nPort">端口号</param>
        /// <param name="pBuf">流数据</param>
        /// <param name="nSize">流数据长度</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_InputData(int nPort, byte[] pBuf, UInt32 nSize);

        /// <summary>
        /// 停止播放
        /// 返回-1表示失败，其他值表示成功
        /// </summary>
        /// <param name="nPort">端口号</param>
        /// <returns>返回-1表示失败，其他值表示成功，不确定，原文档没有</returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_Stop(int nPort);

        /// <summary>
        /// 设置流播放的模式（实时模式或文件模式）
        /// 必须在播放之前设置
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="nMode">模式，0为实时模式，1为文件模式</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetStreamOpenMode(int nPort, int nMode);
        /// <summary>
        /// 注册一个回调函数，
        /// 获得当前表面的device context, 
        /// 你可以在这个DC上画图（或写字），
        /// 就好像在窗口的客户区DC上绘图，
        /// 但这个DC不是窗口客户区的DC，
        /// 而是DirectDraw里的Off-Screen表面的DC。
        /// 注意，如果是使用overlay表面，这个接口无效，你可以直接在窗口上绘图，只要不是透明色就不会被覆盖。
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="DrawFun">回调函数句柄</param>
        /// <param name="nUser">用户数据</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_RigisterDrawFun(int nPort, DrawFun DrawFun, int nUser);

        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="DisplayCBFun">读取图片数据委托</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDisplayCallBack(int nPort, DisplayCBFun DisplayCBFun);
        /// <summary>
        /// 抓图存为JPEG文件
        /// 该函数可在显示回调函数中使用
        /// </summary>
        /// <param name="pBuf">图像数据缓存</param>
        /// <param name="nSize">图像大小</param>
        /// <param name="nWidth">图像宽</param>
        /// <param name="nHeight">图像高</param>
        /// <param name="nType">图像类型YV12</param>
        /// <param name="sFileName">保存jpeg文件路径</param>
        /// <returns>是否成功</returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_ConvertToJpegFile(byte[] pBuf, int nSize, int nWidth, int nHeight, int nType, string sFileName);
        /// <summary>
        /// 关闭数据流
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <returns>是否成功</returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_CloseStream(int nPort);

        /// <summary>
        /// 清空所有缓冲区
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_ResetSourceBuffer(int nPort);

        /// <summary>
        /// 设置播放器使用的定时器；注意：必须在Open之前调用
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="nTimerType">默认情况下0~15路使用TIMER_1，其余使用TIMER_2</param>
        /// <param name="nReserved">保留</param>
        /// <returns>是否成功</returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetTimerType(int nPort, int nTimerType, int nReserved);

        public const int Timer1 = 1;
        public const int Timer2 = 2;

        /// <summary>
        /// 获得当前通道使用的定时器
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="pTimerType">TIMER_1或TIMER_2</param>
        /// <param name="pReserved">保留</param>
        /// <returns>是否成功</returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_GetTimerType(int nPort, out int pTimerType, out int pReserved);


        /// <summary>
        /// PlayM4_SetDisplayBuf
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="nNum"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDisplayBuf(int nPort, int nNum);


        /// <summary>
        /// 设置图像质量
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="bHighQuality">等于1时图像高质量，等于0时低质量，默认为1 </param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetPicQuality(int nPort, bool bHighQuality);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern int PlayM4_GetCaps();

        /// <summary>
        /// 去闪烁
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="bDeflash">true表示去闪烁，默认不去</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDeflash(int nPort, bool bDeflash);

        /// <summary>
        /// 获取可用的通道号
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_GetPort(ref int nPort);

        /// <summary>
        /// 快速播放，每次调用将使当前播放速度加快一倍，最多调用4次；要恢复正常播放调用PlayM4_Play（）,从当前位置开始正常播放；
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_Fast(int nPort);

        /// <summary>
        /// 慢速播放，每次调用将使当前播放速度慢一倍；最多调用4次；要恢复正常播放调用PlayM4_Play（）
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_Slow(int nPort);

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="nPort">端口号</param>
        /// <param name="nPause">nPause=TRUE暂停；否则恢复</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_Pause(int nPort, bool nPause);

        /// <summary>
        /// 得到当前播放的帧序号
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern UInt32 PlayM4_GetCurrentFrameNum(int nPort);

        /// <summary>
        /// 设置当前播放播放位置到指定帧号
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="nFrameNum"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetCurrentFrameNum(int nPort, UInt32 nFrameNum);
        /// <summary>
        /// 单帧进
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_OneByOne(int nPort);


        /// <summary>
        /// 设置不解码B帧帧数；不解码B帧，可以减小CPU利用率，如果码流中没有B帧，那么设置这个值将不会有作用。如在快速播放，和支持多路而CPU利用率太高的情况下可以考虑使用。
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="nNum">nNum 不解码B帧的帧数。对于我们的DS-400XM系列板卡采集的文件，nNum范围是0~2;</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_ThrowBFrameNum(int nPort, int nNum);

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_PlaySound(int nPort);

        /// <summary>
        /// 停止声音
        /// </summary>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_StopSound();

        /// <summary>
        /// 获得错误
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern int PlayM4_GetLastError(int nPort);


        [DllImport(DLLName)]
        public static extern bool PlayM4_FreePort(int nPort);


        /// <summary>
        /// 设置文件播放指针的相对位置（百分比）。
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <param name="fRelativePos">fRelativePos 范围0-100%；</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetPlayPos(int nPort, float fRelativePos);

        /// <summary>
        /// 获得文件播放指针的相对位置；
        /// </summary>
        /// <param name="nPort">通道号</param>
        /// <returns>范围0-100%</returns>
        [DllImport(DLLName)]
        public static extern float PlayM4_GetPlayPos(int nPort);

        /// <summary>
        /// 刷新显示
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_RefreshPlay(int nPort);

        /// <summary>
        /// 获得缓冲区剩余控件
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern int PlayM4_GetSourceBufferRemain(int nPort);

        /// <summary>
        /// 抓图成bmp
        /// </summary>
        /// <param name="playHandle">播放句柄</param>
        /// <param name="data">缓冲区</param>
        /// <param name="dataLength">缓冲区长度</param>
        /// <param name="length">实际长度</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_GetBMP(int playHandle, byte[] data, int dataLength, ref int length);

        /// <summary>
        /// 抓图成jpg
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="pJpeg"></param>
        /// <param name="nBufSize"></param>
        /// <param name="pJpegSize"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_GetJPEG(int nPort, IntPtr pJpeg, int nBufSize, ref int pJpegSize);

        /// <summary>
        /// 设置文件回放结束回调
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="feCallback"></param>
        /// <param name="pUser"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetFileEndCallback(int nPort, FileEndCallback feCallback, IntPtr pUser);

        /// <summary>
        /// 文件结束回调
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="pUser"></param>
        public delegate void FileEndCallback(int nPort, IntPtr pUser);

        /// <summary>
        /// 文件当前播放的时间，单位毫秒
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern int PlayM4_GetPlayedTimeEx(int nPort);

        /// <summary>
        /// 根据时间设置文件播放位置
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="nTime"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetPlayedTimeEx(int nPort, int nTime);

        /// <summary>
        /// 设置解码回调流类型
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nStream">1 视频流，2 音频流，3 复合流</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDecCBStream(int nPort, int nStream);

        /// <summary>
        /// 设置视频帧解码类型
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nFrameType">0 正常解码, 1 只解关键帧,  2 不解视频帧</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDecodeFrameType(int nPort, int nFrameType);

        /// <summary>
        /// 设置解码回调
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="DataCallback"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDecCallBack(int nPort, DecCBFun DataCallback);

        /// <summary>
        /// 解码回调（带目标数据和数据大小和用户指针）
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="DecCBFun">解码回调函数，若不需要回调函数则置为NULL,否则不能为NULL</param>
        /// <param name="pDest">目标数据</param>
        /// <param name="nDestSize">目标数据大小</param>
        /// <param name="nUser">用户指针</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDecCallBackExMend(int nPort, DecCBFun DecCBFun, IntPtr pDest, int nDestSize, int nUser);

        /// <summary>
        /// 解码回调
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="pBuf"></param>
        /// <param name="nSize"></param>
        /// <param name="pFrameInfo"></param>
        /// <param name="nReserved1"></param>
        /// <param name="nReserved2"></param>
        public delegate void DecCBFun(int nPort, IntPtr pBuf, int nSize, ref FRAME_INFO pFrameInfo, int nReserved1, int nReserved2);

        /// <summary>
        /// 获得当前显示帧的时间
        /// </summary>
        /// <param name="nPort"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern int PlayM4_GetSpecialData(int nPort);

        /// <summary>
        /// 获得当前显示帧的时间
        /// </summary>
        /// <param name="nPort"></param>
        /// <param name="pstSystemTime"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_GetSystemTime(int nPort, ref PLAYM4_SYSTEM_TIME pstSystemTime);

        /// <summary>
        /// 设置关闭或开启模块
        /// </summary>
        /// <param name="lPort">播放通道</param>
        /// <param name="nModuFlag">图像增强模块标记，nModuFlag，对应PLAYM4_VIE_MODULES 宏,可组合</param>
        /// <param name="bEnable">是否开启，TRUE 为开启；FALSE 为关闭</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_VIE_SetModuConfig(int lPort, PLAYM4_VIE_MODULES nModuFlag, bool bEnable);

        /// <summary>
        /// 获取开启图像增强模块
        /// </summary>
        /// <param name="lPort"></param>
        /// <param name="pdwModuFlag"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_VIE_GetModuConfig(int lPort, ref PLAYM4_VIE_MODULES pdwModuFlag);

        /// <summary>
        /// 获取开启模块的参数。
        /// </summary>
        /// <param name="lPort"></param>
        /// <param name="pParaConfig"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_VIE_GetParaConfig(int lPort, ref PLAYM4_VIE_PARACONFIG pParaConfig);

        /// <summary>
        /// 设置图像增强区域
        /// </summary>
        /// <param name="lPort">播放通道号</param>
        /// <param name="lRegNum">图像增强区域个数</param>
        /// <param name="pRect">图像增强区域</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_VIE_SetRegion(int lPort, int lRegNum, ref RECT pRect);

        /// <summary>
        /// 设置显示模式
        /// </summary>
        /// <param name="nPort">播放通道号 </param>
        /// <param name="nType">1： 正常分辨率数据送显卡显示  2： 1/4分辨率数据送显卡显示</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDisplayType(int nPort, int nType);

        /// <summary>
        /// 设置或增加显示区域。
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="nRegionNum">显示区域序号，0~(MAX_DISPLAY_WND-1)。如果nRegionNum为0，表示对主要显示窗口（PlayM4_Play中设置的窗口）进行设置，将忽略hDestWnd和bEnable的设置</param>
        /// <param name="pSrcRect">设置在要显示的原始图像上的区域，如：如果原始图像是352*288，那么pSrcRect可设置的范围只能在（0，0，352，288）之中。如果pSrcRect=NULL,将显示整个图像</param>
        /// <param name="hDestWnd">设置显示窗口。如果该区域的窗口已经设置过（打开过），那么该参数被忽略</param>
        /// <param name="bEnable">打开（设置）或关闭显示区域 </param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_SetDisplayRegion(int nPort, int nRegionNum, ref RECT pSrcRect, IntPtr hDestWnd, bool bEnable);

        /// <summary>
        /// 获取原始图像大小
        /// </summary>
        /// <param name="nPort">播放通道号</param>
        /// <param name="pWidth">原始图像的宽度</param>
        /// <param name="pHeight">原始图像的高度</param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_GetPictureSize(int nPort, ref int pWidth, ref int pHeight);

        /// <summary>
        /// 图像模块名
        /// </summary>
        public enum PLAYM4_VIE_MODULES
        {
            /// <summary>
            /// 图像基本调节
            /// </summary>
            PLAYM4_VIE_MODU_ADJ = 0x00000001,
            /// <summary>
            /// 局部增强模块
            /// </summary>
            PLAYM4_VIE_MODU_EHAN = 0x00000002,
            /// <summary>
            /// 去雾模块
            /// </summary>
            PLAYM4_VIE_MODU_DEHAZE = 0x00000004,
            /// <summary>
            /// 去噪模块
            /// </summary>
            PLAYM4_VIE_MODU_DENOISE = 0x00000008,
            /// <summary>
            /// 锐化模块
            /// </summary>
            PLAYM4_VIE_MODU_SHARPEN = 0x00000010,
            /// <summary>
            /// 去块滤波模块
            /// </summary>
            PLAYM4_VIE_MODU_DEBLOCK = 0x00000020,
            /// <summary>
            /// 色彩平衡模块
            /// </summary>
            PLAYM4_VIE_MODU_CRB = 0x00000040,
            /// <summary>
            /// 镜头畸变矫正模块
            /// </summary>
            PLAYM4_VIE_MODU_LENS = 0x00000080,
        }

        /// <summary>
        /// 设置增强参数
        /// </summary>
        /// <param name="lPort"></param>
        /// <param name="pParaConfig"></param>
        /// <returns></returns>
        [DllImport(DLLName)]
        public static extern bool PlayM4_VIE_SetParaConfig(int lPort, ref PLAYM4_VIE_PARACONFIG pParaConfig);

        /// <summary>
        /// 图像处理模块参数
        /// </summary>
        public struct PLAYM4_VIE_PARACONFIG
        {
            /// <summary>
            /// 启用的算法处理模块，在PLAYM4_VIE_MODULES中定义，如 PLAYM4_VIE_MODU_ADJ | PLAYM4_VIE_MODU_EHAN，模块启用后，必须设置相应的参数；
            /// </summary>
            public int moduFlag;
            /// <summary>
            /// 亮度调节值，[-255, 255]
            /// </summary>
            public int brightVal;
            /// <summary>
            /// 对比度调节值，[-256, 255]
            /// </summary>
            public int contrastVal;
            /// <summary>
            /// 饱和度调节值，[-256, 255]
            /// </summary>
            public int colorVal;
            //PLAYM4_VIE_MODU_EHAN
            /// <summary>
            /// 滤波范围，[0, 100]
            /// </summary>
            public int toneScale;
            /// <summary>
            /// 对比度调节，全局对比度增益值，[-256, 255]
            /// </summary>
            public int toneGain;
            /// <summary>
            /// 亮度调节，亮度平均值偏移，[-255, 255]
            /// </summary>
            public int toneOffset;
            /// <summary>
            /// 颜色调节，颜色保真值，[-256, 255]
            /// </summary>
            public int toneColor;
            //PLAYM4_VIE_MODU_DEHAZE
            /// <summary>
            /// 去雾强度，[0, 255]
            /// </summary>
            public int dehazeLevel;
            /// <summary>
            /// 透射值，[0, 255]
            /// </summary>
            public int dehazeTrans;
            /// <summary>
            /// 亮度补偿，[0, 255]
            /// </summary>
            public int dehazeBright;
            //PLAYM4_VIE_MODU_DENOISE
            /// <summary>
            /// 去噪强度，[0, 255]
            /// </summary>
            public int denoiseLevel;
            //PLAYM4_VIE_MODU_SHARPEN
            /// <summary>
            /// 锐化强度，[0, 255]
            /// </summary>
            public int usmAmount;     //
            /// <summary>
            /// 锐化半径，[1, 15]
            /// </summary>
            public int usmRadius;     //
            /// <summary>
            /// 锐化阈值，[0, 255]
            /// </summary>
            public int usmThreshold;  //
            //PLAYM4_VIE_MODU_DEBLOCK
            /// <summary>
            /// 去块强度，[0, 100]
            /// </summary>
            public int deblockLevel;  //
            //PLAYM4_VIE_MODU_LENS
            /// <summary>
            /// 畸变量，[-256, 255]
            /// </summary>
            public int lensWarp;      //
            /// <summary>
            /// 缩放量，[-256, 255]
            /// </summary>
            public int lensZoom;      //
            //PLAYM4_VIE_MODU_CRB
            //无响应参数
        }

        public struct PLAYM4_SYSTEM_TIME //绝对时间 
        {
            public int dwYear;	//年
            public int dwMon;	//月
            public int dwDay;	//日
            public int dwHour;	//时
            public int dwMin;	//分
            public int dwSec;	//秒
            public int dwMs;		//毫秒

            public DateTime ToDateTime()
            {
                return new DateTime(dwYear, dwMon, dwDay, dwHour, dwMin, dwSec, dwMs);
            }
        }

        /// <summary>
        /// 帧信息
        /// </summary>
        public struct FRAME_INFO
        {
            public int nWidth;
            public int nHeight;
            public int nStamp;
            public int nType;
            public int nFrameRate;
            public int dwFrameNum;
        }
    }
}
