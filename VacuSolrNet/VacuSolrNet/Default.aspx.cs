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
    public partial class _Default : System.Web.UI.Page
    {
        public SolrSearchVO solrSearchVO = null;
        public SolrQueryResults<SolrSearchRecord> searchResults = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            var solr = ServiceLocator.Current.GetInstance<VacuSolrServer<SolrSearchRecord>>();//ISolrReadOnlyOperations

            solrSearchVO = new SolrSearchVO(Request);

            ISolrQuery solrQuery = solrSearchVO.BuildQuery();
            QueryOptions queryOption = solrSearchVO.GetQueryOption();

            
            //default => browse handler 호출
            queryOption.QT = "/browse";
            searchResults = solr.Query(solrQuery, queryOption);


            //xml 설정 기본 param 으로 호출
            //queryOption.QT = "";
            //searchResults = solr.Query("Collection1.listPage", solrQuery, queryOption); //Todae2013,Collection1 

            this.ltrResultRecords.Text = SolrUtil.RenderResults(searchResults, solrSearchVO);

            this.ltrRemoveFacet.Text = SolrUtil.RenderRemoveFacets(solrSearchVO);

            this.ltrResultFacets.Text = SolrUtil.RenderFacets(searchResults, solrSearchVO);

            this.ltrSort.Text = SolrUtil.RenderSort(solrSearchVO);

            this.ltrPageNationTop.Text = this.ltrPageNation.Text = StringUtil.getPagingSolr(solrSearchVO.getQueryParam(), solrSearchVO.page, searchResults.NumFound, SolrSearchVO.BLOCK_SIZE, solrSearchVO.pageSize);

            this.ltrPageSizeTop.Text = this.ltrPageSize.Text = SolrUtil.RenderPaging(solrSearchVO);

            this.ltrDebug.Text += SolrUtil.RenderDebugURL(SolrUtil.QueryUrl(solr, solrQuery, queryOption));
        }

    }
}
