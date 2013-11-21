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
using System.Xml;
using org.apache.solr.SolrSharp.Query;
using org.apache.solr.SolrSharp.Query.Highlights;
using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Configuration.Schema;

namespace org.apache.solr.SolrSharp.Results
{
    /// <summary>
    /// SearchResults is an abstraction that hides the details of evaluating a solr index search
    /// response, and returns the results in an object-based structure. SearchResults provides the
    /// base ExecuteSearch method as well as public metadata properties about the results of the
    /// query: TotalResults, the starting record (StartAt), and the number of Rows available for this
    /// set of results.
    /// 
    /// SearchResults requires an implementation utilizing generics. This enables the base class to
    /// build search result objects directly. Using generics, the base class populates SearchRecord
    /// results and Facet results.  Inheriting classes need only define how an inherited SearchRecord
    /// is created and a Facet results are created.  The base class executes those inherited methods
    /// during ExecuteSearch.
    /// </summary>
    /// <typeparam name="T">Class inheritance of type SearchRecord</typeparam>
	public abstract class SearchResults<T>
	{
        //protected int _totalresults;
        //protected int _startat;
        //protected int _rows;

        // yskwun 20130210  private => protected modify
        protected List<T> _searchrecords = new List<T>();

        protected XmlDocument _xmlresults;
        protected XmlNodeList _xnlsearchrecords;

        protected XmlNode _xnlsearchgroups; //yskwun 20131017

        //++ yskwun 20131014
        protected XmlNode _xnlfacetresults_field;
        protected XmlNode _xnlfacetresults_query;
        protected XmlNode _xnlfacetresults_ranges;
        protected XmlNode _xnlfacetresults_pivot;
        //--

        protected XmlNodeList _xnlhighlights;
        protected List<HighlightRecord> _highlightrecords = new List<HighlightRecord>();
        //protected DebugResults _debugResults = null;
        protected XmlNode _xndebugresults;

        protected static readonly string XPATH_RECORDS = "response/result/doc";
        protected static readonly string XPATH_STARTAT = "response/result/@start";
        protected static readonly string XPATH_TOTALRESULTS = "response/result/@numFound";

        protected static readonly string XPATH_GROUPS = "response/lst[@name='grouped']"; //yskwun 20131017

        //++ yskwun 20131014
        protected static readonly string XPATH_FACETRESULTS_FIELD = "response/lst[@name='facet_counts']/lst[@name='facet_fields']";
        protected static readonly string XPATH_FACETRESULTS_QUERY = "response/lst[@name='facet_counts']/lst[@name='facet_queries']";
        protected static readonly string XPATH_FACETRESULTS_RANGES = "response/lst[@name='facet_counts']/lst[@name='facet_ranges']";
        protected static readonly string XPATH_FACETRESULTS_PIVOT = "response/lst[@name='facet_counts']/lst[@name='facet_pivot']";
        //--

        protected static readonly string XPATH_HIGHLIGHTING = "response/lst[@name='highlighting']/lst";
        protected static readonly string XPATH_DEBUG = "response/lst[@name='debug']";

        /// <summary>
        /// Generates search results for the QueryBuilder object. Using this constructor,
        /// search results are immediately available after control is returned to the calling
        /// client application.
        /// </summary>
        /// <param name="queryBuilder">QueryBuilder object to be executed as a search request</param>
        /// 
        //++ yskwun 20130210  default construct add
        public SearchResults()
        {
        }
        //--

        public SearchResults(QueryBuilder queryBuilder)
        {
            this.ExecuteSearch(queryBuilder);
        }

        /// <summary>
        /// Executes a search request using the passed QueryBuilder object. This method populates
        /// the metadata properties about the search (total results, start-at, and available rows).
        /// Additionally, initialization routines for the given type of SearchRecord and any Facet
        /// results are called.
        /// </summary>
        /// <param name="queryBuilder">QueryBuilder object to be executed as a search request</param>
        public void ExecuteSearch(QueryBuilder queryBuilder)
		{
            ExecuteSearch(queryBuilder, queryBuilder.QueryUrl);
		}

