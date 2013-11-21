using System;
using System.Collections.Generic;
using System.Text;
using org.apache.solr.SolrSharp.Query;

namespace Example
{
    public class ExampleCategoryFacet : Facet
    {

        public ExampleCategoryFacet()
            : base("cat")
        {
        }

    }
}
