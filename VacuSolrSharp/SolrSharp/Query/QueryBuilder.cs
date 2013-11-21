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
using System.Collections.Generic;
using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Query.Parameters;
using org.apache.solr.SolrSharp.Query.Highlights;
using System;

namespace org.apache.solr.SolrSharp.Query
{

    /// <summary>
    /// QueryBuilder is an abstraction that hides the details of building a search query
    /// to be executed against a solr index. The class provides the base methods for accepting
    /// parameters to be used in building a search request, which can be quite complex depending
    /// on the items requested.
    /// 
    /// See the examples for inheriting from and using QueryBuilder.
    /// </summary>
	public abstract class QueryBuilder
	{
        /// <summary>
        /// The url for the solr webapp instance used by this SolrSearcher to execute search requests
        /// </summary>
        public string QT { get; set; } //yskwun 20130325 SOLR_SEARCH 제거
        private List<KeyValuePair<string, string>> _searchkeyvalues = new List<KeyValuePair<string, string>>(); //yskwun 20131015 중복가능한 param 구조로 변경 dictionary 대신 List<KeyValuePair> 사용
        private HighlightParameterCollection _highlightParameterCollection = null;
		private string _queryurl = "";
		private List<Facet> _searchfacets = new List<Facet>();
		private int _page = 1;
		private int _rows = 10;
		private Query _query;
        private List<Sort> _sorts = new List<Sort>();
        private bool _isDebugEnabled = false;
        private Query _explainOtherQuery;

        /// <summary>
        /// The url parameter used in a search request that contains the solr index query
        /// </summary>
		public static readonly string SOLR_PARAM_QUERY = "q";
        /// <summary>
        /// The url parameter used in a search request that contains the starting row number (zero-based)
        /// </summary>
		public static readonly string SOLR_PARAM_STARTAT = "start";
        /// <summary>
        /// The url parameter used in a search request that contains the number of rows to return in 
        /// this result set
        /// </summary>
		public static readonly string SOLR_PARAM_ROWS = "rows";
        /// <summary>
        /// The url parameter used in a search request that enables debug information to return in
        /// this result set
        /// </summary>
        public static readonly string SOLR_DEBUG_ENABLED = "debugQuery";
        /// <summary>
        /// The url parameter used in a search request that enables the ExplainOtherQuery
        /// object in use with debugging information for this result set
        /// </summary>
        public static readonly string SOLR_EXPLAIN_OTHER = "explainOther";

        /// <summary>
        /// The underlying SolrSearcher used by this instance
        /// </summary>
        public SolrSearcher SolrSearcher { get; private set; }

        /// <summary>
        /// Constucts an empty, default QueryBuilder object. Sets the local 
        /// SolrSearcher object url to the appropriate location, per configuration.
        /// All constructors call this base constructor.
        /// </summary>
		public QueryBuilder()
		{
            this.SolrSearcher = SolrSearchers.GetSearcher(Mode.Read);
        }

        /// <summary>
        /// Constucts an empty, default QueryBuilder object and adds the Page
        /// parameter.
        /// </summary>
        /// <param name="page">integer to be used as the Page parameter</param>
        public QueryBuilder(int page)
            : this()
        {
            this._page = page;
        }

        /// <summary>
        /// Constructs a QueryBuilder object to execute a keyword search against
        /// the DefaultSearchField, setting the Page parameter to 1.
        /// </summary>
        /// <param name="searchterms">keywords to query against the DefaultSearchField</param>
		public QueryBuilder(string searchterms)
			: this(searchterms, 1)
		{
		}

