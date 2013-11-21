using System;
using System.Collections.Generic;
using System.Web;
using org.apache.solr.SolrSharp.Query;
using org.apache.solr.SolrSharp.Query.Parameters;
using org.apache.solr.SolrSharp.Results;

namespace VacuSolrSharp
{
    public class SolrSearchVO
    {
        public static int BLOCK_SIZE = 10;

        public string q;
        public int page;
        public int pageSize;
        public string sort;
        public string[] fq;

        public bool group;
        public string[] groupFields;

        public static int getBLOCK_SIZE() {
		    return BLOCK_SIZE;
	    }

	    public static void setBLOCK_SIZE(int bLOCK_SIZE) {
		    BLOCK_SIZE = bLOCK_SIZE;
	    }

	    public bool isGroup() {
		    return group;
	    }

	    public void setGroup(bool group) {
		    this.group = group;
	    }

	    public string[] getGroupFields() {
		    return groupFields;
	    }

	public void setGroupFields(string[] groups) {
		this.groupFields = groups;
	}

	    public String getQ() {
		    return q;
	    }

	    public void setQ(String q) {
		    this.q = q;
	    }

	    public int getPage() {
		    return page;
	    }

	    public void setPage(int page) {
		    this.page = page;
	    }

	    public int getPageSize() {
		    return pageSize;
	    }

	    public void setPageSize(int pageSize) {
		    this.pageSize = pageSize;
	    }

	    public String getSort() {
		    return sort;
	    }

	    public void setSort(String sort) {
		    this.sort = sort;
	    }

	    public String[] getFq() {
		    return fq;
	    }

	    public void setFq(String[] fq) {
		    this.fq = fq;
	    }


        public SolrSearchVO(System.Web.HttpRequest req) {
            q = StringUtil.getParam(req["q"], "").Trim();
            page = StringUtil.getParam(req["page"], 1);
            pageSize = StringUtil.getParam(req["pageSize"], 10);
            sort = StringUtil.getParam(req["sort"], "").Trim();
            fq = req.QueryString.GetValues("fq");

            group = StringUtil.getParam(req["group"], "false").Equals("true")?true:false;
            groupFields = req.QueryString.GetValues("group.field");
        }

        public int getStartIdx() {
            return (this.page - 1) * this.pageSize;
        }

        public string getQueryParam() {
            return getQueryParam(this.q, this.fq, this.sort, this.group, this.groupFields);
        }

        public static string getQueryParam(string q, string[] fq, string sort, bool group, string[] groupFields) {
            string queryUrl = "q=" + q;
            if (fq != null) {
                foreach (string val in fq)
                    queryUrl += "&fq=" + HttpUtility.UrlEncode(val.Trim());
            }

            if(group) {
			    queryUrl += "&group=true";
			    if(groupFields != null) {
				    foreach(string s in groupFields)
					    queryUrl += "&group.field="+s;
			    }
            }

            if (!String.IsNullOrEmpty(sort))
                queryUrl += "&sort=" + sort;

            return queryUrl;
        }

        public string UrlSetFacet(string fVal) {
            List<string> fqList = new List<string>();

            bool find = false;
            if (this.fq != null) {
                foreach (string val in this.fq) {
                    if (val.Equals(fVal)) {
                        fqList.Add(fVal);
                        find = true;
                    } else
                        fqList.Add(val);
                }
            }

            if (!find)
                fqList.Add(fVal);

            return getQueryParam(this.q, fqList.ToArray(), this.sort, this.group, this.groupFields);
        }

        public string UrlSetFieldFacet(string fName, string fVal) {
             return UrlSetFacet(fName + ":" + fVal);
        }

        public string UrlSetQueryFacet(string fName) {
            return UrlSetFacet(fName);
        }

        public String UrlPivotFacet(IList<Pivot> linkList) {
            //List<String> fqList = (this.fq == null)? new List<string>(): this.fq.ToList();

            List<String> fqList = fqList = new List<string>();
            if(this.fq != null) {
                foreach (string s in this.fq)
                    fqList.Add(s);
            }

    	    foreach(Pivot pf in linkList) {
	            bool find = false;
                foreach (string val in fqList) {
                    //if (val.StartsWith(fName + ":"))
                    if (val.Equals(pf.Field + ":" + pf.Value.ToString())) {
                        find = true;
                        break;
                    }
	            }
	            if(!find)
                    fqList.Add(pf.Field + ":" + pf.Value.ToString());
    	    }

            return getQueryParam(this.q, fqList.ToArray(), this.sort, this.group, this.groupFields);
        }

        public String UrlSetRangeFacet(RangeFacet f, FacetCount fv) {
    	    String fval = f.Name + ":[" + fv.Value+" TO "
                    + SolrUtil.AddFactVal(fv.Value, f.Gap)
    			    + "}";

            return UrlSetFacet(fval);
        }

        public string UrlRemoveFacet(string str) {
            List<string> fqList = new List<string>();

            if (this.fq != null) {
                foreach (string val in this.fq) {
                    if (!val.Equals(str))
                        fqList.Add(val);
                }
            }

            return getQueryParam(this.q, fqList.ToArray(), this.sort, this.group, this.groupFields);
        }

