using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Repository.Interfaces
{
    public interface ILoggerRepository
    {
        void LogException(Exception ex);
    }
}
