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
using System.Collections.Generic;
using System.Xml;

namespace org.apache.solr.SolrSharp.Configuration.Schema
{
    /// <summary>
    /// CopyField nodes in a solr schema instruct Solr to duplicate any data 
    /// added to the index by copying the value(s) of the "source" field to 
    /// the "dest" field of that document.
    /// </summary>
    public class SolrCopyField
    {

        /// <summary>
        /// Constructs an instance of a copy field.
        /// </summary>
        /// <param name="xnCopyField">XmlNode representing the definition of a single copy field</param>
        /// <param name="solrFields">The possible source fields for this schema, as defined in SolrSchema</param>
        public SolrCopyField(XmlNode xnCopyField, IEnumerable<SolrField> solrFields)
        {
            string fld_source = xnCopyField.Attributes["source"].Value;
            string fld_dest = xnCopyField.Attributes["dest"].Value;

            foreach (SolrField solrField in solrFields)
            {
                if (solrField.Name == fld_source)
                {
                    this.SourceField = solrField;
                }
                else if (solrField.Name == fld_dest)
                {
                    this.DestinationField = solrField;
                }
            }
        }

        /// <summary>
        /// SolrField mapped to the source field
        /// </summary>
        public SolrField SourceField { get; private set; }

        /// <summary>
        /// SolrField mapped to the destination field
        /// </summary>
        public SolrField DestinationField { get; private set; }
    }
}
