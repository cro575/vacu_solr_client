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

namespace org.apache.solr.SolrSharp.Results
{
    /// <summary>
    /// Provides access to additional debugging information related to a 
    /// <see cref="T:SearchResults">SearchResults</see> instance, including 
    /// "explain" info for each of the documents returned.
    /// </summary>
    public class DebugResults
    {

        /// <summary>
        /// Public constructor for DebugResults, expecting an XmlNode with queryable 
        /// information relative to debugging information for a query.
        /// </summary>
        /// <param name="xnode">XmlNode reference to populate this instance</param>
        public DebugResults(XmlNode xnode)
        {
            XmlNode xnqs = SolrSearcher.GetXmlNode(xnode, "str[@name='querystring']");
            if (xnqs != null)
            {
                this._queryString = xnqs.InnerText;
            }
            XmlNode xnpq = SolrSearcher.GetXmlNode(xnode, "str[@name='parsedquery']");
            if (xnpq != null)
            {
                this._parsedQuery = xnpq.InnerText;
            }
            XmlNode xnoq = SolrSearcher.GetXmlNode(xnode, "str[@name='otherQuery']");
            if (xnoq != null)
            {
                this._otherQuery = xnoq.InnerText;
            }

            XmlNodeList xnlExplain = SolrSearcher.GetXmlNodes(xnode, "lst[@name='explain']/str");
            foreach (XmlNode xnExplain in xnlExplain)
            {
                this._explanationRecords.Add(new ExplanationRecord(xnExplain));
            }

            XmlNodeList xnlExplainOther = SolrSearcher.GetXmlNodes(xnode, "lst[@name='explainOther']/str");
            foreach (XmlNode xnExplainOther in xnlExplainOther)
            {
                this._otherqueryExplanationRecords.Add(new ExplanationRecord(xnExplainOther));
            }

        }

        private string _queryString = string.Empty;
        /// <summary>
        /// The query as applied to the referencing 
        /// <see cref="T:SearchResults">SearchResults</see> instance as well 
        /// as the <see cref="ExplanationRecord">ExplanationRecords</see> array.
        /// </summary>
        public string QueryString
        {
            get { return this._queryString; }
        }

        private string _parsedQuery = string.Empty;
        /// <summary>
        /// The underlying executed query, as actually executed
        /// </summary>
        public string ParsedQuery
        {
            get { return this._parsedQuery; }
        }

        private List<ExplanationRecord> _explanationRecords = new List<ExplanationRecord>();
        /// <summary>
        /// Array of ExplanationRecord objects containing detail information 
        /// about debugging and analysis information as it relates to each 
        /// <see cref="T:SearchRecord">SearchRecord</see> in the referencing
        /// <see cref="T:SearchResults">SearchResults</see> instance.
        /// </summary>
        public ExplanationRecord[] ExplanationRecords
        {
            get { return this._explanationRecords.ToArray(); }
        }

        private string _otherQuery = string.Empty;
        /// <summary>
        /// If <see cref="T:SearchResults.ExplainOtherQuery">ExplainOtherQuery</see> is 
        /// added to the referencing <see cref="T:SearchResults">SearchResults</see> 
        /// instance, this is populated with the executed query.
        /// </summary>
        public string OtherQuery
        {
            get { return this._otherQuery; }
        }

        private List<ExplanationRecord> _otherqueryExplanationRecords = new List<ExplanationRecord>();
        /// <summary>
        /// Array of ExplanationRecord objects containing detail information 
        /// about debugging and analysis information as it relates to 
        /// <see cref="T:SearchResults.ExplainOtherQuery">ExplainOtherQuery</see> in 
        /// the referencing <see cref="T:SearchResults">SearchResults</see> instance.
        /// </summary>
        public ExplanationRecord[] OtherQueryExplanationRecords
        {
            get { return this._otherqueryExplanationRecords.ToArray(); }
        }

    }
}
