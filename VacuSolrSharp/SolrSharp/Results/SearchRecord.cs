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
using System.Reflection;
using System.Xml;
using org.apache.solr.SolrSharp.Indexing;

namespace org.apache.solr.SolrSharp.Results
{
    // yskwun 20131008
    public interface ISolrRecord
    {
        void ParaseRecord(XmlNode xnode);
    }

    /// <summary>
    /// SearchRecord is an abstraction to represent a search result for a given search request.
    /// A collection of SearchRecord objects are provided from the SearchResults class.
    /// 
    /// Inheriting classes must implement the Initialize() method, which is called by the
    /// SearchResults class; see the <see cref="T:SearchResults.ExecuteSearch">ExecuteSearch</see> method.
    /// </summary>
    public abstract class SearchRecord : ISolrRecord
	{

        /// <summary>
        /// The XmlNode used to construct this SearchRecord
        /// </summary>
        public XmlNode XNodeRecord { get; private set; }

        public SearchRecord() 
        {
        }

        /// <summary>
        /// Primary constructor used for SearchRecord. Calls Initialize() to populate
        /// the object.
        /// </summary>
        /// <param name="xnode"></param>
		public SearchRecord(XmlNode xnode)
		{
            ParaseRecord(xnode);
		}

        //yskwun 20131008
        public void ParaseRecord(XmlNode xnode) {
            this.XNodeRecord = xnode;
            this.Initialize();
        }

        /// <summary>
        /// Base method that populates the object via reflection.
        /// </summary>
        private void Initialize()
        {
            Type type = this.GetType();
            IndexFieldAttribute indexFieldAttribute = null;
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                indexFieldAttribute = (IndexFieldAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(IndexFieldAttribute));
                if (indexFieldAttribute == null) continue;
                indexFieldAttribute.PropertyInfo = propertyInfo;
                indexFieldAttribute.SetValue(this);
            }

        }

	}
}
