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
    /// Inheritance of the QueryParameterCollection that facilitates queries of specific
    /// IDs from a list.
    /// </summary>
    public class IdListQueryParameterCollection : QueryParameterCollection
    {
        private string _parametername = null;
        private List<IdListQueryParameter> _idlistqueryparameters = null;

        /// <summary>
        /// All Id list queries are structured as OR queries; AND queries will be handled at a later time
        /// </summary>
        /// <param name="groupname">Generic name to identify the group of parameters; not too important, but give it a name</param>
        /// <param name="parameterName">Generic name to identify the group of parameters; not too important, but give it a name</param>
        /// <param name="idlistqueryParameters">List of IdListQueryParameter objects</param>
        public IdListQueryParameterCollection(string groupname, string parameterName, List<IdListQueryParameter> idlistqueryParameters)
            : this(groupname, parameterName, idlistqueryParameters, ParameterJoin.OR)
        {
        }
        internal IdListQueryParameterCollection(string groupname, string parameterName, List<IdListQueryParameter> idlistqueryParameters, ParameterJoin parameterjoin)
            : base(groupname, null, parameterjoin)
        {
            this._parametername = parameterName;
            this._idlistqueryparameters = idlistqueryParameters;
        }

        /// <summary>
        /// Defines the query's join methodology for the query; set to "OR" for the list of IDs.
        /// </summary>
        public override string Separator
        {
            get {return " OR ";}
        }

        /// <summary>
        /// Override used to write the query syntax for the list of IDs.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            string[] arQ = new string[this._idlistqueryparameters.Count];
            int x = 0;
            foreach (IdListQueryParameter qp in this._idlistqueryparameters)
            {
                arQ[x] = this._parametername + ":" + qp.ToString();
                x++;
            }
            return "(" + string.Join(this.Separator, arQ) + ")";
        }
    }
}
