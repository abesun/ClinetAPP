using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ClientAPP.Uti;
using System.IO;
using Newtonsoft.Json;
namespace ClientAPP.API
{
    public class Hik_Open8200API
    {
        #region C++API方式
        public const string DllName = "StdClient_Hikvision30.dll";
        /// <summary>
        /// 是否初始化
        /// </summary>
        private static bool m_init = false;

        /// <summary>
        /// 登录句柄
        /// </summary>
        private static int m_LoginID = -1;

        /// <summary>
        /// 获得登录ID
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="port"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static int GetLoginID(string Ip, int port, string UserName, string Password)
        {
            if (m_LoginID >= 0)
                return m_LoginID;

            if (m_init == false)
            {
                int ret = Std_Initialize();
                if (ret < 0)
                    return ret;
            }
            m_LoginID = Std_Login(Ip, port, UserName, Password);
            return m_LoginID;

        }

        //初始化业务
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>>=成功</returns>
        [DllImport(DllName)]
        private static extern int Std_Initialize();

        /// <summary>
        /// 反初始化
        /// </summary>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_UnInitialize();


        //登录业务
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Host">平台IP地址，最大长度32字节</param>
        /// <param name="Port">平台端口</param>
        /// <param name="Username">用户名，最大长度128字节</param>
        /// <param name="Password">密码，最大长度128字节</param>
        /// <returns></returns>
        [DllImport(DllName)]
        private static extern int Std_Login(string Host, int Port, string Username, string Password);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="LoginHandle"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_Logout(int LoginHandle);


        //实时播放
        /// <summary>
        /// 实时浏览
        /// </summary>
        /// <param name="LoginHandle"></param>
        /// <param name="CameraId"></param>
        /// <param name="PlayWnd"></param>
        /// <param name="StreamType"></param>
        /// <param name="CBF_Stream"></param>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_StartRealPlay(int LoginHandle, string CameraId, IntPtr PlayWnd, int StreamType, StreamCallbackPF CBF_Stream, IntPtr UserData);

        /// <summary>
        /// 停止实时浏览
        /// </summary>
        /// <param name="RealHandle"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_StopRealPlay(int RealHandle);


        [DllImport(DllName)]
        private static extern int Std_GetLastError(IntPtr ErrorDesc);

        public static string GetLastError()
        {
            IntPtr s = Marshal.AllocHGlobal(512);
            Std_GetLastError(s);
            byte[] data = new byte[512];
            Marshal.Copy(s, data, 0, 512);
            string ret = Encoding.UTF8.GetString(data);
            ret = ret.Remove(ret.IndexOf('\0'));
            return ret;
        }


        public delegate void StreamCallbackPF(int RealHandle, int StreamType, IntPtr Data, int DataLen, IntPtr DecoderTag, IntPtr UserData);

        /// <summary>
        /// 云台控制
        /// </summary>
        /// <param name="LoginHandle"></param>
        /// <param name="CameraId"></param>
        /// <param name="PtzCmd"></param>
        /// <param name="Param1"></param>
        /// <param name="Param2"></param>
        /// <param name="Param3"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_PtzCtrl(int LoginHandle, string CameraId, string PtzCmd, int Param1, int Param2, int Param3);

        /// <summary>
        /// 云台控制
        /// </summary>
        /// <param name="LoginHandle"></param>
        /// <param name="CameraId"></param>
        /// <param name="xTop"></param>
        /// <param name="yTop"></param>
        /// <param name="xBottom"></param>
        /// <param name="yBottom"></param>
        /// <param name="bCounter"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_PtzCtrl3D(int LoginHandle, string CameraId, int xTop, int yTop, int xBottom, int yBottom, int bCounter);

        /// <summary>
        /// 回放
        /// </summary>
        /// <param name="LoginHandle"></param>
        /// <param name="CameraID"></param>
        /// <param name="BeginTime">开始时间，时间格式 “YYYYMMDDTHHNNSSZ</param>
        /// <param name="EndTime"></param>
        /// <param name="StorageDev">(不支持设置，传0即可，自动从平台中查找可用存储介质)</param>
        /// <param name="playWnd"></param>
        /// <param name="CBF_Stream">实时流回调函数的地址，允许为NULL</param>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_StreamReplayByTime(int LoginHandle, string CameraID, string BeginTime, string EndTime, int StorageDev, IntPtr playWnd, StreamCallbackPF CBF_Stream, IntPtr UserData);

