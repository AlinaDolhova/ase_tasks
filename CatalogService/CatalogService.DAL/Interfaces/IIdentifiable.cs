using System;

namespace CatalogService.DAL.Interfaces
{
    public interface IIdentifiable
    {
        Guid Id { get; set; }
    }
}
