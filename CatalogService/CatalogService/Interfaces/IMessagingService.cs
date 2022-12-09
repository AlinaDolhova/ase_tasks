using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Model;

namespace CatalogService.BLL.Interfaces
{
    public interface IMessagingService
    {
        Task SendUpdateMessageAsync(ItemUpdatedMessage item);
    }
}
