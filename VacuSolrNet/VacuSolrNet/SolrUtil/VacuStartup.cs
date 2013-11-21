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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;
using SolrNet.Utils;
using SolrNet;


namespace VacuSolrNet {
    /// <summary>
    /// SolrNet initialization manager
    /// </summary>
    public static class VacuStartup {
        public static void Init<T>(string serverURL) {
            var connection = new SolrConnection(serverURL) {
                //Cache = Container.GetInstance<ISolrCache>(),
            };

            Init<T>(connection, "");
        }

        public static void Init<T>(ISolrConnection connection, string cfgFile) {
            Startup.Init<T>(connection);
            //ISolrReadOnlyOperations
            Startup.Container.Register<VacuSolrServer<T>>(c => new VacuSolrServer<T>(c.GetInstance<ISolrBasicOperations<T>>(), Startup.Container.GetInstance<IReadOnlyMappingManager>(), Startup.Container.GetInstance<IMappingValidator>(), cfgFile));
        }
    }
}
