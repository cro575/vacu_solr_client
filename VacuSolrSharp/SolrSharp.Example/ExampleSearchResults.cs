using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using org.apache.solr.SolrSharp.Results;
using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Configuration.Schema;
using org.apache.solr.SolrSharp.Query.Highlights;
using org.apache.solr.SolrSharp.Query;


namespace Example
{
    public class ExampleSearchResults : SearchResults<ExampleSearchRecord>
    {
        private ExampleCategoryFacetResults _categoryfacetresults = null;

        public ExampleSearchResults() : base()
        {
        }

        public ExampleSearchResults(QueryBuilder queryBuilder)
            : base(queryBuilder)
        {
        }

        protected override ExampleSearchRecord InitSearchRecord(XmlNode xn)
        {
            return new ExampleSearchRecord(xn);
        }

        protected override void InitGroupResults(XmlNode node) {
        }

        public override void InitFacetResults_Field(XmlNode xn)
        {
            this._categoryfacetresults = new ExampleCategoryFacetResults(xn);
        }

        public override void InitFacetResults_Query(XmlNode xn)
        {
        }

        public override void InitFacetResults_Ranges(XmlNode xn)
        {
        }

        public override void InitFacetResults_Pivot(XmlNode xn)
        {
        }

        public ExampleCategoryFacetResults ExampleCategoryFacetResults
        {
            get { return this._categoryfacetresults; }
        }

    }
}
