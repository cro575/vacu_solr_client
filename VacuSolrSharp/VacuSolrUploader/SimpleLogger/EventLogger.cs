using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace XairStone.SimpleLogger
{
    class EventLogger : ILogger
    {
        public EventLogger()
        {

        }

        public void LogMessage(string logMessage)
        {
            EventLog.WriteEntry("Logger", logMessage);            
        }
    }
}
