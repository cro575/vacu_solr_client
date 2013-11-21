using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using org.apache.solr.SolrSharp.Results;
using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Configuration.Schema;
using org.apache.solr.SolrSharp.Query.Highlights;
using org.apache.solr.SolrSharp.Query;


namespace VacuSolrSharp
{
    public class VacuSolrResults : SolrSearchResults<VacuSolrRecord>
    {
        private string solrSearchConfigXml = "";
        private IDictionary<string, IDictionary<string, object>> confMap = new Dictionary<string, IDictionary<string, object>>();

        public string last_queryUrl = "";


        public VacuSolrResults() : base()
        {
        }

        public VacuSolrResults(QueryBuilder queryBuilder)
            : base(queryBuilder)
        {
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
                                optMap.Add(new KeyValuePair<string, string>(optName, nodeOpt.InnerText.Trim()));
                            }
                        }
                        taskMap["handler"] = handler;
                        taskMap["optMap"] = optMap;

                        confMap[bizName + "." + taskName] = taskMap;
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

        protected override VacuSolrRecord InitSearchRecord(XmlNode node)
        {
            return new VacuSolrRecord(node);
        }

        private void AddParam(string taskID, SolrQueryBuilder queryBuilder) {
            if (String.IsNullOrEmpty(taskID))
                return;

            IDictionary<string, object> taskMap = confMap[taskID];
            if (taskMap != null) {

                string handler = (string)taskMap["handler"];
                IList<KeyValuePair<string, string>> optMap = (IList<KeyValuePair<string, string>>)taskMap["optMap"];

                if (!String.IsNullOrEmpty(handler) && String.IsNullOrEmpty(queryBuilder.QT))
                    queryBuilder.QT = handler;


                foreach (KeyValuePair<string, string> kvPair in optMap) {
                    queryBuilder.AddSearchParameter(kvPair.Key, kvPair.Value, false);
                }

                /*
                var param = queryBuilder.GetAllParameters(query, options);
                List<KeyValuePair<string, string>> extraParams = new List<KeyValuePair<string, string>>();

                foreach (KeyValuePair<string, string> kvPair in optMap) {

                    bool bFind = false;
                    foreach (var paramPair in param) {
                        if (paramPair.Key.Equals(kvPair.Key)) { //yskwun 중복 처리에 대한 고민필요
                            bFind = true;
                            break;
                        }
                    }
                    if (!bFind)
                        extraParams.Add(new KeyValuePair<string, string>(kvPair.Key, kvPair.Value));
                }

                foreach (KeyValuePair<string, string> kvPair in extraParams) {
                    ((IList<KeyValuePair<string, string>>)options.ExtraParams).Add(kvPair);
                }
                */
            }

            if (String.IsNullOrEmpty(queryBuilder.QT))
                queryBuilder.QT = "select";
        }

        public void ExecuteSearch(SolrSearchVO sp)
        {
            ExecuteSearch(null, sp);
        }

        public void ExecuteSearch(string taskID, SolrSearchVO sp) 
        {
            SolrQueryBuilder queryBuilder = new SolrQueryBuilder();

            sp.setSolrQueryParam(queryBuilder);

            queryBuilder.AddSearchParameter("wt", "xml", true);

            AddParam(taskID, queryBuilder);

            if (String.IsNullOrEmpty(taskID) && String.IsNullOrEmpty(queryBuilder.QT))
                queryBuilder.QT = "browse";

            ExecuteSearch(queryBuilder);

            last_queryUrl = queryBuilder.SolrSearchUrl + "?" + queryBuilder.QueryUrl;

            //ExecuteSearch(sp.q, sp.fq, sp.page, sp.pageSize, sp.sort);
        }

        public void ExecuteSearch(string q, string[] fq, int page, int pageSize, string sort)
        {
            SolrQueryBuilder queryBuilder = new SolrQueryBuilder();
            queryBuilder.QT = "browse";

            queryBuilder.IsDebugEnabled = true;

            //searchResults.ExecuteSearch(queryBuilder,"q=name:ipod&start=0&rows=5&facet=true&facet.field=cat&f.cat.facet.mincount=1&facet.field=manu_exact&f.manu_exact.facet.mincount=1&spellcheck=true&version=2.2");
            //searchResults.ExecuteSearch(queryBuilder, "q=ipod&start=0&rows=5&wt=xml");

            string queryUrl = SolrSearchVO.getSolrQueryParam(q, fq, sort);

            queryUrl += "&start=" + (page - 1) * pageSize;
            queryUrl += "&rows=" + pageSize;

            last_queryUrl = queryBuilder.SolrSearchUrl+"?"+queryUrl+"&wt=xml";

            ExecuteSearch(queryBuilder, queryUrl+"&wt=xml");
        }
    }
}
