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
    /// HighlightRecord is a container objecty for nodes in a solr search results
    /// payload pertaining to highlight parameters. A HighlightRecord is populated
    /// in the initialization of search results, but are used in updating standard
    /// result records.  HighlightRecord instances are used by the SearchResults
    /// class directly, and are not intended for general use.
    /// </summary>
    public class HighlightRecord
    {

        /// <summary>
        /// The XmlNode as used to construct this instance of HighlightRecord.
        /// </summary>
        public XmlNode XNodeRecord { get; private set; }

        /// <summary>
        /// Public constructor that accepts an XmlNode that defines the HighlightRecord.
        /// </summary>
        /// <param name="xn">XmlNode representing this HighlightRecord</param>
        public HighlightRecord(XmlNode xn)
        {
            this.Initialize(xn);
        }

        private void Initialize(XmlNode xn)
        {
            this.XNodeRecord = xn;
            XmlAttribute xaRecordId = this.XNodeRecord.Attributes["name"];
            if (xaRecordId != null)
            {
                this.RecordId = xaRecordId.InnerText;
            }
        }

        /// <summary>
        /// The associated record Id that relates this HighlightRecord to a SearchRecord.
        /// The value is based on the UniqueKey field value for the SearchRecord.
        /// </summary>
        public string RecordId { get; private set; }

        /// <summary>
        /// Gets the highlighted phrases for this record.
        /// </summary>
        /// <param name="fieldname">the search index fieldname on which highlighting is applied</param>
        /// <returns>string array of phrases for this field, with highlighting applied</returns>
        public string[] GetHighlightedPhrases(string fieldname)
        {
            XmlNodeList xnlPhrases = SolrSearcher.GetXmlNodes(this.XNodeRecord, string.Format("arr[@name='{0}']/str", fieldname));
            List<string> listPhrases = new List<string>();
            foreach (XmlNode xn in xnlPhrases)
            {
                listPhrases.Add(xn.InnerText);
            }
            return listPhrases.ToArray();
        }

    }
}