        /// <summary>
        /// Constructs a QueryBuilder object to execute a keyword search against
        /// the DefaultSearchField.
        /// </summary>
        /// <param name="searchterms">keywords to query against the DefaultSearchField</param>
        /// <param name="page">integer to be used as the Page parameter</param>
        public QueryBuilder(string searchterms, int page)
            : this()
		{
            this._query = new Query();
            if (searchterms != null && searchterms != "")
            {
                QueryParameter qp = new QueryParameter(this.SolrSearcher.SolrSchema.DefaultSearchField, searchterms);
                List<QueryParameter> qpList = new List<QueryParameter>();
                qpList.Add(qp);
                QueryParameterCollection qps = new QueryParameterCollection("default", qpList);
                this._query.AddQueryParameters(qps, ParameterJoin.OR);
            }
            this._page = page;
		}

        /// <summary>
        /// Constructs a QueryBuilder object to execute a query based on the 
        /// arKeys and arValues collections. A query request will be formulated based on
        /// aligned index values in arKeys and arValues respectively, i.e.
        /// arKeys[0] will be evaluated against arValues[0]. Use this only to bypass
        /// the default behavior and if you need to set name/value pairs for the url
        /// request directly.
        /// </summary>
        /// <param name="arKeys">Collection of request parameters</param>
        /// <param name="arValues">Collection of request values</param>
        /// <param name="page">integer to be used as the Page parameter</param>
        public QueryBuilder(string[] arKeys, string[] arValues, int page)
            : this()
        {
            //yskwun 20131015 중복가능한 param 구조로 변경 dictionary 대신 List<KeyValuePair> 사용
            List<KeyValuePair<string, string>> searchkeyvalues = new List<KeyValuePair<string, string>>();
            for (int x = 0; x < arKeys.Length; x++)
                searchkeyvalues.Add(new KeyValuePair<string,string>(arKeys[x],arValues[x]));

            this._searchkeyvalues = searchkeyvalues;
            this._page = page;
		}

        //yskwun 20131015 중복가능한 param 구조로 변경 dictionary 대신 List<KeyValuePair> 사용
        public QueryBuilder(List<KeyValuePair<string, string>> searchkeyvalues, int page)
            : this()
        {
            this._searchkeyvalues = searchkeyvalues;
            this._page = page;
        }

        /// <summary>
        /// Constructs a QueryBuilder object to execute a query based on the 
        /// searchkeyvalues dictionary. A query request will be formulated based on
        /// the KeyValuePair objects contained in searchkeyvalues. Use 
        /// this only to bypass the default behavior and if you need to set name/value 
        /// pairs for the url request directly.
        /// </summary>
        /// <param name="searchkeyvalues">Dictionary of parameter/value pairs</param>
        /// <param name="page">integer to be used as the Page parameter</param>
        public QueryBuilder(Dictionary<string, string> searchkeyvalues, int page)
            : this()
        {
            //yskwun 20131015 중복가능한 param 구조로 변경 dictionary 대신 List<KeyValuePair> 사용
            foreach(KeyValuePair<string, string> pair in searchkeyvalues)
                this._searchkeyvalues.Add(pair);
            this._page = page;
        }

