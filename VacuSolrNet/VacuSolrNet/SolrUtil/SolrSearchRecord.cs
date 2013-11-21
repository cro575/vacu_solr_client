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
using SolrNet.Attributes;
using System.Text;
using SolrNet;
using SolrNet.Impl;
using System.Collections;

namespace VacuSolrNet
{
    /*
    public class SolrSearchRecord {

        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("taskid")]
        public string Taskid { get; set; }

        [SolrField("productid")]
        public string Productid { get; set; }

        [SolrField("section_name")]
        public string Section_name { get; set; }

        [SolrField("section_area")]
        public string Section_area { get; set; }

        [SolrField("section_type")]
        public string Section_type { get; set; }

        [SolrField("maintitle")]
        public string Maintitle { get; set; }

        [SolrField("title_k")]
        public string Title_k { get; set; }

        [SolrField("title_c")]
        public string Title_c { get; set; }

        [SolrField("content")]
        public string Content { get; set; }

        [SolrField("explanation")]
        public string Explanation { get; set; }

        [SolrField("url")]
        public string Url { get; set; }

        [SolrField("keywords")]
        public string Keywords { get; set; }

        [SolrField("imagetype")]
        public string Imagetype { get; set; }

        [SolrField("mainimage")]
        public string Mainimage { get; set; }

        private List<string> extendinfo = new List<string>();
        [SolrField("extendinfo")]
        public string[] Extendinfo
        {
            get { return this.extendinfo.ToArray(); }
            set { this.extendinfo.AddRange(value); }
        }

        [SolrField("f_period")]
        public string F_period { get; set; }

        [SolrField("f_region")]
        public string F_region { get; set; }

        [SolrField("f_owner")]
        public string F_owner { get; set; }

        [SolrField("f_docstyle")]
        public string F_docstyle { get; set; }

        [SolrField("score")]
        public float Score { get; set; }


        public void ApplyHighlights(SolrQueryResults<SolrSearchRecord> searchResults)
        {
            HighlightedSnippets hs = searchResults.Highlights[Id];

            foreach(var field in hs)
            {
                if (field.Key.Equals("maintitle"))
                {
                    Maintitle = String.Empty;
                    foreach (var val in field.Value)
                        Maintitle += val.ToString();
                }
                else if (field.Key.Equals("content"))
                {
                    Content = String.Empty;
                    foreach (var val in field.Value)
                        Content += val.ToString();
                }
                else if (field.Key.Equals("explanation"))
                {
                    Explanation = String.Empty;
                    foreach (var val in field.Value)
                        Explanation += val.ToString();
                }
            }
        }

        public string RenderHtml(SolrSearchVO solrSearchVO)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class='result_doc'>")
                .Append("<div><span class='tit'>[통합메타ID]</span> " + Id + "</div>")
                .Append("<div><span class='tit'>[대표표제어]</span> " + Maintitle + "</div>")
                .Append("<div><span class='tit'>[본문]</span> " + Content + "</div>")
                .Append("<div><span class='tit'>[설명문]</span> " + Explanation + "</div>")
                .Append("<div><span class='tit'>[바로가기]</span> <a href='" + Url + "' target='_blank'>" + Url + "</a></div>");

            sb.Append("<div><span class='tit'>[확장필드]</span> ");
            int cnt = 0;
            foreach (var c in Extendinfo)
            {
                if (cnt > 0) sb.Append(", ");
                sb.Append(c);
                cnt++;
            }
            sb.Append("</div>");

            sb.Append("<div><span class='tit'>[Score]</span> " + Score.ToString() + "</div>");

            sb.Append("</div>");

            return sb.ToString();
        }
    }
     */

    public class SolrSearchRecord
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("*")]
        public IDictionary<string, object> Fields { get; set; }

        [SolrField("_version_")]
        public long Version { get; set; }

        [SolrField("score")]
        public float Score { get; set; }

        public void ApplyHighlights(SolrQueryResults<SolrSearchRecord> searchResults)
        {
            if (this.Fields == null)
                return;

            HighlightedSnippets hs = searchResults.Highlights[this.Id];

            foreach(var hl_fld in hs)
            {
                var hl_val = StringUtil.collectionToString(hl_fld.Value, "... &nbsp;&nbsp; ...");

                foreach (KeyValuePair<string, object> kv in this.Fields)
                {
                    if (hl_fld.Key.Equals(kv.Key))
                    {
                        this.Fields[kv.Key] = hl_val;
                        break;
                    }
                }
            }
        }

        public string RenderHtml(SolrSearchVO solrSearchVO, IDictionary<string, IList<SolrSearchRecord>> SimilarResults) {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class='result_doc'>")
                .Append("<div><span class='tit'>[자료ID]</span> " + (String.IsNullOrEmpty(this.Id) ? "" : this.Id) + "</div>");

            if (this.Fields != null) {
                foreach (KeyValuePair<string, object> kv in this.Fields) {
                    sb.Append("<div><span class='tit'>[" + MsgCnvUtil.toCnv(SolrUtil.coll_name,kv.Key) + "]</span> ");

                    if ((kv.Value as ICollection) != null) {
                        ICollection values = kv.Value as ICollection;
                        foreach (object val in values)
                            sb.Append(val.ToString() + " ");
                    } else {
                        sb.Append(kv.Value.ToString());
                    }
                    sb.Append("</div>");
                }
                sb.Append("<div><span class='tit'>[Version]</span> " + this.Version + "</div>");
                sb.Append("<div><span class='tit'>[Score]</span> " + this.Score + "</div>");

                if (SimilarResults != null && SimilarResults.Count>0) {
                    //IList<SolrSearchRecord> sim_docs = SimilarResults[this.Id];
                    IList<SolrSearchRecord> sim_docs;

                    if (!SimilarResults.TryGetValue(this.Id, out sim_docs))
                        sim_docs = null;

                    if (sim_docs != null && sim_docs.Count > 0) {
                        sb.Append("<div class='similar'>");
                        sb.Append("<div class='mtit'>*** 유사자료 목록 ***</div>");
                        foreach (SolrSearchRecord doc in sim_docs) {
                            object title;

                            if (doc.Fields.TryGetValue("maintitle", out title))//name
                                title = " : " + title;
                            else
                                title = "";

                            sb.Append("<div><span class='tit'>[자료ID]</span> " + doc.Id + title.ToString() + "</div>");
                        }
                        sb.Append("</div>");
                    }
                }
            }

            sb.Append("</div>");

            return sb.ToString();
        }

        public string RenderGroupHtml(SolrSearchVO solrSearchVO, IDictionary<string, IList<SolrSearchRecord>> SimilarResults) {
            return RenderHtml(solrSearchVO, SimilarResults);
        }
    }
}