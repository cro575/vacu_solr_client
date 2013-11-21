// Type: XairStone.ConfigForm

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace VacuSolrUploader
{
    public partial class SolrConnectCfgForm : Form
    {
        public SolrConnectCfgForm()
        {
            this.InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Font;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.txtHelp.Text = 
@"솔라접속URL예: http://localhost:8983/solr/collection
이름항목을 중복될 수 없습니다. 이름항목이 비어있는 행은 설정에서 제외됩니다.
";

            Program.config.readSolrConnectCfg();

            foreach(SolrConnectCfg cfg in Program.config.solrConnectListCfg)
            {
                dataGrid.Rows.Add(new object[] { cfg.name, cfg.solrUploadUrl, cfg.checkSecure.ToLower().Equals("true"), cfg.loginID, cfg.loginPW });
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Program.config.solrConnectListCfg.Clear();

            ArrayList cfgList = new ArrayList();
            foreach(DataGridViewRow row in dataGrid.Rows) 
            {
                SolrConnectCfg cfg = new SolrConnectCfg();

                if (row.Cells[0].Value != null) cfg.name = row.Cells[0].Value.ToString().Trim();
                if (row.Cells[1].Value != null) cfg.solrUploadUrl = row.Cells[1].Value.ToString().Trim();
                if (row.Cells[2].Value != null) cfg.checkSecure = row.Cells[2].Value.ToString();
                if (row.Cells[3].Value != null) cfg.loginID = row.Cells[3].Value.ToString().Trim();
                if (row.Cells[4].Value != null) cfg.loginPW = row.Cells[4].Value.ToString().Trim();

                if (String.IsNullOrEmpty(cfg.name))
                {
                    continue;
                }

                if(IsExistName(Program.config.solrConnectListCfg,cfg.name))
                {
                    MessageBox.Show(string.Format("'{0}' 은 중복이름이 존재합니다.",cfg.name));
                    return;
                }
                Program.config.solrConnectListCfg.Add(cfg);
            }

            Program.config.writeSolrConnectCfg();

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        bool IsExistName(ArrayList cfgList,string cfgname)
        {
            foreach (SolrConnectCfg cfg in cfgList)
            {
                if (cfg.name.Equals(cfgname))
                    return true;
            }

            return false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = dataGrid.SelectedRows;

            for (int tmpIndex = 0; tmpIndex < selectedRows.Count; tmpIndex++)
                dataGrid.Rows.Remove(selectedRows[tmpIndex]);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGrid.CurrentRow;
            if (row == null)
                return;

            int rowIndex = row.Index;
            if (rowIndex == 0)
                return;

            dataGrid.Rows.Remove(dataGrid.CurrentRow);
            dataGrid.Rows.Insert(rowIndex - 1, row);

            dataGrid.CurrentCell = row.Cells[0];
            row.Selected = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGrid.CurrentRow;

            if (row == null)
                return;

            int rowIndex = row.Index;
            if (rowIndex >= dataGrid.Rows.Count-2)
                return;

            dataGrid.Rows.Remove(dataGrid.CurrentRow);
            dataGrid.Rows.Insert(rowIndex + 1, row);

            dataGrid.CurrentCell = row.Cells[0];
            row.Selected = true;
        }
    }
}
