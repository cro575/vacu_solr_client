using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml;


namespace XairStone.SimpleLogger
{
    public class FileLogger: ILogger
    {
        #region Data
        private string mFileName;
        private StreamWriter mLogFile = null;

        public string FileName
        {
            get { return mFileName; }
        }
        #endregion

        #region Constructor
        public FileLogger(string filename)
        {
            mFileName = filename;
        }
        public FileLogger(string prefix, string suffix)
        {
            Directory.CreateDirectory(Application.StartupPath + "\\log");

            mFileName = Application.StartupPath + "\\log\\" + prefix + System.DateTime.Now.ToString("yyyyMMdd-HHmmss") + suffix + ".log";
        }
        #endregion

        #region Public methods
        public void Init()
        {
            mLogFile = new StreamWriter(mFileName,true);
            mLogFile.AutoFlush = true;
        }

        public void Terminate()
        {
            if (mLogFile!=null)
                mLogFile.Close();
        }
        #endregion

        #region ILogger Members

        public void LogMessage(string logMessage)
        {
            // FileLogger implements the LogMessage method by writing the incoming
            // message to a file.
            if (mLogFile == null)
                Init();

            mLogFile.WriteLine(logMessage);
        }
        #endregion
    }
}
