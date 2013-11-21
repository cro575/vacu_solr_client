using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using org.apache.solr.SolrSharp.Results;

namespace Example
{
    public class ExFacetResults : FacetResults<string>
    {
        public string facetName;

        public ExFacetResults(string faceName, XmlNode xn)
            : base(faceName, xn)
        {
            this.facetName = faceName;
        }

        protected override string InitFacetObject(object key)
        {
            return key.ToString();
        }
    }


    public class ExampleCategoryFacetResults
    {
        public List<ExFacetResults> _results = new List<ExFacetResults>();

        public ExampleCategoryFacetResults(XmlNode xn)
        {
            XmlNodeList nodeList = xn.SelectNodes("lst");

            foreach(XmlNode child in nodeList)
            {
                string name = ((XmlElement)child).GetAttribute("name");

                _results.Add(new ExFacetResults(name, xn));
            }
        }
    }
}
