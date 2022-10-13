using System.Threading.Tasks;

namespace CatalogService.DAL.Interfaces
{
    public interface IDbContext
    {
        
        Task SaveChangesAsync();
    }
}