	    public static string parseQueryOpr(String word) {
		    string s = word.ToLower();

		    string[] ops =  {"and","or","not","+","-","&&","||","!","(",")","{","}","[","]","^","\"","~","*","?",":","\\"};
		    foreach(string op1 in ops) {
                if (s.Equals(op1))
				    return word;
		    }

            string ret = "";

            string op = ":~+-!(){}[]^*?\"";
            int charCnt = 0;
            for(int i=0; i<word.Length; i++) {
        	    char c = word[i];
        	    if(op.IndexOf(c)>=0) {
        		
        		    if(charCnt>0) {
            		    if(c==':'){
            			    int lastIdx = ret.LastIndexOf('\"');
            			    if(lastIdx>=0) {
            				    String ret1 = "";
            				    if(lastIdx>0)
            					    ret1 += ret.Substring(0,lastIdx-1);
            				    if(lastIdx<ret.Length)
                                    ret1 += ret.Substring(lastIdx + 1);
            				
            				    ret = ret1;
            			    }
            		    } else {
            			    ret+="\"";
            		    }
        		    }
        		
        		    ret+=c;
        		    charCnt=0;
        	    } else {
        		    if(charCnt==0)
        			    ret+="\"";
        		    ret+=c;
        		    charCnt++;
        	    }
            }
		    if(charCnt>0)
			    ret+="\"";
		
		    return ret;
	    }

        public void setSolrQueryParam(SolrQueryBuilder queryBuilder) {
            if (!String.IsNullOrEmpty(this.q)) {
        	    string[] sp = this.q.Split(' ');

        	    string sq = "";
                foreach (string s in sp) {
            	    if(s.Trim().Length<=0)
            		    continue;
                	
            	    string s1 = parseQueryOpr(s);
                    sq += " " + s1;
                }
                sq = sq.Trim();


                Query mainQuery = new Query();
                List<QueryParameter> mainListQp = new List<QueryParameter> {new QueryParameter(null, sq)};
                QueryParameterCollection mainQpc = new QueryParameterCollection("mainQuery", mainListQp);
                mainQuery.AddQueryParameters(mainQpc, ParameterJoin.AND);
                queryBuilder.Query = mainQuery;
                
                //queryBuilder.ExplainOtherQuery = otherQuery;
                //queryBuilder.AddSearchParameter("q", sq, true);
            }

            if (this.fq != null) {
                foreach (string val in this.fq) {
                    string[] sp = StringUtil.pairSplit(val, ':');

                    string fqVal = "";
                    if (sp.Length > 1) {
                        string sp1 = sp[1].Trim();
                        if (!(sp1.StartsWith("[") || sp1.StartsWith("{")))
                            sp1 = "\"" + sp1 + "\"";

                        fqVal = sp[0].Trim() + ":" + sp1;
                    } else {
                        fqVal = val.Trim();
                    }

                    queryBuilder.AddSearchParameter("fq", fqVal, false);
                }
            }

            if(this.group) {
                queryBuilder.AddSearchParameter("group", this.group.ToString().ToLower(), true);
                 if(this.groupFields != null) {
                     foreach(string s in this.groupFields)
                         queryBuilder.AddSearchParameter("group.field", s, false);
                 }
                //queryParams.Add("group.limit", "3");
            }

            if (!String.IsNullOrEmpty(this.sort)) {
                string[] sp = this.sort.Split(' ');
                queryBuilder.AddSort(new Sort(sp[0], "asc".Equals(sp[1]) ? SortOrder.Ascending : SortOrder.Descending));
            }

            //queryBuilder.QT = "browse";

            queryBuilder.IsDebugEnabled = true;

            queryBuilder.Page = this.page;
            queryBuilder.Rows = this.pageSize;
        }

        public static string getSolrQueryParam(string q, string[] fq, string sort) {
            string queryUrl = "";

            if (!String.IsNullOrEmpty(q)) {
                string[] sp = q.Split(' ');
                //queryUrl += "q=" + q;

                string sq = "";
                foreach (string s in sp) {
            	    if(s.Trim().Length<=0)
            		    continue;
            	    sq += " " + parseQueryOpr(s);
                }
                sq = sq.Trim();
                queryUrl += "q=" + sq;
            }

            if (fq != null) {
                foreach (string val in fq) {
                    string[] sp = StringUtil.pairSplit(val, ':');

                    string fqVal = "";
                    if (sp.Length>1) {
                        String sp1 = sp[1].Trim();
                        if (!(sp1.StartsWith("[") || sp1.StartsWith("{")))
                            sp1 = "\"" + sp1 + "\"";

                        fqVal = sp[0].Trim() + ":" + sp1;
                    } else {
                        fqVal = val.Trim();
                    }

                    queryUrl += "&fq=" + "\"" + fqVal+"\"";
                }
            }

            if (!String.IsNullOrEmpty(sort))
                queryUrl += "&sort=" + sort;

            return queryUrl;
        }

    }
}

