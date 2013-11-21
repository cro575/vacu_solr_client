using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using org.apache.solr.SolrSharp.Results;
using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Configuration.Schema;
using org.apache.solr.SolrSharp.Query.Highlights;
using org.apache.solr.SolrSharp.Query;


// yskwun extend 20131008 미구현된 그룹기능 처리
namespace org.apache.solr.SolrSharp.Results
{
    /// <summary>
    /// Pivot facet
    /// </summary>
    public class Pivot {
        /// <summary>
        /// Pivot field name
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Pivot value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Pivot facet count
        /// </summary>
        public int Count { get; set; }

        public List<Pivot> ChildPivots { get; set; }

        public bool HasChildPivots { get; set; }

        /// <summary>
        /// Pivot facet
        /// </summary>
        public Pivot() {
            HasChildPivots = false;
        }
    }

    public class Group<T> {
        /// <summary>
        /// The groupvalue for this group of documents
        /// </summary>
        public string GroupValue { get; set; }

        /// <summary>
        /// Returns the number of matching documents that are found for this groupValue
        /// </summary>
        public int NumFound { get; set; }

        /// <summary>
        /// The actual documents in the group.
        /// You can control the amount of documents in this collection by using the Limit property of the GroupingParameters
        /// </summary>
        public ICollection<T> Documents { get; set; }

        /// <summary>
        /// A single group of documents
        /// </summary>
        public Group() {
            Documents = new List<T>();
        }
    }

    public class GroupedResults<T> {
        /// <summary>
        /// Returns the number of unique matching documents that are grouped. 
        /// </summary>
        public int Matches { get; set; }

        /// <summary>
        /// Grouped documents 
        /// </summary>
        public ICollection<Group<T>> Groups { get; set; }

        /// <summary>
        /// Number of groups that have matched the query.
        /// Only available if <see cref="GroupingParameters.Ngroups"/> is true
        /// </summary>
        public int? Ngroups { get; set; }

        /// <summary>
        /// Constructor for GroupedResults
        /// </summary>
        public GroupedResults() {
            Groups = new List<Group<T>>();
        }
    }

    public class SolrSearchResults<T> : SearchResults<T> where T:ISolrRecord, new()
    {
        public IDictionary<string, GroupedResults<T>> Grouping { set; get; } // yskwun 20130210

        //public IDictionary<string, ICollection<KeyValuePair<string, int>>> FacetFields { get; set; }
        public SolrFacetResults FacetFields { get; set; }
        public IDictionary<string, int> FacetQueries { get; set; }
        public IDictionary<string, IList<Pivot>> FacetPivots { get; set; }
        public IDictionary<string, RangeFacet> FacetRanges { get; set; }

        public SolrSearchResults() : base()
        {
            initFacets();
        }

        public SolrSearchResults(QueryBuilder queryBuilder)
            : base(queryBuilder)
        {
            initFacets();
        }

        public void initFacets() 
        {
            Grouping = new Dictionary<string, GroupedResults<T>>();

            FacetQueries = new Dictionary<string, int>();
            //FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            FacetPivots = new Dictionary<string, IList<Pivot>>();
            FacetRanges = new Dictionary<string, RangeFacet>();
        }

        protected override T InitSearchRecord(XmlNode node) {
            T item = new T();
            item.ParaseRecord(node);
            return item;
        }

        protected override void InitGroupResults(XmlNode node) {

            foreach (XmlNode groupNode in node.ChildNodes) {
                if (groupNode.NodeType != XmlNodeType.Element)
                    continue;

                var key = groupNode.Attributes.GetNamedItem("name").Value;

                Grouping[key] = ParseGroupedResults(groupNode);
            }
        }

        public GroupedResults<T> ParseGroupedResults(XmlNode groupNode) {

            var ngroupNode = groupNode.SelectSingleNode("int[@name='ngroups']");
            var matchesValue = int.Parse(groupNode.SelectSingleNode("int[@name='matches']").InnerText);

            GroupedResults<T> groupResults = new GroupedResults<T>();

            groupResults.Ngroups = ngroupNode == null ? null : (int?)int.Parse(ngroupNode.InnerText);
            groupResults.Matches = matchesValue;

            var groupsLstNodes = groupNode.SelectNodes("arr[@name='groups']/lst");

            foreach (XmlNode node in groupsLstNodes) {
                if (node.NodeType != XmlNodeType.Element)
                    continue;

                groupResults.Groups.Add(ParseGroup(node));
            }

            return groupResults;
        }

        public Group<T> ParseGroup(XmlNode node) {
            var groupValueNode = node.SelectSingleNode("str[@name='groupValue']");
            var doclistNode = node.SelectSingleNode("result[@name='doclist']");

            Group<T> group = new Group<T>();
            group.GroupValue = groupValueNode == null ? "UNMATCHED" : groupValueNode.InnerText;
            group.NumFound = Convert.ToInt32(doclistNode.Attributes.GetNamedItem("numFound").Value);

            foreach(XmlNode docNode in doclistNode.SelectNodes("./doc")) {
                group.Documents.Add(this.InitSearchRecord(docNode));
            }

            return group;
        }

