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
using System.Web;
using System.Xml;

namespace org.apache.solr.SolrSharp.Query
{
	/// <summary>
	/// A facet is a count summary for a given solr index field among aggregate search results.  For
    /// additional information about facets, please see the Solr website.
	/// </summary>
	public abstract class Facet
	{

		private Dictionary<string, string> _facetparams = new Dictionary<string, string>();

		private static readonly string SOLR_FACET = "facet";
		private static readonly string SOLR_FACET_FIELD = "facet.field";
		private static readonly string SOLR_FACET_LIMIT = "facet.limit";
		private static readonly string SOLR_FACET_ZEROS = "facet.zeros";
		private static readonly string SOLR_FACET_MISSING = "facet.missing";

		private enum FacetParameter
		{
			Facet,
			Field,
			Limit,
			Zeros,
			Missing
		}

        /// <summary>
        /// Empty public constructor.
        /// </summary>
		public Facet()
		{
            this.Limit = 10;
		}

        /// <summary>
        /// Adds a facet parameter on the given solr index fieldname
        /// </summary>
        /// <param name="field">Solr index fieldname to query as a facet</param>
		public Facet(string field)
            : this()
		{
            this.Field = field;
		}

		/// <summary>
		/// This param allows you to specify a field which should be treated as a facet. It will iterate over each Term 
		/// in the field and generate a facet count using that Term as the constraint.  When using this param, it is 
		/// important to remember that "Term" is a very specific concept in Lucene -- it relates to the literal field/value 
		/// pairs that are Indexed after any Analysis occurs. For text fields that include stemming, or lowercasing, or word 
		/// splitting you may not get what you expect. If you want both Analysis (for searching) and Faceting on the full 
		/// literal Strings, use copyField to create two versions of the field: one Text and one String. 
		/// Make sure both are indexed="true".
		/// </summary>
        public string Field { get; set; }

		/// <summary>
		/// Set to "true" this param indicates that constraint counts for facet fields should be included even 
		/// if the count is "0", set to "false" or blank and the "0" counts will be supressed to save on the 
		/// amount of data returned in the response.
		/// </summary>
        public bool DisplayZeros { get; set; }

		/// <summary>
		/// Set to "true" this param indicates that in addition to the Term based constraints of a facet field, 
		/// a count of all matching results which have no value for the field should be computed.
		/// </summary>
        public bool DisplayMissing { get; set; }

		/// <summary>
		/// Indicates the maximum number of constraint counts that should be returned for the facet fields, 
        /// similar to "SELECT TOP..." in SQL.  If a non-negative value is specified, the constraints 
        /// (ie: Terms) will be sorted by the facet count (descending) and only the top N terms will be 
        /// returned with their counts.
		/// </summary>
        public int Limit { get; set; }

		private string UrlParamFieldPreset
		{
			get { return "f." + this.Field + "."; }
		}
		private string UrlParamaterKey(FacetParameter eParam)
		{
			string param = "";
			switch (eParam)
			{
				case FacetParameter.Facet:
					param = Facet.SOLR_FACET;
					break;
				case FacetParameter.Field:
					param = Facet.SOLR_FACET_FIELD;
					break;
				case FacetParameter.Limit:
					param = this.UrlParamFieldPreset + Facet.SOLR_FACET_LIMIT;
					break;
				case FacetParameter.Missing:
					param = this.UrlParamFieldPreset + Facet.SOLR_FACET_MISSING;
					break;
				case FacetParameter.Zeros:
					param = this.UrlParamFieldPreset + Facet.SOLR_FACET_ZEROS;
					break;
			}
			return param;
		}

        /// <summary>
        /// Returns syntactically structured parameters for use within a url
        /// as part of an http search request
        /// </summary>
		public string UrlParameters
		{
			get
			{
				this._facetparams.Clear();
				this._facetparams.Add(Facet.SOLR_FACET, "true");
				this._facetparams.Add(this.UrlParamaterKey(FacetParameter.Zeros), this.DisplayZeros.ToString().ToLower());
				this._facetparams.Add(this.UrlParamaterKey(FacetParameter.Missing), this.DisplayMissing.ToString().ToLower());
				this._facetparams.Add(this.UrlParamaterKey(FacetParameter.Limit), this.Limit.ToString());
				this._facetparams.Add(this.UrlParamaterKey(FacetParameter.Field), this.Field);

				string[] arParams = new string[this._facetparams.Count];
				int x = 0;
				foreach (KeyValuePair<string, string> kvp in this._facetparams)
				{
					arParams[x] = kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value);
					x++;
				}

				string urlparams = string.Join("&", arParams);
				return urlparams;
			}
		}


	}
}
