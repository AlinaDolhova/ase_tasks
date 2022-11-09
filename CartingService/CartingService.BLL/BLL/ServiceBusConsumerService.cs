using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using CartingService.BLL.Interfaces;
using CartingService.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
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

        private readonly ICartService cartService;

        public ServiceBusConsumerService(ServiceBusClient client, ServiceBusAdministrationClient serviceBusAdministrationClient,
            ICartService cartService, IConfigurationService configurationService)
        {
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
            var item = JsonConvert.DeserializeObject<ItemUpdatedMessage>(args.Message.Body.ToString());

            this.cartService.UpdateItemInCarts(item.Id, item.Name, item.Price);

            await args.CompleteMessageAsync(args.Message);
        }

        private async Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
        }
    }
}
