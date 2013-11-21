using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using log4net.Config;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;

namespace VacuSolrNet
{
    public class SolrUtil
    {
        static public String coll_name = "todae";
        static private String mlt_title = "maintitle";

        ////////////////// 그룹 기준 필드 설정 //////////////////
        //static public String[] groupFileds = new String[] { "section_name", "section_area", "section_type", "period" }; //todae
        static public String[] groupFileds = new String[] { "manu_exact", "popularity"}; //collection1

        public static string RenderResults(SolrQueryResults<SolrSearchRecord> rsp, SolrSearchVO solrSearchVO) {
            StringBuilder sb = new StringBuilder();

            foreach (var p in rsp) {
                p.ApplyHighlights(rsp);
                sb.Append(p.RenderHtml(solrSearchVO, rsp.SimilarResults));
            }

            if (rsp.NumFound <= 0)
                sb.Append("<div class='result_doc'>검색결과가 존재하지 않습니다.</div>");

             return sb.ToString();
        }

	    public static String RenderGroup(SolrQueryResults<SolrSearchRecord> rsp, SolrSearchVO solrSearchVO)	{
		    if(rsp.Grouping.Count<=0)
			    return "";

		    StringBuilder sb = new StringBuilder();
		
		    foreach(KeyValuePair<string, GroupedResults<SolrSearchRecord>> gcmd in rsp.Grouping) {
			    sb.Append("<div class='result-title'><b>"+MsgCnvUtil.toCnv(coll_name,gcmd.Key)+"</b></div>");
			    sb.Append("<div>총 검색건수 : "+gcmd.Value.Matches+"</div>");
			
			    sb.Append("<div>");
			    foreach(Group<SolrSearchRecord> grp in gcmd.Value.Groups) {
				    sb.Append("<div class='group-value'>"+grp.GroupValue+"<span>("+grp.NumFound+")</span></div>");
				
				    sb.Append("<div class='group-doclist'>");
			
				    foreach(SolrSearchRecord resultDoc in grp.Documents) {
                        resultDoc.ApplyHighlights(rsp);
                        sb.Append(resultDoc.RenderGroupHtml(solrSearchVO, rsp.SimilarResults));
				    }
          		    sb.Append("</div>");
			    }
			    sb.Append("</div>");
		    }


            if(rsp.Grouping.Count <= 0)
	            sb.Append("<div class='result_doc'>검색결과가 존재하지 않습니다.</div>");

	        return sb.ToString();
	    }

        public static string RenderRemoveFacets(SolrSearchVO solrSearchVO) {
            if (solrSearchVO.fq != null && solrSearchVO.fq.Length > 0) {
                StringBuilder sb = new StringBuilder();
                int cnt = 0;
                foreach (string s in solrSearchVO.fq) {
                    if (cnt > 0) sb.Append(" &gt; ");
                    sb.Append("<a href='?" + solrSearchVO.UrlRemoveFacet(s) + "&page=1' class='removeFacet'>" + s + "</a>");

                    cnt++;
                }

               return  "<div class='facet_remove'>" + sb.ToString() + "</div>";
            }

            return "";
        }

        public static string RenderFacets(SolrQueryResults<SolrSearchRecord> rsp, SolrSearchVO solrSearchVO) {
	        StringBuilder sb = new StringBuilder();

            sb.Append(RenderFieldFacets(rsp, solrSearchVO));
            sb.Append(RenderQueryFacets(rsp, solrSearchVO));
            sb.Append(RenderRangeFacets(rsp, solrSearchVO));
            sb.Append(RenderPivotFacets(rsp, solrSearchVO));
	    
	        return sb.ToString();
	    }

