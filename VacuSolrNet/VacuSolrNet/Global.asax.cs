#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using log4net.Config;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;


namespace VacuSolrNet
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class NetApplication : HttpApplication {

        private static readonly string solrURL = ConfigurationManager.AppSettings["solrUrl"];
        private static readonly string credentials = ConfigurationManager.AppSettings["credentials"];

        protected void Application_Start() {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Server.MapPath("/"), "log4net.config")));

            string cfgFile = Path.Combine(Server.MapPath("/config/solrmap/"), "solrSearchConfig.xml");

            var connection = new SolrConnection(solrURL, credentials);
            var loggingConnection = new LoggingConnection(connection);
            VacuStartup.Init<SolrSearchRecord>(loggingConnection, cfgFile);
        }
    }
}