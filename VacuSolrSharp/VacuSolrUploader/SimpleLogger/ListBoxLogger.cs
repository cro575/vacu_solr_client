using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace XairStone.SimpleLogger
{
    class ListBoxLogger : ILogger
    {
        ListBox m_listBox;
        public ListBoxLogger(ListBox listBox)
        {
            m_listBox = listBox;
        }

        public void LogMessage(string logMessage)
        {
            MethodInvoker logDelegate = delegate 
            {
                m_listBox.Items.Add(logMessage);
            };

            if (m_listBox.InvokeRequired)
                m_listBox.Invoke(logDelegate);
            else
                logDelegate();
        }
    }
}
