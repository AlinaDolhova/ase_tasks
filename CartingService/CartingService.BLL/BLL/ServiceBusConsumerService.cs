using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using CartingService.BLL.Interfaces;
using CartingService.Model;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CartingService.BLL
{
    public class ServiceBusConsumerService: IServiceBusConsumerService
    {
        private static string FilterName = "TypeItemUpdatedMessage";

        private readonly ServiceBusClient client;
        private readonly ServiceBusAdministrationClient adminClient;
        private readonly ServiceBusProcessor serviceBusProcessor;
        private readonly IConfigurationService configurationService;
        private readonly ILogger<ServiceBusConsumerService> logger;
        private readonly TelemetryClient telemetryClient;

        private readonly ICartService cartService;

        public ServiceBusConsumerService(
            ServiceBusClient client, 
            ServiceBusAdministrationClient serviceBusAdministrationClient,
            ICartService cartService, 
            IConfigurationService configurationService, 
            ILogger<ServiceBusConsumerService> logger,
            TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
            ServiceBusProcessorOptions _serviceBusProcessorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };

            this.client = client;
            this.serviceBusProcessor = client.CreateProcessor(configurationService.TopicPath, configurationService.SubscriptionName, _serviceBusProcessorOptions);
            this.adminClient = serviceBusAdministrationClient;
            this.cartService = cartService;
            this.configurationService = configurationService;
            this.logger = logger;
        }
        public async Task PrepareFiltersAndHandleMessages()
        {

            serviceBusProcessor.ProcessMessageAsync += ProcessMessagesAsync;
            serviceBusProcessor.ProcessErrorAsync += ProcessErrorAsync;

            await AddFilters();

            await serviceBusProcessor.StartProcessingAsync().ConfigureAwait(false);
        }

        private async Task AddFilters()
        {

            var rules = adminClient.GetRulesAsync(configurationService.TopicPath, configurationService.SubscriptionName)
                .ConfigureAwait(false);

            var ruleProperties = new List<RuleProperties>();
            await foreach (var rule in rules)
            {
                ruleProperties.Add(rule);
            }

            if (!ruleProperties.Any(r => r.Name == FilterName))
            {
                CreateRuleOptions createRuleOptions = new CreateRuleOptions
                {
                    Name = FilterName,
                    Filter = new SqlRuleFilter(@$"type = '{nameof(ItemUpdatedMessage)}'")
                };
                await adminClient.CreateRuleAsync(configurationService.TopicPath, configurationService.SubscriptionName, createRuleOptions)
                    .ConfigureAwait(false);
            }

        }

        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            ServiceBusReceivedMessage message = args.Message;
            if (message.ApplicationProperties.TryGetValue("Diagnostic-Id", out var objectId) && objectId is string diagnosticId)
            {
                var activity = new Activity("ServiceBusProcessor.ProcessItemUpdatedMessage");
                activity.SetParentId(diagnosticId);

                using (var operation = telemetryClient.StartOperation<RequestTelemetry>("Process", activity.RootId, activity.ParentId))
                {
                    telemetryClient.TrackTrace("Received ItemUpdatedMessage message");
                    try
                    {
                        logger.LogDebug("ProcessMessagesAsync: message received: {message}", args);

                        var item = JsonConvert.DeserializeObject<ItemUpdatedMessage>(args.Message.Body.ToString());

                        logger.LogInformation("ProcessMessagesAsync: message received for {id}:", item.Id);

                        this.cartService.UpdateItemInCarts(item.Id, item.Name, item.Price);
                        await args.CompleteMessageAsync(args.Message);
                    }
                    catch (Exception ex)
                    {
                        telemetryClient.TrackException(ex);
                        operation.Telemetry.Success = false;
                        throw;
                    }
                    finally
                    {
                        telemetryClient.TrackTrace("Done");
                    }
                }
            }                
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            logger.LogError("ProcessMessagesAsync: error occured. Details: {args}:", args);

            return Task.CompletedTask;
        }
    }
}
