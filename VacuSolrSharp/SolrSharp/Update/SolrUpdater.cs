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
using System.Net;
using System.Text;
using org.apache.solr.SolrSharp.Indexing;
using org.apache.solr.SolrSharp.Configuration;
using System.IO; // yskwun 20130210  add to solr fileupdate 

namespace org.apache.solr.SolrSharp.Update
{
    /// <summary>
    /// Used for maintaining a solr index. This consists of updates to the index content
    /// with either inserts/updates or deletes, as well as commands for committing changes
    /// and optimizing the index.
    /// </summary>
	public class SolrUpdater
	{
        private SolrSearcher solrsearcher = null;

        public string statusDesc = ""; // yskwun 20130210  modify member valiable

        /// <summary>
        /// The url used to apply updates, additions and deletes to the solr index.
        /// </summary>
        public string SOLR_UPDATE
        {
            get { return this.solrsearcher.SOLR + "update/"; }
        }

        /// <summary>
        /// Constructs an object to apply updates (adds/updates/deletes) to a solr index.
        /// </summary>
        /// <param name="solrSearcher">SolrSearcher instance mapped to the solr index where updates are to be applied</param>
        public SolrUpdater(SolrSearcher solrSearcher)
        {
            this.solrsearcher = solrSearcher;
        }


		/// <summary>
		/// Adds or updates an IndexDocument to the search index.  bCommit is useful for bulk 
        /// insert/updates, i.e. if many records require adding/updating, this can be called 
        /// for the last IndexDocument in a series (which will be applied for all.)
		/// </summary>
		/// <param name="oDoc">IndexDocument to be added/updated in the index</param>
		/// <param name="bCommit">bCommit is useful for bulk insert/updates, i.e. if many records require 
        /// adding/updating, this can be called for the last IndexDocument in a series (which will be 
        /// applied for all.)</param>
		public void PostToIndex(IndexDocument oDoc, bool bCommit)
		{
		    HttpStatusCode eCode = this.ExecuteCommand(oDoc.SerializeToString());
            if (bCommit) this.Commit();
        }

		/// <summary>
		/// Applies an IndexDocument to the solr index in real time, with immediate updating
		/// </summary>
		/// <param name="oDoc">IndexDocument to be applied to the solr index</param>
		public void PostToIndex(IndexDocument oDoc)
		{
			this.PostToIndex(oDoc, true);
		}

        /// <summary>
        /// Executes the COMMIT command on the solr index, causing any uncommitted changes
        /// to be applied to the solr index.
        /// </summary>
        /// <returns>HttpStatusCode</returns>
		public HttpStatusCode Commit()
		{
            return this.ExecuteCommand("<commit/>");
		}

        /// <summary>
        /// Executes the OPTIMIZE command on the solr index, causing any fragmented updates
        /// to be merged to solr index, according to mergeFactor settings per the solr configuration.
        /// </summary>
        /// <returns>HttpStatusCode</returns>
        public HttpStatusCode Optimize()
        {
            return this.ExecuteCommand("<optimize/>");
        }

        //++ yskwun 20130210  post file update
        public HttpStatusCode PostToFile(string fileName, bool bCommit)
        {
            StreamReader SRead = new StreamReader(fileName, true);

            string strBuf = SRead.ReadToEnd();

            SRead.Close();

            HttpStatusCode ret = ExecuteCommand(strBuf);
            if (bCommit) this.Commit();

            return ret;
        }

        public HttpStatusCode ExecuteCommand(string commandtext, bool bCommit)
        {
            HttpStatusCode ret = ExecuteCommand(commandtext);
            if (bCommit) this.Commit();

            return ret;
        }
         //--

         //++ yskwun 20130210  modify
        public HttpStatusCode ExecuteCommand(string commandtext)
        {
            byte[] postBytes = SolrSearcher.GetContentToPost(commandtext, Encoding.UTF8);
            
            statusDesc = string.Empty;
            return SolrSearcher.WebPost(this.SOLR_UPDATE, postBytes, ref statusDesc);
        }
	}
}
