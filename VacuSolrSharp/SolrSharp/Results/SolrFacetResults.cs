using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using org.apache.solr.SolrSharp.Results;
using System.Globalization;

//++ yskwun 20131008 미구현된 패싯기능 처리
namespace org.apache.solr.SolrSharp.Results
{
    public class BasicFacetResults : FacetResults<string>
    {
        public string facetName;

        public BasicFacetResults(string faceName, XmlNode xn)
            : base(faceName, xn)
        {
            this.facetName = faceName;
        }

        protected override string InitFacetObject(object key)
        {
            return key.ToString();
        }
    }


    public class SolrFacetResults
    {
        public List<BasicFacetResults> _results = new List<BasicFacetResults>();

        public SolrFacetResults(XmlNode xn)
        {
            XmlNodeList nodeList = xn.SelectNodes("lst");

            foreach(XmlNode child in nodeList)
            {
                string name = ((XmlElement)child).GetAttribute("name");

                _results.Add(new BasicFacetResults(name, xn));
            }
        }
    }


    public abstract class RfNumber {
        public abstract int intValue();

        public abstract long longValue();

        public abstract float floatValue();

        public abstract double doubleValue();

        public virtual byte byteValue() {
            return (byte)intValue();
        }

        public virtual short shortValue() {
            return (short)intValue();
        }

        public abstract override string ToString();
    }

    public class RfInteger : RfNumber {

        private int value;

        public RfInteger(int value) {
            this.value = value;
        }

        public RfInteger(String s) {
            this.value = int.Parse(s);
        }

        public override byte byteValue() {
            return (byte)value;
        }

        public override short shortValue() {
            return (short)value;
        }

        public override int intValue() {
            return value;
        }

        public override long longValue() {
            return (long)value;
        }

        public override float floatValue() {
            return (float)value;
        }

        public override double doubleValue() {
            return (double)value;
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    public class RfDouble : RfNumber {
        private double value;

        public RfDouble(double value) {
            this.value = value;
        }

        public RfDouble(String s)
            : this(double.Parse(s)) {
        }

        public override byte byteValue() {
            return (byte)value;
        }

        public override short shortValue() {
            return (short)value;
        }

        public override int intValue() {
            return (int)value;
        }

        public override long longValue() {
            return (long)value;
        }

        public override float floatValue() {
            return (float)value;
        }

        public override double doubleValue() {
            return (double)value;
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    public class RfFloat : RfNumber {
        private float value;

        public RfFloat(float value) {
            this.value = value;
        }

        public RfFloat(double value) {
            this.value = (float)value;
        }

        public RfFloat(String s)
            : this(float.Parse(s)) {
        }

        public override  byte byteValue() {
            return (byte)value;
        }

        public override short shortValue() {
            return (short)value;
        }

        public override int intValue() {
            return (int)value;
        }

        public override long longValue() {
            return (long)value;
        }

        public override float floatValue() {
            return value;
        }

        public override double doubleValue() {
            return (double)value;
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    public abstract class RangeFacet {

        public string Name { get; set; }
        public IList<FacetCount> Counts { get; set; }

        public object Start { get; set; }
        public object End { get; set; }
        public object Gap { get; set; }

        public RfNumber Before { get; set; }
        public RfNumber After { get; set; }

        protected RangeFacet(string name, object start, object end, object gap, RfNumber before, RfNumber after) {
            this.Name = name;
            this.Start = start;
            this.End = end;
            this.Gap = gap;
            this.Before = before;
            this.After = after;

            Counts = new List<FacetCount>();
        }

        public void AddCount(string value, int count) {
            Counts.Add(new FacetCount(value, count, this));
        }

        public static object createValue(object typeName, object value) {
            //STR, INT, FLOAT, DOUBLE, LONG, BOOL, NULL, DATE
            switch (typeName.ToString()) {
                case "int": return new RfInteger(value.ToString());
                case "short": return new RfInteger(value.ToString());
                case "long": return new RfInteger(value.ToString());
                case "float": return new RfFloat(value.ToString());
                case "double": return new RfDouble(value.ToString());
                case "date": return DateTimeFieldParser.ParseDate(value.ToString());
            }

            return value;
        }
    }

    public class NumericFacetResult : RangeFacet {

        public NumericFacetResult(string name, RfNumber start, RfNumber end, RfNumber gap, RfNumber before, RfNumber after)
            : base(name, start, end, gap, before, after) {
        }
    }

    public class DateFacetResult : RangeFacet {

        public DateFacetResult(string name, DateTime start, DateTime end, string gap, RfNumber before, RfNumber after)
            : base(name, start, end, gap, before, after) {
        }
    }

    public class FacetCount {

        public string Value { get; set; }
        public int Count { get; set; }
        public RangeFacet rangeFacet;

        public FacetCount(string value, int count, RangeFacet rangeFacet) {
            this.Value = value;
            this.Count = count;
            this.rangeFacet = rangeFacet;
        }

        public RangeFacet RangeFacet {
            get { return rangeFacet; }
        }
    }

    public class DateTimeFieldParser {
        public bool CanHandleSolrType(string solrType) {
            return solrType == "date";
        }

        public bool CanHandleType(Type t) {
            return t == typeof(DateTime);
        }

        public object Parse(XmlNode field, Type t) {
            return ParseDate(field.Value);
        }

        public static DateTime ParseDate(string s) {
            var p = s.Split('-');
            var pSkip1 = new string[p.Length-1];
            for(int i=0; i<(p.Length-1); i++)
                pSkip1[i] = p[i+1];

            //s = p[0].PadLeft(4, '0') + '-' + string.Join("-", p.Skip(1).ToArray());
            s = p[0].PadLeft(4, '0') + '-' + string.Join("-", pSkip1);

            // Mono does not support that exact format string for some reason, however Parse appears to properly handle the input. 
            // Try using the format string, and if that fails, fall back to just a naive Parse.
            DateTime result;
            if (!DateTime.TryParseExact(s, "yyyy-MM-dd'T'HH:mm:ss.FFF'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) {
                result = DateTime.Parse(s, CultureInfo.InvariantCulture);
            }

            return result;
        }
    }

}
