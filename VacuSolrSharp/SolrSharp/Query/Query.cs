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
using org.apache.solr.SolrSharp.Query.Parameters;

namespace org.apache.solr.SolrSharp.Query
{
    /// <summary>
    /// The Query class collects one or more QueryParameterCollection objects to yield
    /// a solr index query statement to be used in an http request.  Passing a Query object
    /// to a constructor of a QueryBuilder object is a structured way of building
    /// a query, no matter how complex, to be executed against a solr index.
    /// </summary>
    public class Query
    {
		private List<QueryParameterCollection> _orlist = new List<QueryParameterCollection>();
		private List<QueryParameterCollection> _andlist = new List<QueryParameterCollection>();

        /// <summary>
        /// Empty public constructor
        /// </summary>
		public Query()
		{
		}

        /// <summary>
        /// Adds a QueryParameterCollection to this instance, along with a ParameterJoin determination
        /// of how to apply the internal QueryParameter objects within the QueryParameterCollection.
        /// </summary>
        /// <param name="qp">QueryParameterCollection object to be added for this search request</param>
        /// <param name="parameterjoin">Enum determining how to apply this QueryParameterCollection in the aggregate search request</param>
		public void AddQueryParameters(QueryParameterCollection qp, ParameterJoin parameterjoin)
        {
            switch (parameterjoin)
            {
                case ParameterJoin.OR:
                    this._orlist.Add(qp);
                    break;
                case ParameterJoin.AND:
                    this._andlist.Add(qp);
                    break;
            }
        }

        /// <summary>
        /// Returns the textual representation of the ParameterJoin enumeration for use in constructing
        /// the query as a properly formatted url string
        /// </summary>
        /// <param name="paramaterjoin">Enumeration value</param>
        /// <returns>string of either "AND" or "OR"</returns>
		private string Separator(ParameterJoin paramaterjoin)
		{
			return (paramaterjoin == ParameterJoin.OR ? " OR " : " AND ");
		}

        /// <summary>
        /// Returns the aggregate solr index search request properly formatted for usage
        /// in an http request
        /// </summary>
        /// <returns>string</returns>
		public override string ToString()
		{
			int x = 0;
			string qor = "";
			string qand = "";

			if (this._orlist.Count > 0)
			{
				string[] arOR = new string[this._orlist.Count];
				foreach (QueryParameterCollection qp in this._orlist)
				{
					arOR[x] = qp.ToString();
					x++;
				}

                // BillKrat.2011.04.23 - do not add parenthesis if not required
                if (_andlist.Count == 0 && arOR.Length == 1)
                    qor = arOR[0];
                else
                    qor = "(" + string.Join(" OR ", arOR) + ")";
			}

			if (this._andlist.Count > 0)
			{
				string[] arAND = new string[this._andlist.Count];
				x = 0;
				foreach (QueryParameterCollection qp in this._andlist)
				{
					arAND[x] = qp.ToString();
					x++;
				}
                // BillKrat.2011.04.23 - do not add parenthesis if not required
                if (_orlist.Count == 0 && arAND.Length == 1)
                    qand = arAND[0];
                else
                    qand = "(" + string.Join(" AND ", arAND) + ")";
			}

            // BillKrat.2011.04.23 - do not add parenthesis if not required
            if (qand.Length == 0)
                return qor;
            if (qor.Length == 0)
                return qand;

            return "(" + qand + qor + ")";
		}
    }
}
