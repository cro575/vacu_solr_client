namespace VacuSolrUploader
{
    partial class SolrUploadForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkSubFolder = new System.Windows.Forms.CheckBox();
            this.btnInputFolder = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.logEdit = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStartFile = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.btnStartTxt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSolrcfg = new VacuSolrUploader.SolrCfgComboBox();
            this.cbInputFolder = new XairStone.CfgComboBox();
            this.btnSolrCmd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkSubFolder
            // 
            this.chkSubFolder.AutoSize = true;
            this.chkSubFolder.Location = new System.Drawing.Point(71, 65);
            this.chkSubFolder.Name = "chkSubFolder";
            this.chkSubFolder.Size = new System.Drawing.Size(96, 16);
            this.chkSubFolder.TabIndex = 26;
            this.chkSubFolder.Text = "하위폴더포함";
            this.chkSubFolder.UseVisualStyleBackColor = true;
            // 
            // btnInputFolder
            // 
            this.btnInputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInputFolder.Location = new System.Drawing.Point(525, 37);
            this.btnInputFolder.Name = "btnInputFolder";
            this.btnInputFolder.Size = new System.Drawing.Size(89, 23);
            this.btnInputFolder.TabIndex = 25;
            this.btnInputFolder.Text = "찾아보기...";
            this.btnInputFolder.UseVisualStyleBackColor = true;
            this.btnInputFolder.Click += new System.EventHandler(this.btnInputFolder_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 43);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 24;
            this.label10.Text = "대상폴더";
            // 
            // logEdit
            // 
            this.logEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logEdit.Location = new System.Drawing.Point(12, 299);
            this.logEdit.Multiline = true;
            this.logEdit.Name = "logEdit";
            this.logEdit.ReadOnly = true;
            this.logEdit.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logEdit.Size = new System.Drawing.Size(602, 138);
            this.logEdit.TabIndex = 28;
            this.logEdit.WordWrap = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(71, 86);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(101, 26);
            this.btnStart.TabIndex = 29;
            this.btnStart.Text = "폴더업로드시작";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(299, 86);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(71, 26);
            this.btnStop.TabIndex = 30;
            this.btnStop.Text = "중지";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStartFile
            // 
            this.btnStartFile.Location = new System.Drawing.Point(178, 86);
            this.btnStartFile.Name = "btnStartFile";
            this.btnStartFile.Size = new System.Drawing.Size(109, 26);
            this.btnStartFile.TabIndex = 32;
            this.btnStartFile.Text = "선택파일업로드...";
            this.btnStartFile.UseVisualStyleBackColor = true;
            this.btnStartFile.Click += new System.EventHandler(this.btnStartFile_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfig.Location = new System.Drawing.Point(525, 5);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(88, 23);
            this.btnConfig.TabIndex = 33;
            this.btnConfig.Text = "설정...";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // txtCommand
            // 
            this.txtCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommand.Location = new System.Drawing.Point(14, 123);
            this.txtCommand.Multiline = true;
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCommand.Size = new System.Drawing.Size(505, 170);
            this.txtCommand.TabIndex = 34;
            this.txtCommand.WordWrap = false;
            // 
            // btnStartTxt
            // 
            this.btnStartTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartTxt.Location = new System.Drawing.Point(525, 123);
            this.btnStartTxt.Name = "btnStartTxt";
            this.btnStartTxt.Size = new System.Drawing.Size(88, 38);
            this.btnStartTxt.TabIndex = 35;
            this.btnStartTxt.Text = "명령어 전송하기";
            this.btnStartTxt.UseVisualStyleBackColor = true;
            this.btnStartTxt.Click += new System.EventHandler(this.btnStartTxt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(140, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 39;
            this.label1.Text = "솔라접속정보";
            // 
            // cbSolrcfg
            // 
            this.cbSolrcfg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSolrcfg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSolrcfg.FormattingEnabled = true;
            this.cbSolrcfg.Location = new System.Drawing.Point(220, 8);
            this.cbSolrcfg.Name = "cbSolrcfg";
            this.cbSolrcfg.Size = new System.Drawing.Size(299, 20);
            this.cbSolrcfg.TabIndex = 38;
            // 
            // cbInputFolder
            // 
            this.cbInputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbInputFolder.FormattingEnabled = true;
            this.cbInputFolder.Location = new System.Drawing.Point(71, 39);
            this.cbInputFolder.Name = "cbInputFolder";
            this.cbInputFolder.Size = new System.Drawing.Size(448, 20);
            this.cbInputFolder.TabIndex = 27;
            // 
            // btnSolrCmd
            // 
            this.btnSolrCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSolrCmd.Location = new System.Drawing.Point(524, 167);
            this.btnSolrCmd.Name = "btnSolrCmd";
            this.btnSolrCmd.Size = new System.Drawing.Size(89, 25);
            this.btnSolrCmd.TabIndex = 42;
            this.btnSolrCmd.Text = "명령어 예 ▼";
            this.btnSolrCmd.UseVisualStyleBackColor = true;
            this.btnSolrCmd.Click += new System.EventHandler(this.btnSolrCmd_Click);
            // 
            // SolrUploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 447);
            this.Controls.Add(this.btnSolrCmd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbSolrcfg);
            this.Controls.Add(this.btnStartTxt);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.btnStartFile);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.logEdit);
            this.Controls.Add(this.cbInputFolder);
            this.Controls.Add(this.chkSubFolder);
            this.Controls.Add(this.btnInputFolder);
            this.Controls.Add(this.label10);
            this.Name = "SolrUploadForm";
            this.Text = "솔라업로드";
            this.Load += new System.EventHandler(this.Form_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XairStone.CfgComboBox cbInputFolder;
        private System.Windows.Forms.CheckBox chkSubFolder;
        private System.Windows.Forms.Button btnInputFolder;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox logEdit;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStartFile;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Button btnStartTxt;
        private SolrCfgComboBox cbSolrcfg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSolrCmd;
    }
}

