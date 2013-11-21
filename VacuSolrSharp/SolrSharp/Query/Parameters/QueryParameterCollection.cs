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

namespace org.apache.solr.SolrSharp.Query.Parameters
{
    /// <summary>
    /// Enumeration defining the methodologies for combining search parameters.
    /// </summary>
	public enum ParameterJoin
	{
        /// <summary>
        /// AND is applied to the contents of the applied query parameters
        /// </summary>
		AND,
        /// <summary>
        /// OR is applied to the contents of the applied query parameters
        /// </summary>
		OR
	}

    /// <summary>
    /// A collection of QueryParameter objects, unified to be applied in a specific grouping
    /// for a search request.
    /// </summary>
	public class QueryParameterCollection
	{
		private List<QueryParameter> _parameters;
        private string _groupname;
        private ParameterJoin _parameterjoin = ParameterJoin.OR;

        /// <summary>
        /// Constructor that names the collection, accepts a List of QueryParameter objects and
        /// applies the default ParameterJoin (OR) as the methodology for the group at search runtime.
        /// </summary>
        /// <param name="groupname">Unique name for this grouping</param>
        /// <param name="listParameters">List of QueryParameter objects to be grouped in search syntax</param>
        public QueryParameterCollection(string groupname, List<QueryParameter> listParameters)
			: this(groupname, listParameters, ParameterJoin.OR)
		{
		}

        /// <summary>
        /// Constructor that names the collection, accepts a List of QueryParameter objects and
        /// applies the associated ParameterJoin as the methodology for the group at search runtime.
        /// </summary>
        /// <param name="groupname">Unique name for this grouping</param>
        /// <param name="listParameters">List of QueryParameter objects to be grouped in search syntax</param>
        /// <param name="parameterjoin">AND/OR methodology to apply to the group</param>
		public QueryParameterCollection(string groupname, List<QueryParameter> listParameters, ParameterJoin parameterjoin)
		{
			this._groupname = groupname;
			this._parameters = listParameters;
			this._parameterjoin = parameterjoin;
		}

        /// <summary>
        /// The QueryParameter collection, exposed as a read-only array
        /// </summary>
		public QueryParameter[] QueryParameterArray
		{
			get { return this._parameters.ToArray(); }
		}

        /// <summary>
        /// The AND/OR methodology to be applied the collection group of QueryParameter objects at search runtime
        /// </summary>
		public ParameterJoin ParameterJoin
		{
			get { return this._parameterjoin; }
			set { this._parameterjoin = value; }
		}

        /// <summary>
        /// Textual representation of ParameterJoin used for syntactical rendering
        /// </summary>
		public virtual string Separator
		{
			get { return (this.ParameterJoin == ParameterJoin.OR ? " OR " : " AND "); }
		}

        /// <summary>
        /// Returns a syntactical rendering of the QueryParameter collection
        /// </summary>
        /// <returns>string</returns>
		public override string ToString()
		{
			string[] arQ = new string[this._parameters.Count];
			int x = 0;
			foreach (QueryParameter qp in this._parameters)
			{
                // BillKrat.2011.04.23 - prevent multiple parenthesis (hard to read query)
                // If qp.Boost==1 then we can store the original string, otherwise wrap
			    var qpBoost = qp.Boost != 1 ? "^" + qp.Boost.ToString() : "";
                if (string.IsNullOrEmpty(qpBoost))
                    arQ[x] = qp.ToString();
                else // BillKrat.2011.04.23 - Original line follows:
                    arQ[x] = "(" + qp.ToString() + ")" + (qp.Boost != 1 ? "^" + qp.Boost.ToString() : "");

				x++;
			}
            // BillKrat.2011.04.23 - if there is only one element then return it
            if (arQ.Length == 1)
                return arQ[0];

			return "(" + string.Join(this.Separator, arQ) + ")";
			
		}
	}

}
