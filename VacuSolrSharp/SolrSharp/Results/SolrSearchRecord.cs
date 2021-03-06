using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using org.apache.solr.SolrSharp.Results;
using org.apache.solr.SolrSharp.Indexing;
using System.Collections;

// yskwun extend 20131008 기본 record 구현체 제공
namespace org.apache.solr.SolrSharp.Results
{
    public class SolrSearchRecord : SearchRecord
    {
        public SolrSearchRecord(XmlNode xnRecord)
            : base(xnRecord)
        {
        }

        [IndexField("id")]
        public string Id { get; set;}

        [IndexField("*")]
        public IDictionary<string, object> Fields { get; set; }

        public string RenderHtml() {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class='result_doc'>")
                .Append("<div><span class='tit'>[자료ID]</span> " + (String.IsNullOrEmpty(this.Id) ? "" : this.Id) + "</div>");

            if (this.Fields != null) {
                foreach (KeyValuePair<string, object> kv in this.Fields) {
                    sb.Append("<div><span class='tit'>[" + kv.Key + "]</span> ");

                    if ((kv.Value as ICollection) != null) {
                        ICollection values = kv.Value as ICollection;
                        foreach (object val in values)
                            sb.Append(val.ToString() + " ");
                    } else
                        sb.Append(kv.Value.ToString());
                    sb.Append("</div>");
                }
            }

            sb.Append("</div>");

            return sb.ToString();
        }

        public string RenderGroupHtml() {
            return RenderHtml();
        }
    }
}
