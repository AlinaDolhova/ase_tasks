using System;
using System.Collections.Generic;
using System.Text;

namespace CartingService.BLL.Interfaces
{
    public interface IConfigurationService
    {
        public string TopicPath { get; }
        public string SubscriptionName { get; }
    }
}
