using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace XairStone.SimpleLogger
{
    class TraceWindowLogger : ILogger
    {
        public TraceWindowLogger()
        {
        }

        public void LogMessage(string logMessage)
        {
            Trace.WriteLine(logMessage);            
        }
    }
}
