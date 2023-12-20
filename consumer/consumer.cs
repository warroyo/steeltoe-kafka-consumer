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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Confluent.Kafka;


/// <summary>
///     An example showing consumer 
///     with a custom OAUTHBEARER token implementation.
/// </summary>
namespace KafkaExample;


public class ConsumerService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _config;
    public ConsumerService(ILogger<ConsumerService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("1. StartAsync has been called.");

        var cConfig = _config.GetSection("Consumer").Get<ConsumerConfig>().ThrowIfContainsNonUserConfigurable();

        var topicName = _config.GetValue<string>("General:TopicName");

        var assigned = false;

        foreach(var config in _config.AsEnumerable()) {
            Console.WriteLine($"{config.Key} = {config.Value}");
        }

        cConfig.Set("enable.auto.offset.store","false");
        using (var consumer = new ConsumerBuilder<string, string>(cConfig)
                                .SetPartitionsAssignedHandler((c, ps) => { assigned = true; }).Build())
        {
            Console.WriteLine("\n-----------------------------------------------------------------------");
            Console.WriteLine($"Consumer {consumer.Name} consuming from topic {topicName}.");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("Ctrl-C to quit.\n");

            consumer.Subscribe(topicName);

            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");
                        try
                        {
                            consumer.StoreOffset(consumeResult);
                        }
                        catch (KafkaException e)
                        {
                            Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Closing consumer.");
                consumer.Close();
            }
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("4. StopAsync has been called.");

        return Task.CompletedTask;
    }


}
