using System;
using System.Xml;
using System.Reflection;
using System.Configuration;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Text;

namespace XairStone
{
	public class ConfigReader : System.Configuration.AppSettingsReader
	{
		private string _cfgFile;

		public string cfgFile
		{
			get	{ return _cfgFile; }
			set	{ _cfgFile= Application.StartupPath + "\\" + value; }
		}

        public void CheckCfigFile()
        {
            if (!File.Exists(cfgFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration></configuration>");
                doc.Save(cfgFile);
            }
        }

        public string GetValue(string key, string defVal)
		{
            XmlDocument doc = new XmlDocument();
            object ro = String.Empty;
            loadDoc(doc);

            return GetValue(doc.DocumentElement, key, defVal);
		}

        public ArrayList GetValueList(string key, string defVal)
        {
            XmlDocument doc = new XmlDocument();
            loadDoc(doc);

            ArrayList val_list = new ArrayList();

            XmlElement nodeEle = (XmlElement)doc.SelectSingleNode(key);
            if (nodeEle == null)
            {
                val_list.Add(defVal);

                return val_list;
            }

            XmlNodeList nodes = nodeEle.SelectNodes("./dataItem");
            foreach (XmlNode node in nodes)
            {
                val_list.Add(node.InnerText);
            }

            return val_list;
        }

        public string GetValue(string key, string defVal, string valType)
        {
            XmlDocument doc = new XmlDocument();
            object ro = String.Empty;
            loadDoc(doc);

            return Convert.ToString(_GetValue(doc.DocumentElement, key, defVal, typeof(string), valType)).Trim();
        }

        public int GetValueInt(string key, int defVal)
        {
            XmlDocument doc = new XmlDocument();
            object ro = String.Empty;
            loadDoc(doc);

            return GetValueInt(doc.DocumentElement, key, defVal);
        }

        public bool GetValueBool(string key, bool defVal)
        {
            XmlDocument doc = new XmlDocument();
            object ro = String.Empty;
            loadDoc(doc);

            return (bool)_GetValue(doc.DocumentElement, key, defVal.ToString(), typeof(bool), "txt");
        }

        public string GetValue(XmlElement eleNode, string key, string defVal)
        {
            return Convert.ToString(_GetValue(eleNode, key, defVal, typeof(string), "txt")).Trim();
        }

        public int GetValueInt(XmlElement eleNode, string key, int defVal)
        {
            return (int)_GetValue(eleNode, key, defVal.ToString(), typeof(int), "txt");
        }

        public object _GetValue(XmlElement eleNode, string key, string defVal, System.Type sType, string valType)
        {
            object ro = defVal;
 
            try
            {
                XmlElement node = (XmlElement)eleNode.SelectSingleNode(key);
                if (node != null)
                {
                    if (valType.Equals("txt"))
                        ro = node.InnerText;
                    else
                        ro = node.GetAttribute("value");
                }

				if (sType == typeof(string))
					return Convert.ToString(ro);
				else if (sType == typeof(bool))
				{
					if (ro.Equals("True") || ro.Equals("False"))
						return Convert.ToBoolean(ro);
					else
						return false;
				}
				else if (sType == typeof(int))
					return Convert.ToInt32(ro);
				else if (sType == typeof(double))
					return Convert.ToDouble(ro);
				else if (sType == typeof(DateTime))
					return Convert.ToDateTime(ro);
				else
					return Convert.ToString(ro);
			}
			catch
			{
				return String.Empty;
			}
		}

        public bool makeFullKeyNode(XmlDocument doc, string key)
        {
            string sNode = key.Substring(0, key.LastIndexOf("/"));
            if (sNode.Length <= 0)
                return false;

            string[] paths = sNode.Split('/');

            string fullpath = "";
            XmlElement parent, root;

            parent = root = doc.DocumentElement;
            int count  = 0;
            foreach (string curpath in paths)
            {
                if(count==0)
                    fullpath += curpath;
                else
                    fullpath += "/" + curpath;
                count++;

                if (String.IsNullOrEmpty(curpath))
                    continue;

                XmlElement ele = (XmlElement)root.SelectSingleNode(fullpath);
                if (ele == null)
                {
                    XmlElement entry = doc.CreateElement(curpath);
                    parent.AppendChild(entry);
                    ele = entry;
                }

                parent = ele;
            }

            return true;
        }

        public bool makeFullKeyNode(XmlElement eleNode, string key)
        {
            XmlDocument doc = eleNode.OwnerDocument;

            string sNode = key.Substring(0, key.LastIndexOf("/"));
            if (sNode.Length <= 0)
                return false;

            string[] paths = sNode.Split('/');

            string fullpath = "";
            XmlElement parent = eleNode;

            int count = 0;
            foreach (string curpath in paths)
            {
                if (count == 0)
                    fullpath += curpath;
                else
                    fullpath += "/" + curpath;
                count++;

                if (String.IsNullOrEmpty(curpath))
                    continue;

                XmlElement ele = (XmlElement)eleNode.SelectSingleNode(fullpath);
                if (ele == null)
                {
                    XmlElement entry = doc.CreateElement(curpath);
                    parent.AppendChild(entry);
                    ele = entry;
                }

                parent = ele;
            }

            return true;
        }

        public bool SetValueInt(string key, int val)
        {
            return SetValue(key, val.ToString());
        }

        public bool SetValue(string key, string val)
		{
			XmlDocument doc = new XmlDocument();
			loadDoc(doc);

            return _SetValue(doc.DocumentElement, key, val, "txt");
		}

        public bool SetValue(string key, string val, string valType)
        {
            XmlDocument doc = new XmlDocument();
            loadDoc(doc);

            return _SetValue(doc.DocumentElement, key, val, valType);
        }

        public bool SetValue(string key, ArrayList val_list)
        {
            if (val_list == null)
                return true;


            XmlDocument doc = new XmlDocument();
            loadDoc(doc);


            XmlElement eleNode = doc.DocumentElement;

            try
            {
                string sNode = "";
                XmlNode node = eleNode;

                // retrieve the target node
                if (key.LastIndexOf("/") > 0)
                {
                    sNode = key.Substring(0, key.LastIndexOf("/"));
                    node = eleNode.SelectSingleNode(sNode);
                    if (node == null)
                    {
                        makeFullKeyNode(eleNode, key);
                        node = eleNode.SelectSingleNode(sNode);
                        if (node == null)
                            return false;
                    }
                }

                // Set element that contains the key
                XmlElement targetElem = (XmlElement)eleNode.SelectSingleNode(key);
                if (targetElem == null)
                {
                    sNode = key.Substring(key.LastIndexOf("/") + 1);

                    targetElem = doc.CreateElement(sNode.Substring(0, sNode.IndexOf("[@")).Trim());
                    sNode = sNode.Substring(sNode.IndexOf("'") + 1);

                    targetElem.SetAttribute("key", sNode.Substring(0, sNode.IndexOf("'")));
                    node.AppendChild(targetElem);
                }

                string key_val = targetElem.GetAttribute("key");
                targetElem.RemoveAll();
                targetElem.SetAttribute("key", key_val);

                foreach (string str in val_list)
                {
                    XmlNode dataItem = doc.CreateElement("dataItem");
                    dataItem.InnerText = str;
                    targetElem.AppendChild(dataItem);
                }

                saveDoc(doc, this._cfgFile);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool SetValue(XmlElement eleNode, string key, string val)
        {
            return _SetValue(eleNode, key, val, "txt");
        }

        public bool _SetValue(XmlElement eleNode, string key, string val, string valType)
        {
            XmlDocument doc = eleNode.OwnerDocument;

            try
            {
                string sNode = "";
                XmlNode node = eleNode;

                // retrieve the target node
                if (key.LastIndexOf("/") > 0)
                {
                    sNode = key.Substring(0, key.LastIndexOf("/"));
                    node = eleNode.SelectSingleNode(sNode);
                    if (node == null)
                    {
                        makeFullKeyNode(eleNode, key);
                        node = eleNode.SelectSingleNode(sNode);
                        if (node == null)
                            return false;
                    }
                }

                // Set element that contains the key
                XmlElement targetElem = (XmlElement)eleNode.SelectSingleNode(key);
                if (targetElem != null)
                {
                    // set new value
                    if (valType.Equals("txt"))
                        targetElem.InnerText = val;
                    else
                        targetElem.SetAttribute("value", val);
                }
                // create new element with key/value pair and add it
                else
                {
					sNode = key.Substring(key.LastIndexOf("/")+1);

                    XmlElement entry = doc.CreateElement(sNode.Substring(0, sNode.IndexOf("[@")).Trim());
                    sNode = sNode.Substring(sNode.IndexOf("'") + 1);

                    entry.SetAttribute("key", sNode.Substring(0, sNode.IndexOf("'")));

                    if (valType.Equals("txt"))
                        entry.InnerText = val;
                    else
                        entry.SetAttribute("value", val);

                    node.AppendChild(entry);
                }

                saveDoc(doc, this._cfgFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

		public void saveDoc (XmlDocument doc, string docPath)
		{
			// save document
			// choose to ignore if web.config since it may cause server sessions interruptions
			if(  this._cfgFile.Equals("web.config") )
				return;
			else
				try
				{
					XmlTextWriter writer = new XmlTextWriter( docPath , null );
					writer.Formatting = Formatting.Indented;
					doc.WriteTo( writer );
					writer.Flush();
					writer.Close();
					return;
				}
				catch
				{}
		}

		public bool removeElement (string key)
		{
			XmlDocument doc = new XmlDocument();
			loadDoc(doc);
			try
			{
				string sNode = key.Substring(0, key.LastIndexOf("//"));
				// retrieve the appSettings node
				XmlNode node =  doc.SelectSingleNode(sNode);
				if( node == null )
					return false;
				// XPath select setting "add" element that contains this key to remove
				XmlNode nd = node.SelectSingleNode(key);
				node.RemoveChild(nd);
				saveDoc(doc, this._cfgFile);
				return true;
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public void loadDoc ( XmlDocument doc )
		{
			// check for type of config file being requested
			/*
			if(  this._cfgFile.Equals("app.config"))
			{
				// use default app.config
				this._cfgFile = ((Assembly.GetEntryAssembly()).GetName()).Name+".exe.config";
			}
			else
				if(  this._cfgFile.Equals("web.config"))
			{
				// use server web.config
				this._cfgFile = System.Web.HttpContext.Current.Server.MapPath("web.config");
			}
			*/
			// load the document
			
			doc.Load(this._cfgFile );
		}

	}
}
