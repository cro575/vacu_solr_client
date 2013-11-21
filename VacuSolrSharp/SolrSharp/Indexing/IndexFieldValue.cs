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
using System.Xml.Serialization;

namespace org.apache.solr.SolrSharp.Indexing
{
    /// <summary>
    /// A name/value pair included in an UpdateIndexDocument
    /// </summary>
	[Serializable]
	[XmlRoot("field")]
	public class IndexFieldValue
	{

        /// <summary>
        /// Empty constructor, required by Xml serialization under .Net
        /// </summary>
        public IndexFieldValue()
        {
        }

        /// <summary>
        /// Simple constructor to create one name/value pair for an IndexDocument.
        /// Standard constructor for use by your application.
        /// </summary>
        /// <param name="name">The solr index fieldname for this field</param>
        /// <param name="value">The value for this field</param>
		public IndexFieldValue(string name, string value)
		{
            this.Name = name;
            this.Value = value;
		}

        /// <summary>
        /// The solr index fieldname for this field
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The value for this field
        /// </summary>
        [XmlText]
        public string Value { get; set; }
	}
}
