using System;
using System.Xml;
using System.Reflection;
using System.Configuration;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Text;

using XairStone;

namespace VacuSolrUploader
{
    public class PairRec
    {
        public string val1 = "";
        public string val2 = "";
        public string val3 = "";
        public string val4 = "";

        public PairRec(string val1, string val2)
        {
            this.val1 = val1;
            this.val2 = val2;
            this.val3 = "";
            this.val4 = "";
        }
        public PairRec(string val1, string val2, string val3)
        {
            this.val1 = val1;
            this.val2 = val2;
            this.val3 = val3;
            this.val4 = "";
        }
        public PairRec(string val1, string val2, string val3, string val4)
        {
            this.val1 = val1;
            this.val2 = val2;
            this.val3 = val3;
            this.val4 = val4;
        }
    }

    public class SolrUploadCfg
    {
        public ArrayList InputFolders = null;
        public string checkSubFolder = "";
        public string fileOpenInitFolder = "";
        public string txtCommand = "";
        public string solrConnectName = "";
    }

    public class SolrConnectCfg
    {
        public string name = "";
        public string solrUploadUrl = ""; //http://localhost:8983/solr/collection/update
        public string checkSecure = "";
        public string loginID = "";
        public string loginPW = "";

        public string DisplayMember
        {
            get { return name; }
        }
    }

    public class Config
    {
        ConfigReader reader = null;

        public SolrUploadCfg solrUploadCfg = new SolrUploadCfg();
        public ArrayList solrConnectListCfg = new ArrayList();

        public Config()
        {
            reader = new ConfigReader();
            reader.cfgFile = "app.config";

            reader.CheckCfigFile();

            readConfig();
        }

        public void readConfig()
        {
        }

        public void writeConfig()
        {
        }

        public void readSolrUploadCfg()
        {
            this.solrUploadCfg.InputFolders = reader.GetValueList("//solrUploadCfg/add[@key='InputFolders']", "");
            this.solrUploadCfg.checkSubFolder = reader.GetValue("//solrUploadCfg/add[@key='checkSubFolder']", "", "txt").Trim();
            this.solrUploadCfg.fileOpenInitFolder = reader.GetValue("//solrUploadCfg/add[@key='fileOpenInitFolder']", "", "txt").Trim();
            this.solrUploadCfg.txtCommand = reader.GetValue("//solrUploadCfg/add[@key='txtCommand']", "", "txt").Trim();
            this.solrUploadCfg.solrConnectName = reader.GetValue("//solrUploadCfg/add[@key='solrConnectName']", "", "txt").Trim();
        }

        public void writeSolrUploadCfg()
        {
            reader.SetValue("//solrUploadCfg/add[@key='InputFolders']", this.solrUploadCfg.InputFolders);
            reader.SetValue("//solrUploadCfg/add[@key='checkSubFolder']", this.solrUploadCfg.checkSubFolder, "txt");
            reader.SetValue("//solrUploadCfg/add[@key='fileOpenInitFolder']", this.solrUploadCfg.fileOpenInitFolder, "txt");
            reader.SetValue("//solrUploadCfg/add[@key='txtCommand']", this.solrUploadCfg.txtCommand, "txt");
            reader.SetValue("//solrUploadCfg/add[@key='solrConnectName']", this.solrUploadCfg.solrConnectName, "txt");
        }

        public void readSolrConnectCfg()
        {
            this.solrConnectListCfg.Clear();

            XmlDocument doc = new XmlDocument();
            reader.loadDoc(doc);

            XmlNodeList nodes = doc.SelectNodes("//SolrConnectList/SolrConnect");
            foreach (XmlElement node in nodes)
            {
                SolrConnectCfg cfg = new SolrConnectCfg();

                cfg.name = node.GetAttribute("name").Trim();
                if (String.IsNullOrEmpty(cfg.name))
                    continue;

                cfg.solrUploadUrl = node.GetAttribute("solrUploadUrl").Trim();
                cfg.checkSecure = node.GetAttribute("checkSecure").Trim();
                cfg.loginID = node.GetAttribute("loginID").Trim();
                cfg.loginPW = node.GetAttribute("loginPW").Trim();

                this.solrConnectListCfg.Add(cfg);
            }
        }

        public void writeSolrConnectCfg()
        {
            XmlDocument doc = new XmlDocument();
            reader.loadDoc(doc);

            XmlNode node = doc.SelectSingleNode("//SolrConnectList");
            if (node == null)
            {
                node = doc.CreateElement("SolrConnectList");
                doc.DocumentElement.AppendChild(node);
            }

            node.RemoveAll();

            foreach (SolrConnectCfg cfg in this.solrConnectListCfg)
            {
                XmlElement nodeItem = doc.CreateElement("SolrConnect");
                nodeItem.SetAttribute("name", cfg.name);
                nodeItem.SetAttribute("solrUploadUrl", cfg.solrUploadUrl);
                nodeItem.SetAttribute("checkSecure", cfg.checkSecure);
                nodeItem.SetAttribute("loginID", cfg.loginID);
                nodeItem.SetAttribute("loginPW", cfg.loginPW);

                node.AppendChild(nodeItem);
            }

            reader.saveDoc(doc, reader.cfgFile);
        }

        public void removeSolrConnectCfg(string name)
        {
            for (int i = 0; i < this.solrConnectListCfg.Count; i++)
            {
                if (((SolrConnectCfg)(this.solrConnectListCfg[i])).name.Equals(name))
                {
                    this.solrConnectListCfg.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