        /// <summary>
        /// Constructs a QueryBuilder object to execute a query based on the 
        /// Query object.  This is the most used constructor for this object and the
        /// "best" way to create a search request, as the Query object and QueryBuilder
        /// work together to save you from having to figure out syntax in building a
        /// (sometimes complex) search query within a url.
        /// </summary>
        /// <param name="query">Query object containing the field(s) to be queried against a solr index</param>
        /// <param name="page">integer to be used as the Page parameter</param>
        public QueryBuilder(Query query, int page)
            : this()
        {
            this._query = query;
			this._page = page;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <param name="solrsearcher">The solrsearcher.</param>
        public QueryBuilder(SolrSearcher solrsearcher)
        {
            this.SolrSearcher = solrsearcher;
        }

        //++ yskwun 20130210  add
		public string SolrSearchUrl
		{
			get {
                if (this.SolrSearcher == null) return null;
                if(String.IsNullOrEmpty(this.QT))
                    return this.SolrSearcher.SOLR;

                return this.SolrSearcher.SOLR + this.QT + "/";
            }
		}
        //--

		/// <summary>
		/// Sets the page value (start) parameter for search results. Overrides any value set from a loaded Dictionary of parameters.
		/// </summary>
		public int Page
		{
			get { return this._page; }
			set { this._page = value; }
		}

        /// <summary>
        /// Sets the rows value (rows" parameter for search results. Overrides any value set from a loaded Dictionary of parameters.
        /// </summary>
		public int Rows
		{
			get { return this._rows; }
			set { this._rows = value; }
		}

        /// <summary>
        /// Calculates the start parameter, based on the page parameter and rows parameter
        /// </summary>
        /// <returns>int representing the "start" parameter</returns>
		private int GetStartFromPage()
		{
			return (this.Page - 1) * this.Rows;
		}

        /// <summary>
        /// Adds a Facet parameter to this query
        /// </summary>
        /// <param name="oFacet">Facet object representing the field to be queried through facets</param>
		public void AddFacet(Facet oFacet)
		{
			this._searchfacets.Add(oFacet);
		}

        /// <summary>
        /// Collection of facets to be applied to this query. This is populated through the
        /// AddFacet method.
        /// </summary>
        public Facet[] Facets
        {
            get { return this._searchfacets.ToArray(); }
        }

        /// <summary>
        /// Ad-hoc method of adding a parameter/value pair to the search query.  Overwrites
        /// the value for the parameter if it already exists in the parameter/value pair collection
        /// for the query.
        /// </summary>
        /// <param name="parameter">Solr field to query</param>
        /// <param name="value">Value to be used in the query</param>
        public void AddSearchParameter(string parameter, string value)
		{
			this.AddSearchParameter(parameter, value, true);
		}

        /// <summary>
        /// Ad-hoc method of adding a parameter/value pair to the search query
        /// </summary>
        /// <param name="parameter">Solr field to query</param>
        /// <param name="value">Value to be used in the query</param>
        /// <param name="overwrite">True/False for overwriting the value for this parameter (if it's has already been set)</param>
		public void AddSearchParameter(string parameter, string value, bool overwrite)
		{
            //yskwun 20131015 중보가능한 key,value구조
            if (overwrite)
                removeSearchParameter(parameter);
            this._searchkeyvalues.Add(new KeyValuePair<string, string>(parameter, value)); 
		}

        //++ yskwun 20131015 
        public void removeSearchParameter(string parameter)
        {
            for (int i = this._searchkeyvalues.Count - 1; i >= 0; i--)
            {
                KeyValuePair<string, string> pair = this._searchkeyvalues[i];
                if(pair.Key.Equals(parameter))
                    this._searchkeyvalues.RemoveAt(i);
            }
        }
        //--

        /// <summary>
        /// The Query object to be applied in this query.
        /// </summary>
		public Query Query
		{
			get { return this._query; }
			set { this._query = value; }
		}

        /// <summary>
        /// Adds a sort parameter to this query.  Adding sort bases is order dependent.  "First in,
        /// first out" is the applied logic.
        /// </summary>
        /// <param name="oSort">Sort object to be added to the Sorts collection</param>
        public void AddSort(Sort oSort)
        {
            this._sorts.Add(oSort);
        }

        /// <summary>
        /// Collection of Sort objects to be applied to this query.
        /// </summary>
        public Sort[] Sorts
        {
            get { return this._sorts.ToArray(); }
        }

        /// <summary>
        /// Returns true if highlighting is enabled in this query. Highlighting is enabled
        /// by passing the fieldname to the HighlightField method.
        /// </summary>
		public bool IsHighlighted
		{
			get
			{
                return (this._highlightParameterCollection != null);
			}
		}

        /// <summary>
        /// Sets the solr parameter for highlighting results in the given fieldname for this query.
        /// </summary>
        public void AddHighlightParameterCollection(HighlightParameterCollection highlightParameterCollection)
        {
            this._highlightParameterCollection = highlightParameterCollection;
        }

        /// <summary>
        /// Gets the referenced HighlightParameterCollection.
        /// </summary>
        /// <returns>the referenced instance of HighlightParameterCollection</returns>
        public HighlightParameterCollection GetHighlightParameterCollection()
        {
            return this._highlightParameterCollection;
        }

        /// <summary>
        /// If set to true, additional debugging information will be included 
        /// in the response, including "explain" info for each of the documents 
        /// returned. The debugging info is meant for human consumption. The
        /// XML format could change in the future.
        /// 
        /// The default value is false.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return this._isDebugEnabled; }
            set 
            {
                this._isDebugEnabled = value;
            }
        }

