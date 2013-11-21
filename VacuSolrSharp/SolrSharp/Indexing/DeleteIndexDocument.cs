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
    /// DeleteIndexDocument is used to remove one or more index documents from a solr index. Remove
    /// items from an index by passing an instance of DeleteIndexDocument to 
    /// <see cref="T:SearchUpdate.PostToIndex">SearchUpdate.PostToIndex()</see>
    /// </summary>
	[Serializable]
	[XmlRoot("delete", Namespace="")]
	public sealed class DeleteIndexDocument : IndexDocument
	{

        /// <summary>
        /// Empty constructor, required by Xml serialization under .Net
        /// </summary>
		public DeleteIndexDocument()
		{
		}

        /// <summary>
        /// Creates a delete document object that can be used to remove a single index document
        /// from a solr index.  The item removed is based on the uniqueKeyId.  When used,
        /// this key will be applied to the field identified as the "uniqueKey", found in
        /// the schema.xml file for a solr webapp instance.
        /// </summary>
        /// <param name="uniqueKeyId">object representing the unique id for a solr index schema</param>
		public DeleteIndexDocument(object uniqueKeyId)
		{
            this.Id = uniqueKeyId.ToString();
		}

        /// <summary>
        /// Creates a delete document object that can be used to remove one or more index documents
        /// from a solr index.  The item removed is based on the deleteQuery, which is a standard 
        /// Lucene-based query.  Items matching the query would be removed from the index.
        /// </summary>
        /// <param name="query">Query object containing the query to be applied for this delete document</param>
		public DeleteIndexDocument(Query.Query query)
		{
            this.Query = query.ToString();
		}

        /// <summary>
        /// The id to use, if populated, in finding documents to delete
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// The query to be used, if populated, in finding documents to delete
        /// </summary>
        [XmlElement("query")]
        public string Query { get; set; }

	}
}
