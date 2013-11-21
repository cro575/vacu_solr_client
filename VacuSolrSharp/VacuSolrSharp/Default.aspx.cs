using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;

namespace VacuSolrSharp
{
    public partial class _Default : System.Web.UI.Page
    {
        public VacuSolrResults searchResults = new VacuSolrResults();
        public SolrSearchVO solrSearchVO = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string cfgFile = Path.Combine(Server.MapPath("/config/solrmap/"), "solrSearchConfig.xml");

            searchResults.SolrSearchConfigXml = cfgFile;

            solrSearchVO = new SolrSearchVO(Request);

            searchResults.ExecuteSearch(solrSearchVO); //default => browse handler 호출
            //searchResults.ExecuteSearch("Collection1.listPage", solrSearchVO); //xml 설정 기본 param 으로 호출

            this.ltrResultRecords.Text = SolrUtil.RenderResults(searchResults, solrSearchVO);

            this.ltrRemoveFacet.Text = SolrUtil.RenderRemoveFacets(solrSearchVO);

            this.ltrResultFacets.Text = SolrUtil.RenderFacets(searchResults, solrSearchVO);

            this.ltrSort.Text = SolrUtil.RenderSort(solrSearchVO);

            this.ltrPageNationTop.Text = this.ltrPageNation.Text = StringUtil.getPagingSolr(solrSearchVO.getQueryParam(), solrSearchVO.page, searchResults.TotalResults, SolrSearchVO.BLOCK_SIZE, solrSearchVO.pageSize);

            this.ltrPageSizeTop.Text = this.ltrPageSize.Text = SolrUtil.RenderPaging(solrSearchVO);

            this.ltrDebug.Text += SolrUtil.RenderDebugURL(searchResults.last_queryUrl);
        }

    }
}
