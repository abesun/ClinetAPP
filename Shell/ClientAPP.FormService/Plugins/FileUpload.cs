using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.Core.Contract.Websocket;
using System.IO;
namespace ClientAPP.FormService.Plugins
{
    public class FileUpload
    {
        /// <summary>
        /// 根据文件名获取ws包
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static WSProtocol GetUploadPicWSP(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            FileStream fs = new FileStream(fileName, FileMode.Open);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();
            string base64 = Convert.ToBase64String(data);

            WSProtocol wsp = new WSProtocol();
            WSPHeader header = new WSPHeader() { BodyType = BodyType.UploadEvent, Time = DateTime.Now.ToString(), Ver = "1.0", SN = "1" };
            wsp.Header = header;

            WSEventUpload body = new WSEventUpload() { EventType = WSEventDefine.FaceImg, Module = WSDefine.VideoModule };
            WSEvent_FileUpload f = new WSEvent_FileUpload() { FileType = fileInfo.Extension, FileData_Base64=base64 };
            body.Params = f;
            wsp.Body = body;


            return wsp;
                
        }
            
    }
}
