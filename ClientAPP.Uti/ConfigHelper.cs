using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
namespace ClientAPP.Uti
{
    /// <summary>
    /// 配置文件帮助
    /// </summary>
    public class ConfigHelper<T>
    {
        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configFile"></param>
        /// <param name="err"></param>
        public static void SaveXML(T config, string configFile, out string err)
        {
            err = "";
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                XmlTextWriter tw = new XmlTextWriter(configFile, Encoding.UTF8)
                {
                    Indentation = 4,
                    Formatting = Formatting.Indented
                };
                xs.Serialize(tw, config);
                tw.Flush();
                tw.Close();
            }catch(Exception ex)
            {
                err = ex.Message;
            }
            return;
            
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static T LoadXML(string configFile,out string err)
        {
            T ret = default;
            err = "";
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                XmlTextReader xr = new XmlTextReader(configFile);
                ret = (T)xs.Deserialize(xr);
                xr.Close();
            }
            catch(Exception ex)
            {
                err = ex.Message;
            }
            return ret;
        }

    }
}
