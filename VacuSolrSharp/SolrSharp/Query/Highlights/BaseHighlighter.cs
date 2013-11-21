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

namespace org.apache.solr.SolrSharp.Query.Highlights
{
    /// <summary>
    /// Enumeration definition of possible fragmenting options for highlighting
    /// </summary>
    public enum HighlightFragmenter
    {
        /// <summary>
        /// Creates fixed-sized fragments with gaps for multi-valued fields
        /// </summary>
        Gap,
        /// <summary>
        /// Creates fragments that "look like" a regular expression (Solr 1.3)
        /// </summary>
        Regex
    }

    /// <summary>
    /// Enumeration definition of possible formatting options for highlighting
    /// </summary>
    public enum HighlightFormatter
    {
        /// <summary>
        /// Surrounds a highlighted term with a customizable pre- and post text snippet
        /// </summary>
        Simple
    }

    /// <summary>
    /// The base implementation for highlight parameters. Highlight parameters carry defaults
    /// that can be superseded on an aggregate (all fields) basis, and/or on an individual
    /// field basis.
    /// 
    /// BaseHighlighter implements the parameters that can be created and applied to a search
    /// query request.
    /// </summary>
    public abstract class BaseHighlighter
    {
        private static readonly int default_snippets = 1;
        private static readonly int default_fragmentsize = 100;
        private static readonly bool default_requirefieldmatch = false;
        private static readonly int default_maxanalyzedchars = 51200;
        private static readonly HighlightFormatter default_highlightformatter = HighlightFormatter.Simple;
        private static readonly string default_simplepretext = "<em>";
        private static readonly string default_simpleposttext = "</em>";
        private static readonly HighlightFragmenter default_highlightfragmenter = HighlightFragmenter.Gap;
        private static readonly double default_regexslop = 0.6;
        private static readonly int default_regexmaxanalyzedchars = 10000;
        /// <summary>
        /// The url parameter used in a search request that indicates if highlighting is to be enabled
        /// </summary>
        public static readonly string SOLR_PARAM_HIGHLIGHT = "hl";
        /// <summary>
        /// The url parameter used in a search request that contains the fieldname to which highlighting
        /// should be applied
        /// </summary>
        public static readonly string SOLR_PARAM_HIGHLIGHTFIELDS = "hl.fl";

        internal BaseHighlighter()
        {
            this.Snippets = BaseHighlighter.default_snippets;
            this.FragmentSize = BaseHighlighter.default_fragmentsize;
            this.RequireFieldMatch = BaseHighlighter.default_requirefieldmatch;
            this.MaxAnalyzedChars = BaseHighlighter.default_maxanalyzedchars;
            this.HighlightFormatter = BaseHighlighter.default_highlightformatter;
            this.SimplePreText = BaseHighlighter.default_simplepretext;
            this.SimplePostText = BaseHighlighter.default_simpleposttext;
            this.HighlightFragmenter = BaseHighlighter.default_highlightfragmenter;
            this.RegexSlop = BaseHighlighter.default_regexslop;
            this.RegexMaxAnalyzedChars = BaseHighlighter.default_regexmaxanalyzedchars;
        }

        /// <summary>
        /// Returns the reference name for name/value pair assignments. Marked
        /// as virtual to allow for custom overrides if needed in implementation
        /// classes.
        /// </summary>
        public virtual string ParameterReference
        {
            get { return BaseHighlighter.SOLR_PARAM_HIGHLIGHT; }
        }

        /// <summary>
        /// The maximum number of highlighted snippets to generate per field. 
        /// Note: it is possible for any number of snippets from zero to this 
        /// value to be generated.
        /// </summary>
        public int Snippets { get; set; }

        /// <summary>
        /// The size, in characters, of fragments to consider for highlighting. 
        /// "0" indicates that the whole field value should be used (no fragmenting).
        /// </summary>
        public int FragmentSize { get; set; }

        /// <summary>
        /// If true, then a field will only be highlighted if the query matched in this 
        /// particular field (normally, terms are highlighted in all requested field 
        /// regardless of which field matched the query).
        /// </summary>
        public bool RequireFieldMatch { get; set; }

        /// <summary>
        /// The number of characters into a document to look for suitable snippets [Solr 1.3+].
        /// </summary>
        public int MaxAnalyzedChars { get; set; }

        /// <summary>
        /// Specify a formatter for the highlight output. Currently the only legal value is "simple", 
        /// which surrounds a highlighted term with a customizable pre- and post text snippet.
        /// </summary>
        public HighlightFormatter HighlightFormatter { get; private set; }

        /// <summary>
        /// The text which appears before a highlighted term when using the simple formatter.
        /// </summary>
        public string SimplePreText { get; set; }

        /// <summary>
        /// The text which appears after a highlighted term when using the simple formatter.
        /// </summary>
        public string SimplePostText { get; set; }

        /// <summary>
        /// Specify a text snippet generator for the highlit text. The standard fragmenter is gap 
        /// (which is so called because it creates fixed-sized fragments with gaps for multi-valued 
        /// fields). Another option is regex, which tries to create fragments that "look like" a 
        /// certain regular expression. [Solr 1.3+]
        /// </summary>
        public HighlightFragmenter HighlightFragmenter { get; set; }

