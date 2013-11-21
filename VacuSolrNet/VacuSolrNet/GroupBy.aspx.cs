using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

using log4net.Config;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;

using Microsoft.Practices.ServiceLocation;
using SolrNet.DSL;
using System.Text;

namespace VacuSolrNet
{
    public partial class GroupBy : System.Web.UI.Page
    {
        public SolrSearchVO solrSearchVO = null;
        public SolrQueryResults<SolrSearchRecord> searchResults = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //ISolrReadOnlyOperations
            var solr = ServiceLocator.Current.GetInstance<VacuSolrServer<SolrSearchRecord>>();

            solrSearchVO = new SolrSearchVO(Request);

            ISolrQuery solrQuery = solrSearchVO.BuildQuery();
            QueryOptions queryOption = solrSearchVO.GetQueryOption();
            queryOption.QT = "/browse";

            searchResults = solr.Query(solrQuery, queryOption);
            //searchResults = solr.Query("Todae.listPage", solrQuery, queryOption);

            this.ltrResultRecords.Text = SolrUtil.RenderGroup(searchResults, solrSearchVO);

            this.ltrRemoveFacet.Text = SolrUtil.RenderRemoveFacets(solrSearchVO);

            this.ltrResultFacets.Text = SolrUtil.RenderFacets(searchResults, solrSearchVO);

            this.ltrDebug.Text += SolrUtil.RenderDebugURL(SolrUtil.QueryUrl(solr, solrQuery, queryOption));

    	
            StringBuilder sb = new StringBuilder();
    	    for(int i=0; i<SolrUtil.groupFileds.Length; i++) {
    		    string field = SolrUtil.groupFileds[i];  
    		    string selected = "";
    		    if(solrSearchVO.groupFields!=null) {
	    		    foreach(string group in solrSearchVO.groupFields) {
	    			    if(field.Equals(group)) { 
	    				    selected = "selected";
	    				    break;
	    			    }
	    		    }
    		    }
    		
    		    if(i==0 && (solrSearchVO.groupFields==null || solrSearchVO.groupFields.Length<=0))
    			    selected = "selected";
    		
                sb.Append("<option value='"+field+"' "+selected+">"+MsgCnvUtil.toCnv(SolrUtil.coll_name, field)+"</option>");
    	    }

            this.ltrGroupFieldSelectOpt.Text = sb.ToString();
        }

    }
}
