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

namespace org.apache.solr.SolrSharp.Query.Parameters
{
    /// <summary>
    /// A field/value pair used as a building block for a Query object. One or QueryParameter objects
    /// can be grouped using a QueryParameterCollection, and subsequently added to a Query object for
    /// used with a QueryBuilder object.
    /// 
    /// This object may be used stand alone or part of a group of a larger collection of QueryParameter
    /// objects in constructing a complex search query.
    /// </summary>
    public class QueryParameter
    {
        //below backslash must be first! so we don't replace other escaped characters again
        private static readonly string[] _specialchars = new string[] { "\\", "+", "-", "&&", "||", "!", "(", ")", "{", "}", "[", "]", "^", "\"", "~", "*", "?", ":" };

        /// <summary>
        /// Constructor that takes a solr index fieldname and value to be applied in a search query
        /// </summary>
        /// <param name="field">Solr index fieldname to query against</param>
        /// <param name="value">Value to be applied in the query</param>
        public QueryParameter(string field, string value)
        {
            this.Field = field;
            this.Value = value.Trim();
            this.Boost = 1;
        }

        /// <summary>
        /// Lucene document boost to be applied to this fieldname/value pair at search runtime
        /// </summary>
        public float Boost { get; set; }

        /// <summary>
        /// Solr index fieldname being searched against
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// Value being used to search against this Field
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Renders a syntactically correct search query for use with a url
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            string _tostring = "";
            bool _quoted = (this.Value.StartsWith("\"") && this.Value.EndsWith("\""));
            bool _bracketed = (this.Value.StartsWith("[") && this.Value.EndsWith("]"));

            if (!_quoted && !_bracketed && this.Value.Contains(" "))
            {
                //split up non quoted phrases
                List<string> listString = new List<string>();
                foreach (string s in this.Value.Split(" ".ToCharArray()))
                    listString.Add(GetSearchField(Field) + "\"" + LuceneEscape(s) + "\"");
                _tostring = "(" + string.Join(" AND ", listString.ToArray()) + ")";
            }
            else if (_quoted)
            {
                char _groupchar = this.Value[0];
                string toEscape = this.Value;
                //strip the grouping characters so we don't hose LuceneEscape
                toEscape = (toEscape.TrimStart(_groupchar)).TrimEnd(_groupchar);
                _tostring = GetSearchField(Field) + _groupchar + LuceneEscape(toEscape) + _groupchar;
            }
            else if (_bracketed)//don't mess with the bracketed query
            {
                _tostring = GetSearchField(Field) + this.Value;
            }
            else //no quotes, brackets, or spaces
            {
                _tostring = GetSearchField(Field)+"\"" + LuceneEscape(this.Value) + "\"";
            }

            return _tostring;
        }

        private string GetSearchField(string fieldName)
        {
            return fieldName == null ? "" : fieldName + ":";
        }

        private static string LuceneEscape(string s)
        {
            string _s = s;
            foreach (string sc in QueryParameter._specialchars)
                _s = _s.Replace(sc, "\\" + sc);
            return _s;
        }
    }
}
