//
//   Licensed to the Apache Software Foundation (ASF) under one or more
//   contributor license agreements.  See the NOTICE file distributed with
//   this work for additional information regarding copyright ownership.
//   The ASF licenses this file to You under the Apache License, Version 2.0
//   (the "License"); you may not use this file except in compliance with
//   the License.  You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
using System;
using System.Collections.Generic;
using System.Text;

namespace org.apache.solr.SolrSharp.Query.Parameters
{
    /// <summary>
    /// Specialized QueryParameter object that constructs a range-based query with 
    /// start and end values
    /// </summary>
    public class RangeQueryParameter : QueryParameter
    {
        /// <summary>
        /// Constructor that sets start and end values for a range query on the 
        /// given solr index fieldname
        /// </summary>
        /// <param name="field">Solr index fieldname to query against</param>
        /// <param name="startvalue">Starting value for the range</param>
        /// <param name="endvalue">Ending value for the range</param>
        public RangeQueryParameter(string field, int startvalue, int endvalue)
            : this(field, startvalue.ToString(), endvalue.ToString())
        {
        }
        /// <summary>
        /// Constructor that sets start and end values for a range query on the 
        /// given solr index fieldname
        /// </summary>
        /// <param name="field">Solr index fieldname to query against</param>
        /// <param name="startvalue">Starting value for the range</param>
        /// <param name="endvalue">Ending value for the range</param>
        public RangeQueryParameter(string field, double startvalue, double endvalue)
            : this(field, RangeQueryParameter.Round(startvalue, 1, 1000000).ToString(), RangeQueryParameter.Round(endvalue, 1, 1000000).ToString())
        {
        }
        internal RangeQueryParameter(string field, string startvalue, string endvalue)
            : base(field, "")
        {
            this.SetStartEndValues(startvalue, endvalue);
        }

        /// <summary>
        /// Renders a syntactically correct usage of the RangeQueryParameter for use in an http search request
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.Field + ":" + this.Value;
        }

        /// <summary>
        /// returns the number nearest x, with a precision of numerator/denominator
        /// example: Round(12.1436, 5, 100) will round x to 12.15 (precision = 5/100 = 0.05)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        private static double Round(double x, int numerator, int denominator)
        {
            long y = (long)Math.Floor(x * denominator + (double)numerator / 2.0);
            return (double)(y - y % numerator) / (double)denominator;
        }

        private void SetStartEndValues(string startvalue, string endvalue)
        {
            string start = "";
            string end = "";
            if (startvalue.StartsWith("-") && endvalue.StartsWith("-"))
            {
                start = (startvalue.CompareTo(endvalue) > 0 ? startvalue : endvalue);
                end = (startvalue.CompareTo(endvalue) > 0 ? endvalue : startvalue);
            }
            else
            {
                start = (startvalue.CompareTo(endvalue) > 0 ? endvalue : startvalue);
                end = (startvalue.CompareTo(endvalue) > 0 ? startvalue : endvalue);
            }
            this.Value = "[" + start + " TO " + end + "]";
        }
    }
}
