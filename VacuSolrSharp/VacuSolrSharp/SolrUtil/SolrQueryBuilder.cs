using System;
using System.Collections.Generic;
using System.Text;
using org.apache.solr.SolrSharp.Query;

namespace VacuSolrSharp
{
    public class SolrQueryBuilder : QueryBuilder
    {

        public SolrQueryBuilder()
            : base()
        {
        }

        public SolrQueryBuilder(string searchterms)
            : base(searchterms)
        {
        }

    }
}