        /// <summary>
        /// 停止回放
        /// </summary>
        /// <param name="ReplayHandle"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_StopStreamReplay(int ReplayHandle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReplayHandle"></param>
        /// <param name="ReplayMode"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_StreamReplayControl(int ReplayHandle, ReplayMode ReplayMode);

        /// <summary>
        /// 回放模式
        /// </summary>
        public enum ReplayMode
        {
            PLAYMODE_STOP = 1,
            PLAYMODE_PAUSE = 2,
            PLAYMODE_RESUME = 3,
            PLAYMODE_ONEBYONE = 4,
            PLAYMODE_ONEBYONEBACK = 5,
            PLAYMODE_16_BACKWARD = 6,
            PLAYMODE_8_BACKWARD = 7,
            PLAYMODE_4_BACKWARD = 8,
            PLAYMODE_2_BACKWARD = 9,
            PLAYMODE_1_BACKWARD = 10,
            PLAYMODE_HALF_BACKWARD = 11,
            PLAYMODE_QUARTER_BACKWARD = 12,
            PLAYMODE_Eighth_BACKWARD = 13,
            PLAYMODE_Eighth_FORWARD = 14,
            PLAYMODE_QUARTER_FORWARD = 15,
            PLAYMODE_HALF_FORWARD = 16,
            PLAYMODE_1_FORWARD = 17,
            PLAYMODE_2_FORWARD = 18,
            PLAYMODE_4_FORWARD = 19,
            PLAYMODE_8_FORWARD = 20
            //PLAYMODE_16_FORWARD = 21
        }


        [DllImport(DllName)]
        public static extern int Std_GetFileReplayPos(int ReplayHandle, ref int PlayTimed);


        [DllImport(DllName)]
        public static extern int Std_SetStreamReplayPos(int ReplayHandle, int PlayTimed);

        [DllImport(DllName)]
        public static extern int Std_StartDownloadByTime(int LoginHandle, string CameraID, string BeginTime, string EndTime, int StorageDev, ref int DownloadSpeed, string filename, byte[] fileExt, int fileExtLen);


        [DllImport(DllName, EntryPoint = "Std_StartDownloadByTime")]
        public static extern int Std_StartDownloadByTime2(int LoginHandle, string CameraID, string BeginTime, string EndTime, int StorageDev, ref int DownloadSpeed, IntPtr filename, byte[] fileExt, int fileExtLen);

        [DllImport(DllName)]
        public static extern int Std_StopDownload(int DownloadHandle);

        /// <summary>
        /// 打开声音
        /// </summary>
        /// <param name="PlayHandle"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_OpenSound(long PlayHandle);
        /// <summary>
        /// 关闭声音
        /// </summary>
        /// <param name="PlayHandle"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_CloseSound(long PlayHandle);


        /// <summary>
        /// 本地录像
        /// </summary>
        /// <param name="RealHandle"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_StartLocalRecord(long RealHandle, IntPtr FileName, IntPtr FileExt, int FileExtLen);

        /// <summary>
        /// 停止本地录像
        /// </summary>
        [DllImport(DllName)]
        public static extern int Std_StopLocalRecord(long RealHandle);

        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="PlayHandle"></param>
        /// <param name="PicFile"></param>
        /// <param name="PicFormat"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_Capture(int PlayHandle, string PicFile, int PicFormat);

        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="PlayHandle"></param>
        /// <param name="PicFile"></param>
        /// <param name="PicFormat"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "Std_Capture")]
        public static extern int Std_Capture2(int PlayHandle, IntPtr PicFile, int PicFormat);


