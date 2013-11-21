using System;
using System.Collections;
using System.Collections.Generic;


namespace VacuSolrUploader
{
    class SolrCfgComboBox : System.Windows.Forms.ComboBox
    {
        public void SetCfg(ArrayList cfgList, string activeName)
        {
            this.DisplayMember = "DisplayMember";

            this.Items.Clear();

            int ix = 0;
            int sel_idx = 0;
            foreach (SolrConnectCfg cfg in cfgList)
            {
                this.Items.Add(cfg);
                if (cfg.name.Equals(activeName))
                    sel_idx = ix;

                ix++;
            }

            if (this.Items.Count > 0)
                this.SelectedIndex = sel_idx;
        }
    }
}
