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
    /// Configuration representation of all solr servers.
    /// </summary>
    internal class SolrServers : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SolrServer();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SolrServer)element).Url + ((SolrServer)element).Mode.ToString();
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        protected override string ElementName
        {
            get
            {
                return "server";
            }
        }
    }
}
