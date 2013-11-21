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
using System.Text.RegularExpressions;
using System.Xml;

namespace org.apache.solr.SolrSharp.Configuration.Schema
{
    /// <summary>
    /// Dynamic fields are similar to "wildcard" fields. If a field name is not 
    /// found, dynamicFields will be used if the name matches any of the patterns.
    /// RESTRICTION: the glob-like pattern in the name attribute must have a "*" only 
    /// at the start or the end.
    /// EXAMPLE:  name="*_i" will match any field ending in _i (like myid_i, z_i)
    /// Longer patterns will be matched first.  if equal size patterns both match, the 
    /// first appearing in the schema will be used.
    /// </summary>
    public class SolrDynamicField : SolrField
    {

        private static readonly string WILDCARD = "*";
        private static readonly string REGEX_CHAR_WILDCARD = ".";
        private static readonly string REGEX_CHAR_STARTLINE = "^";
        private static readonly string REGEX_CHAR_ENDLINE = "$";
        private readonly Regex regexNameMatch = null;

        /// <summary>
        /// Constructs an instance of a dynamic field.
        /// </summary>
        /// <param name="xnDynamicField">XmlNode representing the definition of a single dynamic field</param>
        /// <param name="solrSchema">The underlying SolrSchema where the dynamic field exists</param>
        public SolrDynamicField(XmlNode xnDynamicField, SolrSchema solrSchema)
            : base(xnDynamicField, solrSchema)
        {
            if (this.Name.StartsWith(SolrDynamicField.WILDCARD))
            {
                this.regexNameMatch = new Regex(this.Name.Replace(SolrDynamicField.WILDCARD, SolrDynamicField.REGEX_CHAR_WILDCARD) + SolrDynamicField.REGEX_CHAR_ENDLINE);
            }
            else if (this.Name.EndsWith(SolrDynamicField.WILDCARD))
            {
                this.regexNameMatch = new Regex(SolrDynamicField.REGEX_CHAR_STARTLINE + this.Name.Replace(SolrDynamicField.WILDCARD, SolrDynamicField.REGEX_CHAR_WILDCARD));
            }
            else if (this.Name == SolrDynamicField.WILDCARD)
            {
                this.regexNameMatch = new Regex(SolrDynamicField.REGEX_CHAR_WILDCARD);
            }
        }

        /// <summary>
        /// Evaluates a fieldname to determine if solr pattern matching applies (and the fieldName
        /// is permissible for use.)
        /// </summary>
        /// <param name="fieldName">the name of the field to be evaluated</param>
        /// <returns>true if the dynamic field definition matches the fieldname</returns>
        public bool IsMatch(string fieldName)
        {
            return this.regexNameMatch.IsMatch(fieldName);
        }
    }
}
