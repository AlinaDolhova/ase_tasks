using AutoMapper;
using CatalogService.DAL.Models;
using System.Linq;

namespace CatalogService.Common.MapperProfiles
{
    public class CatalogProfile : Profile
    {
        public CatalogProfile()
        {
            CreateMap<Category, Model.Category>()
               // .ForMember(x => x.Items, x => x.MapFrom(y => y.Items.Select(z => z.Id)))
                .ReverseMap();

            CreateMap<Item, Model.Item>()
                 .ReverseMap();
        }
    }
}
