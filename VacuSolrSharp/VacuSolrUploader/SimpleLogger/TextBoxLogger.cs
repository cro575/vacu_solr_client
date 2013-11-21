using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace XairStone.SimpleLogger
{
    class TextBoxLogger : ILogger
    {
        private TextBox m_textBox;
        public TextBoxLogger(TextBox txtBox)
        {
            m_textBox = txtBox;
        }

        public void LogMessage(string logMessage)
        {
            MethodInvoker logDelegate = delegate { m_textBox.AppendText(logMessage + "\r\n"); };
            if (m_textBox.InvokeRequired)
                m_textBox.Invoke(logDelegate);
            else
                logDelegate();
        }
    }
}