        public delegate void OrgInfoCallBackPF(int getResHandle, ref int iContinue, int iFinish, string FileListXML, IntPtr UserData);
        public delegate void OrgInfoCallBackPF2(int getResHandle, ref int iContinue, int iFinish, IntPtr FileListXML, IntPtr UserData);
        //获取资源列表
        [DllImport(DllName)]
        public static extern int Std_GetResList(int LoginHandle, int ResType, string ParentOrgCode, OrgInfoCallBackPF OrgInfoCBF, IntPtr UserData);
        [DllImport(DllName, EntryPoint = "Std_GetResList")]
        public static extern int Std_GetResList2(int LoginHandle, int ResType, string ParentOrgCode, OrgInfoCallBackPF2 OrgInfoCBF, IntPtr UserData);
        //获取摄像机信息
        [DllImport(DllName)]
        public static extern int Std_GetCameraInfo(int LoginHandle, string CameraId, out string CameraInfo);

        /// <summary>
        /// 获得下载进度
        /// </summary>
        /// <param name="DownloadHandle"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern int Std_GetDownloadPos(int DownloadHandle);

        public static string GetTimeString(DateTime time)
        {
            return time.ToString("yyyyMMddTHHmmssZ");
        }

        public static string GetUTF8String(string s)
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            string ret = Encoding.Default.GetString(data);
            ret += "\0";
            return ret;
        }

        public static IntPtr GetUTF8StringPtr(string s)
        {

            byte[] data = Encoding.UTF8.GetBytes(s);
            IntPtr ret = Marshal.AllocHGlobal(data.Length + 1);
            Marshal.Copy(data, 0, ret, data.Length);
            Marshal.WriteByte(ret, data.Length, 0);
            return ret;
        }

        #endregion


        #region rest方式
        /// <summary>
        /// 根据监控点编号获取视频预览url
        /// </summary>
        /// <param name="cameraCode">监控点编号</param>
        /// <param name="streamIndex">码流类型 默认为主码流 说明：0，主码流 1，表示子码流</param>
        /// <param name="protocol">协议类型（目前仅支持rtsp和hls协议,默认为rtsp协议) 说明：0，rtsp协议； 1，hls协议</param>
        /// <param name="urlBase">服务地址</param>
        /// <param name="appKey">key</param>
        /// <param name="appSecret">密钥</param>
        /// <returns></returns>
        public static string GetPreviewUrl( string cameraCode, int streamIndex,int protocol, string urlBase,string appKey, string appSecret)
        {
            string ret = "";

            string url = $"{urlBase}/api/video/v1/preview?cameraIndexCode={cameraCode}&subStream={streamIndex}&protocol={protocol}";

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/json");

            ret = HttpGet(url, headers, appKey, appSecret, 3000, new List<string>());
            var obj = JsonConvert.DeserializeObject(ret);
            
            return ret;
        }

        /// <summary>
        /// ptz控制
        /// </summary>
        /// <param name="cameraCode">监控点编号</param>
        /// <param name="presetIndex">预置位编号</param>
        /// <param name="action">开始或停止操作(1 开始 0 停止)</param>
        /// <param name="speed">云台速度(取值范围为0-7，默认4)</param>
        /// <param name="command">控制命令(不区分大小写) 说明： LEFT	左转 RIGHT	右转 UP	上转 DOWN	下转 ZOOM_IN	焦距变大 ZOOM_OUT	焦距变小 LEFT_UP	左上 LEFT_DOWN	左下 RIGHT_UP	右上 RIGHT_DOWN	右下 FOCUS_NEAR	焦点前移 FOCUS_FAR	焦点后移 IRIS_ENLARGE 光圈扩大 IRIS_REDUCE	光圈缩小 以下命令presetIndex不可为空： SET_PRESET设置预置点 GOTO_PRESET到预置点</param>
        /// <param name="urlBase"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string Ptz(string cameraCode, int presetIndex, int action, int speed, string command, string urlBase, string appKey, string appSecret)
        {
            string ret = "";
            string url = $"{urlBase}/api/video/v1/ptz?cameraIndexCode={cameraCode}&presetIndex={presetIndex}&action={action}&speed={speed}&command={command}";

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/json");

            ret = HttpGet(url, headers, appKey, appSecret, 3000, new List<string>());

            return ret;
        }

