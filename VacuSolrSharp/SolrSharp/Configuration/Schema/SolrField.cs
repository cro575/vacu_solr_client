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
using System.Xml;

namespace org.apache.solr.SolrSharp.Configuration.Schema
{
    /// <summary>
    /// Maps a "field" node entry in a solr config.xml file to a strongly-typed object.
    /// While mapping all attributes, SolrField also provides a translation between solr 
    /// field class types and C# native types.
    /// </summary>
    public class SolrField
    {
        private readonly SolrSchema solrschema;

        /// <summary>
        /// Gets an xpath-type query expression using the native .Net type and solr
        /// fieldname. Used by result and record classes in querying xml results.
        /// </summary>
        /// <param name="type">native .Net type to be evaluated</param>
        /// <param name="fieldname">solr fieldname to be used in the expression</param>
        /// <returns>string formed as an xpath-style query</returns>
        public static string GetXpathExpression(Type type, string fieldname)
        {
            string expression = SolrType.TypeExpression(type) + "[@name='" + fieldname + "']";
            if (type.IsArray)
            {
                Type arrayType = type.GetElementType();
                return expression + "/" + SolrType.TypeExpression(arrayType);
            }
            return expression;
        }

        //++ yskwun 20130210 add
        public static string GetXpathRootExpression(Type type, string fieldname)
        {
            string expression = SolrType.TypeExpression(type) + "[@name='" + fieldname + "']";
            return expression;
        }
        //--

        /// <summary>
        /// Constructs an object by xpath query of an xml node representing a field in solr.
        /// </summary>
        /// <param name="xnSolrField">XmlNode representing one field</param>
        /// <param name="solrSchema">The underlying SolrSchema</param>
        public SolrField(XmlNode xnSolrField, SolrSchema solrSchema)
        {
            this.Name = xnSolrField.Attributes["name"].Value;
            SolrType solrType = solrSchema.GetSolrType(xnSolrField.Attributes["type"].Value);
            if (solrType != null)
            {
                this.Type = solrType.Type;
            }

            this.OmitNorms = false;
            if (xnSolrField.Attributes["omitNorms"] != null)
            {
                this.OmitNorms = Convert.ToBoolean(xnSolrField.Attributes["omitNorms"].Value);
            }
            this.IsIndexed = false;
            if (xnSolrField.Attributes["indexed"] != null)
            {
                this.IsIndexed = Convert.ToBoolean(xnSolrField.Attributes["indexed"].Value);
            }
            this.IsStored = false;
            if (xnSolrField.Attributes["stored"] != null)
            {
                this.IsStored = Convert.ToBoolean(xnSolrField.Attributes["stored"].Value);
            }
            this.IsMultiValued = false;
            if (xnSolrField.Attributes["multiValued"] != null)
            {
                this.IsMultiValued = Convert.ToBoolean(xnSolrField.Attributes["multiValued"].Value);
            }
            this.IsDefaulted = false;
            if (xnSolrField.Attributes["default"] != null)
            {
                this.IsDefaulted = true;
            }
            this.solrschema = solrSchema;
        }

        /// <summary>
        /// The solr field name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The C# native type equivalent for the solr field
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// When enabled in solr, OmitNorms disables length normalization and index-time 
        /// boosting for this field, which saves memory.  Only full-text fields or fields 
        /// that need an index-time boost need norms.
        /// </summary>
        public bool OmitNorms { get; private set; }

        /// <summary>
        /// When enabled, search queries can use this field for searching or sorting.
        /// </summary>
        public bool IsIndexed { get; private set; }

        /// <summary>
        /// When enabled, search queries can retrieve this field in search results
        /// </summary>
        public bool IsStored { get; private set; }

        /// <summary>
        /// When enabled, this field may contain more than one value per document.
        /// </summary>
        public bool IsMultiValued { get; private set; }

        private string isCopiedName = null;
        private bool isCopied = false;
        private readonly object syncLock = new object();
        /// <summary>
        /// When true, this field exists in the index by having values copied from a
        /// source field. This eliminates the requirement for a copied field to be
        /// included in an update request, as solr will take care of copying the values
        /// over from a source field to this field.
        /// </summary>
        public bool IsCopied
        {
            get
            {
                if (isCopiedName == null)
                {
                    lock (syncLock)
                    {
                        this.isCopiedName = "";
                        foreach (SolrCopyField solrCopyField in this.solrschema.SolrCopyFields)
                        {
                            if (solrCopyField.DestinationField == this)
                            {
                                this.isCopiedName = this.Name;
                                this.isCopied = true;
                                break;
                            }
                        }
                    }
                }
                return this.isCopied;
            }
        }

        /// <summary>
        /// When true, this field may contain a default value for inclusion in the index.
        /// This eliminates the requirement for a copied field to be included in an update 
        /// request, as solr will take care of generating the default value for the field,
        /// per the definition in the schema.xml file. If the field is present in an update
        /// request, the requested field value will be used.
        /// </summary>
        public bool IsDefaulted { get; private set; }

        /// <summary>
        /// An xpath-style expression for this instance.
        /// </summary>
        public string XpathExpression
        {
            get
            {
                return this.IsMultiValued ? 
                    SolrField.GetXpathExpression(Array.CreateInstance(this.Type, 0).GetType(), this.Name) : 
                    SolrField.GetXpathExpression(this.Type, this.Name);
            }
        }

        //++ yskwun 20130210 add
        public string XpathRootExpression
        {
            get
            {
                return this.IsMultiValued ?
                    SolrField.GetXpathRootExpression(Array.CreateInstance(this.Type, 0).GetType(), this.Name) :
                    SolrField.GetXpathRootExpression(this.Type, this.Name);
            }
        }
        //--
    }
}