        /// <summary>
        /// ExplainOtherQuery adds a separate Query to identify a set of documents for comparison
        /// to scoring under debugging. When provided, the ExplanationRecord of each document which 
        /// matches this query, relative the main query (specified by the q parameter) will be 
        /// returned along with the rest of the debugging information.
        /// </summary>
        public Query ExplainOtherQuery
        {
            get { return this._explainOtherQuery; }
            set
            {
                this._explainOtherQuery = value;
                this.IsDebugEnabled = true;
            }
        }

        /// <summary>
        /// Sets the query URL to be used for this query.
        /// </summary>

        //yskwun 20131015 중복가능한 param 구조로 변경 dictionary 대신 List<KeyValuePair> 사용으로 _searchkeyvalues 설정을 AddSearchParameter 함수호출형태로 바꿈
		private void SetQueryUrl()
        {
            #region Add paging,row parameters
            AddSearchParameter(QueryBuilder.SOLR_PARAM_STARTAT,this.GetStartFromPage().ToString());
            AddSearchParameter(QueryBuilder.SOLR_PARAM_ROWS,this.Rows.ToString());
            #endregion

            #region Add Query parameter

        	string queryString = string.Empty;

            if (this.Query != null)
            {
            	queryString = this.Query.ToString();
            }
            AddSearchParameter(QueryBuilder.SOLR_PARAM_QUERY, queryString);

            #endregion

            #region Add debugging parameters
            AddSearchParameter(QueryBuilder.SOLR_DEBUG_ENABLED, this.IsDebugEnabled.ToString().ToLower());
            if (this.ExplainOtherQuery != null)
            {
                AddSearchParameter(QueryBuilder.SOLR_EXPLAIN_OTHER, System.Web.HttpUtility.UrlEncode(this.ExplainOtherQuery.ToString()));
            }
            #endregion

            #region Add sorting parameters
            if (this.Sorts.Length > 0)
            {
                string[] arSorts = new string[this.Sorts.Length + 1];
                for (int j = 0; j < this.Sorts.Length; j++)
                {
                    arSorts[j] = this.Sorts[j].ToString();
                }
                arSorts[this.Sorts.Length] = "score desc";    //Added as a default to bubble up relevance in tie-breakers
                AddSearchParameter("sort", string.Join(",", arSorts));
            }
            #endregion

            List<string> stringparams = new List<string>();

            #region Build Querystring
			foreach (KeyValuePair<string, string> kvp in this._searchkeyvalues)
			{
                stringparams.Add(kvp.Key + "=" + System.Web.HttpUtility.UrlEncode(kvp.Value));
			}
            #endregion

            #region Add highlighting parameters
            if (this._highlightParameterCollection != null)
            {
                stringparams.Add(this._highlightParameterCollection.ToString());
            }
            #endregion

            string urlparams = string.Join("&", stringparams.ToArray());

            #region Add facet parameters
            if (this._searchfacets.Count > 0)
			{
                foreach (Facet oFacet in this._searchfacets)
                {
                    urlparams += "&" + oFacet.UrlParameters;
                }
            }
            #endregion

            this._queryurl = urlparams;
        }

        /// <summary>
        /// Returns the query url to be used when executing this search against a solr index.
        /// </summary>
		public string QueryUrl
		{
			get 
			{
				this.SetQueryUrl();
				return this._queryurl;
			}
		}

		
	}
}