        //public static string GetPlaybackUrl(string )

        #region http请求相关
        #region 常量定义
        /// <summary>
        /// 前面Header
        /// </summary>
        const string X_CA_SIGNATURE = "x-ca-signature";

        /// <summary>
        /// 所有参与签名的Header
        /// </summary>
        const string X_CA_SIGNATURE_HEADERS = "x-ca-signature-headers";
        /// <summary>
        /// 请求时间戳
        /// </summary>
        const string X_CA_TIMESTAMP = "x-ca-timestamp";
        /// <summary>
        /// 请求放重放Nonce,15分钟内保持唯一,建议使用UUID
        /// </summary>
        const string X_CA_NONCE = "x-ca-nonce";

        /// <summary>
        /// APP KEY
        /// </summary>
        const string X_CA_KEY = "x-ca-key";

        #endregion



        private static string HttpGet(string url, Dictionary<string, string> headers, string appKey, string appSecret, int timeout, List<string> signHeaderPrefixList)
        {
            Dictionary<string, string> formParam = new Dictionary<string, string>();
            headers = initialBasicHeader(headers, appKey, appSecret, "GET", url, formParam, signHeaderPrefixList);


            //SimpleHttpClient httpClient("GET", url.c_str(), timeout);
            //for (map<string, string>::iterator it = headers.begin(); it != headers.end(); it++)
            //{
            //    httpClient.setHttpHeader(it->first, it->second);
            //}
            //httpClient.sendHttpRequest();
            //return httpClient.getHttpResponseBody();

            var response = HttpRequestHelper.CreateGetHttpResponse(url, timeout, "", null, headers, "");
            return response;
        }



        static string HttpPost(string url, Dictionary<string, string> headers, string body, int bodySize, string appKey, string appSecret, int timeout, List<string> signHeaderPrefixList)
        {
            Dictionary<string, string> formParam = new Dictionary<string, string>();
            headers = initialBasicHeader(headers, appKey, appSecret, "POST", url, formParam, signHeaderPrefixList);

            //       SimpleHttpClient httpClient("POST", url.c_str(), timeout);
            //for(map<string, string>::iterator it = headers.begin(); it != headers.end(); it++)
            //{
            //	httpClient.setHttpHeader(it->first, it->second);
            //}
            //   httpClient.setHttpContent(body/*////, bodySize*/);
            //httpClient.sendHttpRequest();
            //return httpClient.getHttpResponseBody();

            var response = HttpRequestHelper.CreateGetHttpResponse(url, timeout, "", null, headers, body);
            return response;
        }

        //static string HttpPost(string url, Dictionary<string, string> headers, Dictionary<string, string> bodys, string appKey, string appSecret, int timeout, List<string> signHeaderPrefixList)
        //{
        //    Dictionary <string, string> formParam = new Dictionary<string, string>();
        //    headers = initialBasicHeader(headers, appKey, appSecret, "POST", url, formParam, signHeaderPrefixList);

        //    SimpleHttpClient httpClient("POST", url.c_str(), timeout);
        //    for (map<string, string>::iterator it = headers.begin(); it != headers.end(); it++)
        //    {
        //        httpClient.setHttpHeader(it->first, it->second);
        //    }
        //    httpClient.setHttpMultipartForm(bodys);
        //    httpClient.sendHttpRequest();
        //    return httpClient.getHttpResponseBody();
        //}

        //string HttpUtil::HttpPost(string url, map<string, string> headers, string body, string appKey, string appSecret, int timeout, list<string> signHeaderPrefixList)
        //{
        //    map<string, string> formParam;
        //    headers = initialBasicHeader(headers, appKey, appSecret, "POST", url, formParam, signHeaderPrefixList);

        //    SimpleHttpClient httpClient("POST", url.c_str(), timeout);
        //    for (map<string, string>::iterator it = headers.begin(); it != headers.end(); it++)
        //    {
        //        httpClient.setHttpHeader(it->first, it->second);
        //    }
        //    httpClient.setHttpContent(body);
        //    httpClient.sendHttpRequest();
        //    return httpClient.getHttpResponseBody();
        //}




