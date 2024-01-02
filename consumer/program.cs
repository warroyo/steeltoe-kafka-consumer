// Copyright 2022 Confluent Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Refer to LICENSE for more information.

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.Kubernetes;
using Steeltoe.Extensions.Configuration.Kubernetes.ServiceBinding;




/// <summary>
///     An example showing consumer 
///     with a custom OAUTHBEARER token implementation.
/// </summary>
namespace KafkaExample
{


    class Program
    {
       

        static void Main(string[] args)
        {
   
            HostApplicationBuilder host = Host.CreateApplicationBuilder(args);
            
            host.Logging.AddConsole();

            host.Services.AddHostedService<ConsumerService>();
            host.Configuration.AddKubernetesServiceBindings();            
            
            IHost svc = host.Build();
            svc.RunAsync();
            
        }
    }

}