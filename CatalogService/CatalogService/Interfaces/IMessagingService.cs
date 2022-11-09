using CatalogService.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Interfaces
{
    public interface IMessagingService
    {
        Task SendUpdateMessageAsync(ItemUpdatedMessage item);
    }
}
