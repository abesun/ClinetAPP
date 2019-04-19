using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Core.Contract
{
    /// <summary>
    /// 设备管理器
    /// </summary>
    public interface IDeviceManager
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 运行同步命令
        /// </summary>
        /// <param name="commandString">命令内容</param>
        /// <param name="result">结果</param>
        /// <returns>是否成功</returns>
        bool RunCommand(string commandString, out object result);
    }
}
