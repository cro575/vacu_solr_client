using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace VacuSolrUploader
{
    partial class SolrConnectCfgForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private IContainer components;
        private Button btnOK;
        private Button btnCancel;

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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.col_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_url = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_secure = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_loginid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_loginpw = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtHelp = new System.Windows.Forms.TextBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.Location = new System.Drawing.Point(224, 298);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(97, 25);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.Location = new System.Drawing.Point(327, 298);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 25);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dataGrid
            // 
            this.dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_name,
            this.col_url,
            this.col_secure,
            this.col_loginid,
            this.col_loginpw});
            this.dataGrid.Location = new System.Drawing.Point(12, 60);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowTemplate.Height = 23;
            this.dataGrid.Size = new System.Drawing.Size(625, 230);
            this.dataGrid.TabIndex = 28;
            // 
            // col_name
            // 
            this.col_name.HeaderText = "이름";
            this.col_name.Name = "col_name";
            // 
            // col_url
            // 
            this.col_url.HeaderText = "접속URL";
            this.col_url.Name = "col_url";
            this.col_url.Width = 200;
            // 
            // col_secure
            // 
            this.col_secure.FillWeight = 70F;
            this.col_secure.HeaderText = "보안접속";
            this.col_secure.Name = "col_secure";
            this.col_secure.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.col_secure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.col_secure.Width = 80;
            // 
            // col_loginid
            // 
            this.col_loginid.HeaderText = "접속ID";
            this.col_loginid.Name = "col_loginid";
            // 
            // col_loginpw
            // 
            this.col_loginpw.HeaderText = "접속PW";
            this.col_loginpw.Name = "col_loginpw";
            // 
            // txtHelp
            // 
            this.txtHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHelp.Location = new System.Drawing.Point(13, 13);
            this.txtHelp.Multiline = true;
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.ReadOnly = true;
            this.txtHelp.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHelp.Size = new System.Drawing.Size(624, 40);
            this.txtHelp.TabIndex = 29;
            // 
            // btnDel
            // 
            this.btnDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDel.Location = new System.Drawing.Point(101, 298);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(75, 25);
            this.btnDel.TabIndex = 30;
            this.btnDel.Text = "선택행삭제";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUp.Location = new System.Drawing.Point(13, 298);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(38, 25);
            this.btnUp.TabIndex = 31;
            this.btnUp.Text = "▲";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDown.Location = new System.Drawing.Point(57, 298);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(38, 25);
            this.btnDown.TabIndex = 32;
            this.btnDown.Text = "▼";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // SolrConnectCfgForm
            // 
            this.ClientSize = new System.Drawing.Size(649, 333);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.txtHelp);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "SolrConnectCfgForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "솔라접속 환경설정";
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridView dataGrid;
        private DataGridViewTextBoxColumn col_name;
        private DataGridViewTextBoxColumn col_url;
        private DataGridViewCheckBoxColumn col_secure;
        private DataGridViewTextBoxColumn col_loginid;
        private DataGridViewTextBoxColumn col_loginpw;
        private TextBox txtHelp;
        private Button btnDel;
        private Button btnUp;
        private Button btnDown;

    }
}

