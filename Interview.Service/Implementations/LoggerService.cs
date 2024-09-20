using Interview.Service.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Service.Implementations
{

    public class LoggerService : ILoggerService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public void LogException(Exception ex)
        {
            
                logger.Error(ex, "An error occurred in my repository");
            
        }
    }
}