        /// <summary>
        /// 初始化头部
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="method"></param>
        /// <param name="requestAddress"></param>
        /// <param name="formParam"></param>
        /// <param name="signHeaderPrefixList"></param>
        /// <returns></returns>
        static Dictionary<string, string> initialBasicHeader(Dictionary<string, string> headers, string appKey, string appSecret, string method, string requestAddress, Dictionary<string, string> formParam, List<string> signHeaderPrefixList)
        {


            headers.Add(X_CA_TIMESTAMP, DateTimeHelper.DateTimeToCTime(DateTime.Now).ToString());
            headers.Add(X_CA_NONCE, Guid.NewGuid().ToString());
            headers.Add(X_CA_KEY, appKey);
            headers.Add(X_CA_SIGNATURE, SignUtil.Sign(method, requestAddress, headers, formParam, appSecret, signHeaderPrefixList));

            return headers;
        }

        /// <summary>
        /// 签名相关
        /// </summary>
        private class SignUtil
        {
            //请求Header Accept
            const string HTTP_HEADER_ACCEPT = "Accept";
            //请求Body内容MD5 Header
            const string HTTP_HEADER_CONTENT_MD5 = "Content-MD5";
            //请求Header Content-Type
            const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
            //请求Header UserAgent
            const string HTTP_HEADER_USER_AGENT = "User-Agent";
            //请求Header Date
            const string HTTP_HEADER_DATE = "Date";



            /// <summary>
            /// 计算签名
            /// </summary>
            /// <param name="method">HttpMethod</param>
            /// <param name="url">Path+Query</param>
            /// <param name="headers">Http头</param>
            /// <param name="formParamMap">POST表单参数</param>
            /// <param name="secret">APP密钥</param>
            /// <param name="signHeaderPrefixList">自定义参与签名Header前缀，符合此前缀的header才会签名</param>
            /// <returns>签名后的字符串</returns>
            public static string Sign(string method, string url, Dictionary<string, string> headers, Dictionary<string, string> formParamMap, string secret, List<string> signHeaderPrefixList)
            {
                string result;
                string stringToSign = BuildStringToSign(headers, url, formParamMap, method, signHeaderPrefixList);

                //unsigned char* signedByte = (unsigned char*)malloc(64);
                //memset(signedByte, 0, 64);
                //char* strBase64 = (char*)malloc(64);
                //memset(strBase64, 0, 64);

                byte[] signedByte = new byte[64];
                byte[] strBase64 = new byte[64];

                //unsigned int strSignedLen = 0;

                int strSignedLen = 0;

                //HmacEncode("SHA256", secret, stringToSign, signedByte, strSignedLen);
                //Base64Encode(signedByte, strSignedLen, strBase64, 64);


                //result = strBase64;

                result = EncryptHelper.HmacSHA256(stringToSign, secret);

                //去除所有换行符
                //               int pos = result.find('\n');
                //               while (pos != std::string::npos)
                //{
                //                   result.erase(pos, 1);
                //                   pos = result.find('\n');
                //               }
                result = result.Replace("\n", "");

                //free(signedByte);
                //free(strBase64);
                return result;
            }



