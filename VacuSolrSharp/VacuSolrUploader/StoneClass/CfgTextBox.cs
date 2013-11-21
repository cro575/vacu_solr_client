using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;


namespace XairStone
{
    public class CfgTextBox : System.Windows.Forms.TextBox
    {
        System.Windows.Forms.ContextMenuStrip ctxHisMenu = new System.Windows.Forms.ContextMenuStrip();

        public ArrayList HisItems = new ArrayList();

        public bool _bTrim = true;
        public int _max_list_count = 15;


        public ArrayList GetCfg()
        {
            ArrayList cfg = new ArrayList();

            CurrentUpdate();
            cfg.AddRange(ToArryList().ToArray());

            return cfg;
        }

        public void SetCfg(ArrayList cfg)
        {
            this.HisItems.Clear();
            this.HisItems.AddRange(cfg.ToArray());

            if (this.HisItems.Count > 0)
            {
                this.Text = (string)cfg[0];
            }
        }

        public void CurrentUpdate()
        {
            AddNewData(this.Text);
        }

        public ArrayList ToArryList()
        {
            ArrayList al = new ArrayList();

            foreach (string s in this.HisItems)
            {
                if (String.IsNullOrEmpty(s))
                    continue;
                al.Add(s);
            }

            return al;
        }

        public void AddNewData(string str)
        {
            if(_bTrim)
                str = str.Trim();

            if (String.IsNullOrEmpty(str))
                return;


            for (int i = 0; i < this.HisItems.Count; i++)
            {
                string strTemp = (string)HisItems[i];
                if (strTemp.ToLower().Equals(str.ToLower()))
                {
                    if (i == 0) return;

                    HisItems.RemoveAt(i);
                    break;
                }
            }

            HisItems.Insert(0, str);


            for (int i = this.HisItems.Count - 1; i >= this._max_list_count; i--)
            {
                HisItems.RemoveAt(i);
            }
        }

        public void ShowHisMenu(Control form, Control btn)
        {
            this.ctxHisMenu.Items.Clear();


            for (int i = 0; i < this.HisItems.Count; i++)
            {
                string strTemp = ((string)HisItems[i]).Trim();

                ToolStripMenuItem item = new ToolStripMenuItem();

                item.Name = "item1";

                strTemp = strTemp.Replace("\r\n", " | ");

                if (strTemp.Length > 25)
                    item.Text = strTemp.Substring(0, 25) + "...";
                else
                    item.Text = strTemp;

                item.ToolTipText = strTemp;

                item.Tag = i;

                item.Click += new System.EventHandler(this.MenuItem_Click);

                this.ctxHisMenu.Items.Add(item);
            }

            //this.ContextMenuStrip = this.ctxHisMenu;

            if(btn==null)
                this.ctxHisMenu.Show(form.PointToScreen(new Point(this.Left, this.Top)));
            else
                this.ctxHisMenu.Show(form.PointToScreen(new Point(btn.Left, btn.Bottom)));
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            this.Text = (string)this.HisItems[(int)item.Tag];
        }
    }

}
