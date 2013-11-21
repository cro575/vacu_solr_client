using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;


using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Configuration.Schema;
using org.apache.solr.SolrSharp.Update;
using org.apache.solr.SolrSharp.Indexing;
using org.apache.solr.SolrSharp.Query;
using org.apache.solr.SolrSharp.Query.Parameters;
using org.apache.solr.SolrSharp.Query.Highlights;
using org.apache.solr.SolrSharp.Results;

using XairStone;
using XairStone.SimpleLogger;
using System.Net;

namespace VacuSolrUploader
{
    public partial class SolrUploadForm : Form
    {
        private Logger mLogger;
        private TextBoxLogger textBoxLogger;

        bool bStart = false;
        bool bCancel = false;

        public SolrUploadForm()
        {
            InitializeComponent();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            mLogger.UnRegisterObserver(textBoxLogger);

            Program.config.solrUploadCfg.InputFolders = this.cbInputFolder.GetCfg();
            Program.config.solrUploadCfg.checkSubFolder = this.chkSubFolder.Checked.ToString();
            Program.config.solrUploadCfg.txtCommand = this.txtCommand.Text.Trim();

            SolrConnectCfg con = (SolrConnectCfg)this.cbSolrcfg.SelectedItem;
            if (con != null)
                Program.config.solrUploadCfg.solrConnectName = con.name;

            Program.config.writeSolrUploadCfg();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            mLogger = Logger.Instance;

            textBoxLogger = new TextBoxLogger(this.logEdit);
            mLogger.RegisterObserver(textBoxLogger);

            Program.config.readSolrUploadCfg();

            this.cbInputFolder.SetCfg(Program.config.solrUploadCfg.InputFolders);
            this.chkSubFolder.Checked = Program.config.solrUploadCfg.checkSubFolder.Equals("True");
            this.txtCommand.Text = Program.config.solrUploadCfg.txtCommand;

            Program.config.readSolrConnectCfg();
            this.cbSolrcfg.SetCfg(Program.config.solrConnectListCfg, Program.config.solrUploadCfg.solrConnectName);
        }

        private void btnInputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.cbInputFolder.Text;
            dlg.ShowNewFolderButton = false;

            if (dlg.ShowDialog() == DialogResult.OK)
                this.cbInputFolder.Text = dlg.SelectedPath;
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            SolrConnectCfgForm dlg = new SolrConnectCfgForm();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                SolrConnectCfg con = (SolrConnectCfg)this.cbSolrcfg.SelectedItem;
                if (con != null)
                    Program.config.solrUploadCfg.solrConnectName = con.name;

                this.cbSolrcfg.SetCfg(Program.config.solrConnectListCfg, Program.config.solrUploadCfg.solrConnectName);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.bCancel = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ProcessUpload(0);

        }

        private void btnStartFile_Click(object sender, EventArgs e)
        {
            ProcessUpload(1);
        }

        private void ProcessState(bool bStart)
        {
            if (bStart)
            {
                this.bStart = true;
                this.bCancel = false;
            }
            else
            {
                this.bStart = false;
                this.bCancel = false;
            }

            this.btnStart.Enabled = !bStart;
            this.btnStartFile.Enabled = !bStart;
            this.btnStop.Enabled = bStart;
        }

        private void ProcessUpload(int nType)
        {
            SolrConnectCfg con = (SolrConnectCfg)this.cbSolrcfg.SelectedItem;
            if (con == null)
            {
                MessageBox.Show("솔라접속정보를 선택하세요");
                return;
            }

            string InputFolder = this.cbInputFolder.Text.Trim();

            ArrayList files = new ArrayList();
            if (nType == 0)
            {
                if (string.IsNullOrEmpty(InputFolder))
                {
                    MessageBox.Show("대상폴더를 지정하지 않았습니다.");
                    return;
                }

                if (!Directory.Exists(InputFolder))
                {
                    MessageBox.Show(string.Format("'{0}' 대상폴더는 존재하지 않았습니다.", InputFolder));
                    return;
                }


                SearchOption searchOption = SearchOption.TopDirectoryOnly;
                if (this.chkSubFolder.Checked)
                {
                    searchOption = SearchOption.AllDirectories;
                }

                string[] find_files = System.IO.Directory.GetFiles(InputFolder, "*.xml", searchOption);
                foreach (string fullname in find_files)
                    files.Add(fullname);

                //DirectoryInfo directoryInfo = new DirectoryInfo(InputFolder);
                //foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.xml"))
                //    files.Add(fileInfo.FullName);
            }
            else
            {

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "대상 xml 파일을 선택하세요.";
                dlg.Filter = "대상파일(*.xml)|*.xml|모든 파일(*.*)|*.*";
                dlg.Multiselect = true;

                dlg.InitialDirectory = Program.config.solrUploadCfg.fileOpenInitFolder;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fullname in dlg.FileNames)
                        files.Add(fullname);
                }
                else
                    return;

