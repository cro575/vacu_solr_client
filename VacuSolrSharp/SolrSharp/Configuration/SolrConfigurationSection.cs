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
using System.Globalization;
using System.Threading;

namespace org.apache.solr.SolrSharp.Configuration
{

    /// <summary>
    /// Configuration class representing all solr servers.  Each solr server is equal
    /// to a solr instance; multiple solr instances *may* exist on one physical server.
    /// </summary>
    internal class SolrConfigurationSection : ConfigurationSection
    {
        internal SolrConfigurationSection()
        {
        }

        [ConfigurationProperty("", IsDefaultCollection=true)]
        internal SolrServers SolrServers
        {
            get { return (SolrServers)base[""]; }
        }

        private CultureInfo _cultureInfo = null;
        [ConfigurationProperty("cultureinfo",IsRequired=false)]
        internal CultureInfo CultureInfo
        {
            get 
            {
                if (this._cultureInfo == null)
                {
                    if (base["cultureinfo"] == null)
                    {
                        this._cultureInfo = Thread.CurrentThread.CurrentCulture;
                    }
                    else
                    {
                        this._cultureInfo = (CultureInfo)base["cultureinfo"];
                    }
                }
                return this._cultureInfo;
            }
            set { this._cultureInfo = value; }
        }
    }
}