        //++ yskwun 20130210  url direct call
        public void ExecuteSearch(QueryBuilder queryBuilder, string queryUrl)
        {
            this._xmlresults = SolrSearcher.GetXmlDocumentFromPost(queryBuilder.SolrSearchUrl, queryUrl);

            if (this._xmlresults == null) return;
            this.TotalResults = Convert.ToInt32(SolrSearcher.GetXmlValue(this._xmlresults, SearchResults<T>.XPATH_TOTALRESULTS));
            this.StartAt = Convert.ToInt32(SolrSearcher.GetXmlValue(this._xmlresults, SearchResults<T>.XPATH_STARTAT));
            this._xnlsearchrecords = SolrSearcher.GetXmlNodes(this._xmlresults, SearchResults<T>.XPATH_RECORDS);

            this._xnlsearchgroups = SolrSearcher.GetXmlNode(this._xmlresults, SearchResults<T>.XPATH_GROUPS); //yskwun 20131017

            //++ yskwun 20131014
            this._xnlfacetresults_field = SolrSearcher.GetXmlNode(this._xmlresults, SearchResults<T>.XPATH_FACETRESULTS_FIELD);
            this._xnlfacetresults_query = SolrSearcher.GetXmlNode(this._xmlresults, SearchResults<T>.XPATH_FACETRESULTS_QUERY);
            this._xnlfacetresults_ranges = SolrSearcher.GetXmlNode(this._xmlresults, SearchResults<T>.XPATH_FACETRESULTS_RANGES);
            this._xnlfacetresults_pivot = SolrSearcher.GetXmlNode(this._xmlresults, SearchResults<T>.XPATH_FACETRESULTS_PIVOT);
            //--
            
            this.Rows = this._xnlsearchrecords.Count;

            #region Evaluate highlighting
            //if (queryBuilder.IsHighlighted)
            this._xnlhighlights = SolrSearcher.GetXmlNodes(this._xmlresults, SearchResults<T>.XPATH_HIGHLIGHTING);
            if (this._xnlhighlights != null)
            {
                foreach (XmlNode xn in this._xnlhighlights)
                {
                    this._highlightrecords.Add(new HighlightRecord(xn));
                }
            }
            #endregion

            #region Instantiate each SearchRecord instance, per the Generic "T" type
            foreach (XmlNode xn in this._xnlsearchrecords)
            {
                #region Substitute highlighting, if applicable
                if (this._highlightrecords.Count > 0)
                {
                    SolrField solrField_default =
                        queryBuilder.SolrSearcher.SolrSchema.GetSolrField(queryBuilder.SolrSearcher.SolrSchema.UniqueKey);
                    string xnPath = string.Format("{0}[@name='{1}']", SolrType.TypeExpression(solrField_default.Type), solrField_default.Name);
                    foreach (HighlightRecord hr in this._highlightrecords)
                    {
                        if (hr.RecordId == Convert.ToString(SolrSearcher.GetXmlValue(xn, xnPath)))
                        {
                            //++ yskwun 20130210
                            foreach (XmlNode child in hr.XNodeRecord.ChildNodes)
                            {
                                if (child.NodeType != XmlNodeType.Element)
                                    continue;

                                string solrFieldName = ((XmlElement)child).GetAttribute("name");

                                SolrField solrField = queryBuilder.SolrSearcher.SolrSchema.GetSolrField(solrFieldName);
                                XmlNode xnFieldNode = SolrSearcher.GetXmlNode(xn, solrField.XpathRootExpression);
                                xnFieldNode.InnerXml = child.InnerXml;
                            }

                            /* original source
                            foreach (HighlightParameter hp in queryBuilder.GetHighlightParameterCollection())
                            {
                                string[] hilitephrases = hr.GetHighlightedPhrases(hp.SolrField.Name);
                                foreach (string phrase in hilitephrases)
                                {
                                    //how to substitute?
                                    string stripped = phrase.Replace(hp.SimplePreText, "");
                                    stripped = stripped.Replace(hp.SimplePostText, "");

                                    //find the matching phrase in xn
                                    XmlNodeList xnlSwap = SolrSearcher.GetXmlNodes(xn, hp.SolrField.XpathExpression);
                                    foreach (XmlNode xnSwap in xnlSwap)
                                    {
                                        if (xnSwap.InnerText == stripped)
                                        {
                                            xnSwap.InnerText = phrase;
                                            break;
                                        }
                                    }
                                }
                            }
                            */
                        }
                    }
                }
                #endregion
                this._searchrecords.Add(this.InitSearchRecord(xn));
            }
            #endregion

            if (this._xnlsearchgroups != null) this.InitGroupResults(this._xnlsearchgroups); //yskwun 20131017

            #region Instantiate FacetRecord instances, if populated
            // if (queryBuilder.Facets.Length > 0)
            // yskwun 20130210
            if (this._xnlfacetresults_field != null) this.InitFacetResults_Field(this._xnlfacetresults_field);
            if (this._xnlfacetresults_query != null) this.InitFacetResults_Query(this._xnlfacetresults_query);
            if (this._xnlfacetresults_ranges != null) this.InitFacetResults_Ranges(this._xnlfacetresults_ranges);
            if (this._xnlfacetresults_pivot != null) this.InitFacetResults_Pivot(this._xnlfacetresults_pivot);
            //-
            #endregion

            #region Evaluate debug parameters

            //if (queryBuilder.IsDebugEnabled)
            this._xndebugresults = SolrSearcher.GetXmlNode(this._xmlresults, SearchResults<T>.XPATH_DEBUG);
            if (_xndebugresults != null)
                this.DebugResults = new DebugResults(this._xndebugresults);

            #endregion
        }
        //--

