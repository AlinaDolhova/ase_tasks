using CartingService.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace CartingService.BLL
{
    public class ConfigurationService : IConfigurationService
    {
        public string TopicPath => ConfigurationManager.AppSettings.Get("topic_path");
        public string SubscriptionName => ConfigurationManager.AppSettings.Get("subscription_name");
    }
}
