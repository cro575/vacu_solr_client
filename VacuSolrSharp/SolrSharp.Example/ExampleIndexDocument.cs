using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using org.apache.solr.SolrSharp.Indexing;

namespace Example
{
    [Serializable]
    [XmlRoot("add")]
    public class ExampleIndexDocument : UpdateIndexDocument
    {

        public ExampleIndexDocument()
        {
            //Empty public constructor, as required for xml serialization
        }

        public ExampleIndexDocument(
            string id, 
            string name, 
            string manu, 
            string[] cat, 
            string[] features, 
            string includes, 
            float weight, 
            float price,
            int popularity,
            bool inStock)
        {
            this.Add(new IndexFieldValue("id", id));
            this.Add(new IndexFieldValue("name", name));
            this.Add(new IndexFieldValue("manu", manu));
            foreach (string s in cat)
            {
                this.Add(new IndexFieldValue("cat", s));
            }
            foreach (string s in features)
            {
                this.Add(new IndexFieldValue("features", s));
            }
            this.Add(new IndexFieldValue("includes", includes));
            this.Add(new IndexFieldValue("weight", weight.ToString()));
            this.Add(new IndexFieldValue("price", price.ToString()));
            this.Add(new IndexFieldValue("popularity", popularity.ToString()));
            this.Add(new IndexFieldValue("inStock", inStock.ToString()));

            //New fields added in sample schema.xml for solr 1.2 release
            
            //this.Add(new IndexFieldValue("word", this.CatchAllField));
        }

        private string CatchAllField
        {
            get
            {
                List<string> wordlist = new List<string>();
                foreach (IndexFieldValue ifv in this.FieldValues)
                {
                    wordlist.Add(ifv.Value);
                }
                return string.Join(" ", wordlist.ToArray());
            }
        }
    }
}
