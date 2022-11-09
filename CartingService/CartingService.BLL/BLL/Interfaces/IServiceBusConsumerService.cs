using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CartingService.BLL.Interfaces
{
    public interface IServiceBusConsumerService
    {
        Task PrepareFiltersAndHandleMessages();
    }
}
