using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace VacuSolrSharp
{
    public partial class GroupBy : System.Web.UI.Page
    {
        public VacuSolrResults searchResults = new VacuSolrResults();
        public SolrSearchVO solrSearchVO = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            solrSearchVO = new SolrSearchVO(Request);

            searchResults.ExecuteSearch(solrSearchVO);

            this.ltrResultRecords.Text = SolrUtil.RenderGroup(searchResults, solrSearchVO);

            this.ltrRemoveFacet.Text = SolrUtil.RenderRemoveFacets(solrSearchVO);

            this.ltrResultFacets.Text = SolrUtil.RenderFacets(searchResults, solrSearchVO);

            this.ltrDebug.Text += SolrUtil.RenderDebugURL(searchResults.last_queryUrl);


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
