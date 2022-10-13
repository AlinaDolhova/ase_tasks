using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.DAL.Interfaces
{
    public interface IDeletable
    {
        bool IsDeleted { get; set; }
    }
}
