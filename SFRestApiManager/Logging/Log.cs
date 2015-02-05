using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SFRestApiManager.Logging
{
    internal static class Log
    {
        public static Logger Instance { get; set; }
        static Log()
        {
            //Always watch to see if configuration has changed.
            LogManager.ReconfigExistingLoggers();
            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}
