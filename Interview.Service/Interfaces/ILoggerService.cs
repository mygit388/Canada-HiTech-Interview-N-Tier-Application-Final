using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Service.Interfaces
{
    public interface ILoggerService
    {
         void LogException(Exception ex);
    }
}
