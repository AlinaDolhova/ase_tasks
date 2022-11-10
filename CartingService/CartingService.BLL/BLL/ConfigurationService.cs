using CartingService.BLL.Interfaces;
using Microsoft.Extensions.Configuration;


namespace CartingService.BLL
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration configuration;
        public ConfigurationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string TopicPath => configuration["topic_path"];
        public string SubscriptionName => configuration["subscription_name"];
    }
}
