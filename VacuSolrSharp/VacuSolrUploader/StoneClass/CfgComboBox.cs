using System;
using System.Collections;
using System.Collections.Generic;


namespace XairStone
{
    class CfgComboBox : System.Windows.Forms.ComboBox
    {
        public bool _bTrim = true;
        public int _max_list_count = 15;

        public void CurrentUpdate()
        {
            AddNewData(this.Text);
        }

        public ArrayList GetCfg()
        {
            ArrayList cfg = new ArrayList();

            CurrentUpdate();
            cfg.AddRange(ToArryList().ToArray());

            return cfg;
        }

        public void SetCfg(ArrayList cfg)
        {
            this.Items.AddRange(cfg.ToArray());

            if (this.Items.Count > 0)
                this.SelectedIndex = 0;
        }

        public ArrayList ToArryList()
        {
            ArrayList al = new ArrayList();

            foreach (string s in this.Items)
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


            for (int i = 0; i < this.Items.Count; i++)
            {
                string strTemp = (string)Items[i];
                if (strTemp.ToLower().Equals(str.ToLower()))
                {
                    if (i == 0) return;

                    Items.RemoveAt(i);
                    break;
                }
            }

            Items.Insert(0, str);


            for (int i = this.Items.Count-1; i >= this._max_list_count; i--)
            {
                Items.RemoveAt(i);
            }

            SelectedIndex = 0;
        }
    }

}
