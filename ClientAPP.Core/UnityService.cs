using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
namespace ClientAPP.Core
{
    public class UnityService
    {
        /// <summary>
        /// 默认容器
        /// </summary>
        public static UnityContainer UnityContainer { get; private set; }

        static UnityService() => UnityContainer = new UnityContainer();

        
    }
}
