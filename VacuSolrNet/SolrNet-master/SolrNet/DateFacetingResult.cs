#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System;
using SolrNet.Impl.FieldParsers;

namespace SolrNet {
    /// <summary>
    /// Date faceting result
    /// </summary>
    /// 

    //++ yskwun 20131008
    public abstract class RfNumber {
        public abstract int intValue();

        public abstract long longValue();

        public abstract float floatValue();

        public abstract double doubleValue();

        public byte byteValue() {
            return (byte)intValue();
        }

        public short shortValue() {
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

        public byte byteValue() {
	        return (byte)value;
        }

        public short shortValue() {
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

        public byte byteValue() {
	        return (byte)value;
        }

        public short shortValue() {
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

        public byte byteValue() {
	        return (byte)value;
        }

        public short shortValue() {
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
                case "int": return new SolrNet.RfInteger(value.ToString());
                case "short": return new SolrNet.RfInteger(value.ToString());
                case "long": return new SolrNet.RfInteger(value.ToString());
                case "float": return new SolrNet.RfFloat(value.ToString());
                case "double": return new SolrNet.RfDouble(value.ToString());
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
    //-- yskwun 20131008
   
    public class DateFacetingResult {

		/// <summary>
		/// Date range gap (e.g. "+1DAY")
		/// </summary>
        public string Gap { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public DateTime End { get; set; }

		/// <summary>
		/// The date faceting results.
		/// </summary>
		public IList<KeyValuePair<DateTime, int>> DateResults { get; set; }

        /// <summary>
        /// Other date faceting results.
        /// </summary>
        public IDictionary<FacetDateOther, int> OtherResults { get; set; }

        /// <summary>
        /// Date faceting result
        /// </summary>
		public DateFacetingResult() {
			DateResults = new List<KeyValuePair<DateTime, int>>();
            OtherResults = new Dictionary<FacetDateOther, int>();
        }
    }
    
}