            /// <summary>
            /// 构建待签名字符串
            /// </summary>
            /// <param name="headers">Http头</param>
            /// <param name="url">Path+Query</param>
            /// <param name="formParamMap">POST表单参数</param>
            /// <param name="method">method</param>
            /// <param name="signHeaderPrefixList">自定义参与签名Header前缀</param>
            /// <returns></returns>
            public static string BuildStringToSign(Dictionary<string, string> headers, string url, Dictionary<string, string> formParamMap, string method, List<string> signHeaderPrefixList)
            {
                string strToSign = "";

                //transform(method.begin(), method.end(), method.begin(), toupper);
                //strToSign.append(method);
                //strToSign.append(Constants::LF);

                strToSign += $"{method}\n";

                //if (headers.find(HttpHeader::HTTP_HEADER_ACCEPT) != headers.end())
                //{
                //    strToSign.append(headers[HttpHeader::HTTP_HEADER_ACCEPT]);
                //    strToSign.append(Constants::LF);
                //}

                if (headers.ContainsKey(HTTP_HEADER_ACCEPT))
                {
                    strToSign += $"{headers[HTTP_HEADER_ACCEPT]}\n";
                }

                //if (headers.find(HttpHeader::HTTP_HEADER_CONTENT_MD5) != headers.end())
                //{
                //    strToSign.append(headers[HttpHeader::HTTP_HEADER_CONTENT_MD5]);
                //    strToSign.append(Constants::LF);
                //}

                if (headers.ContainsKey(HTTP_HEADER_CONTENT_MD5))
                {
                    strToSign += $"{headers[HTTP_HEADER_CONTENT_MD5]}\n";
                }

                //if (headers.find(HttpHeader::HTTP_HEADER_CONTENT_TYPE) != headers.end())
                //{
                //    strToSign.append(headers[HttpHeader::HTTP_HEADER_CONTENT_TYPE]);
                //    strToSign.append(Constants::LF);
                //}

                if (headers.ContainsKey(HTTP_HEADER_CONTENT_TYPE))
                {
                    strToSign += $"{headers[HTTP_HEADER_CONTENT_TYPE]}\n";
                }

                //if (headers.find(HttpHeader::HTTP_HEADER_DATE) != headers.end())
                //{
                //    strToSign.append(headers[HttpHeader::HTTP_HEADER_DATE]);
                //    strToSign.append(Constants::LF);
                //}

                if (headers.ContainsKey(HTTP_HEADER_DATE))
                {
                    strToSign += $"{headers[HTTP_HEADER_DATE]}\n";
                }

                strToSign += BuildHeaders(headers, signHeaderPrefixList);
                strToSign += BuildResource(url, formParamMap);

                return strToSign;
            }

            /// <summary>
            /// 构建待签名Path+Query+FormParams
            /// </summary>
            /// <param name="url">Path+Query，如http://10.6.131.112:9999/artemis/findControlUnitByUnitCode?userName=test3&unitCode=0</param>
            /// <param name="formParamMap">POST表单参数</param>
            /// <returns>待签名Path+Query+FormParams</returns>
            static string BuildResource(string url, Dictionary<string, string> formParamMap)
            {
                //去掉 http://10.6.131.112:9999
                //int pos = url.find("://");
                //if (pos != string::npos)
                //{
                //    url = url.substr(pos + 3);
                //}
                //pos = url.find("/");
                //if (pos != string::npos)
                //{
                //    url = url.substr(pos);
                //}

                int pos = url.IndexOf("://");
                if (pos > 0)
                    url = url.Substring(pos);
                pos = url.IndexOf("/");
                if (pos > 0)
                    url = url.Substring(pos);


                string path = url;
                string queryString;
                //query 参数放入formParamMap中一起按字典排序
                //if (url.find("?") != string::npos)
                //{
                //    path = url.substr(0, url.find_first_of("?"));
                //    queryString = url.substr(url.find_first_of("?") + 1);

                //    if (!queryString.empty())
                //    {
                //        vector<string> vtQueryStr;
                //        WinUtil::split(queryString, "&", vtQueryStr);
                //        for (unsigned int i = 0; i < vtQueryStr.size(); i++)
                //        {
                //            if (vtQueryStr[i].find("=") != string::npos)
                //            {
                //                string key = vtQueryStr[i].substr(0, vtQueryStr[i].find_first_of("="));
                //                string value = "";
                //                if (vtQueryStr[i].find_first_of("=") == vtQueryStr[i].find_last_of("="))
                //                {
                //                    value = vtQueryStr[i].substr(vtQueryStr[i].find_first_of("=") + 1);
                //                }

                //                if (formParamMap.find(key) == formParamMap.end())
                //                {
                //                    formParamMap.insert(std::make_pair(key, value));
                //                }
                //            }
                //        }
                //    }
                //}

                pos = url.IndexOf("?");
                if (pos > 0)
                {
                    path = url.Substring(0, pos);
                    queryString = url.Substring(pos + 1);
                    if (string.IsNullOrEmpty(queryString) == false)
                    {
                        string[] vtQueryStr = queryString.Split('&');
                        vtQueryStr.Where(s => s.IndexOf('=') > 0).ToList().ForEach(
                            s =>
                            {
                                string[] strList = s.Split('=');
                                if (strList.Length != 2)
                                {
                                    string key = strList[0];
                                    string value = strList[1];
                                    if (formParamMap.ContainsKey(key) == false)
                                        formParamMap.Add(key, value);
                                }
                            }
                            );
                    }
                }


                string sb = "";
                //sb.append(path);
                sb += path;

                //if (formParamMap.size() > 0)
                //{
                //    sb.append("?");

                //    int flag = 0;
                //    for (map<string, string>::iterator it = formParamMap.begin(); it != formParamMap.end(); it++)
                //    {
                //        if (flag != 0)
                //        {
                //            sb.append("&");
                //        }
                //        flag++;

                //        string key = it->first;
                //        string val = it->second;

                //        if (!key.empty())
                //        {
                //            sb.append(key);
                //        }
                //        if (!val.empty())
                //        {
                //            sb.append("=").append(val);
                //        }
                //    }
                //}
                if (formParamMap.Count > 0)
                {
                    sb += "?";
                    foreach (string key in formParamMap.Keys)
                    {
                        string value = formParamMap[key];
                        if (string.IsNullOrEmpty(key) == false && string.IsNullOrEmpty(value) == false)
                            sb += $"{key}={value}";
                    }
                }

                return sb;
            }

