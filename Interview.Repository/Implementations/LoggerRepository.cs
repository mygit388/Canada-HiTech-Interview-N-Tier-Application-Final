using Interview.Repository.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Repository.Implementations
{
    public class LoggerRepository:ILoggerRepository
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public void LogException(Exception ex)
        {

            logger.Error(ex, "An error occurred in my repository");

        }


    }
}
