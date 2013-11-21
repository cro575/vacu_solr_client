using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using XairStone.SimpleLogger;

namespace VacuSolrUploader
{
    static class Program
    {
        static public Config config = config = new Config();
        static private Logger mLogger;
        static private FileLogger mFileLogger;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            mLogger = Logger.Instance;
            mFileLogger = new FileLogger("XTR_", "");
            mLogger.RegisterObserver(mFileLogger);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SolrUploadForm());

            mFileLogger.Terminate();
        }
    }
}
