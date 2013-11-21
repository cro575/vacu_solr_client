using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using System.Xml;
using System.Xml.Xsl;
using System.Xml.Linq;
using System.IO;

namespace VacuSolrNet
{
    public class Stopwatch
    {
        private long StartTime;
        private long PrevTime;
        private long StopTime;

        public Stopwatch()
        {
            StartTime = 0;
            PrevTime = 0;
            StopTime = 0;
        }

        public void start()
        {
            StopTime = PrevTime = StartTime = System.DateTime.Now.Ticks;
        }

        public void stop()
        {
            PrevTime = StopTime;
            StopTime = System.DateTime.Now.Ticks;
        }

        public string getSecTime(bool bCurrentSet, string msg)
        {
            if (bCurrentSet)
            {
                stop();
            }

            return "<div class='tLog'>## "+msg + (StopTime - StartTime) / 10000000.0F + " ##</div>";
        }

        public string getIntervalTime(bool bCurrentSet, string msg)
        {
            if (bCurrentSet)
            {
                stop();
            }

            return "<div class='tLog'>## "+msg+ + (StopTime - PrevTime) / 10000000.0F + " ##</div>";
        }

    }
}
