using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.Xml;


using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Configuration.Schema;
using org.apache.solr.SolrSharp.Update;
using org.apache.solr.SolrSharp.Indexing;
using org.apache.solr.SolrSharp.Query;
using org.apache.solr.SolrSharp.Query.Parameters;
using org.apache.solr.SolrSharp.Query.Highlights;
using org.apache.solr.SolrSharp.Results;

namespace VacuSolrSharp
{
    public partial class _solr_test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Test_Vacu();
        }

       public void Test_Vacu()
        {
            SolrQueryBuilder queryBuilder = new SolrQueryBuilder();
            queryBuilder.QT = "browse";

            queryBuilder.IsDebugEnabled = true;

            VacuSolrResults searchResults = new VacuSolrResults();

            //searchResults.ExecuteSearch(queryBuilder,"q=name:ipod&start=0&rows=5&facet=true&facet.field=cat&f.cat.facet.mincount=1&facet.field=manu_exact&f.manu_exact.facet.mincount=1&spellcheck=true&version=2.2");
            searchResults.ExecuteSearch(queryBuilder, "q=ipod&start=0&rows=5&wt=xml");


            System.Diagnostics.Trace.WriteLine("");
            System.Diagnostics.Trace.WriteLine(searchResults.TotalResults.ToString());
            int x = 1;
/*
            foreach (SolrSearchRecord searchRecord in searchResults.SearchRecords)
            {
                System.Diagnostics.Trace.WriteLine(x.ToString() + ":" + searchRecord.Id);
                System.Diagnostics.Trace.WriteLine("aaa","Name: " + searchRecord.Name);
                System.Diagnostics.Trace.WriteLine("Manu:" + searchRecord.Manu);
                System.Diagnostics.Trace.WriteLine("InStock: " + searchRecord.InStock);
                System.Diagnostics.Trace.WriteLine("Includes: " + searchRecord.Includes);
                foreach (string s in searchRecord.Features)
                {
                    System.Diagnostics.Trace.WriteLine("Feature: " + s);
                }
                foreach (string s in searchRecord.Cat)
                {
                    System.Diagnostics.Trace.WriteLine("Cat: " + s);
                }
                System.Diagnostics.Trace.WriteLine("TimeStamp: " + searchRecord.TimeStamp.ToString());
            }
*/
            System.Diagnostics.Trace.WriteLine("");
            System.Diagnostics.Trace.WriteLine("=============  Facets  =============");
            if (searchResults.FacetFields != null)
            {
                foreach (BasicFacetResults facetResults in searchResults.FacetFields._results)
                {
                    foreach (KeyValuePair<string, int> kvp in facetResults.Facets)
                    {
                        System.Diagnostics.Trace.WriteLine(string.Format("{0} = {1}: count = {2}", facetResults.facetName, kvp.Key, kvp.Value));
                    }
                }
            }


            System.Diagnostics.Trace.WriteLine("");
            System.Diagnostics.Trace.WriteLine("=============  Traceging  =============");
            if (searchResults.DebugResults != null)
            {
                System.Diagnostics.Trace.WriteLine("Search query: " + searchResults.DebugResults.QueryString);
                System.Diagnostics.Trace.WriteLine("Executed query: " + searchResults.DebugResults.ParsedQuery);
                foreach (ExplanationRecord er in searchResults.DebugResults.ExplanationRecords)
                {
                    System.Diagnostics.Trace.WriteLine("Name: " + er.Name);
                    System.Diagnostics.Trace.WriteLine("ExplainInfo: " + Environment.NewLine + er.ExplainInfo);
                }

                /**************************************************************
                 * Because we added the OtherQuery object in our QueryBuilder
                 * instance above, we can evalute the results by accessing the
                 * OtherQuery property and OtherQueryExplanationRecords array.
                 ***************************************************************/
                System.Diagnostics.Trace.WriteLine("Executed other query: " + searchResults.DebugResults.OtherQuery);
                foreach (ExplanationRecord oer in searchResults.DebugResults.OtherQueryExplanationRecords)
                {
                    System.Diagnostics.Trace.WriteLine("Name: " + oer.Name);
                    System.Diagnostics.Trace.WriteLine("ExplainInfo: " + Environment.NewLine + oer.ExplainInfo);
                }
            }
        }        
    }
}