        /// <summary>
        /// This method is called by ExecuteSearch in constructing type-specific objects
        /// of type SearchRecord.  This constructs the collection of SearchRecords automatically,
        /// using the definition in the inherited object.  Required implementation by the inheriting class.
        /// </summary>
        /// <param name="xn">XmlNode containing fields used for the resultant SearchRecord</param>
        /// <returns>SearchRecord of type T (the inherited type)</returns>
		protected abstract T InitSearchRecord(XmlNode xn);

		protected abstract void InitGroupResults(XmlNode xn); //yskwun 20131017

        /// <summary>
        /// This method is called by ExecuteSearch in constructing type-specific objects
        /// of type FacetResults.  This constructs the collection of FacetResults automatically,
        /// using the definition in the inherited object.  Required implementation by the inheriting class.
        /// </summary>
        /// <param name="xn">XmlNode containing fields used for the resultant FacetResult</param>
        
        // yskwun 20130210
        public abstract void InitFacetResults_Field(XmlNode xn);
        public abstract void InitFacetResults_Query(XmlNode xn);
        public abstract void InitFacetResults_Ranges(XmlNode xn);
        public abstract void InitFacetResults_Pivot(XmlNode xn);
        //-

        /// <summary>
        /// The number of total results for this search request
        /// </summary>
        public int TotalResults { get; protected set; }

        /// <summary>
        /// The starting record (zero-based) for this set of results
        /// </summary>
        public int StartAt { get; protected set; }

        /// <summary>
        /// The number of rows returns for this set of results
        /// </summary>
        public int Rows { get; protected set; }

        /// <summary>
        /// Type-specific set of SearchRecords, representing the results for this page.
        /// </summary>
        public IEnumerable<T> SearchRecords
        {
            get { return this._searchrecords; }
        }
        // BillKrat.2011.04.23 - replace T[] with IEnumerable<T>
        //public T[] SearchRecords
        //{
        //    get { return this._searchrecords.ToArray(); }
        //}

        /// <summary>
        /// If the QueryBuilder object that constructs this SearchResults instance 
        /// has the <see cref="org.apache.solr.SolrSharp.Query.QueryBuilder.IsDebugEnabled"/>IsDebugEnabled property set to true, this object will be created
        /// (not null).
        /// </summary>
        public DebugResults DebugResults { get; protected set; }
	}
}
