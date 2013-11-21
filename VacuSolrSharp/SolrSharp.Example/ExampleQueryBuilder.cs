using System;
using System.Collections.Generic;
using System.Text;
using org.apache.solr.SolrSharp.Query;

namespace Example
{
    public class ExampleQueryBuilder : QueryBuilder
    {

        public ExampleQueryBuilder()
            : base()
        {
        }

        public ExampleQueryBuilder(string searchterms)
            : base(searchterms)
        {
        }

    }
}
