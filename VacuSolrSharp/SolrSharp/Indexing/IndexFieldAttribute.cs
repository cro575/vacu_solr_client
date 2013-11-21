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
using org.apache.solr.SolrSharp.Results;
using org.apache.solr.SolrSharp.Configuration.Schema;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;

namespace org.apache.solr.SolrSharp.Indexing
{
    /// <summary>
    /// Mapping attribute class that provides late-binding of a solr search query results xml
    /// payload to a derived instance of SearchRecord.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class IndexFieldAttribute : Attribute
    {

        /// <summary>
        /// Constructs an instance of an IndexFieldAttribute.  The fieldName parameter
        /// must use the equivalent SolrField name, as defined in the SolrSchema and found
        /// in a solr webapp instance schema.xml file.  The fieldName parameter is case-sensitive.
        /// </summary>
        /// <param name="fieldName">string representing the solr field name to map against</param>
        public IndexFieldAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        /// <summary>
        /// The solr field name to map against
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Reflected property for this instance
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        private string XnodeExpression
        {
            get
            {
                if (this.PropertyInfo != null)
                {
                    var type = this.PropertyInfo.PropertyType;
                    return SolrField.GetXpathExpression(type, this.FieldName);
                }
                return null;
            }
        }

        //++ kys 20130402
        private static object NewDictionary(Type[] typeArgs)
        {
            var genericType = typeof(Dictionary<,>).MakeGenericType(typeArgs);
            return Activator.CreateInstance(genericType);
        }

        private static void SetKV(object dict, object key, object value)
        {
            dict.GetType().GetMethod("set_Item").Invoke(dict, new[] { key, value });
        }

        private static object ConvertTo(string s, Type t)
        {
            var converter = TypeDescriptor.GetConverter(t);
            return converter.ConvertFrom(s);
        }
        //--

        /// <summary>
        /// Binds values to natively defined properties on an inherited instance of SearchRecord.
        /// </summary>
        /// <param name="searchRecord">Instance of SearchRecord to bind the values against</param>
        public void SetValue(SearchRecord searchRecord)
        {
            if (this.FieldName.Equals("*"))
            {
                //++ kys 20130402
                foreach (XmlNode child in searchRecord.XNodeRecord.ChildNodes)
                {
                    if (child.NodeType != XmlNodeType.Element)
                        continue;

                    string solrFieldName = ((XmlElement)child).GetAttribute("name");

                    //if (this.PropertyInfo.PropertyType == typeof(Dictionary<string, object>))
                    if (this.PropertyInfo.PropertyType.IsSubclassOf(typeof(IDictionary<string, object>))
                        || this.PropertyInfo.PropertyType == typeof(IDictionary<string, object>)
                        || this.PropertyInfo.PropertyType.IsSubclassOf(typeof(IDictionary<string, string>))
                        || this.PropertyInfo.PropertyType == typeof(IDictionary<string, string>)
                    )
                    {
                        var typeArgs = this.PropertyInfo.PropertyType.GetGenericArguments();
                        var keyType = typeArgs[0];
                        var valueType = typeArgs[1];

                        var dict = this.PropertyInfo.GetValue(searchRecord, null) ?? NewDictionary(typeArgs);
                        var key = solrFieldName;
                        var value = child.InnerText;
                        SetKV(dict, ConvertTo(key, keyType), value);
                        this.PropertyInfo.SetValue(searchRecord, dict, null);
                    }
                }
                //--
            }
            else
            {
                var xnlvalues = searchRecord.XNodeRecord.SelectNodes(XnodeExpression);

                // BillKrat.2011.04.24 - added check for [] since this would crash process if an
                // array only had a single element defined
                if (xnlvalues.Count == 1 && !PropertyInfo.PropertyType.Name.Contains("[]"))   //single value
                {
                    var xnodevalue = xnlvalues[0];
                    PropertyInfo.SetValue(searchRecord, Convert.ChangeType(xnodevalue.InnerText, PropertyInfo.PropertyType), null);
                }
                else if (xnlvalues.Count >= 1)   //array         // yskwun 20130210  modify
                {
                    var basetype = this.PropertyInfo.PropertyType.GetElementType();
                    var valueArray = Array.CreateInstance(basetype, xnlvalues.Count);
                    for (var i = 0; i < xnlvalues.Count; i++)
                    {
                        valueArray.SetValue(Convert.ChangeType(xnlvalues[i].InnerText, basetype), i);
                    }
                    PropertyInfo.SetValue(searchRecord, valueArray, null);
                }
            }
        }

    }
}
