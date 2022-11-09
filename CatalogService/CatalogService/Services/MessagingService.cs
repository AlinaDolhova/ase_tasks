using Azure.Messaging.ServiceBus;
using CatalogService.BLL.Interfaces;
using CatalogService.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services
{
    public class MessagingService: IMessagingService
    {
        private readonly ServiceBusSender sender;

        public MessagingService(ServiceBusClient client)
        {
            this.sender = client.CreateSender("catalog-topic");
        }

        public async Task SendUpdateMessageAsync(ItemUpdatedMessage item)
        {
            string messageContent = JsonConvert.SerializeObject(item);
            ServiceBusMessage message = new ServiceBusMessage(messageContent);

            message.ApplicationProperties.Add("type", nameof(ItemUpdatedMessage));            

            await sender.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
