using System;
using System.Collections.Generic;
using System.Text;


namespace XairStone.SimpleLogger
{
    public class Logger
    {
        public enum Level
        {
            Error = 0,
            FailureAudit = 1,
            Warning = 2,
            Information = 3,
            SuccessAudit = 4,
            Debug = 5
        }

        #region Data
        private static object mLock = new object();

        private static Logger mLogger = null;


        public static Logger Instance
        {
            get 
            {
                // If this is the first time we�re referring to the
                // singleton object, the private variable will be null.
                if (mLogger == null)
                {
                    // for thread safety, lock an object when
                    // instantiating the new Logger object. This prevents
                    // other threads from performing the same block at the
                    // same time.
                    lock (mLock)
                    {
                        // Two or more threads might have found a null
                        // mLogger and are therefore trying to create a 
                        // new one. One thread will get to lock first, and
                        // the other one will wait until mLock is released.
                        // Once the second thread can get through, mLogger
                        // will have already been instantiated by the first
                        // thread so test the variable again. 
                        if (mLogger == null)
                        {
                            mLogger = new Logger();
                        }
                    }
                }
                return mLogger;
            }
        }

        private List<ILogger> mObservers;

        #endregion

        #region Constructor
        private Logger()
        {
            mObservers = new List<ILogger>();
        }

        public Logger(params ILogger[] loggers)
        {
            mObservers = new List<ILogger>();

            foreach(ILogger logger in loggers)
                RegisterObserver(logger);
        }

        #endregion

        #region Public methods
        public void RegisterObserver(ILogger observer)
        {
            if (!mObservers.Contains(observer))
            {
                mObservers.Add (observer);
            }
        }

        public void UnRegisterObserver(ILogger observer)
        {
            foreach (ILogger test in mObservers)
            {
                if (test == observer)
                {
                    mObservers.Remove(observer);
                    return;
                }
            }
        }

        public string getLogFileName()
        {
            foreach (ILogger test in mObservers)
            {
                FileLogger flogger = test as FileLogger;
                if (flogger != null)
                {
                    return flogger.FileName;
                }
            }
            return null;
        }

        public void Log(string message)
        {
            Log(message, true);
        }

        public void Log(string message, bool bTimeStamp)
        {
            // Apply some basic formatting like the current timestamp
            string formattedMessage = null;

            if(bTimeStamp)
                formattedMessage = string.Format("{0} - {1}", DateTime.Now.ToString(), message);
            else
                formattedMessage =  string.Format("{0} - {1}", "                    ", message);

            foreach (ILogger observer in mObservers)
            {
                observer.LogMessage(formattedMessage);
            }
        }

        public void Log(Logger.Level level, string message)
        {
            Log(level, message, true);
        }

        public void Log(Logger.Level level, string message, bool bTimeStamp)
        {
            // Apply some basic formatting like the current timestamp
            string formattedMessage = null;

            if (bTimeStamp)
                formattedMessage = string.Format("{0} | {1} | - {2}", DateTime.Now.ToString(), level.ToString(), message);
            else
                formattedMessage = string.Format("{0} | {1} | - {2}", "                    ", level.ToString(), message);

            foreach (ILogger observer in mObservers)
            {
                observer.LogMessage(formattedMessage);
            }
        }

        public void Log(Exception ex)
        {
            StringBuilder message = new StringBuilder(ex.Message);
            string trace = ex.StackTrace;

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                message.Append("\n" + ex.Message);
            }
            message.Append("\nStack trace: " + trace);

            Log(message.ToString());
        }
        #endregion
    }
}