            /// <summary>
            /// 构建待签名Http头
            /// </summary>
            /// <param name="headers">请求中所有的Http头</param>
            /// <param name="signHeaderPrefixList">自定义参与签名Header前缀</param>
            /// <returns>待签名Http头</returns>
            static string BuildHeaders(Dictionary<string, string> headers, List<string> signHeaderPrefixList)
            {
                Dictionary<string, string> headersToSign;

                string signHeadersString = "";
                string sb = "";
                signHeaderPrefixList.Remove(X_CA_SIGNATURE);
                signHeaderPrefixList.Remove(HTTP_HEADER_ACCEPT);
                signHeaderPrefixList.Remove(HTTP_HEADER_CONTENT_MD5);
                signHeaderPrefixList.Remove(HTTP_HEADER_CONTENT_TYPE);
                signHeaderPrefixList.Remove(HTTP_HEADER_DATE);
                //signHeaderPrefixList.remove(SystemHeader::X_CA_SIGNATURE);
                //signHeaderPrefixList.remove(HttpHeader::HTTP_HEADER_ACCEPT);
                //signHeaderPrefixList.remove(HttpHeader::HTTP_HEADER_CONTENT_MD5);
                //signHeaderPrefixList.remove(HttpHeader::HTTP_HEADER_CONTENT_TYPE);
                //signHeaderPrefixList.remove(HttpHeader::HTTP_HEADER_DATE);

                //for (map<string, string>::iterator it = headers.begin(); it != headers.end(); it++)
                //{
                //    if (IsHeaderToSign(it->first, signHeaderPrefixList))
                //    {
                //        sb.append(it->first);
                //        sb.append(Constants::SPE2);
                //        if (!it->second.empty())
                //        {
                //            sb.append(it->second);
                //        }
                //        sb.append(Constants::LF);

                //        if (!signHeadersString.empty())
                //        {
                //            signHeadersString.append(",");
                //        }
                //        signHeadersString.append(it->first);
                //    }
                //}

                foreach (string key in headers.Keys)
                {
                    if (signHeaderPrefixList.Exists(s => s == key))
                    {
                        string value = headers[key];
                        sb += $"{key}:{value}\n";
                    }

                    if (string.IsNullOrEmpty(signHeadersString))
                        signHeadersString += ",";
                    signHeadersString += key;
                }


                //headers.insert(std::make_pair(SystemHeader::X_CA_SIGNATURE_HEADERS, signHeadersString));
                headers.Add(X_CA_SIGNATURE_HEADERS, signHeadersString);

                return sb;
            }
        }

        #endregion

        #endregion
    }
}
