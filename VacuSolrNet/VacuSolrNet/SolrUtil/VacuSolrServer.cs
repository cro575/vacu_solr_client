#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;
using SolrNet.Impl;
using SolrNet;
using System.Xml;
using System.Collections;

namespace VacuSolrNet {
    public class VacuSolrServer<T> : SolrServer<T> {

        private string solrSearchConfigXml = "";

        private IDictionary<string, IDictionary<string, object>> confMap = new Dictionary<string, IDictionary<string, object>>();

        public VacuSolrServer(ISolrBasicOperations<T> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidator _schemaMappingValidator)
            : base(basicServer, mappingManager, _schemaMappingValidator) {
        }

        public VacuSolrServer(ISolrBasicOperations<T> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidator _schemaMappingValidator, string solrSearchConfigXml)
            : base(basicServer, mappingManager, _schemaMappingValidator) {

            this.SolrSearchConfigXml = solrSearchConfigXml;
        }

        private void parseConfig(string solrSearchConfigXml) {

            XmlDocument doc = new XmlDocument();

            doc.Load(solrSearchConfigXml);

            confMap.Clear();

            XmlElement root = doc.DocumentElement;
            foreach (XmlNode node in root.ChildNodes) {
                XmlNodeType nodeType1 = node.NodeType;
                string nodeName1 = node.Name;
                if (nodeType1 != XmlNodeType.Element)
                    continue;

                if (nodeName1.Equals("business")) {
                    string bizName = node.Attributes.GetNamedItem("name").Value;
                    if (String.IsNullOrEmpty(bizName))
                        continue;

                    foreach (XmlNode nodeTask in node.ChildNodes) {
                        XmlNodeType nodeType2 = nodeTask.NodeType;
                        string nodeName2 = nodeTask.Name;
                        if (nodeType2 != XmlNodeType.Element)
                            continue;

                        if (!nodeName2.Equals("task"))
                            continue;

                        string taskName = nodeTask.Attributes.GetNamedItem("name").Value;
                        string handler = nodeTask.Attributes.GetNamedItem("handler").Value;

                        if (String.IsNullOrEmpty(taskName))
                            continue;

                        IDictionary<string, object> taskMap = new Dictionary<string, object>();
                        IList<KeyValuePair<string, string>> optMap = new List<KeyValuePair<string, string>>();

                        foreach (XmlNode nodeOpt in nodeTask.ChildNodes) {
                            XmlNodeType nodeType3 = nodeOpt.NodeType;
                            string nodeName3 = nodeOpt.Name;
                            if (nodeType3 == XmlNodeType.Element) {
                                string optName = nodeOpt.Attributes.GetNamedItem("name").Value;
                                optMap.Add(new KeyValuePair<string,string>(optName,nodeOpt.InnerText.Trim()));
                            }
                        }
                        taskMap["handler"] =  handler;
                        taskMap["optMap"] =  optMap;
                        
                        confMap[bizName + "." + taskName] =  taskMap;
                    }
                }
            }

            Console.WriteLine("confMap:{0}", confMap);
        }

        public string SolrSearchConfigXml {
            get { return this.solrSearchConfigXml; }
            set { 
                this.solrSearchConfigXml = value;
                parseConfig(this.solrSearchConfigXml);
            }
        }

        private SolrQueryExecuter<T> QueryExecuter {
            get {
                SolrBasicServer<T> solrServer = (BasicServer as SolrBasicServer<T>);
                SolrQueryExecuter<T> queryExecuter = (solrServer.QueryExecuter as SolrQueryExecuter<T>);
                return queryExecuter;
            }
        }

        private void AddParam(string taskID, ISolrQuery query, QueryOptions options) {
            if (String.IsNullOrEmpty(taskID))
                return;

		    IDictionary<string, object> taskMap = confMap[taskID];
		    if(taskMap!=null) {
		
			    string handler = (string)taskMap["handler"];
                IList<KeyValuePair<string, string>> optMap = (IList<KeyValuePair<string, string>>)taskMap["optMap"];

                if (!String.IsNullOrEmpty(handler) && String.IsNullOrEmpty(options.QT))
                    options.QT = "/" + handler;


                var allParams = QueryExecuter.GetAllParameters(query, options);
                List<KeyValuePair<string, string>> extraParams = new List<KeyValuePair<string, string>>();

                string[] multiEables = { "facet.field", "facet.query", "facet.pivot", "facet.range" };
                foreach (KeyValuePair<string, string> kvPair in optMap) {

                    //중복 처리에 대한 고민필요
                    if (Array.IndexOf(multiEables, kvPair.Key) >= 0) {
                        extraParams.Add(new KeyValuePair<string, string>(kvPair.Key, kvPair.Value));
                        continue;
                    }

                    bool bFind = false;
                    foreach(var paramPair in allParams) {

                        if (!paramPair.Key.Equals(kvPair.Key))
                            continue;

                        if(paramPair.Key.Equals("rows")) {
                            if(options.Rows.HasValue )
                                bFind = true;
                        } else {
                            bFind = true;
                        }

                        if(bFind)
                            break;
                    }

                    if(!bFind)
                        extraParams.Add(new KeyValuePair<string, string>(kvPair.Key, kvPair.Value));
			     }

                foreach (KeyValuePair<string, string> kvPair in extraParams) {
                    ((IList<KeyValuePair<string, string>>)options.ExtraParams).Add(kvPair);
                }
            }

            if (String.IsNullOrEmpty(options.QT))
                options.QT = "/select";
	    }

        public SolrQueryResults<T> Query(string taskID, ISolrQuery query, QueryOptions options) {

            AddParam(taskID, query, options);
            return BasicServer.Query(query, options);

            //if (bMergeHL) SolrUtil.mergeHighlightsResults(rsp);
        }

        public ResponseHeader add(T doc, bool bCommit) {

            ResponseHeader hd = BasicServer.AddWithBoost(new List<KeyValuePair<T, double?>> { new KeyValuePair<T, double?>(doc,null) }, null);
            if (bCommit)
                BasicServer.Commit(null);

            return hd;
        }

        public ResponseHeader deleteById(string id, bool bCommit) {

            ResponseHeader hd = BasicServer.Delete(new List<string> { id }, null, null);
            if (bCommit)
                BasicServer.Commit(null);

            return hd;
        }
    }
}