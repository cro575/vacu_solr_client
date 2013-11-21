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
using System.Configuration;
using System.Threading;

namespace org.apache.solr.SolrSharp.Configuration
{
    /// <summary>
    /// The collection of accessible solr servers.  Each server is defined by
    /// its URL.
    /// </summary>
    public class SolrSearchers
    {
        private static readonly List<SolrSearcher> _searchers = new List<SolrSearcher>();

        /// <summary>
        /// Static constructor to read the configuration section for the application.  Misconfiguration
        /// throws a ConfigurationErrorsException.  Each <see cref="SolrServer">SolrServer</see> is represented
        /// as a <see cref="SolrSearcher">SolrSearcher</see>.
        /// </summary>
        static SolrSearchers()
        {
            try
            {
                SolrConfigurationSection sysSection = ConfigurationManager.GetSection("solr") as SolrConfigurationSection;
                Thread.CurrentThread.CurrentCulture = sysSection.CultureInfo;
                SolrServers servers = sysSection.SolrServers;
                foreach (SolrServer ss in servers)
                {
                    SolrSearchers._searchers.Add(new SolrSearcher(ss.Url, ss.Mode, true)); // yskwun 20130210  secure option add
                }
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("Solr configuration error", ex.InnerException);
            }
        }

        /// <summary>
        /// Returns a SolrSearcher from the pool.
        /// </summary>
        /// <param name="searcherMode">Mode for the configured searcher.</param>
        /// <returns>The first SolrSearcher matching the mode, or null</returns>
        public static SolrSearcher GetSearcher(Mode searcherMode)
        {
            foreach (SolrSearcher searcher in SolrSearchers._searchers)
                if (searcher.Supports(searcherMode))
                    return searcher;
            return null;
        }

    }
}