        /// <summary>
        /// Factor by which the regex fragmenter can stray from the ideal fragment size 
        /// (given by hl.fragsize) to accomodate the regular expression. For instance, a slop of 
        /// 0.2 with fragsize of 100 should yield fragments between 80 and 120 characters in length. 
        /// It is usually good to provide a slightly smaller fragsize when using the regex fragmenter.
        /// </summary>
        public double RegexSlop { get; set; }

        /// <summary>
        /// The regular expression for fragmenting. This could be used to extract 
        /// sentence.
        /// </summary>
        public string RegexPattern { get; set; }

        /// <summary>
        /// Only analyze this many characters from a field when using the regex fragmenter 
        /// (after which, the fragmenter produces fixed-sized fragments). Applying a complicated 
        /// regex to a huge field is expensive.
        /// </summary>
        public int RegexMaxAnalyzedChars { get; set; }

        /// <summary>
        /// Override of ToString, but set as abstract to force implementation by 
        /// inheritors.
        /// </summary>
        /// <returns>string</returns>
        public abstract override string ToString();

        /// <summary>
        /// Writes out values of properties on a name/value basis for incorporation into
        /// a querystring. For efficiency, only differences from default values are added
        /// to the rendered output.
        /// </summary>
        /// <returns>string as a querystring component</returns>
        protected string RenderParameters()
        {
            List<string> listOutput = new List<string>();
            if (!IsDefault(this.Snippets, BaseHighlighter.default_snippets))
            {
                listOutput.Add(this.ParameterReference + "snippets=" + this.Snippets.ToString());
            }
            if (!IsDefault(this.FragmentSize, BaseHighlighter.default_fragmentsize))
            {
                listOutput.Add(this.ParameterReference + "fragsize=" + this.FragmentSize.ToString());
            }
            if (!IsDefault(this.RequireFieldMatch, BaseHighlighter.default_requirefieldmatch))
            {
                listOutput.Add(this.ParameterReference + "requireFieldMatch=" + this.RequireFieldMatch.ToString().ToLower());
            }
            if (!IsDefault(this.MaxAnalyzedChars, BaseHighlighter.default_maxanalyzedchars))
            {
                listOutput.Add(this.ParameterReference + "maxAnalyzedChars=" + this.MaxAnalyzedChars.ToString());
            }
            if (this.HighlightFormatter != HighlightFormatter.Simple)
            {
                listOutput.Add(this.ParameterReference + "formatter=" + Enum.GetName(typeof(HighlightFormatter), this.HighlightFormatter));
            }
            if (!IsDefault(this.SimplePreText, BaseHighlighter.default_simplepretext) && !(IsDefault(this.SimplePostText, HighlightParameter.default_simpleposttext)))
            {
                listOutput.Add(this.ParameterReference + "simplepretext=" + this.SimplePreText + "&" + this.ParameterReference + "simpleposttext=" + this.SimplePostText);
            }
            if (this.HighlightFragmenter == HighlightFragmenter.Regex)
            {
                if (!IsDefault(this.RegexPattern, string.Empty))
                {
                    listOutput.Add(this.ParameterReference + "regexpattern=" + this.RegexPattern);
                }
                if (!IsDefault(this.RegexSlop, BaseHighlighter.default_regexslop))
                {
                    listOutput.Add(this.ParameterReference + "regexslop=" + this.RegexSlop.ToString());
                }
                if (!IsDefault(this.RegexMaxAnalyzedChars, BaseHighlighter.default_regexmaxanalyzedchars))
                {
                    listOutput.Add(this.ParameterReference + "regexmaxAnalyzedChars=" + this.RegexMaxAnalyzedChars.ToString());
                }
            }
            if (listOutput.Count > 0)
            {
                return string.Join("&", listOutput.ToArray());
            }
            return string.Empty;
        }

        /// <summary>
        /// Evaluates two values to determine if they are equal. Used to determine
        /// if a difference against the default value exists.
        /// </summary>
        /// <param name="property">property value to be evaluated</param>
        /// <param name="defaultvalue">default value to be evaluated</param>
        /// <returns>true if the values are equal</returns>
        protected static bool IsDefault(double property, double defaultvalue)
        {
            return (property == defaultvalue);
        }

        /// <summary>
        /// Evaluates two values to determine if they are equal. Used to determine
        /// if a difference against the default value exists.
        /// </summary>
        /// <param name="property">property value to be evaluated</param>
        /// <param name="defaultvalue">default value to be evaluated</param>
        /// <returns>true if the values are equal</returns>
        protected static bool IsDefault(int property, int defaultvalue)
        {
            return (property == defaultvalue);
        }

        /// <summary>
        /// Evaluates two values to determine if they are equal. Used to determine
        /// if a difference against the default value exists.
        /// </summary>
        /// <param name="property">property value to be evaluated</param>
        /// <param name="defaultvalue">default value to be evaluated</param>
        /// <returns>true if the values are equal</returns>
        protected static bool IsDefault(string property, string defaultvalue)
        {
            return (property == defaultvalue);
        }

        /// <summary>
        /// Evaluates two values to determine if they are equal. Used to determine
        /// if a difference against the default value exists.
        /// </summary>
        /// <param name="property">property value to be evaluated</param>
        /// <param name="defaultvalue">default value to be evaluated</param>
        /// <returns>true if the values are equal</returns>
        protected static bool IsDefault(bool property, bool defaultvalue)
        {
            return (property == defaultvalue);
        }


    }
}
