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
using System.Text;

namespace org.apache.solr.SolrSharp.Configuration
{
    /// <summary>
    /// Enumeration defining the operation mode for a given solr server.  This is useful
    /// when using solr's replication model.  Otherwise, a stand-alone solr server uses the
    /// ReadWrite mode.
    /// </summary>
    [FlagsAttribute]
    public enum Mode
    {
        /// <summary>
        /// Solr webapp instance supports Read requests
        /// </summary>
        Read        = 1,
        /// <summary>
        /// Solr webapp instance supports Write requests
        /// </summary>
        Write       = 2,
        /// <summary>
        /// Solr webapp instance supports both Read and Write requests
        /// </summary>
        ReadWrite   = 3
    }

    /// <summary>
    /// Representation of solr server instance.  A solr server instance is denoted by its 
    /// URL.
    /// </summary>
    internal class SolrServer : ConfigurationElement
    {
        /// <summary>
        /// The Url of the solr server instance.  The Url must be unique within the configuration section.
        /// </summary>
        [ConfigurationProperty("url", IsKey = true, IsRequired = true)]
        public string Url
        {
            get { return base["url"].ToString(); }
            set { base["url"] = value; }
        }

        /// <summary>
        /// The mode of a solr server instance.  Using solr's replication model, servers
        /// can be structured in "write" mode (Master) or "read" mode (Slave).  For stand-alone
        /// servers, ReadWrite is the appropriate setting.
        /// </summary>
        private Mode _mode = Mode.ReadWrite;
        [ConfigurationProperty("mode", IsRequired = true)]
        public Mode Mode
        {
            get
            {
                switch (base["mode"].ToString())
                {
                    case "read":
                        this._mode = Mode.Read;
                        break;
                    case "write":
                        this._mode = Mode.Write;
                        break;
                    case "readwrite":
                    default:
                        this._mode = Mode.ReadWrite;
                        break;
                }
                return this._mode;
            }
            set { this._mode = value; }
        }

        public override string ToString()
        {
            return this.Url;
        }

    }
}