        public static string RenderPivotFacets(SolrQueryResults<SolrSearchRecord> rsp, SolrSearchVO solrSearchVO) {
            if (rsp.FacetPivots.Count <= 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (var f in rsp.FacetPivots) {
				string key =f.Key;
				IList<Pivot> valList = f.Value;
				
				sb.Append("<li class='facet pivot'>[");
				int depth = 0;
				foreach(string s in key.Split(',')) { 
					if(depth!=0)
						sb.Append(" > ");
					sb.Append(MsgCnvUtil.toCnv(coll_name,s));
					depth++;
				}
				sb.Append("]<ul>");
				
				IList<Pivot> pfAncester = new List<Pivot>();
				foreach (Pivot pf in valList) {
					if (pf.Count <= 0)
						continue;
					RenderPivotField(sb, solrSearchVO, pfAncester, pf);
				}
	
				sb.Append("</ul>").Append("</li>");
			}
            return sb.ToString();
        }

        public static string RenderRangeFacets(SolrQueryResults<SolrSearchRecord> rsp, SolrSearchVO solrSearchVO) {

            if (rsp.FacetRanges.Count <= 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (var rf in rsp.FacetRanges) {
                sb.Append("<li class='facet range'>[").Append(MsgCnvUtil.toCnv(coll_name, rf.Key)).Append("]<ul>");

                RangeFacet f = rf.Value;

                if (f.Before != null && f.Before.intValue()>0) {
		    	    string fval = f.Name + ":[* TO " + toRangeStr(f.Start)+"}";
                    sb.Append("<li><a href='?" + solrSearchVO.UrlSetFacet(fval) + "'>" + "*-" + toRangeStr(f.Start) + "</a><span>(" + f.Before + ")</span></li>");
			    }

                IList<FacetCount> list = f.Counts;
				foreach (FacetCount fv in list) {
					if (fv.Count <= 0)
						continue;
                    sb.Append("<li><a href='?" + solrSearchVO.UrlSetRangeFacet(f, fv) + "'>" + fv.Value + "-" + SolrUtil.AddFactVal(fv.Value, f.Gap) + "</a><span>(" + fv.Count + ")</span></li>");
				}

                if (f.After != null && f.After.intValue() > 0) {
                    string fval = f.Name + ":[" + toRangeStr(f.End) + " TO *}";
                    sb.Append("<li><a href='?" + solrSearchVO.UrlSetFacet(fval) + "'>" + toRangeStr(f.End) + "-*" + "</a><span>(" + f.After + ")</span></li>");
			    }

                sb.Append("</ul>").Append("</li>");
			}
            return sb.ToString();
        }

        public static string toRangeStr(object val) {
		    if (val is DateTime)
			    return ((DateTime)val).ToString("yyyy-MM-ddTHH:mm:ssZ");
		    else
			    return val.ToString();
	    }

        public static string RenderFieldFacets(SolrQueryResults<SolrSearchRecord> rsp, SolrSearchVO solrSearchVO) {
            if (rsp.FacetFields.Count <= 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (var f in rsp.FacetFields) {
                if (f.Value.Count > 0) {
                    sb.Append("<li class='facet'>[")
                    .Append(MsgCnvUtil.toCnv(coll_name, f.Key))
                    .Append("]<ul>");

                    foreach (var fv in f.Value) {
                        if (fv.Value.ToString().Equals("0") || fv.Value.ToString().Equals(""))
                            continue;
                        sb.Append("<li><a href='?" + solrSearchVO.UrlSetFieldFacet(f.Key, fv.Key) + "'>" + fv.Key + "</a><span>(" + fv.Value + ")</span></li>");
                    }
                    sb.Append("</ul>")
                    .Append("</li>");
                }
            }
            return sb.ToString();
        }

        public static string RenderQueryFacets(SolrQueryResults<SolrSearchRecord> rsp, SolrSearchVO solrSearchVO) {
            if (rsp.FacetQueries.Count <= 0)
                return "";

            StringBuilder sb = new StringBuilder();

            sb.Append("<li class='facet'>[")
            .Append("쿼리 필터")
            .Append("]<ul>");

            foreach (var f in rsp.FacetQueries) {
                if (f.Value <= 0)
                    continue;
	            sb.Append("<li><a href='?" + solrSearchVO.UrlSetQueryFacet(f.Key) + "'>" + f.Key + "</a><span>(" + f.Value + ")</span></li>");
            }
            sb.Append("</ul>")
            .Append("</li>");

            return sb.ToString();
        }

        public static object AddFactVal(string value, object gap) {

		    if (gap.GetType().Equals(typeof (RfInteger)))
			    return StringUtil.intParse(value)+StringUtil.intParse(gap.ToString());
		    else if (gap.GetType().Equals(typeof (RfFloat)))
			    return StringUtil.parseFloat(value)+StringUtil.parseFloat(gap.ToString());
		    else if (gap.GetType().Equals(typeof (RfDouble)))
			    return StringUtil.intParse(value)+StringUtil.intParse(gap.ToString());
		    else
                return value + gap.ToString();
        }

	    public static void RenderPivotField(StringBuilder sb, SolrSearchVO solrSearchVO, IList<Pivot> pfAncester, Pivot pf) {
		
		    List<Pivot> linkList = new List<Pivot>();
		    linkList.AddRange(pfAncester);
		    linkList.Add(pf);
		
		    sb.Append("<li><a href='?" + solrSearchVO.UrlPivotFacet(linkList)+ "'>" + pf.Value + "</a><span>(" + pf.Count + ")</span>");
		
		    pfAncester.Add(pf);
		    if(pf.HasChildPivots) {
			    sb.Append("<ul>");
			    foreach (Pivot pfSub in pf.ChildPivots) {
				    if (pfSub.Count <= 0)
					    continue;
				    RenderPivotField(sb, solrSearchVO,pfAncester, pfSub);
			    }
			    sb.Append("</ul>");
		    }
		    pfAncester.Remove(pf);
		    sb.Append("</li>");
	    }

        public static string RenderPaging(SolrSearchVO solrSearchVO) {
            StringBuilder sb = new StringBuilder();
            string paramUrl = SolrSearchVO.getQueryParam(solrSearchVO.q, solrSearchVO.fq, solrSearchVO.sort, solrSearchVO.group, solrSearchVO.groupFields);

            foreach (int i in new int[] { 10, 20, 50, 100, 200 }) {
                sb.Append(" | ");

                if (i == solrSearchVO.pageSize)
                    sb.Append("<span>" + i + "</span>");
                else
                    sb.Append("<a href='?" + paramUrl + "&pageSize=" + i + "&page=1'>" + i + "</a>");
            }

            return sb.ToString();
        }

        ////////////////// 소팅 기준 필드 설정 //////////////////
        static string[] sort_list = new String[] { "name asc", "price asc" }; //collection1
        //static string[] sort_list = new String[] { "maintitle asc", "section_name asc" }; //todae
        //static String[] sort_list = new String[] { "maintitle_s asc", "maintitle_s desc", "keywords_s asc", "keywords_s desc", "period asc", "period desc" };

        public static string RenderSort(SolrSearchVO solrSearchVO) {
            StringBuilder sb = new StringBuilder();
            string paramUrl = SolrSearchVO.getQueryParam(solrSearchVO.q, solrSearchVO.fq, "", solrSearchVO.group, solrSearchVO.groupFields);

            if (solrSearchVO.sort.Equals(""))
                sb.Append("<a href='?" + paramUrl + "&page=1'><strong>기본(score)</strong></a>");
            else
                sb.Append("<a href='?" + paramUrl + "&page=1'>기본(score)</a>");

            foreach (string s in sort_list) {
                if (s.Equals(solrSearchVO.sort))
                    sb.Append(" | <a href='?" + paramUrl + "&sort=" + s + "'><strong>" + MsgCnvUtil.toCnv(coll_name, s) + "</strong></a>");
                else
                    sb.Append(" | <a href='?" + paramUrl + "&sort=" + s + "'>" + MsgCnvUtil.toCnv(coll_name, s) + "</a>");
            }

            return sb.ToString();
        }

        public static string RenderDebugURL(string queryUrl) {
            return "<div style='background-color:#fff'><a href='" + queryUrl + "'>" + queryUrl + "</a></div>";
        }

        public static string QueryUrl(ISolrReadOnlyOperations<SolrSearchRecord> solr,ISolrQuery query, QueryOptions options) {

            SolrBasicServer<SolrSearchRecord> solrServer = ((solr as SolrServer<SolrSearchRecord>).BasicServer as SolrBasicServer<SolrSearchRecord>);
            SolrQueryExecuter<SolrSearchRecord> queryExecuter = (solrServer.QueryExecuter as SolrQueryExecuter<SolrSearchRecord>);
            //queryExecuter.Handler = "/browse";
            LoggingConnection logConnection = (solrServer.Connection as LoggingConnection);
            SolrConnection connection = (logConnection.Connection as SolrConnection);
            
            var param = queryExecuter.GetAllParameters(query, options);
            string handlerUrl = queryExecuter.Handler;
            if (!string.IsNullOrEmpty(options.QT))
                handlerUrl = options.QT;

            var u = new UriBuilder(connection.ServerURL);
            u.Path += handlerUrl;
            u.Query = connection.GetQuery(param);

            return u.Uri.ToString();
        }
    }
}
