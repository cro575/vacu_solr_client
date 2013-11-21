//
//   Licensed to the Apache Software Foundation (ASF) under one or more
//   contributor license agreements.  See the NOTICE file distributed with
//   this work for additional information regarding copyright ownership.
//   The ASF licenses this file to You under the Apache License, Version 2.0
//   (the "License"); you may not use this file except in compliance with
//   the License.  You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Configuration.Schema;

namespace org.apache.solr.SolrSharp.Results
{
    /// <summary>
    /// Provides the underlying explanation for scoring using the given query 
    /// parameters as it relates to specific documents returned in the 
    /// <see cref="T:SearchResults">SearchResults</see> instance.
    /// </summary>
    public class ExplanationRecord
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xnode"></param>
        public ExplanationRecord(XmlNode xnode)
        {
            if (xnode.Attributes["name"] != null)
            {
                this._name = xnode.Attributes["name"].InnerText;
                string[] arName = this._name.Split(",".ToCharArray());
                SolrSearcher solrSearcher = SolrSearchers.GetSearcher(Mode.Read);
                if (arName[0].StartsWith(solrSearcher.SolrSchema.UniqueKey + "="))
                {
                    string[] arKey = arName[0].Split("=".ToCharArray());
                    this._uniqueRecordKey = arKey[1];
                }
            }

            this._explainInfo = xnode.InnerText;
            string[] splitter = new string[] { "  " };
            List<string> listinfo = new List<string>(this._explainInfo.Split(splitter, StringSplitOptions.None));
            for (int x = 0; x < listinfo.Count; x++)
            {
                if (listinfo[x] == "")
                {
                    listinfo[x] += "  ";
                }
                else
                {
                    listinfo[x] += Environment.NewLine + "  ";
                }
            }
            this._explainInfo = string.Join("", listinfo.ToArray());
        }

        private string _name = string.Empty;
        /// <summary>
        /// Name of this record
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        private string _explainInfo = string.Empty;
        /// <summary>
        /// The explanation information for this record
        /// </summary>
        public string ExplainInfo
        {
            get { return this._explainInfo; }
        }

        /// <summary>
        /// Returns ExplainInfo for rendering in HTML format.
        /// </summary>
        /// <returns>string suitable for HTML representation</returns>
        public string GetExplainInfoAsHTML()
        {
            string explainhtml = this.ExplainInfo.Replace(Environment.NewLine, "<br />");
            explainhtml = explainhtml.Replace(" ", "&nbsp;");
            return explainhtml;
        }

        private string _uniqueRecordKey = string.Empty;
        /// <summary>
        /// The value of the UniqueKey, as defined by the solr schema, and
        /// the corresponding record in the SearchResults SearchRecord array
        /// </summary>
        public string UniqueRecordKey
        {
            get { return this._uniqueRecordKey; }
        }
    }
}
