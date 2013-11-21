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
namespace org.apache.solr.SolrSharp.Query
{
    /// <summary>
    /// Enumeration definition of possible sort orders for a given field
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Sort the field on an ascending basis for its data type
        /// </summary>
        Ascending,
        /// <summary>
        /// Sort the field on a descending basis for its data type
        /// </summary>
        Descending
    }

    /// <summary>
    /// Container for sort parameters to be applied to a search.
    /// </summary>
    public class Sort
    {

        /// <summary>
        /// Constructor that sets a SortOrder to be applied using a given solr index field
        /// </summary>
        /// <param name="sortfield">Solr index field to use for the sort basis</param>
        /// <param name="esortorder">Enumeration value dictating the order</param>
        public Sort(string sortfield, SortOrder esortorder)
        {
            this.SortField = sortfield;
            this.SortOrder = esortorder;
        }

        /// <summary>
        /// The solr index field to use for the sort basis.
        /// </summary>
        public string SortField { get; private set; }

        /// <summary>
        /// The ordering to be applied for the sort.
        /// </summary>
        public SortOrder SortOrder { get; private set; }

        /// <summary>
        /// Override used to create the proper syntax for applying a sort to a search request
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.SortField + " " + (this.SortOrder == SortOrder.Ascending ? "asc" : "desc");
        }

    }
}