                Program.config.solrUploadCfg.fileOpenInitFolder = dlg.InitialDirectory;
            }

            this.cbInputFolder.CurrentUpdate();

            ArrayList errMsgList = new ArrayList();

            mLogger.Log("");
            mLogger.Log("############################### XML 파일업로드 시작 ###############################");
            mLogger.Log("InputFolder : " + InputFolder);
            mLogger.Log("solrUploadUrl : " + con.solrUploadUrl.ToString());
            mLogger.Log("checkSecure : " + con.checkSecure.ToString());
            mLogger.Log("loginID : " + con.loginID.ToString());
            mLogger.Log("loginPW : " + con.loginPW.ToString());
            mLogger.Log("");

            int totalFileCnt = 0;
            int successFileCnt = 0;

            try
            {
                ProcessState(true);


                SolrSearcher solrSearcher = new SolrSearcher(con.solrUploadUrl, Mode.Read, false);

                SolrSearcher.bSecure_Connet = con.checkSecure.ToLower().Equals("true");
                SolrSearcher.loginID = con.loginID;
                SolrSearcher.loginPW = con.loginPW;

                totalFileCnt = files.Count;

                int count = 0;
                string msg;
                foreach (string fullname in files)
                {
                    if (this.bCancel)
                    {
                        mLogger.Log(Logger.Level.Error, "====사용자의 작업중지 요청으로 작업을 중지합니다.===");
                        throw new Exception("====사용자의 작업중지 요청으로 작업을 중지합니다.===");
                    }

                    try
                    {
                        SolrUpdater oUpdate = new SolrUpdater(solrSearcher);
                        HttpStatusCode retCode = oUpdate.PostToFile(fullname, true);

                        if (retCode == HttpStatusCode.OK)
                        {
                            successFileCnt++;
                            mLogger.Log(string.Format("{0}/{1} {2} {3}.", count + 1, totalFileCnt, Path.GetFileName(fullname), retCode.ToString()));
                        }
                        else
                        {
                            msg = string.Format("{0}/{1} {2}  Fail. (Cause:{3} {4})", count + 1, totalFileCnt, Path.GetFileName(fullname), retCode.ToString(), oUpdate.statusDesc);
                            mLogger.Log(Logger.Level.Error, msg);
                            errMsgList.Add(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        msg = string.Format("{0}/{1} {2}  Fail. (Cause:{3})", count + 1, totalFileCnt, Path.GetFileName(fullname), ex.Message);

                        mLogger.Log(Logger.Level.Error, msg);
                        errMsgList.Add(msg);
                    }

                    count++;

                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                errMsgList.Add(ex.Message);
            }

            finally
            {
                ProcessState(false);
            }


            mLogger.Log(string.Format("{0}/{1} 파일 전송을 완료 하였습니다. (오류건수:{2})", successFileCnt, totalFileCnt, errMsgList.Count));

            if (errMsgList.Count > 0)
            {
                mLogger.Log(errMsgList.Count + " 건의 처리오류가 발생");

                for (int i = 0; i < Math.Min(10, errMsgList.Count); i++)
                    mLogger.Log(string.Format("오류{0} : {1} ", i + 1, errMsgList[i]));
            }
        }

        private void btnStartTxt_Click(object sender, EventArgs e)
        {
            SolrConnectCfg con = (SolrConnectCfg)this.cbSolrcfg.SelectedItem;
            if (con == null)
            {
                MessageBox.Show("솔라접속정보를 선택하세요");
                return;
            }

            string txtCmd = this.txtCommand.Text.Trim();
            if(txtCmd.Length<=0)
            {
                mLogger.Log(string.Format("명령어를 입력하세요."));
                return;
            }

            mLogger.Log("");
            mLogger.Log("############################### 명령어 전송 ###############################");
            mLogger.Log("solrUploadUrl : " + con.solrUploadUrl.ToString());
            mLogger.Log("checkSecure : " + con.checkSecure.ToString());
            mLogger.Log("loginID : " + con.loginID.ToString());
            mLogger.Log("loginPW : " + con.loginPW.ToString());
            mLogger.Log("");


            SolrSearcher solrSearcher = new SolrSearcher(con.solrUploadUrl, Mode.Read, false);

            SolrSearcher.bSecure_Connet = con.checkSecure.ToLower().Equals("true");
            SolrSearcher.loginID = con.loginID;
            SolrSearcher.loginPW = con.loginPW;

            try
            {
                SolrUpdater oUpdate = new SolrUpdater(solrSearcher);
                HttpStatusCode retCode = oUpdate.ExecuteCommand(txtCmd,true);

                if (retCode == HttpStatusCode.OK)
                {
                    mLogger.Log(string.Format("command {0}.",retCode.ToString()));
                }
                else
                {
                    mLogger.Log(Logger.Level.Error, string.Format("command  Fail. (Cause:{0} {1})",retCode.ToString(), oUpdate.statusDesc));
                }
            }
            catch (Exception ex)
            {
                mLogger.Log(Logger.Level.Error, ex.Message);
            }
        }

        private void btnSolrCmd_Click(object sender, EventArgs e) {

            System.Windows.Forms.ContextMenuStrip ctxCmdMenu = new System.Windows.Forms.ContextMenuStrip();

            var items = new[]
            {
                new { tit = "삭제예(id)", tag = "<delete><id>SP2514N</id></delete>" },
                new { tit = "삭제예(Query)", tag = "<delete><query>name:DDR</query></delete>" },
                new { tit = "삭제예(전체)", tag = "<delete><query>*:*</query></delete>" },
                new { tit = "Commit", tag = "<commit/>" },
                new { tit = "Optimize", tag = "<optimize/>" }
            };


            foreach (var item in items) {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Name = item.tag;
                menuItem.Text = item.tit;
                menuItem.ToolTipText = item.tit;
                menuItem.Tag = item.tag;
                menuItem.Click += new System.EventHandler(this.CmdMenuItem_Click);
                ctxCmdMenu.Items.Add(menuItem);
            }

            //this.ContextMenuStrip = this.ctxCmdMenu;

            ctxCmdMenu.Show(this.PointToScreen(new Point(this.btnSolrCmd.Left, this.btnSolrCmd.Bottom)));
        }

        private void CmdMenuItem_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            this.txtCommand.Text += "\r\n" + (string)item.Tag;
        }

    }
}
