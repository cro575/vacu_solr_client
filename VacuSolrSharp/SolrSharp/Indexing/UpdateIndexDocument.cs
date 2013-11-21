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
using System.Xml.Serialization;

namespace org.apache.solr.SolrSharp.Indexing
{
    /// <summary>
    /// Abstracted class for updating or adding to a solr index.  For a specific index,
    /// a class should inherit from UpdateIndexDocument.
    /// </summary>
	[Serializable]
	[XmlRoot("add")]
	public abstract class UpdateIndexDocument: IndexDocument
	{
		private List<IndexFieldValue> _listFieldValues = new List<IndexFieldValue>();

        /// <summary>
        /// Empty constructor, required by Xml serialization under .Net
        /// </summary>
        public UpdateIndexDocument()
		{
		}

        /// <summary>
        /// Adds an IndexFieldValue object to the internal collection
        /// </summary>
        /// <param name="fieldValue">IndexFieldValue object containing the fieldname and value</param>
		public virtual void Add(IndexFieldValue fieldValue)
		{
			this._listFieldValues.Add(fieldValue);
		}
        /// <summary>
        /// Adds a fieldname/value pair as an IndexFieldValue object to the internal collection
        /// </summary>
        /// <param name="fieldName">Solr index fieldname for this value</param>
        /// <param name="fieldValue">Value for this solr index fieldname</param>
		public virtual void Add(string fieldName, string fieldValue)
		{
			IndexFieldValue fieldvalue = new IndexFieldValue(fieldName, fieldValue);
			this.Add(fieldvalue);
		}

        /// <summary>
        /// The collection of fieldname/value pair objects to be serialized
        /// as a part of this document
        /// </summary>
		[XmlArray("doc")]
		[XmlArrayItem("field", Type = typeof(IndexFieldValue))]
		public IndexFieldValue[] FieldValues
		{
			get { return this._listFieldValues.ToArray(); }
			set
			{
				this._listFieldValues.Clear();
				this._listFieldValues.AddRange(value);
			}
		}

	}
}
