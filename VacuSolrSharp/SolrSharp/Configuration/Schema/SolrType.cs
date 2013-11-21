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
using System.Xml;

namespace org.apache.solr.SolrSharp.Configuration.Schema
{
    /// <summary>
    /// Maps a "fieldtype" node entry in a solr config.xml file to a strongly-typed object.
    /// While mapping all attributes, the primary purpose of SolrType is to provide a translation
    /// between solr field class types and C# native types.
    /// </summary>
    public class SolrType
    {
        /// <summary>
        /// Generates a string value that identifies the solr "type" based on the
        /// native .Net type.
        /// </summary>
        /// <param name="type">.Net type to evaluate</param>
        /// <returns>string representing an equivalent solr "type"</returns>
        public static string TypeExpression(Type type)
        {
            if (type.IsArray)
            {
                return "arr";
            }
            string descriptor;
            if (!typeMap.TryGetValue(type, out descriptor))
                throw new InvalidOperationException("Unrecognized type " + type + "!");
            return descriptor;
        }

        protected static readonly Dictionary<Type, string> typeMap;

        /// <summary>
        /// Initializes the type map
        /// </summary>
        static SolrType()
        {
            typeMap = new Dictionary<Type, string>
                          {
                              {typeof (string), "str"},
                              {typeof (int), "int"},
                              {typeof (DateTime), "date"},
                              {typeof (float), "float"},
                              {typeof (bool), "bool"},
                              {typeof (long), "long"},
                              {typeof (uint), "int"},
                              {typeof (ulong), "long"},
                              {typeof (sbyte), "int"},
                              {typeof (byte), "int"},
                              {typeof (short), "int"},
                              {typeof (ushort), "int"}
                          };

            // As Java doesn't recognize unsigned types, they are marshalled as 
            // signed types.

            // (s)byte and (u)short aren't recognized by Solr, and are therefore
            // considered integers; users are cautioned that if such fields are
            // long-typed in the Solr schema they will not be found when parsing
            // the result XML. Additionally, overflow errors may result in
            // exceptions, so choose your types wisely!
        }

        /// <summary>
        /// Constructs an object by xpath query of an xml node representing a fieldtype in solr.
        /// </summary>
        /// <param name="xnSolrType">XmlNode representing one field type</param>
        public SolrType(XmlNode xnSolrType)
        {
            this.Name = xnSolrType.Attributes["name"].Value;
            this.Type = SolrSchema.GetNativeType(xnSolrType.Attributes["class"].Value);
            this.OmitNorms = false;
            if (xnSolrType.Attributes["omitNorms"] != null)
            {
                this.OmitNorms = Convert.ToBoolean(xnSolrType.Attributes["omitNorms"].Value);
            }
            this.SortMissingLast = false;
            if (xnSolrType.Attributes["sortMissingLast"] != null)
            {
                this.SortMissingLast = Convert.ToBoolean(xnSolrType.Attributes["sortMissingLast"].Value);
            }
            this.SortMissingFirst = false;
            if (xnSolrType.Attributes["sortMissingFirst"] != null)
            {
                this.SortMissingFirst = Convert.ToBoolean(xnSolrType.Attributes["sortMissingFirst"].Value);
            }
        }

        /// <summary>
        /// The solr fieldtype name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The C# native type equivalent for the solr type
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// When enabled in solr, OmitNorms disables length normalization and index-time 
        /// boosting for a field of this type, which saves memory.  Only full-text fields 
        /// or fields that need an index-time boost need norms.
        /// </summary>
        public bool OmitNorms { get; private set; }

        /// <summary>
        /// When enabled, a sort on fields of this type will force documents without the 
        /// field to be listed in search results after documents with the field, regardless 
        /// of the requested sort order.
        /// </summary>
        public bool SortMissingLast { get; private set; }

        /// <summary>
        /// When enabled, a sort on field of this type will force documents without the 
        /// field to be listed in search results before documents with the field, regardless of 
        /// the requested sort order.
        /// </summary>
        public bool SortMissingFirst { get; private set; }
    }
}
