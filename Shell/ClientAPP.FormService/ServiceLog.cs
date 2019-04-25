using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientAPP.Core.Contract.Log;
using log4net;
namespace ClientAPP.FormService
{
    public class ServiceLog : IAppLog
    {
        ILog LogModule;

        public ServiceLog()
        {
            LogModule = log4net.LogManager.GetLogger("Log4net.Logging"); 
        }

        public void Debug(string Log)
        {
            LogModule.Debug(Log);
        }

        public void Error(string Log)
        {
            LogModule.Error(Log);
        }

        public void Error(Exception ex)
        {
            LogModule.Error(ex);
        }

        public void Fatal(string Log)
        {
            LogModule.Fatal(Log);
        }

        public void Info(string Log)
        {
            LogModule.Info(Log);
        }

        public void Warn(string Log)
        {
            LogModule.Warn(Log);
        }
    }
}
