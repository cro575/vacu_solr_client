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
    /// FacetResults is an abstraction from the details of determining the results from a 
    /// query using facet parameters, and returns the results in an object-based structure.
    /// 
    /// FacetResults requires an implementation utilizing generics. This enables the base class to
    /// build facet result objects directly. Using generics, the base class populates Facet results.  
    /// Inheriting classes need only define how a Facet object is initialized. The base class executes 
    /// those inherited methods as part of a chain of calls from SearchResults.ExecuteSearch.
    /// </summary>
    /// <typeparam name="T">Type-specific object</typeparam>
    public abstract class FacetResults<T>
	{
		private string _fieldname;
        private List<KeyValuePair<T, int>> _facetresults = new List<KeyValuePair<T, int>>();
		private readonly static string XPATH_FIELDNAME = "name";

        /// <summary>
        /// Base constructor for FacetResults, using a solr index facet based on the fieldname
        /// parameter and building the results based on the XmlNode parameter.
        /// </summary>
        /// <param name="fieldname">Solr index fieldname used as a Facet query parameter in a QueryBuilder object</param>
        /// <param name="xn">XmlNode representing the facet value results from this search request</param>
		protected FacetResults(string fieldname, XmlNode xn)
		{
			this._fieldname = fieldname;
            XmlNodeList xnl = SolrSearcher.GetXmlNodes(xn, string.Format("lst[@name='{0}']/int", this._fieldname));
			foreach (XmlNode node in xnl)
			{
				object key = node.Attributes[FacetResults<T>.XPATH_FIELDNAME].Value;
				int value = Convert.ToInt32(node.InnerText);
				T facetobject = this.InitFacetObject(key);
                if (facetobject != null)
                {
                    this._facetresults.Add(new KeyValuePair<T, int>(facetobject, value));
                }
			}

		}

        /// <summary>
        /// This method is called by the object constructor in instantiating type-specific objects
        /// of type T.  This constructs the collection of Facets automatically, using the definition 
        /// in the inherited object.  Required implementation by the inheriting class.
        /// 
        /// The passed object "key" is the solr index field value. This value, typically an int or string,
        /// should be used to construct an object of type T.
        /// </summary>
        /// <param name="key">object value that can be used to initialize an object of type T</param>
        /// <returns>object of type T</returns>
		protected abstract T InitFacetObject(object key);

        /// <summary>
        /// Collection of KeyValuePair objects using the type-specific T as key, with the
        /// number of results for that facet as the value.
        /// </summary>
		public List<KeyValuePair<T, int>> Facets
		{
			get { return this._facetresults; }
		}

	}
}
