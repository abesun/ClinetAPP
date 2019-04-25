using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Uti
{
    /// <summary>
    /// 文件相关
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 获得不带非法字符的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetCorrectFileName(string fileName)
        {
            return fileName.Replace(@"\", "").Replace(@"/", "").Replace(@"?", "").Replace(@":", "").Replace(@"*", "")
                .Replace("\"", "").Replace(@">", "").Replace(@"<", "").Replace(@"|", "");
        }
    }
}
