using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using org.apache.solr.SolrSharp.Results;
using org.apache.solr.SolrSharp.Indexing;

namespace Example
{
    public class ExampleSearchRecord : SearchRecord
    {

        public ExampleSearchRecord(XmlNode xnRecord)
            : base(xnRecord)
        {
        }

        private string id = null;
        [IndexField("id")]
        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        private string sku = null;
        [IndexField("sku")]
        public string Sku
        {
            get { return this.sku; }
            set { this.sku = value; }
        }

        private string name = null;
        [IndexField("name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string manu = null;
        [IndexField("manu")]
        public string Manu
        {
            get { return this.manu; }
            set { this.manu = value; }
        }

        private List<string> cat = new List<string>();
        [IndexField("cat")]
        public string[] Cat
        {
            get { return this.cat.ToArray(); }
            set { this.cat.AddRange(value); }
        }

        private List<string> features = new List<string>();
        [IndexField("features")]
        public string[] Features
        {
            get { return this.features.ToArray(); }
            set { this.features.AddRange(value); }
        }

        private string includes = null;
        [IndexField("includes")]
        public string Includes
        {
            get { return this.includes; }
            set { this.includes = value; }
        }

        private float weight = float.MinValue;
        [IndexField("weight")]
        public float Weight
        {
            get { return this.weight; }
            set { this.weight = value; }
        }

        private float price = float.MinValue;
        [IndexField("price")]
        public float Price
        {
            get { return this.price; }
            set { this.price = value; }
        }

        private int popularity = int.MinValue;
        [IndexField("popularity")]
        public int Popularity
        {
            get { return this.popularity; }
            set { this.popularity = value; }
        }

        private bool instock = false;
        [IndexField("inStock")]
        public bool InStock
        {
            get { return this.instock; }
            set { this.instock = value; }
        }

        private DateTime timestamp = DateTime.MinValue;
        [IndexField("timestamp")]
        public DateTime TimeStamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

    }
}
