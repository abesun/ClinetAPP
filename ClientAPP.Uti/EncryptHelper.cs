using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Uti
{
    /// <summary>
    /// 加密相关
    /// </summary>
    public class EncryptHelper
    {
        /// <summary>
        /// SHA256 加密
        /// </summary>
        /// <param name="strIN"></param>
        /// <returns></returns>
        public static string SHA256Encrypt(string strIN)
        {
            //string strIN = getstrIN(strIN);
            byte[] tmpByte;
            SHA256 sha256 = new SHA256Managed();
            tmpByte = sha256.ComputeHash(GetKeyByteArray(strIN));

            StringBuilder rst = new StringBuilder();
            for (int i = 0; i < tmpByte.Length; i++)
            {
                rst.Append(tmpByte[i].ToString("x2"));
            }
            sha256.Clear();
            return rst.ToString();
            //return GetStringValue(tmpByte);
        }

        private static string GetStringValue(byte[] Byte)
        {
            string tmpString = "";
            UTF8Encoding Asc = new UTF8Encoding();
            tmpString = Asc.GetString(Byte);
            return tmpString;
        }

        private static byte[] GetKeyByteArray(string strKey)
        {
            UTF8Encoding Asc = new UTF8Encoding();
            int tmpStrLen = strKey.Length;
            byte[] tmpByte = new byte[tmpStrLen - 1];
            tmpByte = Asc.GetBytes(strKey);
            return tmpByte;
        }

        /// <summary>
        /// HmacSHA256加密 输出为base64字符串
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string HmacSHA256(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Base64(string message)
        {
            System.Text.Encoding encode = Encoding.UTF8;
            byte[] bytedata = encode.GetBytes(message);
            string strPath = Convert.ToBase64String(bytedata, 0, bytedata.Length);
            return strPath;
        }

    }
}