        public override void InitFacetResults_Field(XmlNode node)
        {
            if (node == null)
                return;

            this.FacetFields = new SolrFacetResults(node);

            /*
            var d = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();

            var facetFields = node.Elements("lst")
                .Where(X.AttrEq("name", "facet_fields"))
                .SelectMany(x => x.Elements());
            foreach (var fieldNode in facetFields) {
                var field = fieldNode.Attribute("name").Value;
                var c = new List<KeyValuePair<string, int>>();
                foreach (var facetNode in fieldNode.Elements()) {
                    var nameAttr = facetNode.Attribute("name");
                    var key = nameAttr == null ? "" : nameAttr.Value;
                    var value = Convert.ToInt32(facetNode.Value);
                    c.Add(new KeyValuePair<string, int>(key, value));
                }
                d[field] = c;
            }

            FacetFields = d;
            */ 
        }

        public override void InitFacetResults_Query(XmlNode node)
        {
            if (node == null)
                return;

            var d = new Dictionary<string, int>();

            foreach (XmlNode fieldNode in node.ChildNodes) {
                if (fieldNode.NodeType != XmlNodeType.Element)
                    continue;

                var key = fieldNode.Attributes.GetNamedItem("name").Value;
                var value = Convert.ToInt32(fieldNode.InnerText);
                d[key] = value;
            }

            FacetQueries = d;
        }

        public override void InitFacetResults_Ranges(XmlNode node)
        {
            if (node == null)
                return;

            var d = new Dictionary<string, RangeFacet>();
            foreach (XmlNode fieldNode in node.ChildNodes) {
                if (fieldNode.NodeType != XmlNodeType.Element)
                    continue;

                var name = fieldNode.Attributes.GetNamedItem("name").Value;
                d[name] = ParseRangeFacetingNode(fieldNode);
            }

            FacetRanges = d;

        }

        public RangeFacet ParseRangeFacetingNode(XmlNode node) {

            var facetname = node.Attributes.GetNamedItem("name").Value;
            var gapName = node.SelectSingleNode("./*[@name='gap']").Name;
            RangeFacet r;

            if(gapName.Equals("str") || gapName.Equals("date"))
                r = new DateFacetResult(facetname, new DateTime(), new DateTime(), null, null, null);//string name, DateTime start, DateTime end, string gap, Number before, Number after
            else
                r = new NumericFacetResult(facetname, null, null, null, null, null);//string name, Number start, Number end, Number gap, Number before, Number after

            foreach (XmlNode rangeFacetingNode in node.ChildNodes) {
                if (rangeFacetingNode.NodeType != XmlNodeType.Element)
                    continue;

                var name = rangeFacetingNode.Attributes.GetNamedItem("name").Value;
                switch (name) {
                    case "gap":
                        r.Gap = RangeFacet.createValue(rangeFacetingNode.Name, rangeFacetingNode.InnerText);
                        break;
                    case "start":
                        r.Start = RangeFacet.createValue(rangeFacetingNode.Name, rangeFacetingNode.InnerText);
                        break;
                    case "end":
                        r.End = RangeFacet.createValue(rangeFacetingNode.Name, rangeFacetingNode.InnerText);
                        break;
                    case "before":
                        r.Before = (RfNumber)RangeFacet.createValue(rangeFacetingNode.Name, rangeFacetingNode.InnerText);
                        break;
                    case "after":
                        r.After = (RfNumber)RangeFacet.createValue(rangeFacetingNode.Name, rangeFacetingNode.InnerText);
                        break;
                    case "counts":
                        foreach (XmlNode countNode in rangeFacetingNode.ChildNodes) {
                            if (countNode.NodeType != XmlNodeType.Element)
                                continue;

                            r.AddCount(countNode.Attributes.GetNamedItem("name").Value, int.Parse(countNode.InnerText));
                        }
                        break;
                    default:
                        Console.WriteLine(rangeFacetingNode.Name + ":" + name + ":" + rangeFacetingNode.InnerText);
                        break;
                }
            }
            return r;
        }

        public override void InitFacetResults_Pivot(XmlNode node)
        {
            if (node == null)
                return;

            var d = new Dictionary<string, IList<Pivot>>();
            foreach (XmlNode fieldNode in node.ChildNodes) {
                if(fieldNode.NodeType != XmlNodeType.Element)
                    continue;

                var name = fieldNode.Attributes.GetNamedItem("name").Value;
                d[name] = ParsePivotFacetingNode(fieldNode);
            }

            FacetPivots = d;
        }

        public List<Pivot> ParsePivotFacetingNode(XmlNode node) {
            List<Pivot> l = new List<Pivot>();

            var pivotNodes = node.SelectNodes("lst");
            if (pivotNodes != null) {
                foreach (XmlNode pivotNode in pivotNodes) {
                    l.Add(ParsePivotNode(pivotNode));
                }
            }

            return l;
        }

        public Pivot ParsePivotNode(XmlNode node) {
            Pivot pivot = new Pivot();

            pivot.Field = node.SelectSingleNode("str[@name='field']").InnerText;
            pivot.Value = node.SelectSingleNode("*[@name='value']").InnerText;
            pivot.Count = Convert.ToInt32(node.SelectSingleNode("int[@name='count']").InnerText);


            var childPivotNodes = node.SelectSingleNode("arr[@name='pivot']");
            if (childPivotNodes != null) {
                pivot.HasChildPivots = true;
                pivot.ChildPivots = new List<Pivot>();

                foreach (XmlNode childNode in childPivotNodes.ChildNodes) {
                    if(childNode.NodeType != XmlNodeType.Element)
                        continue;
                    pivot.ChildPivots.Add(ParsePivotNode(childNode));
                }
            }

            return pivot;
        }
    }
}
